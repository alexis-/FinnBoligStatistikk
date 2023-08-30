namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class BedroomsRange
  {
    #region Properties & Fields - Public

    [JsonProperty("start")]
    public int? Start { get; set; }

    [JsonProperty("end")]
    public int? End { get; set; }

    #endregion
  }
}
