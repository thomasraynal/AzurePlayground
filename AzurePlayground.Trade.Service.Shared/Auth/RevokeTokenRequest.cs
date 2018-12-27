using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class RevokeTokenRequestValidator : AbstractValidator<RevokeTokenRequest>
    {
        public RevokeTokenRequestValidator()
        {
            RuleFor(request => request.Token).NotEmpty().WithMessage("Token should be set");
        }
    }

    public class RevokeTokenRequest
    {
        public string Token { get; set; }
    }
}
