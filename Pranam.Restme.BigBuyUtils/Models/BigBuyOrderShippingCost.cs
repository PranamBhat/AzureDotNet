using System.Collections.Generic;

namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyOrderShippingCost
    {
        public List<BigBuyOrderShippingOption> ShippingOptions { get; set; }
    }

    public class BigBuyOrderShippingOption
    {
        public BigBuyShippingService ShippingService { get; set; }
        public decimal Cost { get; set; }
        public decimal Weight { get; set; }
    }
}