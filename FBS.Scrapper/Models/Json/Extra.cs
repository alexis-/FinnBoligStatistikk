namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Extra
  {
    #region Properties & Fields - Public

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("label")]
    public string? Label { get; set; }

    [JsonProperty("values")]
    public List<string>? Values { get; set; }

    #endregion
  }
}
