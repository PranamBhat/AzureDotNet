using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyProductCategoryUtils
    {
        private const string URL_ListAllCategories = "/rest/catalog/categories.json?isoCode={0}&_format={1}";

        public Task<List<BigBuyCategory>> GetAllProductCategoriesAsync(string apiKey, bool isProduction = true,
            string isoCode = "en", string resultFormat = "json")
        {
            using (var rest = new Rest(isProduction ? BigBuyConstants.ENDPOINT_LIVE : BigBuyConstants.ENDPOINT_TEST))
            {
                return rest.GetAsync<List<BigBuyCategory>>(string.Format(URL_ListAllCategories, isoCode,
                    resultFormat));
            }
        }
    }
}