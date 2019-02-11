using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{

    public class Price : IPrice
    {
        public Price()
        {
        }

        public Price(Guid id, string asset, double value, DateTime date)
        {
            Id = id;
            Asset = asset;
            Value = value;
            Date = date;
        }

        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Asset { get; set; }

        public double Value { get; set; }

        public override string ToString()
        {
            return $"{Asset} - {Date.ToShortDateString()} - {Value}";
        }
    }
}
