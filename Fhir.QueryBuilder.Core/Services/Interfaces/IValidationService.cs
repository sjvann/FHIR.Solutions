namespace Fhir.QueryBuilder.Services.Interfaces
{
    public class ValidationResult
    {
        /// <summary>預設為成功；加入任何 <see cref="Errors"/> 後應設為 false。</summary>
        public bool IsValid { get; set; } = true;

        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();

        public void AddError(string error)
        {
            Errors.Add(error);
            IsValid = false;
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }
    }

    public interface IValidationService
    {
        ValidationResult ValidateUrl(string url);
        ValidationResult ValidateSearchParameter(string parameterName, string parameterValue, string parameterType);
        ValidationResult ValidateQuery(string query);
        ValidationResult ValidateResourceType(string resourceType, IEnumerable<string>? supportedResources = null);
        ValidationResult ValidateConnectionSettings(string serverUrl, string? token = null);
        ValidationResult ValidateQueryComplexity(string query);
        ValidationResult ValidateFhirVersion(string version);
        Task<ValidationResult> ValidateServerCapabilityAsync(string serverUrl, CancellationToken cancellationToken = default);
    }
}
