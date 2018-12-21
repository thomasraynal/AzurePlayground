using System.Threading.Tasks;

namespace AzurePlayground.Service.Shared
{
    public interface IPublisher
    {
        Task Start();
        Task Stop();
    }
}