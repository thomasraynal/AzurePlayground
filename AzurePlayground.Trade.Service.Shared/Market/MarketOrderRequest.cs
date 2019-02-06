using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class MarketOrderRequestValidator : AbstractValidator<MarketOrderRequest>
    {
        public MarketOrderRequestValidator()
        {
            RuleFor(request => request.TradeId).NotEqual(Guid.Empty).NotEmpty().WithMessage("TradeId should be set");
            RuleFor(request => request.Asset).NotEmpty().WithMessage("Asset should be set");
            RuleFor(request => request.Volume).NotEmpty().WithMessage("Volume should be set");
            RuleFor(request => request.Way).NotEmpty().WithMessage("Way should be set");
        }
    }

    public class MarketOrderRequest
    {
        public Guid TradeId { get; set; }
        public TradeWay Way { get; set; }
        public String Asset { get; }
        public double Volume { get; }
    }
}
