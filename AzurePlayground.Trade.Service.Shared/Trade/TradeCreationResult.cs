using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class TradeCreationResultValidator : AbstractValidator<TradeCreationResult>
    {
        public TradeCreationResultValidator()
        {
            RuleFor(request => request.TradeId).NotEqual(Guid.Empty).WithMessage("TradeId should be set");
            RuleFor(request => request.TradeStatus).NotEmpty().WithMessage("TradeStatus should be set");
            RuleFor(request => request.Reason).NotEmpty().WithMessage("Reason should be set");
        }
    }

    public class TradeCreationResult
    {
        public Guid TradeId { get; set; }

        public TradeStatus TradeStatus { get; set; }

        public String Reason { get; set; }
    }
}
