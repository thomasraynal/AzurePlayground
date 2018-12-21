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
            RuleFor(request => request.Counterparty).NotEmpty().WithMessage("Counterparty should be set");
            RuleFor(request => request.Asset).NotEmpty().WithMessage("Asset should be set");
            RuleFor(request => request.Way).NotEmpty().WithMessage("Way should be set");
            RuleFor(request => request.Volume).NotEmpty().WithMessage("Volume should be set");
            RuleFor(request => request.Price).NotEmpty().WithMessage("Price should be set");
        }
    }

    public class TradeCreationRequest
    {

        public string Counterparty { get; set; }

        public string Asset { get; set; }

        public TradeWay Way { get; set; }

        public double Price { get; set; }

        public double Volume { get; set; }

    }
}
