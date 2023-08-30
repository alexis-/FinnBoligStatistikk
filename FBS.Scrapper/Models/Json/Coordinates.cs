namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Coordinates
  {
    #region Properties & Fields - Public

    [JsonProperty("lat")]
    public double? Lat { get; set; }

    [JsonProperty("lon")]
    public double? Lon { get; set; }

    #endregion
  }
}
