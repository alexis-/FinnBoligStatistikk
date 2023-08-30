namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Params
  {
    #region Properties & Fields - Public

    [JsonProperty("page")]
    public List<string>? Page { get; set; }

    [JsonProperty("sort")]
    public List<string>? Sort { get; set; }

    #endregion
  }
}
