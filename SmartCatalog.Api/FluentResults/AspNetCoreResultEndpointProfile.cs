using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using SmartCatalog.BusinessLogic.FluentResults.Errors;
using SmartCatalog.Infrastructure.Entities;

namespace SmartCatalog.Api.FluentResults;

public sealed class AspNetCoreResultEndpointProfile : DefaultAspNetCoreResultEndpointProfile
{
    // Сюда попадаем, когда результат запроса имеет проблемы(Result.Fail) и надо вернуть соответствующую ошибку.
    public override ActionResult TransformFailedResultToActionResult(FailedResultToActionResultTransformationContext context)
    {
        var error = context.Result.Errors.FirstOrDefault();
        if (error is ConflictError)
        {
            var obj = error.Metadata.Values.FirstOrDefault();
            return obj switch
            {
                Product product => new ConflictObjectResult(product),
                _ => new ConflictObjectResult(error.Message),
            };
        }
        return error?.GetType().Name switch
        {
            nameof(NotFoundError) => new NotFoundObjectResult(error.Message),
            nameof(BadRequestError) => new BadRequestObjectResult(error.Message),
            _ => new BadRequestObjectResult("For some reason, when converting errors, you got into the \"default\" block."),
        };
    }

    // Сюда попадаем, когда результат запроса успешный. По идее, возвращать мы ничего не должны кроме статуса "200", однако, сюда мы попадаем из-за костыльного метода "WithSuccess", а точнее из-за отсутствия нужных перегрузок в методе "Result.Ok". Поэтому возвращаем объект словно мы в TransformOkValueResultToActionResult.
    public override ActionResult TransformOkNoValueResultToActionResult(OkResultToActionResultTransformationContext<Result> context)
    {
        var success = context.Result.Successes.FirstOrDefault();
        return new OkObjectResult(success?.Message);
    }

    // Сюда попадаем, когда результат запроса успешный и надо вернуть объект.
    public override ActionResult TransformOkValueResultToActionResult<T>(OkResultToActionResultTransformationContext<Result<T>> context)
    {
        return new OkObjectResult(context.Result.Value);
    }
}
