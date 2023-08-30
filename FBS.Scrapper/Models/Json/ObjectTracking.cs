namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class ObjectTracking
  {
    #region Properties & Fields - Public

    [JsonProperty("selectionFilters")]
    public List<object>? SelectionFilters { get; set; }

    [JsonProperty("sortingType")]
    public string? SortingType { get; set; }

    [JsonProperty("numItems")]
    public int? NumItems { get; set; }

    [JsonProperty("pageNumber")]
    public int? PageNumber { get; set; }

    [JsonProperty("layout")]
    public string? Layout { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    #endregion
  }
}
