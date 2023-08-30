namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Logo
  {
    #region Properties & Fields - Public

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("path")]
    public string? Path { get; set; }

    #endregion
  }
}
