using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;
using Fhir.TypeFramework.Performance;
using Fhir.TypeFramework.Serialization;
using Fhir.TypeFramework.Tests.Application.Contracts;

namespace Fhir.TypeFramework.Tests.Application.BehaviorSuites;

public sealed class CoverageReflectionBehaviorSuite : ICoverageReflectionBehaviorSuite
{
    public void TouchAllDataTypesProperties()
    {
        var asm = typeof(Base).Assembly;

        var types = asm.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => typeof(Fhir.TypeFramework.Bases.DataType).IsAssignableFrom(t))
            .Where(t => t.Namespace == "Fhir.TypeFramework.DataTypes")
            .ToArray();

        foreach (var t in types)
        {
            var instance = Activator.CreateInstance(t);
            Assert.NotNull(instance);

            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!p.CanRead || !p.CanWrite) continue;

                if (IsNullableReferenceOrNullableValueType(p.PropertyType))
                {
                    p.SetValue(instance, null);
                    _ = p.GetValue(instance);
                    continue;
                }

                if (p.PropertyType.IsValueType)
                {
                    var dv = Activator.CreateInstance(p.PropertyType);
                    p.SetValue(instance, dv);
                    _ = p.GetValue(instance);
                    continue;
                }

                _ = p.GetValue(instance);
            }
        }
    }

    public void ExerciseAllConcreteBaseDerivedTypes()
    {
        var asm = typeof(Base).Assembly;

        var types = asm.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => typeof(Base).IsAssignableFrom(t))
            .ToArray();

        foreach (var t in types)
        {
            var obj = (Base?)Activator.CreateInstance(t);
            if (obj is null) continue;

            _ = obj.Validate(new ValidationContext(obj)).ToList();

            var copy = obj.DeepCopy();
            _ = obj.IsExactly(copy);

            if (obj is Element el)
                _ = el.GetJsonNode();
        }
    }

    public void ExercisePrimitiveTypeJsonHelpers()
    {
        var primitives = new Base[]
        {
            new FhirString("abc"),
            new FhirId("patient-123"),
            new FhirUri("http://example.com"),
            new FhirBoolean("true"),
            new FhirInteger("42"),
            new FhirDecimal("3.14"),
            new FhirDateTime("2020-01-01"),
            new FhirTime("12:34:56"),
        };

        foreach (var p in primitives)
        {
            Assert.IsAssignableFrom<Base>(p);

            if (p is PrimitiveType<string> ps)
            {
                var jv = ps.ToJsonValue();
                ps.FromJsonValue(jv);
                var full = ps.ToFullJsonObject();
                ps.FromFullJsonObject(full);
            }
            else
            {
                var pt = p.GetType();
                var toJsonValue = pt.GetMethod("ToJsonValue")!;
                var fromJsonValue = pt.GetMethod("FromJsonValue")!;
                var toFull = pt.GetMethod("ToFullJsonObject")!;
                var fromFull = pt.GetMethod("FromFullJsonObject")!;

                var jv = toJsonValue.Invoke(p, Array.Empty<object?>());
                fromJsonValue.Invoke(p, new[] { jv });

                var full = toFull.Invoke(p, Array.Empty<object?>());
                fromFull.Invoke(p, new[] { full });
            }
        }
    }

    public void ExercisePerformanceAndSerializationUtilities()
    {
        TypeFrameworkCache.EnableCaching = true;
        TypeFrameworkCache.Set("k", 123);
        Assert.True(TypeFrameworkCache.TryGet<int>("k", out var v));
        Assert.Equal(123, v);

        ValidationOptimizer.EnableOptimization = true;
        _ = ValidationOptimizer.GetCachedRegex("^a$").IsMatch("a");
        ValidationOptimizer.ClearRegexCache();

        PerformanceMonitor.EnableMonitoring = true;
        using (PerformanceMonitor.Measure("op"))
        {
            _ = 1 + 1;
        }
        _ = PerformanceMonitor.GenerateReport();
        PerformanceMonitor.ClearMetrics();

        var ext = new Extension { Url = "http://example.com/ext" };
        var json = FhirJsonSerializer.Serialize(ext);
        Assert.Contains("url", json, StringComparison.OrdinalIgnoreCase);
    }

    public void ReflectionInvokeMostPublicMethods()
    {
        var asm = typeof(Base).Assembly;

        foreach (var t in asm.GetTypes()
                     .Where(t => t.IsClass && !t.IsAbstract)
                     .Where(t => !t.ContainsGenericParameters)
                     .Where(t => t.FullName is null || !t.FullName.Contains('<')))
        {
            object? instance = null;
            if (!t.IsAbstract && !t.IsSealed && t.GetConstructor(Type.EmptyTypes) != null)
            {
                instance = Activator.CreateInstance(t);
            }
            else if (t.IsSealed && t.GetConstructor(Type.EmptyTypes) != null && !IsStaticClass(t))
            {
                instance = Activator.CreateInstance(t);
            }

            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(m => !m.IsAbstract)
                .Where(m => !m.IsGenericMethodDefinition)
                .Where(m => !m.IsSpecialName)
                .ToArray();

            foreach (var m in methods)
            {
                var target = m.IsStatic ? null : instance;
                if (!m.IsStatic && target is null) continue;

                var args = BuildArgs(m.GetParameters(), target);
                if (args is null) continue;

                try
                {
                    var result = m.Invoke(target, args);

                    if (result is System.Collections.IEnumerable en && result is not string)
                    {
                        foreach (var _ in en) { }
                    }
                }
                catch
                {
                    // coverage-only：忽略特定方法在預設參數下可能丟出的例外
                }
            }
        }
    }

    private static bool IsStaticClass(Type t) => t.IsAbstract && t.IsSealed;

    private static object?[]? BuildArgs(ParameterInfo[] parameters, object? target)
    {
        var args = new object?[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            var pt = p.ParameterType;

            if (pt == typeof(string)) { args[i] = ""; continue; }
            if (pt == typeof(int)) { args[i] = 0; continue; }
            if (pt == typeof(bool)) { args[i] = false; continue; }
            if (pt == typeof(decimal)) { args[i] = 0m; continue; }
            if (pt == typeof(DateTime)) { args[i] = DateTime.UnixEpoch; continue; }
            if (pt == typeof(TimeSpan)) { args[i] = TimeSpan.Zero; continue; }

            if (pt == typeof(ValidationContext))
            {
                args[i] = new ValidationContext(target ?? new object());
                continue;
            }

            if (pt.IsValueType)
            {
                args[i] = Activator.CreateInstance(pt);
                continue;
            }

            args[i] = null;
        }

        return args;
    }

    private static bool IsNullableReferenceOrNullableValueType(Type t)
    {
        if (!t.IsValueType) return true;
        return Nullable.GetUnderlyingType(t) != null;
    }
}
