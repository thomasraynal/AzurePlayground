using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class TradeValidator : AbstractValidator<ITrade>
    {
        public TradeValidator()
        {
            RuleFor(request => request.Id).NotEqual(Guid.Empty).NotEmpty().WithMessage("Id should be set");
            RuleFor(request => request.Asset).NotEmpty().WithMessage("Asset should be set");
            RuleFor(request => request.Status).NotEmpty().WithMessage("Status should be set");
            RuleFor(request => request.Way).NotEmpty().WithMessage("Way should be set");
            RuleFor(request => request.Volume).NotEmpty().WithMessage("Volume should be set");
            RuleFor(request => request.Currency).NotEmpty().WithMessage("Currency should be set");
        }
    }

    public interface ITrade
    {
        Guid Id { get; set; }
        DateTime Date { get; set; }
        String Marketplace { get; set; }
        String Counterparty { get; set; }
        String Asset { get; set; }
        TradeStatus Status { get; set; }
        TradeWay Way { get; set; }
        double Price { get; set; }
        String Currency { get; set; }
        double Volume { get; set; }

         

        String TradeService { get; set; }
        String ComplianceService { get; set; }
    }
}
