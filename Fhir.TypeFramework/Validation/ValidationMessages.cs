namespace Fhir.TypeFramework.Validation;

public static class ValidationMessages
{
    public static class PrimitiveTypes
    {
        public const string StringTooLong = "String value exceeds maximum length of {0} characters";
        public const string StringTooLargeBytes = "String value exceeds maximum byte size of {0} bytes (UTF-8)";
        public const string InvalidId = "Invalid FHIR ID format (must be 1-64 characters, only letters, digits, hyphens and dots)";
        public const string InvalidUri = "Invalid FHIR URI format";
        public const string MustBePositiveInteger = "Value must be a positive integer";
    }

    public static class Formatters
    {
        public static string StringTooLong(int maxLength) => string.Format(PrimitiveTypes.StringTooLong, maxLength);
        public static string StringTooLargeBytes(int maxBytes) => string.Format(PrimitiveTypes.StringTooLargeBytes, maxBytes);
    }
}

