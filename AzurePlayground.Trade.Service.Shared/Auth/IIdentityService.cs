using Refit;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Shared
{
    public interface IIdentityService
    {
        [Post("/identity/user")]
        Task<IEnumerable<Claim>> GetUser(string token);

        [Post("/identity/login")]
        Task<UserProfile> Login(LoginRequest request);

        [Post("/identity/logout")]
        Task Logout([Header("Authorization")] string token);

        [Post("/identity/revoke")]
        Task Revoke(RevokeTokenRequest request);
    }
}