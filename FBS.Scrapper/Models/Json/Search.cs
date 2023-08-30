namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Search
  {
    #region Properties & Fields - Public

    [JsonProperty("items")]
    public List<object>? Items { get; set; }

    [JsonProperty("@type")]
    public string? Type { get; set; }

    [JsonProperty("@id")]
    public string? Id { get; set; }

    #endregion
  }
}
