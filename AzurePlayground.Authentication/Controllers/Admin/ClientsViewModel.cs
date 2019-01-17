using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientsViewModel
    {
        public List<ClientViewModel> Clients { get; set; }
    }

    public class ClientViewModel
    {
        public ClientViewModel()
        {
            Grants = new List<ClientGrantViewModel>();
        }

        public Client Client { get; set; }
        public List<ClientGrantViewModel> Grants { get; set; }
    }

    public class ClientGrantViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string SubjectId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Expires { get; set; }
    }
}
