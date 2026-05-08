using Fhir.QueryBuilder.QueryBuilders;
using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.Examples
{
    /// <summary>
    /// 進階搜尋功能使用範例
    /// </summary>
    public static class AdvancedSearchExamples
    {
        /// <summary>
        /// 範例 1：使用 Chaining 搜尋
        /// 搜尋特定醫院的患者的觀察記錄
        /// </summary>
        public static string Example1_ChainSearch(IFhirQueryBuilder builder)
        {
            return builder
                .ForResource("Observation")
                .WhereString("status", "final")
                .Chain("patient.organization.name", "General Hospital")
                .Chain("patient.name", "John")
                .Count(50)
                .Sort("effectiveDateTime")
                .BuildQueryString();
            
            // 結果: status=final&patient.organization.name=General%20Hospital&patient.name=John&_count=50&_sort=effectiveDateTime
        }

        /// <summary>
        /// 範例 2：使用 Reverse Chaining 搜尋
        /// 搜尋有特定檢驗結果的患者
        /// </summary>
        public static string Example2_ReverseChainSearch(IFhirQueryBuilder builder)
        {
            return builder
                .ForResource("Patient")
                .WhereString("active", "true")
                .ReverseChain("Observation", "patient", "code=http://loinc.org|8480-6")
                .ReverseChain("DiagnosticReport", "subject", "status=final")
                .Include("Patient:organization")
                .BuildQueryString();
            
            // 結果: active=true&_has=Observation%3Apatient%3Acode%3Dhttp%3A%2F%2Floinc.org%7C8480-6&_has=DiagnosticReport%3Asubject%3Astatus%3Dfinal&_include=Patient%3Aorganization
        }

        /// <summary>
        /// 範例 3：使用 Composite 參數搜尋
        /// 搜尋特定代碼和數值範圍的觀察記錄
        /// </summary>
        public static string Example3_CompositeSearch(IFhirQueryBuilder builder)
        {
            // 使用擴展方法建立組件
            var systolicBPComponents = CompositeParameterExtensions.CreateTokenQuantityComponents(
                "http://loinc.org", "8480-6", 120, "mmHg", "http://unitsofmeasure.org");
            
            var diastolicBPComponents = CompositeParameterExtensions.CreateTokenQuantityComponents(
                "http://loinc.org", "8462-4", 80, "mmHg", "http://unitsofmeasure.org");

            return builder
                .ForResource("Observation")
                .WhereString("status", "final")
                .WhereComposite("component-code-value-quantity", systolicBPComponents)
                .WhereComposite("component-code-value-quantity", diastolicBPComponents)
                .WhereDate("effectiveDateTime", "2023-01-01", SearchPrefix.GreaterEqual)
                .Include("Observation:patient")
                .Sort("effectiveDateTime")
                .BuildQueryString();
        }

        /// <summary>
        /// 範例 4：使用 _filter 參數進行複雜搜尋
        /// 使用 _filter 進行複雜的條件組合
        /// </summary>
        public static string Example4_FilterSearch(IFhirQueryBuilder builder)
        {
            // 建立複雜的 filter 表達式
            var nameFilter = FilterParameterExtensions.CreateContainsFilter("name.family", "Smith");
            var ageFilter = FilterParameterExtensions.CreateRangeFilter("birthDate", "1980-01-01", "2000-12-31");
            var statusFilter = FilterParameterExtensions.CreateEqualFilter("active", "true");
            
            var combinedFilter = FilterParameterExtensions.CreateAndFilter(
                nameFilter,
                ageFilter,
                statusFilter
            );

            return builder
                .ForResource("Patient")
                .Filter(combinedFilter)
                .Count(100)
                .Sort("name.family", "name.given")
                .Summary("true")
                .Elements("id", "name", "birthDate", "active")
                .BuildQueryString();
        }

        /// <summary>
        /// 範例 5：完整的進階搜尋
        /// 結合所有進階功能的複雜查詢
        /// </summary>
        public static string Example5_ComplexAdvancedSearch(IFhirQueryBuilder builder)
        {
            // 建立 Composite 組件
            var vitalSignComponents = CompositeParameterExtensions.CreateTokenQuantityComponents(
                "http://loinc.org", "8310-5", 37, "Cel", "http://unitsofmeasure.org");

            // 建立 Filter 表達式
            var dateFilter = FilterParameterExtensions.CreateRangeFilter(
                "effectiveDateTime", "2023-01-01", "2023-12-31");
            var statusFilter = FilterParameterExtensions.CreateEqualFilter("status", "final");
            var combinedFilter = FilterParameterExtensions.CreateAndFilter(dateFilter, statusFilter);

            return builder
                .ForResource("Observation")
                
                // 基本搜尋條件
                .WhereString("category", "vital-signs")
                .WhereToken("code", "8310-5", "http://loinc.org")
                
                // Chaining 搜尋
                .Chain("patient.name", "John")
                .Chain("patient.organization.type", "hospital")
                
                // Composite 搜尋
                .WhereComposite("component-code-value-quantity", vitalSignComponents)
                
                // Filter 搜尋
                .Filter(combinedFilter)
                
                // Include 相關資源
                .Include("Observation:patient")
                .Include("Observation:encounter")
                .RevInclude("DiagnosticReport:result")
                
                // 結果控制
                .Count(50)
                .Offset(0)
                .Sort("effectiveDateTime", "-valueQuantity.value")
                .Total("accurate")
                .Summary("false")
                .Elements("id", "status", "code", "valueQuantity", "effectiveDateTime", "patient")
                
                .BuildQueryString();
        }

        /// <summary>
        /// 範例 6：醫療場景 - 搜尋糖尿病患者的血糖監測記錄
        /// </summary>
        public static string Example6_DiabetesMonitoring(IFhirQueryBuilder builder)
        {
            // 血糖檢測代碼和正常範圍
            var glucoseComponents = CompositeParameterExtensions.CreateTokenQuantityComponents(
                "http://loinc.org", "33747-0", 100, "mg/dL");

            // 患者條件：有糖尿病診斷
            var diabetesFilter = FilterParameterExtensions.CreateContainsFilter(
                "code.coding.code", "E11"); // ICD-10 糖尿病代碼

            return builder
                .ForResource("Observation")
                .WhereToken("code", "33747-0", "http://loinc.org") // 血糖檢測
                .Chain("patient.condition.code", "E11") // 患者有糖尿病
                .WhereDate("effectiveDateTime", "2023-01-01", SearchPrefix.GreaterEqual)
                .Filter("valueQuantity.value gt 126") // 血糖值 > 126 mg/dL
                .Include("Observation:patient")
                .Include("Observation:encounter")
                .RevInclude("MedicationRequest:patient")
                .Count(100)
                .Sort("-effectiveDateTime")
                .BuildQueryString();
        }

        /// <summary>
        /// 範例 7：藥物相互作用檢查
        /// </summary>
        public static string Example7_DrugInteractionCheck(IFhirQueryBuilder builder)
        {
            // 搜尋同時使用特定藥物的患者
            var warfarinFilter = FilterParameterExtensions.CreateEqualFilter(
                "medicationCodeableConcept.coding.code", "11289");
            var aspirinFilter = FilterParameterExtensions.CreateEqualFilter(
                "medicationCodeableConcept.coding.code", "1191");
            
            var drugCombinationFilter = FilterParameterExtensions.CreateOrFilter(
                warfarinFilter, aspirinFilter);

            return builder
                .ForResource("MedicationRequest")
                .WhereString("status", "active")
                .Filter(drugCombinationFilter)
                .Chain("patient.age", "ge65")
                .Include("MedicationRequest:patient")
                .Include("MedicationRequest:encounter")
                .RevInclude("Observation:patient")
                .Count(50)
                .Sort("authoredOn")
                .Elements("id", "status", "medicationCodeableConcept", "patient", "authoredOn")
                .BuildQueryString();
        }

        /// <summary>
        /// 範例 8：品質指標監控
        /// </summary>
        public static string Example8_QualityMetrics(IFhirQueryBuilder builder)
        {
            // HbA1c 檢測結果
            var hba1cComponents = CompositeParameterExtensions.CreateTokenQuantityComponents(
                "http://loinc.org", "4548-4", 7, "%");

            // 時間範圍過濾
            var timeFilter = FilterParameterExtensions.CreateRangeFilter(
                "effectiveDateTime", "2023-01-01", "2023-12-31");
            
            // 結果範圍過濾
            var resultFilter = FilterParameterExtensions.CreateRangeFilter(
                "valueQuantity.value", "7", "10");
            
            var qualityFilter = FilterParameterExtensions.CreateAndFilter(
                timeFilter, resultFilter);

            return builder
                .ForResource("Observation")
                .WhereToken("code", "4548-4", "http://loinc.org") // HbA1c
                .WhereString("status", "final")
                .Chain("patient.condition.code", "E11") // 糖尿病患者
                .Filter(qualityFilter)
                .Include("Observation:patient")
                .RevInclude("CarePlan:patient")
                .Count(200)
                .Sort("effectiveDateTime")
                .Total("accurate")
                .Summary("data")
                .BuildQueryString();
        }

        /// <summary>
        /// 範例 9：建立完整的 URL
        /// </summary>
        public static string Example9_CompleteUrl(IFhirQueryBuilder builder, string baseUrl)
        {
            var query = builder
                .ForResource("Patient")
                .WhereString("name", "John")
                .WhereString("active", "true")
                .Chain("organization.name", "General Hospital")
                .Include("Patient:organization")
                .Count(20)
                .Sort("name.family");

            return query.BuildUrl(baseUrl);
            // 結果: https://hapi.fhir.org/baseR4/Patient?name=John&active=true&organization.name=General%20Hospital&_include=Patient%3Aorganization&_count=20&_sort=name.family
        }

        /// <summary>
        /// 範例 10：錯誤處理和驗證
        /// </summary>
        public static (string query, bool isValid, IEnumerable<string> errors) Example10_ValidationExample(IFhirQueryBuilder builder)
        {
            var query = builder
                .WhereString("name", "John") // 忘記設定資源類型
                .Count(-5) // 無效的 count 值
                .BuildQueryString();

            var isValid = builder.IsValid();
            var errors = builder.GetValidationErrors();

            return (query, isValid, errors);
        }
    }
}
