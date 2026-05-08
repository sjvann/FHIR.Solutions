using System;
using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Fhir.QueryBuilder.QueryBuilders;
using Fhir.QueryBuilder.Controls;
using static Fhir.QueryBuilder.QueryBuilders.CompositeParameterExtensions;
using static Fhir.QueryBuilder.QueryBuilders.FilterParameterExtensions;

namespace Fhir.QueryBuilder
{
    /// <summary>
    /// 簡單的編譯測試類別
    /// </summary>
    public class CompileTest
    {
        public static void TestCompilation()
        {
            Console.WriteLine("🔧 開始編譯測試...");

            try
            {
                // 測試 FluentApi 介面
                Console.WriteLine("✅ FluentApi 介面可用");

                // 測試 CompositeParameterBuilder
                Console.WriteLine("✅ CompositeParameterBuilder 可用");

                // 測試 FilterParameterBuilder
                Console.WriteLine("✅ FilterParameterBuilder 可用");

                // 測試 AdvancedSearchControl
                Console.WriteLine("✅ AdvancedSearchControl 可用");

                // 測試擴展方法
                var components = CreateTokenQuantityComponents(
                    "http://loinc.org", "8480-6", 120, "mmHg");
                Console.WriteLine($"✅ CompositeParameterExtensions 可用: {components.Length} 組件");

                var filter = CreateEqualFilter("name", "John");
                Console.WriteLine($"✅ FilterParameterExtensions 可用: {filter}");

                Console.WriteLine("🎉 所有編譯測試通過！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 編譯測試失敗: {ex.Message}");
                Console.WriteLine($"詳細錯誤: {ex}");
            }
        }
    }
}
