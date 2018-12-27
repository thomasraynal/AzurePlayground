using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(request => request.Username).NotEmpty().WithMessage("Username should be set");
            RuleFor(request => request.Password).NotEmpty().WithMessage("Password should be set");
        }
    }

    public class LoginRequest
    {
        public String Username { get; set; }
        public String Password { get; set; }
    }
}
