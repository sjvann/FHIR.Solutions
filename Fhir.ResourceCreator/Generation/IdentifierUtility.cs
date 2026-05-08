namespace FhirResourceCreator.Generation;

public static class IdentifierUtility
{
    public static string ToSafeClrIdentifier(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "_";

        var chars = name.ToCharArray();
        if (!char.IsLetter(chars[0]) && chars[0] != '_')
            return "_" + name;

        return name;
    }

    public static string ToPascalCase(string segment)
    {
        if (string.IsNullOrEmpty(segment))
            return "";

        if (segment.Length == 1)
            return segment.ToUpperInvariant();

        return char.ToUpperInvariant(segment[0]) + segment[1..];
    }
}
