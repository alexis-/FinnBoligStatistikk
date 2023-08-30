namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Label
  {
    #region Properties & Fields - Public

    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("text")]
    public string? Text { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    #endregion
  }
}
