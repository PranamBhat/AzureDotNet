namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyProductTag
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string LinkRewrite { get; set; }
        public string Language { get; set; }
    }

    public class BigBuyProductTagAssociation
    {
        public long Id { get; set; }
        public string[] Tag { get; set; }
        public string Sku { get; set; }
    }
}