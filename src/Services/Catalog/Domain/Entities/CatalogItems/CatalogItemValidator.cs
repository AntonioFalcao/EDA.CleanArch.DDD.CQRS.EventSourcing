using Domain.Abstractions.Validators;
using FluentValidation;

namespace Domain.Entities.CatalogItems;

public class CatalogItemValidator : EntityValidator<CatalogItem, Guid>
{
    public CatalogItemValidator()
    {
        RuleFor(item => item.Quantity)
            .GreaterThan(0);
    }
}