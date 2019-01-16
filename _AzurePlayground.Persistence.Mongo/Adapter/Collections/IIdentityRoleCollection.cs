using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Persistence.Mongo
{
    public interface IIdentityRoleCollection<TRole> where TRole : MongoRole
    {
        Task<TRole> FindByNameAsync(string normalizedName);
        Task<TRole> FindByIdAsync(string roleId);
        Task<IEnumerable<TRole>> GetAllAsync();
        Task<TRole> CreateAsync(TRole obj);
        Task UpdateAsync(TRole obj);
        Task DeleteAsync(TRole obj);
    }
}
