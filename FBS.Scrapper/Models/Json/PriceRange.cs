namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class PriceRange
  {
    #region Properties & Fields - Public

    [JsonProperty("amount_from")]
    public int? AmountFrom { get; set; }

    [JsonProperty("amount_to")]
    public int? AmountTo { get; set; }

    [JsonProperty("currency_code")]
    public string? CurrencyCode { get; set; }

    #endregion
  }
}
