using Fhir.QueryBuilder.QueryBuilders;
using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder
{
    /// <summary>
    /// 測試進階功能的簡單程式
    /// </summary>
    public class TestAdvancedFeatures
    {
        public static void Run(string[] args)
        {
            Console.WriteLine("🚀 FHIR Query Builder 進階功能測試");
            Console.WriteLine("=".PadRight(50, '='));

            try
            {
                // 先執行簡單測試
                SimpleTest.TestBasicFunctionality();

                Console.WriteLine("\n📋 執行完整測試...");
                var host = CreateHost();
                RunTests(host.Services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 錯誤: {ex.Message}");
                Console.WriteLine($"詳細: {ex}");
            }

            Console.WriteLine("\n✅ 測試完成！按任意鍵結束...");
            Console.ReadKey();
        }

        static IHost CreateHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // 註冊日誌
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.SetMinimumLevel(LogLevel.Information);
                    });

                    // 註冊查詢建構器
                    services.AddTransient<ISearchParameterBuilder, StringParameterBuilder>();
                    services.AddTransient<ISearchParameterBuilder, DateParameterBuilder>();
                    services.AddTransient<ISearchParameterBuilder, NumberParameterBuilder>();
                    services.AddTransient<ISearchParameterBuilder, TokenParameterBuilder>();
                    services.AddTransient<ISearchParameterBuilder, ReferenceParameterBuilder>();
                    services.AddTransient<ISearchParameterBuilder, QuantityParameterBuilder>();
                    services.AddTransient<ISearchParameterBuilder, UriParameterBuilder>();
                    services.AddTransient<ISearchParameterBuilder, CompositeParameterBuilder>();
                    services.AddTransient<ISearchParameterBuilder, FilterParameterBuilder>();
                    services.AddSingleton<ISearchParameterFactory, SearchParameterFactory>();

                    // 註冊驗證服務（模擬）
                    services.AddSingleton<Fhir.QueryBuilder.Services.Interfaces.IValidationService, MockValidationService>();

                    // 註冊 Fluent API
                    services.AddTransient<IFhirQueryBuilder, FhirQueryBuilder>();
                })
                .Build();
        }

        static void RunTests(IServiceProvider services)
        {
            var builder = services.GetRequiredService<IFhirQueryBuilder>();

            Console.WriteLine("\n📋 測試 1: 基本 Chaining 搜尋");
            TestChaining(builder);

            Console.WriteLine("\n📋 測試 2: Reverse Chaining 搜尋");
            TestReverseChaining(builder);

            Console.WriteLine("\n📋 測試 3: Composite 參數");
            TestComposite(builder);

            Console.WriteLine("\n📋 測試 4: Filter 參數");
            TestFilter(builder);

            Console.WriteLine("\n📋 測試 5: 新的控制參數");
            TestControlParameters(builder);

            Console.WriteLine("\n📋 測試 6: 複雜查詢組合");
            TestComplexQuery(builder);

            Console.WriteLine("\n📋 測試 7: 參數建構器工廠");
            TestParameterFactory(services);
        }

        static void TestChaining(IFhirQueryBuilder builder)
        {
            try
            {
                var query = builder
                    .ForResource("Observation")
                    .WhereString("status", "final")
                    .Chain("patient.name", "John")
                    .BuildQueryString();

                Console.WriteLine($"✅ Chaining 查詢: {query}");
                
                if (query.Contains("patient.name=John"))
                {
                    Console.WriteLine("✅ Chaining 參數正確");
                }
                else
                {
                    Console.WriteLine("❌ Chaining 參數錯誤");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Chaining 測試失敗: {ex.Message}");
            }
        }

        static void TestReverseChaining(IFhirQueryBuilder builder)
        {
            try
            {
                var query = builder
                    .ForResource("Patient")
                    .ReverseChain("Observation", "patient", "code=xyz")
                    .BuildQueryString();

                Console.WriteLine($"✅ Reverse Chaining 查詢: {query}");
                
                if (query.Contains("_has="))
                {
                    Console.WriteLine("✅ Reverse Chaining 參數正確");
                }
                else
                {
                    Console.WriteLine("❌ Reverse Chaining 參數錯誤");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Reverse Chaining 測試失敗: {ex.Message}");
            }
        }

        static void TestComposite(IFhirQueryBuilder builder)
        {
            try
            {
                var query = builder
                    .ForResource("Observation")
                    .WhereComposite("component-code-value-quantity", 
                        "http://loinc.org|8480-6", 
                        "120", 
                        "mmHg")
                    .BuildQueryString();

                Console.WriteLine($"✅ Composite 查詢: {query}");
                
                if (query.Contains("component-code-value-quantity=") && query.Contains("$"))
                {
                    Console.WriteLine("✅ Composite 參數正確");
                }
                else
                {
                    Console.WriteLine("❌ Composite 參數錯誤");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Composite 測試失敗: {ex.Message}");
            }
        }

        static void TestFilter(IFhirQueryBuilder builder)
        {
            try
            {
                var query = builder
                    .ForResource("Patient")
                    .Filter("name co 'John' and birthDate ge 1990-01-01")
                    .BuildQueryString();

                Console.WriteLine($"✅ Filter 查詢: {query}");
                
                if (query.Contains("_filter="))
                {
                    Console.WriteLine("✅ Filter 參數正確");
                }
                else
                {
                    Console.WriteLine("❌ Filter 參數錯誤");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Filter 測試失敗: {ex.Message}");
            }
        }

        static void TestControlParameters(IFhirQueryBuilder builder)
        {
            try
            {
                var query = builder
                    .ForResource("Patient")
                    .Count(50)
                    .Offset(100)
                    .Total("estimate")
                    .Contained("false")
                    .BuildQueryString();

                Console.WriteLine($"✅ 控制參數查詢: {query}");
                
                var hasCount = query.Contains("_count=50");
                var hasOffset = query.Contains("_offset=100");
                var hasTotal = query.Contains("_total=estimate");
                var hasContained = query.Contains("_contained=false");
                
                if (hasCount && hasOffset && hasTotal && hasContained)
                {
                    Console.WriteLine("✅ 所有控制參數正確");
                }
                else
                {
                    Console.WriteLine($"❌ 控制參數錯誤: Count={hasCount}, Offset={hasOffset}, Total={hasTotal}, Contained={hasContained}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 控制參數測試失敗: {ex.Message}");
            }
        }

        static void TestComplexQuery(IFhirQueryBuilder builder)
        {
            try
            {
                var query = builder
                    .ForResource("Observation")
                    .WhereString("status", "final")
                    .Chain("patient.name", "John")
                    .WhereComposite("component-code-value-quantity", "http://loinc.org|8480-6", "120")
                    .Include("Observation:patient")
                    .Filter("effectiveDateTime ge 2023-01-01")
                    .Count(50)
                    .Sort("effectiveDateTime")
                    .BuildQueryString();

                Console.WriteLine($"✅ 複雜查詢: {query}");
                
                var components = new[]
                {
                    "status=final",
                    "patient.name=John",
                    "component-code-value-quantity=",
                    "_include=",
                    "_filter=",
                    "_count=50",
                    "_sort="
                };
                
                var allPresent = components.All(c => query.Contains(c));
                
                if (allPresent)
                {
                    Console.WriteLine("✅ 複雜查詢所有組件正確");
                }
                else
                {
                    Console.WriteLine("❌ 複雜查詢缺少某些組件");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 複雜查詢測試失敗: {ex.Message}");
            }
        }

        static void TestParameterFactory(IServiceProvider services)
        {
            try
            {
                var factory = services.GetRequiredService<ISearchParameterFactory>();
                
                var compositeBuilder = factory.GetBuilder("composite");
                var filterBuilder = factory.GetBuilder("filter");
                
                Console.WriteLine($"✅ Composite Builder: {compositeBuilder?.GetType().Name ?? "null"}");
                Console.WriteLine($"✅ Filter Builder: {filterBuilder?.GetType().Name ?? "null"}");
                
                var supportedTypes = factory.GetSupportedParameterTypes();
                Console.WriteLine($"✅ 支援的參數類型: {string.Join(", ", supportedTypes)}");
                
                if (supportedTypes.Contains("composite") && supportedTypes.Contains("filter"))
                {
                    Console.WriteLine("✅ 工廠正確註冊新的建構器");
                }
                else
                {
                    Console.WriteLine("❌ 工廠缺少新的建構器");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 工廠測試失敗: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 模擬驗證服務
    /// </summary>
    public class MockValidationService : Fhir.QueryBuilder.Services.Interfaces.IValidationService
    {
        public Fhir.QueryBuilder.Services.Interfaces.ValidationResult ValidateUrl(string url)
        {
            return new Fhir.QueryBuilder.Services.Interfaces.ValidationResult { IsValid = true };
        }

        public Fhir.QueryBuilder.Services.Interfaces.ValidationResult ValidateSearchParameter(string parameterName, string parameterValue, string parameterType)
        {
            return new Fhir.QueryBuilder.Services.Interfaces.ValidationResult { IsValid = true };
        }

        public Fhir.QueryBuilder.Services.Interfaces.ValidationResult ValidateQuery(string query)
        {
            return new Fhir.QueryBuilder.Services.Interfaces.ValidationResult { IsValid = true };
        }

        public Fhir.QueryBuilder.Services.Interfaces.ValidationResult ValidateResourceType(string resourceType, IEnumerable<string>? supportedResources = null)
        {
            return new Fhir.QueryBuilder.Services.Interfaces.ValidationResult { IsValid = true };
        }

        public Fhir.QueryBuilder.Services.Interfaces.ValidationResult ValidateConnectionSettings(string serverUrl, string? token = null)
        {
            return new Fhir.QueryBuilder.Services.Interfaces.ValidationResult { IsValid = true };
        }

        public Fhir.QueryBuilder.Services.Interfaces.ValidationResult ValidateQueryComplexity(string query)
        {
            return new Fhir.QueryBuilder.Services.Interfaces.ValidationResult { IsValid = true };
        }

        public Fhir.QueryBuilder.Services.Interfaces.ValidationResult ValidateFhirVersion(string version)
        {
            return new Fhir.QueryBuilder.Services.Interfaces.ValidationResult { IsValid = true };
        }

        public Task<Fhir.QueryBuilder.Services.Interfaces.ValidationResult> ValidateServerCapabilityAsync(string serverUrl, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new Fhir.QueryBuilder.Services.Interfaces.ValidationResult { IsValid = true });
        }
    }
}
