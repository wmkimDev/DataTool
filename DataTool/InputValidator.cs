using System.Collections.Generic;

namespace DataTool;

public class ValidationResult
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }

    private ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new ValidationResult(true);
    public static ValidationResult Error(string message) => new ValidationResult(false, message);
}

public class InputValidator
{
    private readonly Dictionary<string, string> _requiredFields = new();
    private readonly Dictionary<string, (bool isEnabled, string value)> _conditionalFields = new();

    public InputValidator AddRequiredField(string fieldName, string value)
    {
        _requiredFields[fieldName] = value;
        return this;
    }

    public InputValidator AddConditionalField(string fieldName, bool isEnabled, string value)
    {
        _conditionalFields[fieldName] = (isEnabled, value);
        return this;
    }

    public ValidationResult Validate()
    {
        foreach (var field in _requiredFields)
        {
            if (string.IsNullOrWhiteSpace(field.Value))
            {
                var josa = DetermineJosa(field.Key);
                return ValidationResult.Error($"{field.Key}{josa} 비어있습니다.");
            }
        }

        foreach (var field in _conditionalFields)
        {
            var (isEnabled, value) = field.Value;
            if (isEnabled && string.IsNullOrWhiteSpace(value))
            {
                var josa = DetermineJosa(field.Key);
                return ValidationResult.Error($"{field.Key}{josa} 비어있습니다.");
            }
        }

        return ValidationResult.Success();
    }
    
    private static string DetermineJosa(string text)
    {
        if (string.IsNullOrEmpty(text)) return "이";
        
        char lastChar = text[^1];
        if (char.GetUnicodeCategory(lastChar) != System.Globalization.UnicodeCategory.OtherLetter)
        {
            return "이"; // Default for non-Korean characters
        }

        // Determine particle based on final consonant (받침)
        int unicode = lastChar - 0xAC00;                // Starting point of Hangul in Unicode
        if (unicode < 0 || unicode > 11171) return "이"; // Check if within Hangul range
        
        return (unicode % 28 == 0) ? "가" : "이";       // Check for 받침
    }
}