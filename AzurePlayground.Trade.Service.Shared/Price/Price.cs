using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{

    public class Price : IPrice
    {
        private readonly Guid _id;
        private readonly DateTime _date;
        private readonly string _asset;
        private readonly double _value;

        public Price(Guid id, string asset, double value, DateTime date)
        {
            _id = id;
            _date = date;
            _asset = asset;
            _value = value;
        }

        public Guid Id => _id;

        public DateTime Date => _date;

        public string Asset => _asset;

        public double Value => _value;

        public IEnumerable<string> GetCacheInvalidationTags()
        {
            yield return _asset;
        }

        public int GetCacheKey()
        {
            return _asset.GetHashCode();
        }
    }
}
