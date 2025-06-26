# Finn.no API DTO Models

These DTO models for the Finn.no API were built from real API data calls.

---

### 1. Market Listing DTOs (`GET /api/`)

These models represent the list of all available markets and submarkets on Finn.no.

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.Markets.MarketListDto.cs
//

using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.Markets;

/// <summary>
/// Represents the root response from the market listing endpoint, which is a list of markets.
/// This DTO is an alias for a list of MarketDto objects.
/// </summary>
public class MarketListDto : List<MarketDto>
{
}

/// <summary>
/// Represents a single market or submarket.
/// </summary>
public class MarketDto
{
    /// <summary>
    /// The unique identifier for the market.
    /// Example: "bap", "realestate"
    /// </summary>
    [JsonPropertyName("market-id")]
    public string MarketId { get; set; }

    /// <summary>
    /// The identifier used in search URLs. Can be the same as market-id or more specific.
    /// Example: "bap", "realestate-homes"
    /// </summary>
    [JsonPropertyName("search-id")]
    public string SearchId { get; set; }

    /// <summary>
    /// The constant key used to identify the market in API calls.
    /// Example: "SEARCH_ID_BAP_COMMON", "SEARCH_ID_REALESTATE_HOMES"
    /// </summary>
    [JsonPropertyName("search-key")]
    public string SearchKey { get; set; }

    /// <summary>
    /// The human-readable name of the market.
    /// Example: "Torget", "Boliger til salgs"
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; }

    /// <summary>
    /// The type of the market, indicating if it's an internal API or an external link.
    /// Example: "classified", "external"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Indicates if this market is currently selected in the UI. Always false in this context.
    /// </summary>
    [JsonPropertyName("selected")]
    public bool Selected { get; set; }

    /// <summary>
    /// A list of related API links for this market.
    /// </summary>
    [JsonPropertyName("links")]
    public List<LinkDto> Links { get; set; }

    /// <summary>
    /// A list of sub-markets, if any.
    /// </summary>
    [JsonPropertyName("submarkets")]
    public List<MarketDto>? Submarkets { get; set; }
    
    /// <summary>
    /// The direct URL for markets of type "external".
    /// Example: "https://www.finn.no/reise/feriehus-hytteutleie/resultat/"
    /// </summary>
    [JsonPropertyName("link")]
    public string? ExternalLink { get; set; }
}

/// <summary>
/// Represents a hyperlink related to a market.
/// </summary>
public class LinkDto
{
    /// <summary>
    /// The full URL for the API endpoint.
    /// Example: "https://apps.finn.no/api/search/bap?appsmarkethint=bap"
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// The relation type of the link, describing its purpose.
    /// Example: "search", "filters"
    /// </summary>
    [JsonPropertyName("rel")]
    public string Rel { get; set; }
}
```

### 2. Search Result DTOs (`GET /search/{marketId}?client=ANDROID`)

These models represent the response from a search query.

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.Search.SearchResultDto.cs
//

using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.Search;

/// <summary>
/// Root object for a search result response.
/// </summary>
public class SearchResultDto
{
    /// <summary>
    /// The list of ad summaries found in the search result.
    /// </summary>
    [JsonPropertyName("docs")]
    public List<AdSummaryDocDto> Docs { get; set; }

    /// <summary>
    /// A list of filter objects available for the search.
    /// </summary>
    [JsonPropertyName("filters")]
    public List<FilterDto> Filters { get; set; }

    /// <summary>
    /// Metadata about the search query and its results.
    /// </summary>
    [JsonPropertyName("metadata")]
    public SearchMetadataDto Metadata { get; set; }
}

/// <summary>
/// Represents a single ad summary in the search result list.
/// Contains a mix of properties from different ad types (e.g., real estate, projects).
/// </summary>
public class AdSummaryDocDto
{
    /// <summary>
    /// The primary type of the ad.
    /// Example: "realestate", "realestate_project", "BAP"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// The unique identifier for the ad, often the same as the Finnkode.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    /// <summary>
    /// The numeric Finnkode for the ad.
    /// </summary>
    [JsonPropertyName("ad_id")]
    public long AdId { get; set; }

    /// <summary>
    /// The search key of the market this ad belongs to.
    /// Example: "SEARCH_ID_REALESTATE_HOMES", "SEARCH_ID_REALESTATE_NEWBUILDINGS"
    /// </summary>
    [JsonPropertyName("main_search_key")]
    public string MainSearchKey { get; set; }

    /// <summary>
    /// The main title or heading of the ad.
    /// Example: "Lys og attraktiv 2-r fra 2009 m/solrik og skjermet markterrasse..."
    /// </summary>
    [JsonPropertyName("heading")]
    public string Heading { get; set; }

    /// <summary>
    /// The general location of the ad.
    /// Example: "Thorvald Meyers gate 14, Oslo"
    /// </summary>
    [JsonPropertyName("location")]
    public string Location { get; set; }

    /// <summary>
    /// The main image for the ad.
    /// </summary>
    [JsonPropertyName("image")]
    public ImageDto? Image { get; set; }
    
    /// <summary>
    /// A list of URLs for the first few images in the ad gallery.
    /// </summary>
    [JsonPropertyName("image_urls")]
    public List<string>? ImageUrls { get; set; }

    /// <summary>
    /// A list of promotional flags associated with the ad.
    /// Example: ["highlight_listing_estate_pluss"], ["private"]
    /// </summary>
    [JsonPropertyName("flags")]
    public List<string>? Flags { get; set; }

    /// <summary>
    /// Styling information for the ad, like highlight colors.
    /// Example: ["highlight_colour:#262830", "zoom_level:14"]
    /// </summary>
    [JsonPropertyName("styling")]
    public List<string>? Styling { get; set; }

    /// <summary>
    /// The timestamp of when the ad was published or last updated, in Unix milliseconds.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// A list of labels displayed on the ad, such as "Kommer for salg".
    /// </summary>
    [JsonPropertyName("labels")]
    public List<LabelDto>? Labels { get; set; }

    /// <summary>
    /// The direct URL to the ad on the Finn.no website.
    /// </summary>
    [JsonPropertyName("canonical_url")]
    public string? CanonicalUrl { get; set; }

    /// <summary>
    /// A list of extra properties for the ad. The structure is highly variable.
    /// Example: [{"id": 2, "label": "Produkttype", "values": ["Stasjonær PC"]}]
    /// </summary>
    [JsonPropertyName("extras")]
    public List<object>? Extras { get; set; } // Structure varies, defined as object for now.

    /// <summary>
    /// The logo of the advertising organization.
    /// </summary>
    [JsonPropertyName("logo")]
    public LogoDto? Logo { get; set; }

    /// <summary>
    /// The name of the advertising real estate agency or company.
    /// Example: "PrivatMegleren Aksept", "DNB Eiendom AS"
    /// </summary>
    [JsonPropertyName("organisation_name")]
    public string? OrganisationName { get; set; }
    
    /// <summary>
    /// For letting ads, specifies the name of an external partner syndicating the ad.
    /// Example: "Qasa"
    /// </summary>
    [JsonPropertyName("external_partner_name")]
    public string? ExternalPartnerName { get; set; }

    /// <summary>
    /// The geographical coordinates of the property.
    /// </summary>
    [JsonPropertyName("coordinates")]
    public CoordinatesDto? Coordinates { get; set; }

    /// <summary>
    /// A numeric type identifier for the ad.
    /// </summary>
    [JsonPropertyName("ad_type")]
    public int? AdType { get; set; }
    
    /// <summary>
    /// For letting ads, specifies whether the property is furnished.
    /// Example: "Møblert", "Delvis møblert", "Umøblert"
    /// </summary>
    [JsonPropertyName("furnished_state")]
    public string? FurnishedState { get; set; }

    // --- Real Estate specific properties ---
    /// <summary>
    /// The asking price for the property.
    /// </summary>
    [JsonPropertyName("price_suggestion")]
    public PriceDto? PriceSuggestion { get; set; }

    /// <summary>
    /// The total price including associated costs.
    /// </summary>
    [JsonPropertyName("price_total")]
    public PriceDto? PriceTotal { get; set; }

    /// <summary>
    /// The shared monthly costs (felleskostnader).
    /// </summary>
    [JsonPropertyName("price_shared_cost")]
    public PriceDto? PriceSharedCost { get; set; }

    /// <summary>
    /// The range of living area (P-ROM or BRA).
    /// </summary>
    [JsonPropertyName("area_range")]
    public AreaRangeDto? AreaRange { get; set; }
    
    /// <summary>
    /// The area of the property, typically used for non-ranged values.
    /// </summary>
    [JsonPropertyName("area")]
    public AreaDto? Area { get; set; }

    /// <summary>
    /// The area of the plot (tomteareal).
    /// </summary>
    [JsonPropertyName("area_plot")]
    public AreaDto? AreaPlot { get; set; }

    /// <summary>
    /// The type of ownership.
    /// Example: "Eier (Selveier)", "Andel"
    /// </summary>
    [JsonPropertyName("owner_type_description")]
    public string? OwnerTypeDescription { get; set; }

    /// <summary>
    /// The type of property.
    /// Example: "Leilighet", "Enebolig"
    /// </summary>
    [JsonPropertyName("property_type_description")]
    public string? PropertyTypeDescription { get; set; }

    /// <summary>
    /// A list of scheduled viewing times in ISO 8601 format.
    /// Example: ["2025-06-29T11:30:00.000+00:00"]
    /// </summary>
    [JsonPropertyName("viewing_times")]
    public List<string>? ViewingTimes { get; set; }

    /// <summary>
    /// The number of bedrooms.
    /// </summary>
    [JsonPropertyName("number_of_bedrooms")]
    public int? NumberOfBedrooms { get; set; }
    
    /// <summary>
    /// The name of the local area or neighborhood.
    /// Example: "Ringnes Park:", "Ski sentrum"
    /// </summary>
    [JsonPropertyName("local_area_name")]
    public string? LocalAreaName { get; set; }

    // --- Real Estate Project specific properties ---
    /// <summary>
    /// The suggested price range for units in a development project.
    /// </summary>
    [JsonPropertyName("price_range_suggestion")]
    public PriceRangeDto? PriceRangeSuggestion { get; set; }

    /// <summary>
    /// The total price range for units in a development project.
    /// </summary>
    [JsonPropertyName("price_range_total")]
    public PriceRangeDto? PriceRangeTotal { get; set; }
    
    /// <summary>
    /// The range of bedrooms available in a development project.
    /// </summary>
    [JsonPropertyName("bedrooms_range")]
    public BedroomsRangeDto? BedroomsRange { get; set; }
}

// --- Common Reusable DTOs for Search ---

public class ImageDto
{
    /// <summary>
    /// The full URL to the image.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// The relative path of the image on the CDN.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; }

    /// <summary>
    /// The height of the image in pixels.
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// The width of the image in pixels.
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// The aspect ratio of the image (width / height).
    /// </summary>
    [JsonPropertyName("aspect_ratio")]
    public double AspectRatio { get; set; }
}

public class LabelDto
{
    /// <summary>
    /// The identifier for the label type.
    /// Example: "coming_for_sale", "private"
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The display text for the label.
    /// Example: "Kommer for salg", "Privat"
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// The type or importance of the label.
    /// Example: "PRIMARY", "SECONDARY"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class LogoDto
{
    /// <summary>
    /// The full URL to the logo image.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// The path part of the logo URL, often identical to the full URL.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; }
}

public class PriceDto
{
    /// <summary>
    /// The price amount as a long integer.
    /// </summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    /// <summary>
    /// The currency code for the price.
    /// Example: "NOK"
    /// </summary>
    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; set; }
    
    /// <summary>
    /// The unit symbol for the price.
    /// Example: "kr"
    /// </summary>
    [JsonPropertyName("price_unit")]
    public string? PriceUnit { get; set; }
}

public class AreaDto
{
    /// <summary>
    /// The size of the area.
    /// </summary>
    [JsonPropertyName("size")]
    public int Size { get; set; }
    
    /// <summary>
    /// The unit of measurement for the area.
    /// Example: "m²"
    /// </summary>
    [JsonPropertyName("unit")]
    public string Unit { get; set; }
    
    /// <summary>
    /// A description of what the area represents.
    /// Example: "tomtestørrelse", "p-rom"
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class AreaRangeDto
{
    /// <summary>
    /// The starting size of the range.
    /// </summary>
    [JsonPropertyName("size_from")]
    public int SizeFrom { get; set; }

    /// <summary>
    /// The ending size of the range.
    /// </summary>
    [JsonPropertyName("size_to")]
    public int SizeTo { get; set; }

    /// <summary>
    /// The unit of measurement for the area.
    /// Example: "m²"
    /// </summary>
    [JsonPropertyName("unit")]
    public string Unit { get; set; }

    /// <summary>
    /// A description of what the area represents.
    /// Example: "size", "p-rom"
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class PriceRangeDto
{
    /// <summary>
    /// The starting price of the range.
    /// </summary>
    [JsonPropertyName("amount_from")]
    public long AmountFrom { get; set; }

    /// <summary>
    /// The ending price of the range.
    /// </summary>
    [JsonPropertyName("amount_to")]
    public long AmountTo { get; set; }

    /// <summary>
    /// The currency code for the price range.
    /// Example: "NOK"
    /// </summary>
    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; set; }
}

public class BedroomsRangeDto
{
    /// <summary>
    /// The starting number of bedrooms in the range.
    /// </summary>
    [JsonPropertyName("start")]
    public int Start { get; set; }

    /// <summary>
    /// The ending number of bedrooms in the range.
    /// </summary>
    [JsonPropertyName("end")]
    public int End { get; set; }
}

public class CoordinatesDto
{
    /// <summary>
    /// The latitude of the location.
    /// </summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>
    /// The longitude of the location.
    /// </summary>
    [JsonPropertyName("lon")]
    public double Longitude { get; set; }
    
    /// <summary>
    /// The accuracy level of the coordinates.
    /// </summary>
    [JsonPropertyName("accuracy")]
    public int Accuracy { get; set; }
}

// --- Filter DTOs for Search ---

public class FilterDto
{
    /// <summary>
    /// The human-readable name of the filter.
    /// Example: "Prisantydning", "Boligtype"
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    /// <summary>
    /// The parameter name for the filter.
    /// Example: "price", "property_type"
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// The list of selectable items for this filter.
    /// </summary>
    [JsonPropertyName("filter_items")]
    public List<FilterItemDto> FilterItems { get; set; }

    /// <summary>
    /// The type of filter control to display.
    /// Example: "STANDARD_FILTER", "RANGE_FILTER", "QUERY_FILTER", "LINK_FILTER"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// The minimum value for a range filter.
    /// </summary>
    [JsonPropertyName("min_value")]
    public long? MinValue { get; set; }

    /// <summary>
    /// The maximum value for a range filter.
    /// </summary>
    [JsonPropertyName("max_value")]
    public long? MaxValue { get; set; }

    /// <summary>
    /// The step increment for a range filter.
    /// </summary>
    [JsonPropertyName("step")]
    public int? Step { get; set; }

    /// <summary>
    /// The unit for a range filter.
    /// Example: "kr", "m²", "år"
    /// </summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>
    /// The 'from' parameter name for a range filter.
    /// Example: "price_from"
    /// </summary>
    [JsonPropertyName("name_from")]
    public string? NameFrom { get; set; }

    /// <summary>
    /// The 'to' parameter name for a range filter.
    /// Example: "price_to"
    /// </summary>
    [JsonPropertyName("name_to")]
    public string? NameTo { get; set; }

    /// <summary>
    /// Indicates if the range filter represents a year.
    /// </summary>
    [JsonPropertyName("is_year")]
    public bool? IsYear { get; set; }

    /// <summary>
    /// The number of filter items to initially display before a "show more" button.
    /// </summary>
    [JsonPropertyName("item_display_count")]
    public int? ItemDisplayCount { get; set; }
    
    /// <summary>
    /// For filters of type LINK_FILTER, this is the target URL.
    /// Example: "https://hjelpesenter.finn.no/hc/no/articles/115001536449"
    /// </summary>
    [JsonPropertyName("link")]
    public string? Link { get; set; }
    
    /// <summary>
    /// For filters of type LINK_FILTER, this is the display text for the link.
    /// Example: "Hva er dette?"
    /// </summary>
    [JsonPropertyName("link_text")]
    public string? LinkText { get; set; }
}

public class FilterItemDto
{
    /// <summary>
    /// The human-readable name of the filter option.
    /// Example: "Leilighet"
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    /// <summary>
    /// The parameter name for the filter option.
    /// Example: "property_type"
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// The value to be sent in the query for this option.
    /// Example: "3"
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }

    /// <summary>
    /// The number of results that match this filter option.
    /// </summary>
    [JsonPropertyName("hits")]
    public int Hits { get; set; }

    /// <summary>
    /// A list of nested sub-options for this filter item.
    /// </summary>
    [JsonPropertyName("filter_items")]
    public List<FilterItemDto> FilterItems { get; set; }

    /// <summary>
    /// Indicates if this filter option is currently selected.
    /// </summary>
    [JsonPropertyName("selected")]
    public bool Selected { get; set; }
}


// --- Metadata DTOs for Search ---

public class SearchParamsDto
{
    /// <summary>
    /// The sorting parameter used for the search.
    /// Example: "PUBLISHED_DESC"
    /// </summary>
    [JsonPropertyName("sort")]
    public List<string>? Sort { get; set; }

    /// <summary>
    /// The page number parameter for the search.
    /// Example: "2"
    /// </summary>
    [JsonPropertyName("page")]
    public List<string>? Page { get; set; }
}

public class SearchMetadataDto
{
    /// <summary>
    /// The query parameters used in the search request.
    /// </summary>
    [JsonPropertyName("params")]
    public SearchParamsDto Params { get; set; }

    /// <summary>
    /// The internal search key for the market.
    /// Example: "SEARCH_ID_REALESTATE_HOMES"
    /// </summary>
    [JsonPropertyName("search_key")]
    public string SearchKey { get; set; }

    /// <summary>
    /// The number of results returned on the current page.
    /// </summary>
    [JsonPropertyName("num_results")]
    public int NumResults { get; set; }

    /// <summary>
    /// Information about the total number of results found.
    /// </summary>
    [JsonPropertyName("result_size")]
    public ResultSizeDto ResultSize { get; set; }

    /// <summary>
    /// Information about the pagination state.
    /// </summary>
    [JsonPropertyName("paging")]
    public PagingDto Paging { get; set; }

    /// <summary>
    /// The human-readable title for the search.
    /// Example: "Bolig til salgs"
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// The primary market vertical.
    /// Example: "REALESTATE"
    /// </summary>
    [JsonPropertyName("vertical")]
    public string Vertical { get; set; }

    /// <summary>
    /// The sorting method used for the results.
    /// Example: "PUBLISHED_DESC"
    /// </summary>
    [JsonPropertyName("sort")]
    public string Sort { get; set; }

    /// <summary>
    /// A unique identifier for this specific search query instance.
    /// Example: "ce7c07e9-f55f-4570-bddb-14d04a760ecb"
    /// </summary>
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }

    /// <summary>
    /// Tracking information for analytics.
    /// </summary>
    [JsonPropertyName("tracking")]
    public SearchTrackingDto Tracking { get; set; }
    
    /// <summary>
    /// A boolean indicating if the current page is the last page of results.
    /// </summary>
    [JsonPropertyName("is_end_of_paging")]
    public bool IsEndOfPaging { get; set; }

    /// <summary>
    /// The timestamp of when the search was performed, in Unix milliseconds.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }
    
    /// <summary>
    /// A list of currently applied filters.
    /// </summary>
    [JsonPropertyName("selected_filters")]
    public List<object> SelectedFilters { get; set; }

    /// <summary>
    /// The time taken by the Quest search component, in milliseconds.
    /// </summary>
    [JsonPropertyName("quest_time")]
    public int? QuestTime { get; set; }

    /// <summary>
    /// The time taken by the Solr search engine, in milliseconds.
    /// </summary>
    [JsonPropertyName("solr_time")]
    public int? SolrTime { get; set; }

    /// <summary>
    /// The total elapsed time for the Solr query, in milliseconds.
    /// </summary>
    [JsonPropertyName("solr_elapsed_time")]
    public int? SolrElapsedTime { get; set; }

    /// <summary>
    /// Indicates if the current search is savable by the user.
    /// </summary>
    [JsonPropertyName("is_savable_search")]
    public bool IsSavableSearch { get; set; }

    /// <summary>
    /// A human-readable description of the search key.
    /// Example: "Bolig til salgs"
    /// </summary>
    [JsonPropertyName("search_key_description")]
    public string SearchKeyDescription { get; set; }

    /// <summary>
    /// A human-readable description of the vertical.
    /// Example: "Eiendom"
    /// </summary>
    [JsonPropertyName("vertical_description")]
    public string VerticalDescription { get; set; }

    /// <summary>
    /// A collection of descriptive texts for the search page.
    /// </summary>
    [JsonPropertyName("descriptions")]
    public SearchDescriptionsDto Descriptions { get; set; }

    /// <summary>
    /// Information related to guided search suggestions.
    /// </summary>
    [JsonPropertyName("guided_search")]
    public GuidedSearchDto GuidedSearch { get; set; }

    /// <summary>
    /// A list of available actions for the search results.
    /// </summary>
    [JsonPropertyName("actions")]
    public List<object> Actions { get; set; }
}

public class ResultSizeDto
{
    /// <summary>
    /// The total number of individual ads matching the query.
    /// </summary>
    [JsonPropertyName("match_count")]
    public int MatchCount { get; set; }

    /// <summary>
    /// The total number of grouped results (e.g., projects).
    /// </summary>
    [JsonPropertyName("group_count")]
    public int GroupCount { get; set; }
}

public class PagingDto
{
    /// <summary>
    /// The query parameter used for paging.
    /// Example: "page"
    /// </summary>
    [JsonPropertyName("param")]
    public string Param { get; set; }

    /// <summary>
    /// The current page number.
    /// </summary>
    [JsonPropertyName("current")]
    public int Current { get; set; }

    /// <summary>
    /// The last available page number.
    /// </summary>
    [JsonPropertyName("last")]
    public int Last { get; set; }
}

public class SearchTrackingDto
{
    /// <summary>
    /// The tracking object for the listing.
    /// </summary>
    [JsonPropertyName("object")]
    public TrackingObjectDto Object { get; set; }

    /// <summary>
    /// The tracking object for the vertical.
    /// </summary>
    [JsonPropertyName("vertical")]
    public TrackingVerticalDto Vertical { get; set; }
}

public class TrackingObjectDto
{
    /// <summary>
    /// The type of sorting used.
    /// Example: "published_desc"
    /// </summary>
    [JsonPropertyName("sortingType")]
    public string SortingType { get; set; }

    /// <summary>
    /// The total number of items.
    /// </summary>
    [JsonPropertyName("numItems")]
    public int NumItems { get; set; }

    /// <summary>
    /// The current page number.
    /// </summary>
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }

    /// <summary>
    /// The layout of the search results.
    /// Example: "Grid"
    /// </summary>
    [JsonPropertyName("layout")]
    public string Layout { get; set; }

    /// <summary>
    /// The type of the tracking object.
    /// Example: "Listing"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    /// <summary>
    /// Filters selected for tracking purposes.
    /// </summary>
    [JsonPropertyName("selectionFilters")]
    public List<object> SelectionFilters { get; set; }
}

public class TrackingVerticalDto
{
    /// <summary>
    /// The name of the vertical.
    /// Example: "realestate"
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// The name of the sub-vertical.
    /// Example: "homes"
    /// </summary>
    [JsonPropertyName("subVertical")]
    public string SubVertical { get; set; }
}

public class SearchDescriptionsDto
{
    /// <summary>
    /// The main title of the page.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// The heading displayed on the page.
    /// </summary>
    [JsonPropertyName("heading")]
    public string Heading { get; set; }

    /// <summary>
    /// The text used when saving the search.
    /// </summary>
    [JsonPropertyName("saved_search")]
    public string SavedSearch { get; set; }

    /// <summary>
    /// A human-readable version of the search key.
    /// </summary>
    [JsonPropertyName("search_key")]
    public string SearchKey { get; set; }

    /// <summary>
    /// A human-readable version of the vertical.
    /// </summary>
    [JsonPropertyName("vertical")]
    public string Vertical { get; set; }

    /// <summary>
    /// Canonical search parameters for SEO purposes.
    /// </summary>
    [JsonPropertyName("canonical_search_params")]
    public string CanonicalSearchParams { get; set; }
}

public class GuidedSearchDto
{
    /// <summary>
    /// A list of guided search suggestions.
    /// </summary>
    [JsonPropertyName("suggestions")]
    public List<object> Suggestions { get; set; }

    /// <summary>
    /// Tracking information for the guided search feature.
    /// </summary>
    [JsonPropertyName("tracking")]
    public GuidedSearchTrackingDto Tracking { get; set; }
}

public class GuidedSearchTrackingDto
{
    /// <summary>
    /// Search-specific tracking data.
    /// </summary>
    [JsonPropertyName("search")]
    public GuidedSearchTrackingSearchDto Search { get; set; }

    /// <summary>
    /// Vertical-specific tracking data.
    /// </summary>
    [JsonPropertyName("vertical")]
    public TrackingVerticalDto Vertical { get; set; }

    /// <summary>
    /// The name of the tracking event.
    /// Example: "Guided Search"
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// The user intent being tracked.
    /// Example: "Search"
    /// </summary>
    [JsonPropertyName("intent")]
    public string Intent { get; set; }

    /// <summary>
    /// The type of interaction being tracked.
    /// Example: "Click"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class GuidedSearchTrackingSearchDto
{
    /// <summary>
    /// A list of items related to the search tracking.
    /// </summary>
    [JsonPropertyName("items")]
    public List<object> Items { get; set; }

    /// <summary>
    /// The type of the search metadata object.
    /// Example: "SearchMetadata"
    /// </summary>
    [JsonPropertyName("@type")]
    public string Type { get; set; }

    /// <summary>
    /// A unique identifier for the search metadata.
    /// Example: "sdrn:finn:searchmetadata:c9d7f372-f532-459f-aba8-5b9837208fb5"
    /// </summary>
    [JsonPropertyName("@id")]
    public string Id { get; set; }
}
```

### 3. Ad View DTOs (`GET /adview/{adId}`)

These models represent the detailed view of a single ad. The structure varies significantly by ad type (`schemaName`).

#### 3.1. Common Ad View Wrapper

This is the generic structure that wraps the ad-type-specific payload.

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.AdViewDto.cs
//

using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView;

/// <summary>
/// A generic wrapper for any ad view response. The 'Ad' property holds the type-specific data.
/// </summary>
/// <typeparam name="T">The type of the ad payload (e.g., RealEstateSaleAdDto).</typeparam>
public class AdViewDto<T>
{
    /// <summary>
    /// The main ad payload, with a structure that varies by ad type.
    /// </summary>
    [JsonPropertyName("ad")]
    public T Ad { get; set; }

    /// <summary>
    /// Metadata about the ad, including its version and history.
    /// </summary>
    [JsonPropertyName("meta")]
    public AdViewMetaDto Meta { get; set; }
}

public class AdViewMetaDto
{
    /// <summary>
    /// The unique Finnkode of the ad.
    /// </summary>
    [JsonPropertyName("adId")]
    public long AdId { get; set; }
    
    /// <summary>
    /// The mode of the ad status, indicating if it's active.
    /// Example: "PLAY", "STOP"
    /// </summary>
    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    /// <summary>
    /// The timestamp of the last edit to the ad.
    /// </summary>
    [JsonPropertyName("edited")]
    public DateTimeOffset Edited { get; set; }

    /// <summary>
    /// A list of historical versions and broadcast times for the ad.
    /// </summary>
    [JsonPropertyName("history")]
    public List<AdViewHistoryItemDto> History { get; set; }

    /// <summary>
    /// The ID of the owner/advertiser.
    /// </summary>
    [JsonPropertyName("ownerId")]
    public long OwnerId { get; set; }

    /// <summary>
    /// The version string of the ad.
    /// Example: "1.1", "56.4"
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; }

    /// <summary>
    /// The timestamp when this specific version of the ad occurred.
    /// </summary>
    [JsonPropertyName("occurred")]
    public DateTimeOffset Occurred { get; set; }

    /// <summary>
    /// A unique resource name for the owner.
    /// Example: "sdrn:finn:organisation:1392690507", "sdrn:finn:user:653363010"
    /// </summary>
    [JsonPropertyName("ownerUrn")]
    public string OwnerUrn { get; set; }

    /// <summary>
    /// The schema name identifying the type of the ad.
    /// Example: "realestate-home", "realestate-development-project", "recommerce-sell", "realestate-planned", "realestate-development-single", "realestate-plot", "realestate-leisure-sale", "realestate-development-project-leisure", "realestate-leisure-plot", "realestate-business-sale", "realestate-business-plot", "realestate-letting", "realestate-letting-external"
    /// </summary>
    [JsonPropertyName("schemaName")]
    public string SchemaName { get; set; }

    /// <summary>
    /// The version of the ad schema.
    /// Example: "0.17.1", "0.8.0"
    /// </summary>
    [JsonPropertyName("schemaVersion")]
    public string SchemaVersion { get; set; }
}

public class AdViewHistoryItemDto
{
    /// <summary>
    /// The status mode of this history entry.
    /// Example: "PLAY", "STOP"
    /// </summary>
    [JsonPropertyName("mode")]
    public string Mode { get; set; }

    /// <summary>
    /// The version string for this history entry.
    /// Example: "1.1", "16.1"
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; }

    /// <summary>
    /// The timestamp when this version was broadcasted.
    /// </summary>
    [JsonPropertyName("broadcasted")]
    public DateTimeOffset Broadcasted { get; set; }
}
```

#### 3.2. Real Estate Sale Ad DTO (Unified)

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstateSaleAdDto.cs
//

using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstate;

/// <summary>
/// Unified DTO for the detailed view of a real estate ad for sale.
/// This model is designed to handle multiple schemas by making type-specific fields nullable.
/// Used for schemas: 'realestate-home', 'realestate-development-single', 'realestate-leisure-sale', 'realestate-business-sale'.
/// </summary>
public class RealEstateSaleAdDto
{
    /// <summary>
    /// The full title of the ad.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    /// <summary>
    /// The name of the project this unit belongs to, if applicable.
    /// Example: "Smart 2-roms med flott uteplass i nytt boligprosjekt!"
    /// </summary>
    [JsonPropertyName("projectName")]
    public string? ProjectName { get; set; }

    /// <summary>
    /// Information about the property's plot.
    /// </summary>
    [JsonPropertyName("plot")]
    public PlotDto? Plot { get; set; }

    /// <summary>
    /// Information about the building's area/size.
    /// </summary>
    [JsonPropertyName("size")]
    public SizeDto? Size { get; set; }
    
    /// <summary>
    /// Represents a range of area, usually for commercial or multi-unit properties.
    /// </summary>
    [JsonPropertyName("areaRange")]
    public AdAreaRangeDto? AreaRange { get; set; }

    /// <summary>
    /// The floor number the apartment is on.
    /// </summary>
    [JsonPropertyName("floor")]
    public int? Floor { get; set; }
    
    /// <summary>
    /// The total number of floors in the building or unit.
    /// </summary>
    [JsonPropertyName("numberOfFloors")]
    public int? NumberOfFloors { get; set; }

    /// <summary>
    /// Detailed price information for the property.
    /// </summary>
    [JsonPropertyName("price")]
    public DetailedPriceDto Price { get; set; }

    /// <summary>
    /// The total number of rooms.
    /// </summary>
    [JsonPropertyName("rooms")]
    public int? Rooms { get; set; }
    
    /// <summary>
    /// The total number of bedrooms.
    /// </summary>
    [JsonPropertyName("bedrooms")]
    public int? Bedrooms { get; set; }
    
    /// <summary>
    /// The total number of beds, typically for leisure properties.
    /// </summary>
    [JsonPropertyName("beds")]
    public int? Beds { get; set; }

    /// <summary>
    /// A list of all images for the ad, including descriptions.
    /// </summary>
    [JsonPropertyName("images")]
    public List<AdImageDto> Images { get; set; }

    /// <summary>
    /// Information about the different phases of the development project, applicable for single units.
    /// </summary>
    [JsonPropertyName("phases")]
    public ProjectPhasesDto? Phases { get; set; }

    /// <summary>
    /// Contact information for the advertiser.
    /// </summary>
    [JsonPropertyName("contact")]
    public List<ContactDto>? Contact { get; set; }

    /// <summary>
    /// Detailed location information for the property.
    /// </summary>
    [JsonPropertyName("location")]
    public AdViewLocationDto Location { get; set; }

    /// <summary>
    /// A list of scheduled viewing times.
    /// </summary>
    [JsonPropertyName("viewings")]
    public List<ViewingDto>? Viewings { get; set; }
    
    /// <summary>
    /// Indicates if the property is sold or disposed.
    /// </summary>
    [JsonPropertyName("disposed")]
    public bool? Disposed { get; set; }
    
    /// <summary>
    /// A text displayed when the property is sold.
    /// Example: "Solgt"
    /// </summary>
    [JsonPropertyName("disposedText")]
    public string? DisposedText { get; set; }

    /// <summary>
    /// Indicates if the advertiser is anonymous.
    /// </summary>
    [JsonPropertyName("anonymous")]
    public bool? Anonymous { get; set; }
    
    /// <summary>
    /// Cadastral information (matrikkelinformasjon) for the property.
    /// </summary>
    [JsonPropertyName("cadastres")]
    public List<CadastreDto>? Cadastres { get; set; }
    
    /// <summary>
    /// A list of documents associated with the ad (e.g., prospectus).
    /// </summary>
    [JsonPropertyName("documents")]
    public List<DocumentDto>? Documents { get; set; }
    
    /// <summary>
    /// The specific type of the ad view.
    /// Example: "realestate-home", "realestate-business-sale"
    /// </summary>
    [JsonPropertyName("adViewType")]
    public string AdViewType { get; set; }
    
    /// <summary>
    /// A list of facility keywords.
    /// Example: ["Barnevennlig", "Garasje/P-plass", "Bredbåndstilknytning"]
    /// </summary>
    [JsonPropertyName("facilities")]
    public List<string>? Facilities { get; set; }
    
    /// <summary>
    /// A list of floor plan images.
    /// </summary>
    [JsonPropertyName("floorplans")]
    public List<object>? Floorplans { get; set; }

    /// <summary>
    /// Information about shared costs (felleskostnader).
    /// </summary>
    [JsonPropertyName("sharedCost")]
    public SharedCostDto? SharedCost { get; set; }
    
    /// <summary>
    /// The energy efficiency rating of the property.
    /// </summary>
    [JsonPropertyName("energyLabel")]
    public EnergyLabelDto? EnergyLabel { get; set; }
    
    /// <summary>
    /// A list of descriptive text sections with headings from the ad.
    /// </summary>
    [JsonPropertyName("generalText")]
    public List<GeneralTextSectionDto>? GeneralText { get; set; }
    
    /// <summary>
    /// Indicates if this is a unit for letting.
    /// </summary>
    [JsonPropertyName("lettingUnit")]
    public bool? LettingUnit { get; set; }
    
    /// <summary>
    /// An external ID used by the advertiser.
    /// </summary>
    [JsonPropertyName("externalAdId")]
    public string? ExternalAdId { get; set; }
    
    /// <summary>
    /// Text describing the zoning and regulation details.
    /// </summary>
    [JsonPropertyName("regulations")]
    public string? Regulations { get; set; }
    
    /// <summary>
    /// Additional property information, often a summary or highlights.
    /// </summary>
    [JsonPropertyName("propertyInfo")]
    public List<GeneralTextSectionDto>? PropertyInfo { get; set; }

    /// <summary>
    /// The type of property. Can be a string or a list of strings. Handle with care.
    /// Example (string): "Gårdsbruk/Småbruk"
    /// Example (list): ["Kontor", "Butikk/Handel"]
    /// </summary>
    [JsonPropertyName("propertyType")]
    public object PropertyType { get; set; }
    
    /// <summary>
    /// A list of email addresses for the seller.
    /// </summary>
    [JsonPropertyName("sellerEmails")]
    public List<string>? SellerEmails { get; set; }

    /// <summary>
    /// The advertiser's internal reference number.
    /// </summary>
    [JsonPropertyName("advertiserRef")]
    public string? AdvertiserRef { get; set; }

    /// <summary>
    /// Links to more information, like bidding pages or prospectus orders.
    /// </summary>
    [JsonPropertyName("moreInfoLinks")]
    public List<MoreInfoLinkDto>? MoreInfoLinks { get; set; }
    
    /// <summary>
    /// The type of ownership for the property.
    /// Example: "Eier (Selveier)"
    /// </summary>
    [JsonPropertyName("ownershipType")]
    public string OwnershipType { get; set; }

    /// <summary>
    /// The year the property was most recently renovated.
    /// </summary>
    [JsonPropertyName("renovatedYear")]
    public int? RenovatedYear { get; set; }

    /// <summary>
    /// A summary text, often with HTML.
    /// </summary>
    [JsonPropertyName("summaryUnsafe")]
    public string? SummaryUnsafe { get; set; }
    
    /// <summary>
    /// URL to an external prospectus or detailed view.
    /// </summary>
    [JsonPropertyName("prospectusView")]
    public string? ProspectusView { get; set; }

    /// <summary>
    /// The human-readable label for the ad view type.
    /// Example: "Bolig til salgs", "Nybygg enkeltenhet til salgs", "Fritidsbolig til salgs", "Næringseiendom til salgs"
    /// </summary>
    [JsonPropertyName("adViewTypeLabel")]
    public string? AdViewTypeLabel { get; set; }
    
    /// <summary>
    /// URL to order the prospectus.
    /// </summary>
    [JsonPropertyName("prospectusOrder")]
    public string? ProspectusOrder { get; set; }

    /// <summary>
    /// URL to order project materials or prospectus.
    /// </summary>
    [JsonPropertyName("orderUrl")]
    public string? OrderUrl { get; set; }

    /// <summary>
    /// The year the property was constructed.
    /// </summary>
    [JsonPropertyName("constructionYear")]
    public int? ConstructionYear { get; set; }
    
    /// <summary>
    /// Indicates if the advertiser is a third party.
    /// </summary>
    [JsonPropertyName("thirdPartyAdvertiser")]
    public bool? ThirdPartyAdvertiser { get; set; }
    
    /// <summary>
    /// Indicates if change of ownership insurance is available.
    /// </summary>
    [JsonPropertyName("changeOfOwnershipInsurance")]
    public bool? ChangeOfOwnershipInsurance { get; set; }

    /// <summary>
    /// Information about the acquisition/takeover process.
    /// </summary>
    [JsonPropertyName("acquisition")]
    public AcquisitionDto? Acquisition { get; set; }
    
    /// <summary>
    /// Links and information related to electronic bidding.
    /// </summary>
    [JsonPropertyName("electronicBid")]
    public ElectronicBidDto? ElectronicBid { get; set; }
    
    /// <summary>
    /// The name of the local area or neighborhood.
    /// Example: "Forra", "Sand sentrum"
    /// </summary>
    [JsonPropertyName("localAreaName")]
    public string? LocalAreaName { get; set; }

    /// <summary>
    /// Information regarding preemption rights (forkjøpsrett).
    /// Example: "Borettslaget og boligbyggelagets medlemmer har forkjøpsrett..."
    /// </summary>
    [JsonPropertyName("preemption")]
    public string? Preemption { get; set; }

    /// <summary>
    /// Details about the housing cooperative (borettslag).
    /// </summary>
    [JsonPropertyName("housingCooperative")]
    public HousingCooperativeDto? HousingCooperative { get; set; }
    
    /// <summary>
    /// Description of the property's situation/location, may contain HTML.
    /// </summary>
    [JsonPropertyName("situationUnsafe")]
    public string? SituationUnsafe { get; set; }

    /// <summary>
    /// Essential information about the property's condition, may contain HTML.
    /// </summary>
    [JsonPropertyName("essentialInfoUnsafe")]
    public string? EssentialInfoUnsafe { get; set; }

    /// <summary>
    /// Information about shared debt, may contain HTML.
    /// </summary>
    [JsonPropertyName("sharedDebtInfoUnsafe")]
    public string? SharedDebtInfoUnsafe { get; set; }

    /// <summary>
    /// URL to an embedded video tour of the property.
    /// </summary>
    [JsonPropertyName("videoUrl")]
    public string? VideoUrl { get; set; }

    /// <summary>
    /// URL to a virtual tour (e.g., 3D view) of the project.
    /// </summary>
    [JsonPropertyName("virtualViewingUrl")]
    public string? VirtualViewingUrl { get; set; }
    
    /// <summary>
    /// Description of how to access the property, may contain HTML.
    /// Example: "Veg helt fram. Ligger 800 moh med flott utsikt."
    /// </summary>
    [JsonPropertyName("accessUnsafe")]
    public string? AccessUnsafe { get; set; }
    
    /// <summary>
    /// A general description of the property's location type.
    /// Example: "På fjellet", "Ved sjøen", "Innlandet"
    /// </summary>
    [JsonPropertyName("situation")]
    public string? Situation { get; set; }
    
    /// <summary>
    /// The number of available parking spots.
    /// </summary>
    [JsonPropertyName("parkingSpots")]
    public int? ParkingSpots { get; set; }
    
    /// <summary>
    /// A general description of the property's condition, may contain HTML.
    /// </summary>
    [JsonPropertyName("conditionUnsafe")]
    public string? ConditionUnsafe { get; set; }

    /// <summary>
    /// General content description, may contain HTML (for commercial properties).
    /// </summary>
    [JsonPropertyName("contentUnsafe")]
    public string? ContentUnsafe { get; set; }
    
    /// <summary>
    /// The property's elevation in meters above sea level.
    /// </summary>
    [JsonPropertyName("metersAboveSeaLevel")]
    public int? MetersAboveSeaLevel { get; set; }
    
    /// <summary>
    /// URL to download the property prospectus.
    /// </summary>
    [JsonPropertyName("prospectusDownload")]
    public string? ProspectusDownload { get; set; }
    
    /// <summary>
    /// URL to the technical condition report (tilstandsrapport).
    /// </summary>
    [JsonPropertyName("technicalReportUrl")]
    public string? TechnicalReportUrl { get; set; }

    // --- Commercial-specific properties ---
    
    /// <summary>
    /// The annual rental income for a commercial property.
    /// </summary>
    [JsonPropertyName("rentalIncome")]
    public long? RentalIncome { get; set; }

    /// <summary>
    /// The number of separate office spaces in a commercial property.
    /// </summary>
    [JsonPropertyName("officeSpaces")]
    public int? OfficeSpaces { get; set; }
}
```

#### 3.3. Real Estate Letting Ad DTO (Unified)

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstateLettingAdDto.cs
//

using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstate;

/// <summary>
/// Unified DTO for a property-for-rent ad, covering residential, commercial, and external partner ads.
/// This model is designed to handle multiple schemas by making type-specific fields nullable and using 'object' for polymorphic properties.
/// Used for schemas: 'realestate-letting', 'realestate-letting-external', 'realestate-business-letting'.
/// </summary>
public class RealEstateLettingAdDto
{
    /// <summary>
    /// The full title of the ad.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Information about the building's area/size.
    /// </summary>
    [JsonPropertyName("size")]
    public SizeDto? Size { get; set; }

    /// <summary>
    /// Represents a range of area, usually for commercial properties.
    /// </summary>
    [JsonPropertyName("areaRange")]
    public AdAreaRangeDto? AreaRange { get; set; }

    /// <summary>
    /// The floor number the property is on.
    /// </summary>
    [JsonPropertyName("floor")]
    public int? Floor { get; set; }

    /// <summary>
    /// Detailed price information, including monthly rent and deposit.
    /// </summary>
    [JsonPropertyName("price")]
    public DetailedPriceDto? Price { get; set; }

    /// <summary>
    /// The total number of bedrooms.
    /// </summary>
    [JsonPropertyName("bedrooms")]
    public int? Bedrooms { get; set; }

    /// <summary>
    /// The number of separate office spaces.
    /// </summary>
    [JsonPropertyName("officeSpaces")]
    public int? OfficeSpaces { get; set; }

    /// <summary>
    /// A list of all images for the ad.
    /// </summary>
    [JsonPropertyName("images")]
    public List<AdImageDto>? Images { get; set; }
    
    /// <summary>
    /// Contact information for the advertiser.
    /// </summary>
    [JsonPropertyName("contact")]
    public List<ContactDto>? Contact { get; set; }
    
    /// <summary>
    /// Information and links related to the digital rental contract.
    /// </summary>
    [JsonPropertyName("contract")]
    public ContractDto? Contract { get; set; }
    
    /// <summary>
    /// Indicates if the property is no longer available.
    /// </summary>
    [JsonPropertyName("disposed")]
    public bool? Disposed { get; set; }

    /// <summary>
    /// Detailed location information for the property.
    /// </summary>
    [JsonPropertyName("location")]
    public AdViewLocationDto? Location { get; set; }
    
    /// <summary>
    /// The time period for which the property is available for rent.
    /// </summary>
    [JsonPropertyName("timespan")]
    public TimespanDto? Timespan { get; set; }
    
    /// <summary>
    /// A list of scheduled viewing times.
    /// </summary>
    [JsonPropertyName("viewings")]
    public List<ViewingDto>? Viewings { get; set; }
    
    /// <summary>
    /// Indicates if the advertiser is anonymous.
    /// </summary>
    [JsonPropertyName("anonymous")]
    public bool? Anonymous { get; set; }

    /// <summary>
    /// The specific type of the ad view.
    /// Example: "realestate-letting", "realestate-business-letting"
    /// </summary>
    [JsonPropertyName("adViewType")]
    public string? AdViewType { get; set; }
    
    /// <summary>
    /// A human-readable label for the ad view type.
    /// Example: "Bolig til leie"
    /// </summary>
    [JsonPropertyName("adViewTypeLabel")]
    public string? AdViewTypeLabel { get; set; }
    
    /// <summary>
    /// A list of facility keywords. Can be strings or IdValue objects.
    /// Example (string list): ["Balkong/Terrasse", "Moderne"]
    /// Example (object list): [{"id": 1, "value": "Garasje/P-plass"}]
    /// </summary>
    [JsonPropertyName("facilities")]
    public object? Facilities { get; set; }
    
    /// <summary>
    /// A list of floor plan images.
    /// </summary>
    [JsonPropertyName("floorplans")]
    public List<object>? Floorplans { get; set; }
    
    /// <summary>
    /// The furnishing status of the property. Can be a string or an object with id/value.
    /// Example (string): "Møblert"
    /// Example (object): {"id": 1, "value": "Møblert"}
    /// </summary>
    [JsonPropertyName("furnishing")]
    public object? Furnishing { get; set; }
    
    /// <summary>
    /// The energy efficiency rating of the property.
    /// </summary>
    [JsonPropertyName("energyLabel")]
    public EnergyLabelDto? EnergyLabel { get; set; }
    
    /// <summary>
    /// Additional property information, often a summary or highlights.
    /// </summary>
    [JsonPropertyName("propertyInfo")]
    public List<GeneralTextSectionDto>? PropertyInfo { get; set; }
    
    /// <summary>
    /// The type of property. Can be a string, a list of strings, or an object with id/value.
    /// Example (string): "Leilighet"
    /// Example (list): ["Kontor"]
    /// </summary>
    [JsonPropertyName("propertyType")]
    public object? PropertyType { get; set; }
    
    /// <summary>
    /// Links to more information.
    /// </summary>
    [JsonPropertyName("moreInfoLinks")]
    public List<MoreInfoLinkDto>? MoreInfoLinks { get; set; }
    
    /// <summary>
    /// Indicates if animals are allowed in the property.
    /// </summary>
    [JsonPropertyName("animalsAllowed")]
    public bool? AnimalsAllowed { get; set; }
    
    /// <summary>
    /// Indicates if the landlord participates in the "FINN Hjerterom" initiative for refugees.
    /// </summary>
    [JsonPropertyName("refugeesWelcome")]
    public bool? RefugeesWelcome { get; set; }

    /// <summary>
    /// The description of the property, typically for external or business ads.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The external partner's ID for the ad.
    /// </summary>
    [JsonPropertyName("externalAdId")]
    public string? ExternalAdId { get; set; }
    
    /// <summary>
    /// The name of the external partner providing the ad.
    /// Example: "Qasa"
    /// </summary>
    [JsonPropertyName("externalPartnerName")]
    public string? ExternalPartnerName { get; set; }
    
    /// <summary>
    /// The year the property was most recently renovated.
    /// </summary>
    [JsonPropertyName("renovatedYear")]
    public int? RenovatedYear { get; set; }

    /// <summary>
    /// The number of available parking spots.
    /// </summary>
    [JsonPropertyName("parkingSpots")]
    public int? ParkingSpots { get; set; }

    /// <summary>
    /// Information about the acquisition/takeover process for commercial lettings.
    /// </summary>
    [JsonPropertyName("acquisition")]
    public AcquisitionDto? Acquisition { get; set; }

    /// <summary>
    /// Description of how to access the property, may contain HTML.
    /// </summary>
    [JsonPropertyName("accessUnsafe")]
    public string? AccessUnsafe { get; set; }

    /// <summary>
    /// A general description of the property's condition, may contain HTML.
    /// </summary>
    [JsonPropertyName("conditionUnsafe")]
    public string? ConditionUnsafe { get; set; }

    /// <summary>
    /// Description of the property's situation/location, may contain HTML.
    /// </summary>
    [JsonPropertyName("situationUnsafe")]
    public string? SituationUnsafe { get; set; }
}
```

#### 3.4. Real Estate Plot DTO (Unified)

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstatePlotAdDto.cs
//

using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstate;

/// <summary>
/// Unified DTO for the detailed view of a real estate plot (tomt) for sale.
/// This model handles residential, leisure, and commercial plots.
/// Used for schemas: 'realestate-plot', 'realestate-leisure-plot', 'realestate-business-plot'.
/// </summary>
public class RealEstatePlotAdDto
{
    /// <summary>
    /// The full title of the ad.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Information about the property's plot.
    /// </summary>
    [JsonPropertyName("plot")]
    public PlotDto? Plot { get; set; }

    /// <summary>
    /// Detailed price information for the plot. Can be null for "price on request".
    /// </summary>
    [JsonPropertyName("price")]
    public DetailedPriceDto? Price { get; set; }

    /// <summary>
    /// A list of all images for the ad.
    /// </summary>
    [JsonPropertyName("images")]
    public List<AdImageDto>? Images { get; set; }

    /// <summary>
    /// Contact information for the advertiser. Can be an empty list.
    /// </summary>
    [JsonPropertyName("contact")]
    public List<ContactDto>? Contact { get; set; }

    /// <summary>
    /// Indicates if the plot is sold or disposed.
    /// </summary>
    [JsonPropertyName("disposed")]
    public bool? Disposed { get; set; }

    /// <summary>
    /// Detailed location information for the plot.
    /// </summary>
    [JsonPropertyName("location")]
    public AdViewLocationDto? Location { get; set; }

    /// <summary>
    /// A list of scheduled viewing times. Can be an empty list.
    /// </summary>
    [JsonPropertyName("viewings")]
    public List<ViewingDto>? Viewings { get; set; }

    /// <summary>
    /// Indicates if the advertiser is anonymous.
    /// </summary>
    [JsonPropertyName("anonymous")]
    public bool? Anonymous { get; set; }

    /// <summary>
    /// Cadastral information (matrikkelinformasjon) for the plot. Can be an empty list.
    /// </summary>
    [JsonPropertyName("cadastres")]
    public List<CadastreDto>? Cadastres { get; set; }

    /// <summary>
    /// A list of documents associated with the ad. Can be an empty list.
    /// </summary>
    [JsonPropertyName("documents")]
    public List<DocumentDto>? Documents { get; set; }
    
    /// <summary>
    /// Links and information related to electronic bidding.
    /// </summary>
    [JsonPropertyName("electronicBid")]
    public ElectronicBidDto? ElectronicBid { get; set; }

    /// <summary>
    /// The specific type of the ad view.
    /// Example: "realestate-plot", "realestate-leisure-plot", "realestate-business-plot"
    /// </summary>
    [JsonPropertyName("adViewType")]
    public string? AdViewType { get; set; }

    /// <summary>
    /// A list of facility keywords. Can be an empty list.
    /// </summary>
    [JsonPropertyName("facilities")]
    public List<string>? Facilities { get; set; }

    /// <summary>
    /// A list of descriptive text sections with headings from the ad. Can be an empty list.
    /// </summary>
    [JsonPropertyName("generalText")]
    public List<GeneralTextSectionDto>? GeneralText { get; set; }

    /// <summary>
    /// Additional property information, often a summary or highlights.
    /// </summary>
    [JsonPropertyName("propertyInfo")]
    public List<GeneralTextSectionDto>? PropertyInfo { get; set; }

    /// <summary>
    /// The type of property.
    /// Example: "Tomter", "Hyttetomt", "Næringstomt"
    /// </summary>
    [JsonPropertyName("propertyType")]
    public string? PropertyType { get; set; }

    /// <summary>
    /// A list of email addresses for the seller. Can be an empty list.
    /// </summary>
    [JsonPropertyName("sellerEmails")]
    public List<string>? SellerEmails { get; set; }

    /// <summary>
    /// Links to more information. Can be an empty list.
    /// </summary>
    [JsonPropertyName("moreInfoLinks")]
    public List<MoreInfoLinkDto>? MoreInfoLinks { get; set; }

    /// <summary>
    /// A human-readable label for the ad view type.
    /// Example: "Tomter til salgs", "Fritidstomter til salgs"
    /// </summary>
    [JsonPropertyName("adViewTypeLabel")]
    public string? AdViewTypeLabel { get; set; }

    /// <summary>
    /// Description of the property's situation/location, may contain HTML.
    /// </summary>
    [JsonPropertyName("situationUnsafe")]
    public string? SituationUnsafe { get; set; }

    /// <summary>
    /// Text describing the zoning and regulation details.
    /// </summary>
    [JsonPropertyName("regulations")]
    public string? Regulations { get; set; }

    /// <summary>
    /// An external ID used by the advertiser.
    /// </summary>
    [JsonPropertyName("externalAdId")]
    public string? ExternalAdId { get; set; }

    /// <summary>
    /// The advertiser's internal reference number.
    /// </summary>
    [JsonPropertyName("advertiserRef")]
    public string? AdvertiserRef { get; set; }

    /// <summary>
    // The type of trade for a commercial plot.
    /// Example: "SELL"
    /// </summary>
    [JsonPropertyName("plotTradeType")]
    public string? PlotTradeType { get; set; }

    /// <summary>
    /// Description of how to access the property, may contain HTML.
    /// </summary>
    [JsonPropertyName("accessUnsafe")]
    public string? AccessUnsafe { get; set; }
    
    /// <summary>
    /// Represents a range of area, usually for commercial or multi-unit properties.
    /// </summary>
    [JsonPropertyName("areaRange")]
    public AdAreaRangeDto? AreaRange { get; set; }
}
```

#### 3.5. Real Estate Planned DTOs (`schemaName: realestate-planned`)

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstatePlannedAdDto.cs
//

using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstate;

/// <summary>
/// DTO for a planned real estate project that is not yet under construction.
/// </summary>
public class RealEstatePlannedAdDto
{
    /// <summary>
    /// The main title of the planned project ad.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    /// <summary>
    /// A list of images or illustrations for the planned project.
    /// </summary>
    [JsonPropertyName("images")]
    public List<AdImageDto> Images { get; set; }
    
    /// <summary>
    /// Information about the estimated timeline for the project.
    /// </summary>
    [JsonPropertyName("phases")]
    public ProjectPhasesDto? Phases { get; set; }
    
    /// <summary>
    /// Location information for the planned project.
    /// </summary>
    [JsonPropertyName("location")]
    public AdViewLocationDto Location { get; set; }

    /// <summary>
    /// Indicates if the property is a leisure/holiday home.
    /// </summary>
    [JsonPropertyName("isLeisure")]
    public bool? IsLeisure { get; set; }

    /// <summary>
    /// The specific type of the ad view.
    /// Example: "realestate-planned"
    /// </summary>
    [JsonPropertyName("adViewType")]
    public string AdViewType { get; set; }
    
    /// <summary>
    /// The name of the project.
    /// </summary>
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; }
    
    /// <summary>
    /// The starting number of bedrooms in the planned range.
    /// </summary>
    [JsonPropertyName("bedroomsFrom")]
    public int? BedroomsFrom { get; set; }

    /// <summary>
    /// The ending number of bedrooms in the planned range.
    /// </summary>
    [JsonPropertyName("bedroomsTo")]
    public int? BedroomsTo { get; set; }

    /// <summary>
    /// An external ID used by the advertiser.
    /// </summary>
    [JsonPropertyName("externalAdId")]
    public string? ExternalAdId { get; set; }
    
    /// <summary>
    /// A URL to the project's external website.
    /// Example: "https://solis-konnerud.no"
    /// </summary>
    [JsonPropertyName("externalUrl")]
    public string? ExternalUrl { get; set; }
    
    /// <summary>
    /// Descriptive text sections for the project.
    /// </summary>
    [JsonPropertyName("propertyInfo")]
    public List<GeneralTextSectionDto> PropertyInfo { get; set; }
    
    /// <summary>
    /// A list of property types included in the project.
    /// Example: ["Enebolig","Leilighet"]
    /// </summary>
    [JsonPropertyName("propertyType")]
    public List<string> PropertyType { get; set; }

    /// <summary>
    /// The advertiser's internal reference number.
    /// </summary>
    [JsonPropertyName("advertiserRef")]
    public string? AdvertiserRef { get; set; }

    /// <summary>
    /// Links to more information, such as prospectus orders or viewing registrations.
    /// </summary>
    [JsonPropertyName("moreInfoLinks")]
    public List<MoreInfoLinkDto>? MoreInfoLinks { get; set; }

    /// <summary>
    /// The type of ownership for the units.
    /// Example: "Eier (Selveier)"
    /// </summary>
    [JsonPropertyName("ownershipType")]
    public string OwnershipType { get; set; }
    
    /// <summary>
    /// The name of the local area or neighborhood.
    /// Example: "Konnerud"
    /// </summary>
    [JsonPropertyName("localAreaName")]
    public string? LocalAreaName { get; set; }

    /// <summary>
    /// A human-readable label for the ad type.
    /// Example: "Nybygg planlagt"
    /// </summary>
    [JsonPropertyName("adViewTypeLabel")]
    public string? AdViewTypeLabel { get; set; }
}
```

#### 3.6. Real Estate Development Project DTOs (`schemaName: realestate-development-project`, `realestate-development-project-leisure`)

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstateDevelopmentAdDto.cs
//

using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstate;

/// <summary>
/// DTO for the detailed view of a real estate development project, including leisure projects.
/// Used for schemas: 'realestate-development-project' and 'realestate-development-project-leisure'.
/// </summary>
public class RealEstateDevelopmentProjectAdDto
{
    /// <summary>
    /// The full title of the project ad.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// A short description or tagline for the project.
    /// </summary>
    [JsonPropertyName("shortDescription")]
    public string? ShortDescription { get; set; }

    /// <summary>
    /// A list of images and illustrations for the project.
    /// </summary>
    [JsonPropertyName("images")]
    public List<AdImageDto> Images { get; set; }
    
    /// <summary>
    /// Information about the different phases of the development project.
    /// </summary>
    [JsonPropertyName("phases")]
    public ProjectPhasesDto? Phases { get; set; }
    
    /// <summary>
    /// Indicates if the project's units are all sold or disposed.
    /// </summary>
    [JsonPropertyName("disposed")]
    public bool? Disposed { get; set; }
    
    /// <summary>
    /// Detailed location information for the project.
    /// </summary>
    [JsonPropertyName("location")]
    public AdViewLocationDto Location { get; set; }
    
    /// <summary>
    /// Cadastral information for the project's land plot.
    /// </summary>
    [JsonPropertyName("cadastres")]
    public List<CadastreDto>? Cadastres { get; set; }
    
    /// <summary>
    /// Information about scheduled viewings or open houses.
    /// </summary>
    [JsonPropertyName("viewings")]
    public List<ViewingDto>? Viewings { get; set; }
    
    /// <summary>
    /// A URL where potential buyers can register for a viewing.
    /// </summary>
    [JsonPropertyName("viewingRegistrationUrl")]
    public string? ViewingRegistrationUrl { get; set; }
    
    /// <summary>
    /// The specific type of the ad view.
    /// Example: "realestate-development-project", "realestate-development-project-leisure"
    /// </summary>
    [JsonPropertyName("adViewType")]
    public string AdViewType { get; set; }
    
    /// <summary>
    /// A human-readable label for the ad type.
    /// Example: "Nybygg prosjekt", "Nybygg prosjekt fritid"
    /// </summary>
    [JsonPropertyName("adViewTypeLabel")]
    public string? AdViewTypeLabel { get; set; }
    
    /// <summary>
    /// A list of available facilities for the project.
    /// Example: ["Garasje/P-plass", "Hage", "Balkong/terrasse", "Turterreng"]
    /// </summary>
    [JsonPropertyName("facilities")]
    public List<string>? Facilities { get; set; }
    
    /// <summary>
    /// The name of the development project.
    /// </summary>
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; }
    
    /// <summary>
    /// An external ID used by the advertiser.
    /// Example: "PFS-55210224"
    /// </summary>
    [JsonPropertyName("externalAdId")]
    public string? ExternalAdId { get; set; }
    
    /// <summary>
    /// An external ID used by the advertiser, with different casing.
    /// Example: "16.25531"
    /// </summary>
    [JsonPropertyName("externalAdid")]
    public string? ExternalAdid { get; set; }

    /// <summary>
    /// The advertiser's internal reference number.
    /// </summary>
    [JsonPropertyName("advertiserRef")]
    public string? AdvertiserRef { get; set; }
    
    /// <summary>
    /// A list of descriptive text sections for the project.
    /// </summary>
    [JsonPropertyName("propertyInfo")]
    public List<GeneralTextSectionDto>? PropertyInfo { get; set; }
    
    /// <summary>
    /// The type of ownership for the units in the project.
    /// Example: "Eier (Selveier)", "Andel"
    /// </summary>
    [JsonPropertyName("ownershipType")]
    public string OwnershipType { get; set; }

    /// <summary>
    /// The total number of floors in the project's buildings.
    /// </summary>
    [JsonPropertyName("numberOfFloors")]
    public int? NumberOfFloors { get; set; }

    /// <summary>
    /// URL for ordering a prospectus or other materials.
    /// </summary>
    [JsonPropertyName("orderUrl")]
    public string? OrderUrl { get; set; }
    
    /// <summary>
    /// URL to the project's own homepage.
    /// Example: "https://gruahageby.no/"
    /// </summary>
    [JsonPropertyName("projectUrl")]
    public string? ProjectUrl { get; set; }
    
    /// <summary>
    /// Links to more information, like the realtor's site or viewing registration.
    /// </summary>
    [JsonPropertyName("moreInfoLinks")]
    public List<MoreInfoLinkDto>? MoreInfoLinks { get; set; }

    /// <summary>
    /// URL to a virtual tour (e.g., 3D view) of the project.
    /// </summary>
    [JsonPropertyName("virtualViewingUrl")]
    public string? VirtualViewingUrl { get; set; }
    
    /// <summary>
    /// The name of the local area or neighborhood.
    /// Example: "BUDOR/LØTEN"
    /// </summary>
    [JsonPropertyName("localAreaName")]
    public string? LocalAreaName { get; set; }
    
    /// <summary>
    /// A general description of the property's location type.
    /// Example: "På fjellet", "Ved sjøen"
    /// </summary>
    [JsonPropertyName("situation")]
    public string? Situation { get; set; }
    
    /// <summary>
    /// The property's elevation in meters above sea level.
    /// </summary>
    [JsonPropertyName("metersAboveSeaLevel")]
    public int? MetersAboveSeaLevel { get; set; }
    
    /// <summary>
    /// Written directions to the property.
    /// </summary>
    [JsonPropertyName("directionDescription")]
    public string? DirectionDescription { get; set; }
}
```

#### 3.7. BAP / Torget Ad DTOs (`schemaName: recommerce-sell`)

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.BapAdDto.cs
//

using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.Bap;

/// <summary>
/// DTO for a "Buy and Sell" (Torget) ad.
/// </summary>
public class BapAdDto
{
    /// <summary>
    /// The selling price of the item.
    /// Example: 900
    /// </summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>
    /// The title of the ad.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// A list of extra attributes, such as condition or product type.
    /// </summary>
    [JsonPropertyName("extras")]
    public List<BapExtraDto>? Extras { get; set; }

    /// <summary>
    /// A list of images for the item.
    /// </summary>
    [JsonPropertyName("images")]
    public List<AdImageDto> Images { get; set; }

    /// <summary>
    /// The category hierarchy for the item.
    /// </summary>
    [JsonPropertyName("category")]
    public BapCategoryDto Category { get; set; }

    /// <summary>
    /// The location of the item/seller.
    /// </summary>
    [JsonPropertyName("location")]
    public AdViewLocationDto Location { get; set; }
    
    /// <summary>
    /// The condition of the item.
    /// </summary>
    [JsonPropertyName("condition")]
    public BapConditionDto Condition { get; set; }

    /// <summary>
    /// The specific type of the ad view.
    /// Example: "recommerce-sell"
    /// </summary>
    [JsonPropertyName("adViewType")]
    public string AdViewType { get; set; }

    /// <summary>
    /// The detailed description of the item.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class BapExtraDto
{
    /// <summary>
    /// The identifier for the extra attribute.
    /// Example: "condition", "type"
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The human-readable label for the attribute.
    /// Example: "Tilstand", "Produkttype"
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; }

    /// <summary>
    /// The value of the attribute.
    /// Example: "Pent brukt - I god stand"
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }

    /// <summary>
    /// The numeric ID for the attribute value.
    /// </summary>
    [JsonPropertyName("valueId")]
    public int ValueId { get; set; }
}

public class BapCategoryDto
{
    /// <summary>
    /// The numeric ID of the category.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// The name of the category.
    /// Example: "Harddisk og lagring", "Data", "Elektronikk og hvitevarer"
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }

    /// <summary>
    /// The parent category, forming a hierarchy.
    /// </summary>
    [JsonPropertyName("parent")]
    public BapCategoryDto? Parent { get; set; }
}

public class BapConditionDto
{
    /// <summary>
    /// The numeric ID for the condition.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// The human-readable description of the condition.
    /// Example: "Pent brukt - I god stand"
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }
}
```

#### 3.8. Common DTOs for Ad Views

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.AdViewCommonDto.cs
//

using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.AdView.RealEstate;

/// <summary>
/// A generic DTO for key-value pairs where the key is an ID and the value is a string.
/// </summary>
/// <typeparam name="TId">The type of the ID (e.g., int, string).</typeparam>
public class IdValueDto<TId>
{
    /// <summary>
    /// The identifier for the item.
    /// </summary>
    [JsonPropertyName("id")]
    public TId Id { get; set; }

    /// <summary>
    /// The string value or display name for the item.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }
}

/// <summary>
/// Represents a range of area/size, typically for commercial properties or projects.
/// </summary>
public class AdAreaRangeDto
{
    /// <summary>
    /// The ending area size of the range.
    /// </summary>
    [JsonPropertyName("areaTo")]
    public int? AreaTo { get; set; }

    /// <summary>
    /// The starting area size of the range.
    /// </summary>
    [JsonPropertyName("areaFrom")]
    public int? AreaFrom { get; set; }
}

public class PlotDto
{
    /// <summary>
    /// The area of the plot in square meters.
    /// </summary>
    [JsonPropertyName("area")]
    public long? Area { get; set; }

    /// <summary>
    /// Indicates if the plot is owned (true) or leased (false).
    /// </summary>
    [JsonPropertyName("owned")]
    public bool? Owned { get; set; }
    
    /// <summary>
    /// Indicates if the plot is a leasehold point.
    /// </summary>
    [JsonPropertyName("leaseholdPoint")]
    public bool? LeaseholdPoint { get; set; }

    /// <summary>
    /// Description of the plot's condition, may contain HTML.
    /// </summary>
    [JsonPropertyName("conditionUnsafe")]
    public string? ConditionUnsafe { get; set; }

    /// <summary>
    /// The annual leasehold fee (festeavgift).
    /// </summary>
    [JsonPropertyName("leaseholdFee")]
    public long? LeaseholdFee { get; set; }

    /// <summary>
    /// The year the leasehold agreement expires.
    /// </summary>
    [JsonPropertyName("leaseholdYear")]
    public int? LeaseholdYear { get; set; }
}

public class SizeDto
{
    /// <summary>
    /// The usable area (BRA) in square meters.
    /// </summary>
    [JsonPropertyName("usable")]
    public int? Usable { get; set; }

    /// <summary>
    /// The primary room area (P-ROM) in square meters.
    /// </summary>
    [JsonPropertyName("primary")]
    public int? Primary { get; set; }

    /// <summary>
    /// Gross area (BTA) of the property.
    /// </summary>
    [JsonPropertyName("gross")]
    public int? Gross { get; set; }
    
    /// <summary>
    /// Living area (BOA) of the property, often same as P-ROM.
    /// </summary>
    [JsonPropertyName("living")]
    public int? Living { get; set; }

    /// <summary>
    /// A detailed HTML breakdown of the area calculation.
    /// </summary>
    [JsonPropertyName("descriptionUnsafe")]
    public string? DescriptionUnsafe { get; set; }
    
    /// <summary>
    /// External usable area (e.g., balconies and others), in square meters.
    /// </summary>
    [JsonPropertyName("usableAreaE")]
    public int? UsableAreaE { get; set; }

    /// <summary>
    /// Internal usable area, in square meters.
    /// </summary>
    [JsonPropertyName("usableAreaI")]
    public int? UsableAreaI { get; set; }

    /// <summary>
    /// The open area (TBA - Terrasse/Balkong Areal) in square meters.
    /// </summary>
    [JsonPropertyName("openArea")]
    public int? OpenArea { get; set; }

    /// <summary>
    /// Built-in usable area (e.g., in-glassed balcony), in square meters.
    /// </summary>
    [JsonPropertyName("usableAreaB")]
    public int? UsableAreaB { get; set; }

    /// <summary>
    /// Area of a specific floor, sometimes used in multi-level units.
    /// </summary>
    [JsonPropertyName("floor")]
    public int? Floor { get; set; }
}

public class DetailedPriceDto
{
    /// <summary>
    /// The total price including all costs.
    /// </summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>
    /// The currency code.
    /// Example: "NOK"
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
    
    /// <summary>
    /// The property tax value (formuesverdi).
    /// </summary>
    [JsonPropertyName("taxValue")]
    public long? TaxValue { get; set; }

    /// <summary>
    /// The asking price (prisantydning).
    /// </summary>
    [JsonPropertyName("suggestion")]
    public long? Suggestion { get; set; }
    
    /// <summary>
    /// The sum of all sales costs (omkostninger).
    /// </summary>
    [JsonPropertyName("salesCostSum")]
    public long? SalesCostSum { get; set; }
    
    /// <summary>
    /// The estimated value of the property.
    /// </summary>
    [JsonPropertyName("estimatedValue")]
    public long? EstimatedValue { get; set; }
    
    /// <summary>
    /// A detailed HTML breakdown of the costs.
    /// </summary>
    [JsonPropertyName("salesCostIncludesUnsafe")]
    public string? SalesCostIncludesUnsafe { get; set; }
    
    /// <summary>
    /// The annual municipal fees (kommunale avgifter).
    /// Example: 11552
    /// </summary>
    [JsonPropertyName("municipalFees")]
    public long? MunicipalFees { get; set; }

    /// <summary>
    /// The share of collective assets (andel fellesformue).
    /// Example: 43003
    /// </summary>
    [JsonPropertyName("collectiveAssets")]
    public long? CollectiveAssets { get; set; }

    /// <summary>
    /// The share of collective debt (andel fellesgjeld).
    /// Example: 359000
    /// </summary>
    [JsonPropertyName("collectiveDebt")]
    public long? CollectiveDebt { get; set; }
    
    /// <summary>
    /// The property tax (eiendomsskatt).
    /// Example: 829
    /// </summary>
    [JsonPropertyName("estateTax")]
    public long? EstateTax { get; set; }

    /// <summary>
    /// Indicates if shared costs are hedged against interest rate changes.
    /// </summary>
    [JsonPropertyName("sharedCostHedge")]
    public bool? SharedCostHedge { get; set; }

    /// <summary>
    /// Fare for the loan associated with the property.
    /// </summary>
    [JsonPropertyName("loanFare")]
    public long? LoanFare { get; set; }

    /// <summary>
    /// The total loan value on the property.
    /// </summary>
    [JsonPropertyName("loanValue")]
    public long? LoanValue { get; set; }
    
    /// <summary>
    /// For letting ads, the required security deposit amount. Note: This is often a string.
    /// Example: "45000"
    /// </summary>
    [JsonPropertyName("deposit")]
    public string? Deposit { get; set; }

    /// <summary>
    /// For letting ads, the monthly rent amount.
    /// Example: 15000
    /// </summary>
    [JsonPropertyName("monthly")]
    public long? Monthly { get; set; }

    /// <summary>
    /// For letting ads, specifies what is included in the rent.
    /// Example: "Oppvarming", "Internett"
    /// </summary>
    [JsonPropertyName("includes")]
    public string? Includes { get; set; }
}

public class AdImageDto : ImageDto
{
    /// <summary>
    /// A description or caption for the image.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// The URI for the image, often used as an alternative to url/path.
    /// </summary>
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}

public class AdViewLocationDto
{
    /// <summary>
    /// Detailed geographical position information.
    /// </summary>
    [JsonPropertyName("position")]
    public PositionDto Position { get; set; }

    /// <summary>
    /// The postal code for the address.
    /// Example: "2910"
    /// </summary>
    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; }

    /// <summary>
    /// The name of the postal area.
    /// Example: "Aurdal"
    /// </summary>
    [JsonPropertyName("postalName")]
    public string PostalName { get; set; }
    
    /// <summary>
    /// The street address.
    /// Example: "Vestringslinna 895"
    /// </summary>
    [JsonPropertyName("streetAddress")]
    public string? StreetAddress { get; set; }
    
    /// <summary>
    /// The country code.
    /// Example: "NO"
    /// </summary>
    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// The country name.
    /// Example: "Norge"
    /// </summary>
    [JsonPropertyName("countryName")]
    public string? CountryName { get; set; }
}

public class PositionDto
{
    /// <summary>
    /// The latitude of the location.
    /// </summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    /// <summary>
    /// The longitude of the location.
    /// </summary>
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }

    /// <summary>
    /// The accuracy level of the coordinates.
    /// </summary>
    [JsonPropertyName("accuracy")]
    public int Accuracy { get; set; }
    
    /// <summary>
    /// A dictionary of links to different map views. Keys are map types.
    /// Example Keys: "norortho", "finnhybrid", "finnvector"
    /// </summary>
    [JsonPropertyName("links")]
    public Dictionary<string, MapLinkDto>? Links { get; set; }
    
    /// <summary>
    /// URL to a static map image.
    /// </summary>
    [JsonPropertyName("mapImage")]
    public string? MapImage { get; set; }
}

public class MapLinkDto
{
    /// <summary>
    /// The URL for the map view.
    /// Example: "https://www.finn.no/map?adId=412915499&lat=60.949&..."
    /// </summary>
    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    /// <summary>
    /// The title of the map view.
    /// Example: "Flyfoto", "Hybridkart"
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
}

public class ViewingDto
{
    /// <summary>
    /// The end time of the viewing.
    /// Example: "13:00"
    /// </summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>
    /// The date of the viewing in dd.MM.yyyy format.
    /// Example: "22.06.2025"
    /// </summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>
    /// The start time of the viewing.
    /// Example: "11:00"
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>
    /// The date of the viewing in ISO 8601 format.
    /// </summary>
    [JsonPropertyName("dateIso")]
    public DateTimeOffset? DateIso { get; set; }
    
    /// <summary>
    /// Viewing end time in iCalendar format (UTC).
    /// Example: "20250629T130000Z"
    /// </summary>
    [JsonPropertyName("iCalendarTo")]
    public string? ICalendarTo { get; set; }

    /// <summary>
    /// Viewing start time in iCalendar format (UTC).
    /// Example: "20250629T120000Z"
    /// </summary>
    [JsonPropertyName("iCalendarFrom")]
    public string? ICalendarFrom { get; set; }
    
    /// <summary>
    /// A descriptive note about the viewing arrangements.
    /// Example: "Visning etter avtale"
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}

public class CadastreDto
{
    /// <summary>
    /// The land number (gårdsnummer).
    /// </summary>
    [JsonPropertyName("landNumber")]
    public int? LandNumber { get; set; }

    /// <summary>
    /// The title number (bruksnummer).
    /// </summary>
    [JsonPropertyName("titleNumber")]
    public int? TitleNumber { get; set; }
    
    /// <summary>
    /// The leasehold number (festenummer), if applicable.
    /// </summary>
    [JsonPropertyName("leaseholdNumber")]
    public int? LeaseholdNumber { get; set; }
    
    /// <summary>
    /// The section number (seksjonsnummer), if applicable.
    /// </summary>
    [JsonPropertyName("sectionNumber")]
    public int? SectionNumber { get; set; }

    /// <summary>
    /// The municipality number (kommunenummer).
    /// </summary>
    [JsonPropertyName("municipalityNumber")]
    public int MunicipalityNumber { get; set; }
    
    /// <summary>
    /// The part ownership number (festenummer), if applicable.
    /// </summary>
    [JsonPropertyName("partOwnershipNumber")]
    public int? PartOwnershipNumber { get; set; }

    /// <summary>
    /// The apartment number (leilighetsnummer), if applicable.
    /// </summary>
    [JsonPropertyName("apartmentNumber")]
    public string? ApartmentNumber { get; set; }
}

public class EnergyLabelDto
{
    /// <summary>
    /// The energy class rating. This can be a simple string or an object with id/value.
    /// Example (string): "A", "G"
    /// Example (object): { "id": 1, "value": "A" }
    /// </summary>
    [JsonPropertyName("class")]
    public object Class { get; set; }

    /// <summary>
    /// The color associated with the energy class.
    /// Example: "RED", "GREEN", "DARK_GREEN"
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }
}

public class GeneralTextSectionDto
{
    /// <summary>
    /// The heading of the text section.
    /// Example: "Kort om eiendommen", "Beliggenhet"
    /// </summary>
    [JsonPropertyName("heading")]
    public string? Heading { get; set; }

    /// <summary>
    /// The content of the section as an HTML string.
    /// </summary>
    [JsonPropertyName("textUnsafe")]
    public string? TextUnsafe { get; set; }
    
    /// <summary>
    /// An alternative title for the section, used in PropertyInfo.
    /// Example: "Beskrivelse", "Standard", "Adkomst"
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Indicates if the content is HTML.
    /// </summary>
    [JsonPropertyName("isHtml")]
    public bool? IsHtml { get; set; }
    
    /// <summary>
    /// An alternative content field, used in PropertyInfo.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class ContactDto
{
    /// <summary>
    /// The mobile phone number of the contact.
    /// </summary>
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; }
}

public class DocumentDto
{
    /// <summary>
    /// The URL of the document.
    /// </summary>
    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    /// <summary>
    /// A description of the document.
    /// Example: "Tilstandsrapport_12938-1403_1"
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class SharedCostDto
{
    /// <summary>
    /// The amount of the shared cost.
    /// </summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    /// <summary>
    /// A description (potentially HTML) of what the shared cost includes.
    /// </summary>
    [JsonPropertyName("includesUnsafe")]
    public string IncludesUnsafe { get; set; }

    /// <summary>
    /// The shared cost amount after any initial interest-only period ends.
    /// </summary>
    [JsonPropertyName("amountAfterInterestonlyPeriod")]
    public long? AmountAfterInterestonlyPeriod { get; set; }
}

public class MoreInfoLinkDto
{
    /// <summary>
    /// The URL of the link.
    /// </summary>
    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    /// <summary>
    /// The display text for the link.
    /// Example: "Gi bud", "Bestill salgsoppgave"
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
}

/// <summary>
/// Contains URLs related to electronic bidding for the property.
/// </summary>
public class ElectronicBidDto
{
    /// <summary>
    /// The direct URL to the electronic bidding platform.
    /// </summary>
    [JsonPropertyName("bidUrl")]
    public string? BidUrl { get; set; }

    /// <summary>
    /// A URL to a help page or information about the bidding process.
    /// </summary>
    [JsonPropertyName("infoUrl")]
    public string? InfoUrl { get; set; }
}

/// <summary>
/// Represents details of a housing cooperative (borettslag).
/// </summary>
public class HousingCooperativeDto
{
    /// <summary>
    /// The name of the housing cooperative.
    /// Example: "Wexels plass"
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The organization number of the cooperative.
    /// Example: "946514292"
    /// </summary>
    [JsonPropertyName("organisationNumber")]
    public string? OrganisationNumber { get; set; }
}

/// <summary>
/// Represents information regarding the acquisition or takeover of the property.
/// </summary>
public class AcquisitionDto
{
    /// <summary>
    /// A note from the seller about the takeover process.
    /// Example: "Overtakelse etter nærmere avtale..."
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// The takeover date in dd.MM.yyyy format.
    /// Example: "01.01.2026"
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }
    
    /// <summary>
    /// The takeover date in ISO 8601 format.
    /// </summary>
    [JsonPropertyName("fromIso")]
    public DateTimeOffset? FromIso { get; set; }
}

/// <summary>
/// Represents information about a digital rental contract.
/// </summary>
public class ContractDto
{
    /// <summary>
    /// URL to an informational page about the digital contract service.
    /// </summary>
    [JsonPropertyName("infoUrl")]
    public string? InfoUrl { get; set; }

    /// <summary>
    /// A list of features or attributes of the contract service.
    /// Example: ["Digital signering", "Gratis tjeneste"]
    /// </summary>
    [JsonPropertyName("attributes")]
    public List<string>? Attributes { get; set; }

    /// <summary>
    /// The direct URL to create or view the contract for this specific ad.
    /// </summary>
    [JsonPropertyName("contractUrl")]
    public string? ContractUrl { get; set; }
}

/// <summary>
/// Represents the rental period for a property.
/// </summary>
public class TimespanDto
{
    /// <summary>
    /// The end date of the rental period in YYYY-MM-DD format.
    /// </summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>
    /// The start date of the rental period in YYYY-MM-DD format.
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }
}

/// <summary>
/// Represents the estimated timeline and current phase for a real estate project (planned or in development).
/// </summary>
public class ProjectPhasesDto
{
    /// <summary>
    /// The current phase of the project. Only present for active development projects.
    /// Example: "PHASE_SALE_START", "PHASE_MOVE_IN", "PHASE_DEVELOPMENT_START"
    /// </summary>
    [JsonPropertyName("current")]
    public string? Current { get; set; }

    /// <summary>
    /// A description of the planning status or estimated completion.
    /// Example: "Gjennomført", "Q4 2024", "1. Kvartal 2025 (Q1)"
    /// </summary>
    [JsonPropertyName("planning")]
    public string? Planning { get; set; }

    /// <summary>
    /// The estimated start of sales.
    /// Example: "Startet", "Q1 2025", "22.02.2025"
    /// </summary>
    [JsonPropertyName("sale_start")]
    public string? SaleStart { get; set; }

    /// <summary>
    /// The estimated time of acquisition/takeover.
    /// Example: "2023/2024", "Q2 2027", "4. Kvartal 2026"
    /// </summary>
    [JsonPropertyName("acquisition")]
    public string? Acquisition { get; set; }

    /// <summary>
    /// The estimated start of construction.
    /// Example: "2022", "Q2 2025", "3. Kvartal 2025 (Q3)"
    /// </summary>
    [JsonPropertyName("development_start")]
    public string? DevelopmentStart { get; set; }
}
```

### 4. Neighborhood Profile DTOs (`GET /getProfile`)

These models represent the "Om nærområdet" widget data, which is highly structured and polymorphic.

```csharp
//
// File: FinnStatistikk.Infrastructure.Finn.Models.Dto.Profile.NeighbourhoodProfileDto.cs
//

using System.Text.Json;
using System.Text.Json.Serialization;

namespace FinnStatistikk.Infrastructure.Finn.Models.Dto.Profile;

/// <summary>
/// Root DTO for the neighborhood profile widget.
/// </summary>
public class NeighbourhoodProfileDto
{
    /// <summary>
    /// The main heading for the widget.
    /// Example: "Om nærområdet"
    /// </summary>
    [JsonPropertyName("heading")]
    public string Heading { get; set; }
    
    /// <summary>
    /// The version identifier for the widget API.
    /// Example: "finn-widget-api:p.63:2827ee9120b957a9bced23d029fe60c203c8fc9c"
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; }

    /// <summary>
    /// The URL for the "Read More" link.
    /// Example: "https://www.finn.no/areaprofile/412915499?..."
    /// </summary>
    [JsonPropertyName("readMoreLink")]
    public string ReadMoreLink { get; set; }
    
    /// <summary>
    /// A list of cards displaying neighborhood information. The list is polymorphic.
    /// </summary>
    [JsonPropertyName("cards")]
    public List<ProfileCardBaseDto> Cards { get; set; }
    
    /// <summary>
    /// A list of promotional or informational banners.
    /// </summary>
    [JsonPropertyName("banners")]
    public List<BannerDto> Banners { get; set; }
}

// --- Card DTOs (Polymorphic) ---

/// <summary>
/// Custom JsonConverter to handle the polymorphic 'cards' array by inspecting the 'type' property.
/// </summary>
public class ProfileCardConverter : JsonConverter<ProfileCardBaseDto>
{
    public override ProfileCardBaseDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var jsonObject = jsonDoc.RootElement;
        var type = jsonObject.GetProperty("type").GetString();

        return type switch
        {
            "POI" => JsonSerializer.Deserialize<PoiProfileCardDto>(jsonObject.GetRawText(), options),
            "NO-CONTENT" => JsonSerializer.Deserialize<NoContentProfileCardDto>(jsonObject.GetRawText(), options),
            "BUTTON" => JsonSerializer.Deserialize<ButtonProfileCardDto>(jsonObject.GetRawText(), options),
            _ => throw new NotSupportedException($"Card type '{type}' is not supported.")
        };
    }

    public override void Write(Utf8JsonWriter writer, ProfileCardBaseDto value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}

/// <summary>
/// Base class for a card in the neighborhood profile widget.
/// </summary>
[JsonConverter(typeof(ProfileCardConverter))]
public abstract class ProfileCardBaseDto
{
    /// <summary>
    /// The type of the card, used for deserialization.
    /// Example: "POI", "NO-CONTENT", "BUTTON"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

/// <summary>
/// A card displaying Points of Interest (POI).
/// </summary>
public class PoiProfileCardDto : ProfileCardBaseDto
{
    [JsonPropertyName("data")]
    public PoiDataDto Data { get; set; }
}

/// <summary>
/// A card that acts as a prompt or link when no direct POI data is available.
/// </summary>
public class NoContentProfileCardDto : ProfileCardBaseDto
{
    [JsonPropertyName("data")]
    public NoContentDataDto Data { get; set; }
}

/// <summary>
/// A card that is primarily a large call-to-action button.
/// </summary>
public class ButtonProfileCardDto : ProfileCardBaseDto
{
    [JsonPropertyName("data")]
    public ButtonDataDto Data { get; set; }
}


// --- Card Data DTOs ---

public class PoiDataDto
{
    /// <summary>
    /// The title of the POI card.
    /// Example: "Gangavstand til offentlig transport", "Skoler"
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// The text for the link to more details.
    /// Example: "Mer om transport", "Mer om skoler"
    /// </summary>
    [JsonPropertyName("linkText")]
    public string LinkText { get; set; }

    /// <summary>
    /// The URL path for the details link.
    /// Example: "https://www.finn.no/areaprofile/412915499/transport?..."
    /// </summary>
    [JsonPropertyName("linkPath")]
    public string LinkPath { get; set; }

    /// <summary>
    /// A list of specific points of interest with distances.
    /// </summary>
    [JsonPropertyName("pois")]
    public List<PoiItemDto> Pois { get; set; }

    /// <summary>
    /// The name of the SVG icon associated with this card.
    /// Example: "publicTransport", "school"
    /// </summary>
    [JsonPropertyName("svg")]
    public string Svg { get; set; }
}

public class PoiItemDto
{
    /// <summary>
    /// The name of the POI type.
    /// Example: "Buss", "Barneskole", "Dagligvare"
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// The method of travel for the distance calculation.
    /// Example: "walk", "drive"
    /// </summary>
    [JsonPropertyName("distanceType")]
    public string DistanceType { get; set; }

    /// <summary>
    /// The calculated distance or travel time.
    /// Example: "7 min", "8 min"
    /// </summary>
    [JsonPropertyName("distance")]
    public string Distance { get; set; }
}

public class NoContentDataDto
{
    /// <summary>
    /// The main text or question on the card.
    /// Example: "Hva er beste transportalternativ til jobben min?"
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// The text for the call-to-action link.
    /// Example: "Finn reisetider"
    /// </summary>
    [JsonPropertyName("linkText")]
    public string LinkText { get; set; }

    /// <summary>
    /// The URL path for the link.
    /// Example: "https://www.finn.no/areaprofile/412915499/reisetid?..."
    /// </summary>
    [JsonPropertyName("linkPath")]
    public string LinkPath { get; set; }

    /// <summary>
    /// The name of the SVG icon for the card.
    /// Example: "stopwatch", "compare"
    /// </summary>
    [JsonPropertyName("svg")]
    public string Svg { get; set; }
    
    /// <summary>
    /// An optional name of a banner to display.
    /// Example: "travelTimeBanner"
    /// </summary>
    [JsonPropertyName("alternativeBanner")]
    public string? AlternativeBanner { get; set; }
}

public class ButtonDataDto
{
    /// <summary>
    /// The title text on the button card.
    /// Example: "Vil du se hele oversikten?"
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// The text on the button itself.
    /// Example: "Utforsk nabolaget"
    /// </summary>
    [JsonPropertyName("linkText")]
    public string LinkText { get; set; }

    /// <summary>
    /// The URL path for the button's link.
    /// Example: "https://www.finn.no/areaprofile/412915499?..."
    /// </summary>
    [JsonPropertyName("linkPath")]
    public string LinkPath { get; set; }

    /// <summary>
    /// The name of the SVG icon for the card.
    /// Example: "houseWeather"
    /// </summary>
    [JsonPropertyName("svg")]
    public string Svg { get; set; }
}


// --- Banner DTOs ---

public class BannerDto
{
    /// <summary>
    /// The unique name/identifier for the banner.
    /// Example: "travelTimeBanner"
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// The data content of the banner.
    /// </summary>
    [JsonPropertyName("data")]
    public BannerDataDto Data { get; set; }
}

public class BannerDataDto
{
    /// <summary>
    /// The main text of the banner, may contain HTML.
    /// Example: "<strong>Er det trygt der du bor?</strong><br/>Gi oss din vurdering av nabolaget ditt!"
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// The text for the banner's call-to-action link.
    /// Example: "Svar nå"
    /// </summary>
    [JsonPropertyName("linkText")]
    public string LinkText { get; set; }

    /// <summary>
    /// The URL path for the banner's link.
    /// Example: "https://finn.no/nabolag/sporsmal?..."
    /// </summary>
    [JsonPropertyName("linkPath")]
    public string LinkPath { get; set; }

    /// <summary>
    /// The name of the SVG icon for the banner.
    /// Example: "allAges"
    /// </summary>
    [JsonPropertyName("svg")]
    public string Svg { get; set; }
    
    /// <summary>
    /// A CSS class to apply to the button for styling.
    /// Example: "bannerBtnFlexBasis110"
    /// </summary>
    [JsonPropertyName("buttonClass")]
    public string ButtonClass { get; set; }
}
```