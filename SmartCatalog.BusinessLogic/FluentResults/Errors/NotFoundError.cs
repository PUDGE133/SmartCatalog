using FluentResults;

namespace SmartCatalog.BusinessLogic.FluentResults.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string message)
    {
        Message = message;
    }
}
