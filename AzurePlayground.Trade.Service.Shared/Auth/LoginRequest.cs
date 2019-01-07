using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public String Username { get; set; }
        [Required]
        public String Password { get; set; }
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
    }
}
