namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Tracking
  {
    #region Properties & Fields - Public

    [JsonProperty("object")]
    public ObjectTracking? ObjectTracking { get; set; }

    [JsonProperty("vertical")]
    public Vertical? Vertical { get; set; }

    [JsonProperty("search")]
    public Search? Search { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("intent")]
    public string? Intent { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    #endregion
  }
}
