namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Price
  {
    #region Properties & Fields - Public

    [JsonProperty("amount")]
    public int? Amount { get; set; }

    [JsonProperty("currency_code")]
    public string? CurrencyCode { get; set; }

    #endregion
  }
}
