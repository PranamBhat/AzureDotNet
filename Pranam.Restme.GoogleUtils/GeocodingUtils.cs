using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Pranam.Restme.GoogleUtils.Models;

namespace Pranam.Restme.GoogleUtils
{
    public class GeocodingUtils
    {
        public const string RequestUrl = "https://maps.googleapis.com/maps/api/geocode/";


        public static async Task<GeoAddress> GetGeoAddressAsync(string apiKey, string originAddress,
            GeoUnit geoUnit = GeoUnit.Metric, RequestOutputFormat outputFormat = RequestOutputFormat.Json)
        {
            if (apiKey.IsNotNullOrEmpty() && originAddress.IsNotNullOrEmpty())
            {
                var path = $"{(outputFormat == RequestOutputFormat.Json ? "json" : "xml")}?key={apiKey}";
                if (geoUnit != GeoUnit.Metric)
                    path += "&units=imperial";

                path += $"&address={originAddress}";
                using (var rest = new Rest(new Uri(RequestUrl)))
                {
                    var result = await rest.GetAsync<string>(path);
                    if (result.IsNotNullOrEmpty())
                    {
                        var jobject = JObject.Parse(result);
                        if (jobject.ContainsKey("results"))
                        {
                            var valueResult = jobject["results"].ToObject<GeoAddress[]>();
                            if (valueResult?.Length > 0)
                            {
                                return valueResult[0];
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}