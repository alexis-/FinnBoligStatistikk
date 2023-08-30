namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class AreaRange
  {
    #region Properties & Fields - Public

    [JsonProperty("size_from")]
    public int? SizeFrom { get; set; }

    [JsonProperty("size_to")]
    public int? SizeTo { get; set; }

    [JsonProperty("unit")]
    public string? Unit { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    #endregion
  }
}
