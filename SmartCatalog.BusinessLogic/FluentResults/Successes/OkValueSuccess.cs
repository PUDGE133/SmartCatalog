using FluentResults;

namespace SmartCatalog.BusinessLogic.FluentResults.Successes;

public class OkValueSuccess : Success
{
    public OkValueSuccess(string message)
    {
        Message = message;
    }
}
