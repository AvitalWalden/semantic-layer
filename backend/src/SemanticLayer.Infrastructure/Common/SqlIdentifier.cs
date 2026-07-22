using System.Text.RegularExpressions;

namespace SemanticLayer.Infrastructure.Common;

/// <summary>
/// Guards and quotes SQL identifiers. Identifiers used to build dynamic queries
/// originate from the semantic store (populated by introspection), but they are
/// still validated against a strict whitelist and quoted as a defense-in-depth
/// measure against SQL injection.
/// </summary>
public static partial class SqlIdentifier
{
    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_]*$")]
    private static partial Regex IdentifierRegex();

    public static bool IsValid(string? identifier) =>
        !string.IsNullOrEmpty(identifier) && IdentifierRegex().IsMatch(identifier);

    /// <summary>Validates and double-quotes an identifier for use in SQL.</summary>
    public static string Quote(string identifier)
    {
        if (!IsValid(identifier))
            throw new ArgumentException($"Invalid SQL identifier: '{identifier}'.", nameof(identifier));
        return $"\"{identifier}\"";
    }
}
