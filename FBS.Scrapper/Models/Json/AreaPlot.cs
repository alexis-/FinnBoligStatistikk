namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class AreaPlot
  {
    #region Properties & Fields - Public

    [JsonProperty("size")]
    public int? Size { get; set; }

    [JsonProperty("unit")]
    public string? Unit { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    #endregion
  }
}
