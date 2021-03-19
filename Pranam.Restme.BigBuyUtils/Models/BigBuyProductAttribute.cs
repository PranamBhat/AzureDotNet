namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyProductAttribute
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long AttributeGroup { get; set; }
        public string IsoCode { get; set; }
    }

    public class BigBuyProductAttributeGroup
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string IsoCode { get; set; }
    }
}