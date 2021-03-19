using System.Collections.Generic;

namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyProductStock
    {
        public int Quantity { get; set; }
        public int MinHandlingDays { get; set; }
        public int MaxHandlingDays { get; set; }
    }

    public class BigBuyProductStockAssociation
    {
        public long Id { get; set; }
        public List<BigBuyProductStock> Stocks { get; set; }
        public string Sku { get; set; }
    }
}