using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class Asset
    {
        public Asset(string name, double price)
        {
            this.Name = name;
            this.Price = price;
        }

        public string Name { get; set; }

        public double Price { get; set; }
    }
}
