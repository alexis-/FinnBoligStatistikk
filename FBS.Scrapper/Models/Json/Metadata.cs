namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Metadata
  {
    #region Properties & Fields - Public

    [JsonProperty("params")]
    public Params? Params { get; set; }

    [JsonProperty("search_key")]
    public string? SearchKey { get; set; }

    [JsonProperty("num_results")]
    public int? NumResults { get; set; }

    [JsonProperty("quest_time")]
    public int? QuestTime { get; set; }

    [JsonProperty("solr_time")]
    public int? SolrTime { get; set; }

    [JsonProperty("solr_elapsed_time")]
    public int? SolrElapsedTime { get; set; }

    [JsonProperty("result_size")]
    public ResultSize? ResultSize { get; set; }

    [JsonProperty("paging")]
    public Paging? Paging { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("is_savable_search")]
    public bool? IsSavableSearch { get; set; }

    [JsonProperty("search_key_description")]
    public string? SearchKeyDescription { get; set; }

    [JsonProperty("vertical")]
    public string? Vertical { get; set; }

    [JsonProperty("vertical_description")]
    public string? VerticalDescription { get; set; }

    [JsonProperty("sort")]
    public string? Sort { get; set; }

    [JsonProperty("uuid")]
    public string? Uuid { get; set; }

    [JsonProperty("tracking")]
    public Tracking? Tracking { get; set; }

    [JsonProperty("guided_search")]
    public GuidedSearch? GuidedSearch { get; set; }

    [JsonProperty("actions")]
    public List<object>? Actions { get; set; }

    [JsonProperty("is_end_of_paging")]
    public bool? IsEndOfPaging { get; set; }

    #endregion
  }
}
