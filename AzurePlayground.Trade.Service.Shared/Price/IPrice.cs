using Dasein.Core.Lite.Shared;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class PriceValidator : AbstractValidator<IPrice>
    {
        public PriceValidator()
        {
            RuleFor(request => request.Id).NotEmpty().NotEqual(Guid.Empty).WithMessage("Id should be set");
            RuleFor(request => request.Asset).NotEmpty().WithMessage("Asset should be set");
            RuleFor(request => request.Value).NotEmpty().WithMessage("Value should be set");
        }
    }

    public interface IPrice
    {
        Guid Id { get; set; }
        DateTime Date { get; set; }
        String Asset { get; set; }
        double Value { get; set; }
    }
}
