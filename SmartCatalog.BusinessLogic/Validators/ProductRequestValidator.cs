using FluentValidation;
using SmartCatalog.Contracts.Requests;

namespace SmartCatalog.BusinessLogic.Validators;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(pr => pr.Name)
            .MaximumLength(150)
            .WithMessage($"{nameof(ProductRequest.Name)} cannot be longer than 150 characters.");
        RuleFor(pr => pr.Description)
            .MaximumLength(150)
            .WithMessage($"{nameof(ProductRequest.Description)} cannot be longer than 150 characters.");
        RuleFor(pr => pr.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage($"{nameof(ProductRequest.Price)} cannot be less than \"0\".");
        RuleFor(pr => pr.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage($"{nameof(ProductRequest.Stock)} cannot be less than \"0\".");
    }
}