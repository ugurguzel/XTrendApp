namespace XTrendApp.Web.Selectors.Amazon;

public static class AmazonDetailSelectors
{
    public const string ProductInformation = "#prodDetails";

    public const string Section = ".a-expander-container";

    public const string SectionTitle = ".a-expander-prompt";

    public const string Table = "table";

    public const string Row = "tbody tr";

    public const string Key = "th";

    public const string Value = "td";

    // PRICE

    public const string Price = ".apexPriceToPay .a-offscreen";

    public const string DealPrice = ".reinventPricePriceToPayMargin .a-offscreen";

    public const string CorePrice = ".a-price .a-offscreen";

    // AVAILABILITY

    public const string Availability = "#availability";

    // CURRENCY

    public const string Currency = ".apexPriceToPay .a-price-symbol";
}