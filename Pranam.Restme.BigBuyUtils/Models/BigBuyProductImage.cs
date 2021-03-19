using System.Collections.Generic;

namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyProductImage
    {
        public long Id { get; set; }
        public bool IsCover { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class BigBuyProductImageCollection
    {
        public long Id { get; set; }
        public List<BigBuyProductImage> Images { get; set; }
    }
}