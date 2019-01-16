using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace AzurePlayground.Persistence.Mongo
{
    public interface IIdentityUserCollection<TUser> where TUser : MongoUser
    {
        Task<TUser> FindByEmailAsync(string normalizedEmail);
        Task<TUser> FindByUserNameAsync(string username);
        Task<TUser> FindByNormalizedUserNameAsync(string normalizedUserName);
        Task<TUser> FindByLoginAsync(string loginProvider, string providerKey);
        Task<IEnumerable<TUser>> FindUsersByClaimAsync(string claimType, string claimValue);
        Task<IEnumerable<TUser>> FindUsersInRoleAsync(string roleName);
        Task<IEnumerable<TUser>> GetAllAsync();
        Task<TUser> CreateAsync(TUser obj);
        Task UpdateAsync(TUser obj);
        Task DeleteAsync(TUser obj);
        Task<TUser> FindByIdAsync(string itemId);
    }
}
