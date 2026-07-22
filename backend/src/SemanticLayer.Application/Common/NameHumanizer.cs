using System.Globalization;
using System.Text;

namespace SemanticLayer.Application.Common;

/// <summary>
/// Turns a physical identifier into a default business-friendly name,
/// e.g. "first_name" -> "First Name", "job_titles" -> "Job Titles".
/// Used only to seed defaults; user/metadata values take precedence.
/// </summary>
public static class NameHumanizer
{
    public static string Humanize(string physicalName)
    {
        if (string.IsNullOrWhiteSpace(physicalName))
            return physicalName;

        var words = physicalName
            .Replace('_', ' ')
            .Replace('-', ' ')
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var sb = new StringBuilder();
        foreach (var word in words)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(char.ToUpper(word[0], CultureInfo.InvariantCulture));
            if (word.Length > 1) sb.Append(word[1..]);
        }

        return sb.ToString();
    }
}
