namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Vertical
  {
    #region Properties & Fields - Public

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("subVertical")]
    public string? SubVertical { get; set; }

    #endregion
  }
}
