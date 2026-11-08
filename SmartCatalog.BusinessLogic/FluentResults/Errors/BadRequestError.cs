using FluentResults;

namespace SmartCatalog.BusinessLogic.FluentResults.Errors;

public class BadRequestError : Error
{
    public BadRequestError(string message)
    {
        Message = message;
    }
}
