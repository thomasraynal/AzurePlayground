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
            RuleFor(request => request.Date).NotEqual(DateTime.MinValue).WithMessage("Date should be set");
            RuleFor(request => request.Counterparty).NotEmpty().WithMessage("Counterparty should be set");
            RuleFor(request => request.Asset).NotEmpty().WithMessage("Asset should be set");
            RuleFor(request => request.Status).NotEmpty().WithMessage("Status should be set");
            RuleFor(request => request.Way).NotEmpty().WithMessage("Way should be set");
            RuleFor(request => request.PriceOnTransaction).NotEmpty().WithMessage("PriceOnTransaction should be set");
            RuleFor(request => request.Volume).NotEmpty().WithMessage("Volume should be set");
        }
    }

    public interface ITrade
    {
        Guid Id { get; }
        DateTime Date { get; }
        String Counterparty { get; }
        String Asset { get; }
        TradeStatus Status { get; set; }
        TradeWay Way { get; }
        double PriceOnTransaction { get; }
        double Volume { get; }
    }
}
