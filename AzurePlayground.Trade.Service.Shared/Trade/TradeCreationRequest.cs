using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AzurePlayground.Service.Shared
{

    public class TradeCreationRequestValidator : AbstractValidator<TradeCreationRequest>
    {
        public TradeCreationRequestValidator()
        {
            RuleFor(request => request.Asset).NotEmpty().WithMessage("Asset should be set");
            RuleFor(request => request.Way).NotEmpty().WithMessage("Way should be set");
            RuleFor(request => request.Volume).NotEmpty().WithMessage("Volume should be set");
            RuleFor(request => request.Currency).NotEmpty().WithMessage("Currency should be set");
        }
    }

    public class TradeCreationRequest
    {
        public string Asset { get; set; }
        public TradeWay Way { get; set; }
        public string Currency { get; set; }
        public double Volume { get; set; }
        public string Trader { get; set; }
    }
}
