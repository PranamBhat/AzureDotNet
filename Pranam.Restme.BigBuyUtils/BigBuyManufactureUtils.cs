using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyManufactureUtils
    {
        private const string URL_ListAllManufactures = "/rest/catalog/manufacturers.{0}";

        public Task<List<BigBuyManufacture>> GetAllManufacturers(string apiKey, bool isProduction = true,
            string resultFormat = "json")
        {
            using (var rest = new Rest(isProduction ? BigBuyConstants.ENDPOINT_LIVE : BigBuyConstants.ENDPOINT_TEST))
            {
                return rest.GetAsync<List<BigBuyManufacture>>(string.Format(URL_ListAllManufactures, resultFormat));
            }
        }
    }
}