using System;

namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ParentCategory { get; set; }
        public string Url { get; set; }
        public DateTime DateUpd { get; set; }
        public DateTime DateAdd { get; set; }
        public string[] UrlImages { get; set; }
        public string IsoCode { get; set; }
    }

    public class BigBuyProductCategoryAssociation
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long Category { get; set; }
        public int Position { get; set; }
    }
}