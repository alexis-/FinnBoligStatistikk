namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class SearchResult
  {
    #region Properties & Fields - Public

    [JsonProperty("docs")]
    public List<Doc>? Docs { get; set; }

    //[JsonProperty("filters")]
    //public List<object>? Filters { get; set; }

    [JsonProperty("metadata")]
    public Metadata? Metadata { get; set; }

    #endregion
  }
}
