namespace Fhir.QueryBuilder.QueryCore.Validation;

/// <summary>查詢建構／驗證結果（與 UI 層 <see cref="Services.Interfaces.ValidationResult"/> 區分）。</summary>
public sealed class QueryValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<ValidationIssue> Errors { get; } = new();

    public List<ValidationIssue> Warnings { get; } = new();

    public void AddError(string message) => Errors.Add(new ValidationIssue { Message = message });

    public void AddWarning(string message) => Warnings.Add(new ValidationIssue { Message = message });
}

public sealed class ValidationIssue
{
    public string Message { get; set; } = string.Empty;
}
