namespace Fhir.QueryBuilder.QueryBuilders.FluentApi
{
    public enum SearchModifier
    {
        None,
        Exact,
        Contains,
        Missing,
        Text,
        Not,
        Above,
        Below,
        In,
        NotIn,
        Identifier
    }

    public enum SearchPrefix
    {
        None,
        Equal,      // eq
        NotEqual,   // ne
        GreaterThan,    // gt
        LessThan,       // lt
        GreaterEqual,   // ge
        LessEqual,      // le
        StartsAfter,    // sa
        EndsBefore,     // eb
        Approximate     // ap
    }

    public interface IFhirQueryBuilder
    {
        IFhirQueryBuilder ForResource(string resourceType);
        IFhirQueryBuilder ForResource<T>() where T : class;
        
        // String parameters
        IFhirQueryBuilder WhereString(string parameterName, string value, SearchModifier modifier = SearchModifier.None);
        
        // Date parameters
        IFhirQueryBuilder WhereDate(string parameterName, DateTime value, SearchPrefix prefix = SearchPrefix.None);
        IFhirQueryBuilder WhereDate(string parameterName, string value, SearchPrefix prefix = SearchPrefix.None);
        
        // Number parameters
        IFhirQueryBuilder WhereNumber(string parameterName, decimal value, SearchPrefix prefix = SearchPrefix.None);
        
        // Token parameters
        IFhirQueryBuilder WhereToken(string parameterName, string code, string? system = null);
        
        // Reference parameters
        IFhirQueryBuilder WhereReference(string parameterName, string id);
        IFhirQueryBuilder WhereReference(string parameterName, string resourceType, string id);
        IFhirQueryBuilder WhereReferenceUrl(string parameterName, string url);
        
        // Quantity parameters
        IFhirQueryBuilder WhereQuantity(string parameterName, decimal number, string? code = null, string? system = null, SearchPrefix prefix = SearchPrefix.None);
        
        // URI parameters
        IFhirQueryBuilder WhereUri(string parameterName, string uri);
        
        // Generic parameter method
        IFhirQueryBuilder Where(string parameterName, string value, SearchModifier modifier = SearchModifier.None);
        
        // Include and RevInclude
        IFhirQueryBuilder Include(string includePath);
        IFhirQueryBuilder RevInclude(string revIncludePath);

        // Chaining and Reverse Chaining
        IFhirQueryBuilder Chain(string chainPath, string value);
        IFhirQueryBuilder ReverseChain(string resourceType, string searchParam, string value);

        // Composite parameters
        IFhirQueryBuilder WhereComposite(string parameterName, params string[] components);

        // Filter parameter
        IFhirQueryBuilder Filter(string filterExpression);

        // Result modifiers
        IFhirQueryBuilder Count(int count);
        IFhirQueryBuilder Offset(int offset);
        IFhirQueryBuilder MaxResults(int maxResults);
        IFhirQueryBuilder Summary(string summaryType = "true");
        IFhirQueryBuilder Elements(params string[] elements);
        IFhirQueryBuilder Sort(params string[] sortFields);
        IFhirQueryBuilder Total(string totalMode = "accurate");
        IFhirQueryBuilder Contained(string containedMode = "true");
        
        // Build methods
        string BuildUrl(string baseUrl);
        string BuildQueryString();
        
        // Validation
        bool IsValid();
        IEnumerable<string> GetValidationErrors();
    }
}
