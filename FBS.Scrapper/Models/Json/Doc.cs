namespace FBS.Scrapper.Models.Json
{
  using Newtonsoft.Json;

  public class Doc
  {
    #region Properties & Fields - Public

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("main_search_key")]
    public string? MainSearchKey { get; set; }

    [JsonProperty("heading")]
    public string? Heading { get; set; }

    [JsonProperty("location")]
    public string? Location { get; set; }

    [JsonProperty("image")]
    public Image? Image { get; set; }

    [JsonProperty("flags")]
    public List<string>? Flags { get; set; }

    [JsonProperty("styling")]
    public List<string>? Styling { get; set; }

    [JsonProperty("timestamp")]
    public int Timestamp { get; set; }

    [JsonProperty("labels")]
    public List<Label>? Labels { get; set; }

    [JsonProperty("extras")]
    public List<Extra>? Extras { get; set; }

    [JsonProperty("price")]
    public Price? Price { get; set; }

    [JsonProperty("distance")]
    public double? Distance { get; set; }

    [JsonProperty("trade_type")]
    public string? TradeType { get; set; }

    [JsonProperty("logo")]
    public Logo? Logo { get; set; }

    [JsonProperty("price_range_suggestion")]
    public PriceRange? PriceRangeSuggestion { get; set; }

    [JsonProperty("price_range_total")]
    public PriceRange? PriceRangeTotal { get; set; }

    [JsonProperty("area_range")]
    public AreaRange? AreaRange { get; set; }

    [JsonProperty("organisation_name")]
    public string? OrganisationName { get; set; }

    [JsonProperty("owner_type_description")]
    public string? OwnerTypeDescription { get; set; }

    [JsonProperty("property_type_description")]
    public string? PropertyTypeDescription { get; set; }

    [JsonProperty("viewing_times")]
    public List<DateTime?>? ViewingTimes { get; set; }

    [JsonProperty("coordinates")]
    public Coordinates? Coordinates { get; set; }

    [JsonProperty("ad_type")]
    public int? AdType { get; set; }

    [JsonProperty("image_urls")]
    public List<string>? ImageUrls { get; set; }

    [JsonProperty("bedrooms_range")]
    public BedroomsRange? BedroomsRange { get; set; }

    [JsonProperty("ad_id")]
    public int? AdId { get; set; }

    [JsonProperty("price_suggestion")]
    public Price? PriceSuggestion { get; set; }

    [JsonProperty("price_total")]
    public Price? PriceTotal { get; set; }

    [JsonProperty("price_shared_cost")]
    public Price? PriceSharedCost { get; set; }

    [JsonProperty("area_plot")]
    public AreaPlot? AreaPlot { get; set; }

    [JsonProperty("number_of_bedrooms")]
    public int? NumberOfBedrooms { get; set; }

    [JsonProperty("local_area_name")]
    public string? LocalAreaName { get; set; }

    [JsonProperty("area")]
    public Area? Area { get; set; }

    #endregion
  }
}
