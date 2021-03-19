using System;

namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyProductInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string IsoCode { get; set; }
        public DateTime DateUpdDescription { get; set; }
        public string Sku { get; set; }
    }
}