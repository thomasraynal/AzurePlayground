using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Authentication
{
    public class TestResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //private TestUserService _service;

        //public TestResourceOwnerPasswordValidator(TestUserService service)
        //{
        //    _service = service;
        //}

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //var user = _service.Users.FirstOrDefault(u => u.Password == context.Password && u.Username == context.UserName);

            //if (null != user)
            //{
            //    context.Result = new GrantValidationResult(
            //                      subject: user.SubjectId,
            //                      authenticationMethod: "custom",
            //                      claims: user.Claims);
            //}
            //else
            //{
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid user or password");
            //}

            return Task.CompletedTask;

        }
    }
}
