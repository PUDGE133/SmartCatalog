using FluentResults;

namespace SmartCatalog.BusinessLogic.FluentResults.Errors;

public class ConflictError : Error
{
    public ConflictError(string message)
    {
        Message = message;
    }
}
