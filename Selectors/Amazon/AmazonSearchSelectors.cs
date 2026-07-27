namespace XTrendApp.Web.Selectors.Amazon
{
    public static class AmazonSearchSelectors
    {
        //public const string ProductCard = "[data-component-type='s-search-result']";

        public const string ProductCard = ".puis-card-container";

        public const string AsinAttribute = "data-csa-c-item-id";

        public const string Link = "a[href*='/dp/']";

        public const string Title = "h2 span";

        public const string Image = "img.s-image";

        public const string Price = ".a-price";

        public const string ListPrice = ".a-price.a-text-price";

        public const string Rating = ".a-icon-alt";

        public const string ReviewCount = ".s-underline-text";
        
        public const string BoughtLastMonth = ".a-size-base.a-color-secondary";

        public const string VariationCount = ".s-variation-options-link";
    }
}