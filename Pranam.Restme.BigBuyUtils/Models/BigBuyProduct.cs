using System;
using System.Collections.Generic;

namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyProduct
    {
        public long Id { get; set; }
        public long Manufacturer { get; set; }
        public string Sku { get; set; }
        public string Ean13 { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Depth { get; set; }
        public DateTime DateUpd { get; set; }
        public long Category { get; set; }
        public List<BigBuyProductCategoryAssociation> Categories { get; set; }
        public DateTime DateUpdDescription { get; set; }
        public DateTime DateUpdImages { get; set; }
        public DateTime DateUpdStock { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public DateTime DateAdd { get; set; }
        public string Video { get; set; }
        public int Active { get; set; }
        public BigBuyProductImageCollection Images { get; set; }
        public List<BigBuyProductTagAssociation> Tags { get; set; }
        public decimal TaxRate { get; set; }
        public long TaxId { get; set; }
        public decimal InShopPrice { get; set; }
    }
}