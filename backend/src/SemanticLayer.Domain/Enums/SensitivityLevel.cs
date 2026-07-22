namespace SemanticLayer.Domain.Enums;

/// <summary>
/// Data classification for a field. Sourced from the external metadata file;
/// this attribute does not exist in the physical database.
/// </summary>
public enum SensitivityLevel
{
    Public = 0,
    Internal = 1,
    Confidential = 2,
    Restricted = 3
}
