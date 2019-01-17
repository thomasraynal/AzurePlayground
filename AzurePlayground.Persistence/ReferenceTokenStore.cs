//using IdentityServer4.Models;
//using IdentityServer4.Stores;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace AzurePlayground.Persistence
//{
//    public class ReferenceTokenStore : IReferenceTokenStore
//    {
//        protected IRepository _dbRepository;

//        public ReferenceTokenStore(IRepository repository)
//        {
//            _dbRepository = repository;
//        }

//        /// <summary>
//        /// Stores the reference token asynchronous.
//        /// </summary>
//        /// <param name="token">The token.</param>
//        /// <returns></returns>
//        public Task<string> StoreReferenceTokenAsync(Token token)
//        {
//            return CreateItemAsync(token, token.ClientId, token.SubjectId, token.CreationTime, token.Lifetime);
//        }

//        /// <summary>
//        /// Gets the reference token asynchronous.
//        /// </summary>
//        /// <param name="handle">The handle.</param>
//        /// <returns></returns>
//        public Task<Token> GetReferenceTokenAsync(string handle)
//        {
//            return GetItemAsync(handle);
//        }

//        public Task RemoveReferenceTokenAsync(string handle)
//        {
//            return RemoveItemAsync(handle);
//        }

//        public Task RemoveReferenceTokensAsync(string subjectId, string clientId)
//        {
//            return RemoveAllAsync(subjectId, clientId);
//        }
//    }
//}
//}
