namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class ResultSize
  {
    #region Properties & Fields - Public

    [JsonProperty("match_count")]
    public int? MatchCount { get; set; }

    [JsonProperty("group_count")]
    public int? GroupCount { get; set; }

    #endregion
  }
}
