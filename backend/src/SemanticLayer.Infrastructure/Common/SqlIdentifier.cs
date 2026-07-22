using System.Text.RegularExpressions;

namespace SemanticLayer.Infrastructure.Common;

public static partial class SqlIdentifier
{
    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_]*$")]
    private static partial Regex IdentifierRegex();

    public static bool IsValid(string? identifier) =>
        !string.IsNullOrEmpty(identifier) && IdentifierRegex().IsMatch(identifier);

    public static string Quote(string identifier)
    {
        if (!IsValid(identifier))
            throw new ArgumentException($"Invalid SQL identifier: '{identifier}'.", nameof(identifier));
        return $"\"{identifier}\"";
    }
}
