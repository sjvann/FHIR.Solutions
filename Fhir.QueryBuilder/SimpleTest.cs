using System;

namespace Fhir.QueryBuilder
{
    public class SimpleTest
    {
        public static void TestBasicFunctionality()
        {
            Console.WriteLine("🚀 簡單功能測試");
            
            try
            {
                // 測試 Composite 參數擴展方法
                var components = Fhir.QueryBuilder.QueryBuilders.CompositeParameterExtensions
                    .CreateTokenQuantityComponents("http://loinc.org", "8480-6", 120, "mmHg");
                
                Console.WriteLine($"✅ Composite 組件: [{string.Join(", ", components)}]");
                
                // 測試 Filter 參數擴展方法
                var equalFilter = Fhir.QueryBuilder.QueryBuilders.FilterParameterExtensions
                    .CreateEqualFilter("name", "John");
                
                Console.WriteLine($"✅ Equal Filter: {equalFilter}");
                
                var rangeFilter = Fhir.QueryBuilder.QueryBuilders.FilterParameterExtensions
                    .CreateRangeFilter("birthDate", "1990-01-01", "2000-12-31");
                
                Console.WriteLine($"✅ Range Filter: {rangeFilter}");
                
                Console.WriteLine("✅ 所有基本功能測試通過！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 測試失敗: {ex.Message}");
                Console.WriteLine($"詳細: {ex}");
            }
        }
    }
}
