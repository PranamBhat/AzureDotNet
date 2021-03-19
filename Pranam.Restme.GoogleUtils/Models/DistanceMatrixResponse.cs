using Newtonsoft.Json;

namespace Pranam.Restme.GoogleUtils.Models
{
    public class DistanceMatrixResponse
    {
        [JsonProperty(PropertyName = "destination_addresses")]
        public string[] DestinationAddresses { get; set; }

        [JsonProperty(PropertyName = "origin_addresses")]
        public string[] OriginAddresses { get; set; }

        public DistanceMatrixResponseRow[] Rows { get; set; }

        public string Status { get; set; }
    }


    public class DistanceMatrixResponseRow
    {
        public DistanceMatrixDistance[] Elements { get; set; }
    }

    public class DistanceMatrixDistance
    {
        public DistanceMatrixResponseDistanceValue Distance { get; set; }
        public DistanceMatrixResponseDistanceValue Duration { get; set; }
        public string Status { get; set; }
    }

    public class DistanceMatrixResponseDistanceValue
    {
        public string Text { get; set; }
        public long Value { get; set; }
    }
}