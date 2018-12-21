using Dasein.Core.Lite.Shared;
using Refit;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Shared
{
    public interface IUserService
    {
        [Post("/api/v1/auth")]
        Task<TradeServiceToken> Login(Credentials credentials);
    }
}