namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyProductVariationAssociation
    {
        public long Id { get; set; }
        public long Product { get; set; }
        public string Sku { get; set; }
        public string Ean13 { get; set; }
        public decimal ExtraWeight { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal RetailPrice { get; set; }
    }
    
    public class BigBuyProductVariation
    {
        
    }
}