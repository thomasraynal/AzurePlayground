using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class PriceCreationRequestValidator : AbstractValidator<PriceCreationRequest>
    {
        public PriceCreationRequestValidator()
        {
            RuleFor(request => request.Asset).NotEmpty().WithMessage("Asset should be set");
            RuleFor(request => request.Date).NotEmpty().WithMessage("Date should be set");
            RuleFor(request => request.Value).NotEmpty().WithMessage("Value should be set");
        }
    }

    public class PriceCreationRequest
    {
        public PriceCreationRequest(string asset, double value, DateTime date)
        {
            Asset = asset;
            Value = value;
            Date = date;
        }

        public DateTime Date { get; set; }
        public string Asset { get; set; }
        public double Value { get; set; }
    }
}
