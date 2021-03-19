using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pranam.Restme.GoogleUtils.Models;

namespace Pranam.Restme.GoogleUtils
{
    public class DistanceMatrixUtils
    {
        public const string RequestUrl = "https://maps.googleapis.com/maps/api/distancematrix/";

        public static Task<DistanceMatrixResponse> GetDistanceAsync(string apiKey, List<string> originAddresses,
            List<string> destinationAddresses,
            GeoUnit geoUnit = GeoUnit.Metric, RequestOutputFormat outputFormat = RequestOutputFormat.Json)
        {
            if (apiKey.IsNotNullOrEmpty() && originAddresses?.Count > 0 &&
                destinationAddresses?.Count() > 0)
            {
                var path = $"{(outputFormat == RequestOutputFormat.Json ? "json" : "xml")}?key={apiKey}";
                if (geoUnit != GeoUnit.Metric)
                    path += "&units=imperial";
                var origins = string.Join("|", originAddresses.Select(item => item.Trim()));
                var destinations = string.Join("|", destinationAddresses.Select(item => item.Trim()));

                path += $"&origins={origins}&destinations={destinations}";
                using (var rest = new Rest(new Uri(RequestUrl)))
                {
                    return rest.GetAsync<DistanceMatrixResponse>(path);
                }
            }

            return Task.FromResult(default(DistanceMatrixResponse));
        }

        public static async Task<DistanceMatrixDistance> GetDistanceAsync(string apiKey, string originAddress,
            string destinationAddress, GeoUnit geoUnit = GeoUnit.Metric,
            RequestOutputFormat outputFormat = RequestOutputFormat.Json)
        {
            var result = await GetDistanceAsync(apiKey, new List<string> {originAddress},
                new List<string> {destinationAddress},
                geoUnit, outputFormat);
            if (result?.Status == "OK" && result.Rows?.Length > 0 && result.Rows[0].Elements?.Length > 0)
            {
                return result.Rows[0].Elements[0];
            }

            return null;
        }

        public static async Task<List<DistanceMatrixDistance>> GetDistancesAsync(string apiKey, string originAddress,
            List<string> destinationAddresses, GeoUnit geoUnit = GeoUnit.Metric,
            RequestOutputFormat outputFormat = RequestOutputFormat.Json)
        {
            var result = await GetDistanceAsync(apiKey, new List<string> {originAddress},
                destinationAddresses,
                geoUnit, outputFormat);
            if (result?.Status == "OK" && result.Rows?.Length > 0 && result.Rows[0].Elements?.Length > 0)
            {
                return result.Rows[0].Elements?.ToList();
            }

            return null;
        }
    }
}