namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class GuidedSearch
  {
    #region Properties & Fields - Public

    [JsonProperty("suggestions")]
    public List<object>? Suggestions { get; set; }

    [JsonProperty("tracking")]
    public Tracking? Tracking { get; set; }

    #endregion
  }
}
