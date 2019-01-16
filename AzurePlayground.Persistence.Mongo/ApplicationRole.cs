using AspNetCore.Identity.MongoDbCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Persistence.Mongo
{
    public class ApplicationRole : MongoIdentityRole<Guid>
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }
}
