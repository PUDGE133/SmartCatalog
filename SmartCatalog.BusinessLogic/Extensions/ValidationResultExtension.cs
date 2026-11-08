using FluentValidation.Results;

namespace SmartCatalog.BusinessLogic.Extensions;

public static class ValidationResultExtension
{
    public static string GetFailReport(this ValidationResult result)
    {
        return string.Join("\n", result.GetFailMessages());
    }

    public static IEnumerable<string> GetFailMessages(this ValidationResult result)
    {
        return result.Errors.Select(vf => $"Property {vf.PropertyName} failed validation. Error: \"{vf.ErrorMessage}\"");
    }
}
