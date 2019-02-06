using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Shared
{
    public class PriceDto : IPrice
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Asset { get; set; }

        public double Value { get; set; }

        public IEnumerable<string> GetCacheInvalidationTags()
        {
            throw new NotImplementedException();
        }

        public int GetCacheKey()
        {
            throw new NotImplementedException();
        }
    }

    public class PriceConverter : JsonConverter
    {
        public JsonSerializerSettings Settings
        {
            get
            {
                return AppCore.Instance.Get<JsonSerializerSettings>();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPrice);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JObject.Load(reader);
            var dto = JsonConvert.DeserializeObject<PriceDto>(token.ToString(), Settings);
            return new Price(dto.Id, dto.Asset, dto.Value, dto.Date);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    public class TradeServiceJsonSerializer : ServiceJsonSerializerSettings
    {
        public TradeServiceJsonSerializer()
        {
            Converters.Add(new AbstractConverter<ITrade,Trade>());
            Converters.Add(new PriceConverter());
        }
    } 
}
