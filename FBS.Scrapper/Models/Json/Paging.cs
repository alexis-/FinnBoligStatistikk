namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Paging
  {
    #region Properties & Fields - Public

    [JsonProperty("param")]
    public string? Param { get; set; }

    [JsonProperty("current")]
    public int? Current { get; set; }

    [JsonProperty("last")]
    public int? Last { get; set; }

    #endregion
  }
}
