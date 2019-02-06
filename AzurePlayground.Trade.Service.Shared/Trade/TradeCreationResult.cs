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
        }
    }

    public class TradeCreationResult
    {
        public Guid TradeId { get; set; }
    }
}
