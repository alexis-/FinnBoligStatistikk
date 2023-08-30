namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Image
  {
    #region Properties & Fields - Public

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("path")]
    public string? Path { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("aspect_ratio")]
    public double? AspectRatio { get; set; }

    #endregion
  }
}
