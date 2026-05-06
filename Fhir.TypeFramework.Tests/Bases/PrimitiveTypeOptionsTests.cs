using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.TypeFramework.Tests.Bases;

public sealed class PrimitiveTypeOptionsTests
{
    [Fact]
    public void Deferred_SyncsTypedValueOnFirstValueRead()
    {
        var prev = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Deferred;
            var i = new FhirInteger();
            i.StringValue = "1";
            i.StringValue = "2";
            i.StringValue = "99";
            Assert.Equal(99, i.Value);
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prev;
        }
    }

    [Fact]
    public void EnsureTypedValueParsed_SyncsWithoutReadingValueProperty()
    {
        var prev = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Deferred;
            var i = new FhirInteger { StringValue = "42" };
            i.EnsureTypedValueParsed();
            Assert.Equal(42, i.Value);
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prev;
        }
    }

    [Fact]
    public void IsExactly_ComparesStringNotStaleTyped()
    {
        var prev = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Deferred;
            var a = new FhirInteger { StringValue = "5" };
            var b = new FhirInteger { StringValue = "5" };
            _ = a.Value;
            Assert.True(a.IsExactly(b));
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prev;
        }
    }

    [Fact]
    public void Eager_MatchesLegacy_StringValue_Assigns_Parse_Per_Set()
    {
        var prevTiming = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Eager;
            var i = new FhirInteger();
            i.StringValue = "7";
            Assert.Equal(7, i.Value);
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prevTiming;
        }
    }

    [Fact]
    public void ValidateBeforeJsonWrite_WhenTrue_Throws_On_Invalid_Value()
    {
        var prevV = PrimitiveTypeOptions.ValidateBeforeJsonWrite;
        try
        {
            PrimitiveTypeOptions.ValidateBeforeJsonWrite = true;
            var id = new FhirId("bad id");
            Assert.Throws<InvalidOperationException>(() => id.ToJsonValue());
        }
        finally
        {
            PrimitiveTypeOptions.ValidateBeforeJsonWrite = prevV;
        }
    }

    [Fact]
    public void ValidateBeforeJsonWrite_WhenFalse_Allows_Invalid_String_In_Json()
    {
        var prevV = PrimitiveTypeOptions.ValidateBeforeJsonWrite;
        try
        {
            PrimitiveTypeOptions.ValidateBeforeJsonWrite = false;
            var id = new FhirId("bad id");
            var jv = id.ToJsonValue();
            Assert.NotNull(jv);
            Assert.Equal("bad id", jv!.GetValue<string>());
        }
        finally
        {
            PrimitiveTypeOptions.ValidateBeforeJsonWrite = prevV;
        }
    }

    [Fact]
    public void ValidateBeforeJsonWrite_WhenTrue_Accepts_Valid_Primitive()
    {
        var prevV = PrimitiveTypeOptions.ValidateBeforeJsonWrite;
        try
        {
            PrimitiveTypeOptions.ValidateBeforeJsonWrite = true;
            var id = new FhirId("patient-123");
            var jv = id.ToJsonValue();
            Assert.NotNull(jv);
            var full = id.ToFullJsonObject();
            Assert.NotNull(full);
        }
        finally
        {
            PrimitiveTypeOptions.ValidateBeforeJsonWrite = prevV;
        }
    }

    [Fact]
    public void DeepCopy_Preserves_Deferred_State()
    {
        var prev = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Deferred;
            var a = new FhirInteger { StringValue = "8" };
            var b = (FhirInteger)a.DeepCopy();
            Assert.Equal("8", b.StringValue);
            _ = b.Value;
            Assert.Equal(8, b.Value);
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prev;
        }
    }

    [Fact]
    public void Eager_EnsureTypedValueParsed_Is_NoOp()
    {
        var prev = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Eager;
            var i = new FhirInteger("3");
            i.EnsureTypedValueParsed();
            Assert.Equal(3, i.Value);
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prev;
        }
    }
}
