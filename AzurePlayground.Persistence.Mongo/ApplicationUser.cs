using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Persistence.Mongo
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public ApplicationUser() : base()
        {
        }

        public ApplicationUser(string userName) : base(userName)
        {
        }
    }
}
