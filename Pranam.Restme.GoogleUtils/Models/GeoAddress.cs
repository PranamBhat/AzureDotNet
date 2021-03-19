using System.Linq;
using Newtonsoft.Json;

namespace Pranam.Restme.GoogleUtils.Models
{
    public class GeoAddress
    {
        [JsonProperty(PropertyName = "address_components")]
        public GeoAddressComponent[] AddressComponents { get; set; }

        [JsonProperty(PropertyName = "formatted_address")]
        public string FormattedAddress { get; set; }


        /// <summary>
        /// In UK it is county (such as Lancashire), in US it is the next level under State
        /// </summary>
        public GeoAddressComponent CountyProvince => AddressComponents?.FirstOrDefault(item =>
            item.Types?.Contains(GeoTypes.AdministrativeAreaLevel2) == true);

        public string CountyProvinceName => CountyProvince?.ShortName;
        public string CountyProvinceFullName => CountyProvince?.LongName;

        /// <summary>
        /// In UK it is region (such as England, Scotland etc), in US it is the State
        /// </summary>
        public GeoAddressComponent RegionState => AddressComponents?.FirstOrDefault(item =>
            item.Types?.Contains(GeoTypes.AdministrativeAreaLevel1) == true);

        public string RegionStateName => RegionState?.ShortName;
        public string RegionStateFullName => RegionState?.LongName;

        /// <summary>
        /// District
        /// </summary>
        public GeoAddressComponent District => AddressComponents?.FirstOrDefault(item =>
            item.Types?.Contains(GeoTypes.AdministrativeAreaLevel3) == true);

        public string DistrictName => District?.ShortName;
        public string DistrictFullName => District?.LongName;


        public GeoAddressComponent Country => AddressComponents?.FirstOrDefault(item =>
            item.Types?.Contains(GeoTypes.Country) == true);

        public string CountryName => Country?.ShortName;
        public string CountryFullName => Country?.LongName;


        public GeoAddressComponent TownCity =>
            AddressComponents?.FirstOrDefault(item => item.Types?.Contains(GeoTypes.PostalTown) == true)
            ?? AddressComponents?.FirstOrDefault(item => item.Types?.Contains(GeoTypes.Locality) == true);

        public string TownCityName => TownCity?.ShortName;
        public string TownCityFullName => TownCity?.LongName;

        public GeoAddressComponent StreetRoad =>
            AddressComponents?.FirstOrDefault(item => item.Types?.Contains(GeoTypes.StreetAddress) == true)
            ?? AddressComponents?.FirstOrDefault(item => item.Types?.Contains(GeoTypes.Route) == true);

        public string StreetRoadName => StreetRoad?.ShortName;
        public string StreetRoadFullName => StreetRoad?.LongName;
    }


    public class GeoAddressComponent
    {
        [JsonProperty(PropertyName = "long_name")]
        public string LongName { get; set; }

        [JsonProperty(PropertyName = "short_name")]
        public string ShortName { get; set; }

        [JsonProperty(PropertyName = "types")] public string[] Types { get; set; }
    }


    /// <summary>
    /// https://developers.google.com/maps/documentation/geocoding/intro
    /// </summary>
    public class GeoTypes
    {
        public const string AdministrativeAreaLevel1 = "administrative_area_level_1";
        public const string AdministrativeAreaLevel2 = "administrative_area_level_2";
        public const string AdministrativeAreaLevel3 = "administrative_area_level_3";

        public const string PostalCode = "postal_code";

        public const string Route = "route";
        public const string StreetAddress = "street_address";

        public const string Locality = "locality";
        public const string SubLocality = "sublocality";


        public const string Political = "political";
        public const string Country = "country";

        public const string PostalTown = "postal_town";
        public const string PointOfInterest = "point_of_interest";
        public const string Parking = "parking";
        public const string PostBox = "post_box";
        public const string BuildingAddress = "room";
        public const string StreetNumber = "street_number";
        public const string BusStation = "bus_station";
    }
}