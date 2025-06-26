# Catalogue of Analytical Goals

To fulfill the core objective, the platform must be capable of generating a wide range of quantitative and qualitative insights. The following catalogue defines the specific analytical goals that will guide the data collection strategy and database design. Each goal is categorized to ensure a comprehensive view of the market.

---

## **Category 1: Market-Level Analysis (The Big Picture)**

*Goal: To understand the overall health, volume, and momentum of the real estate market at a macro level, identifying systemic trends, risks, and opportunities.*

### **Real Estate Sales Market**

1.  **Metric: Total Active Sales Inventory**
    *   **Objective:** Establish the fundamental baseline of market supply.
    *   **Method:** `COUNT(DISTINCT Ads.FinnAdId)` where `Ads.IsActive = true` AND `MarketId` corresponds to a sales market.
    *   **Data Fields:** `Ads.FinnAdId`, `Ads.IsActive`, `Ads.MarketId`.
    *   **Computation Variants:**
        *   **Granularity:** Daily snapshot.
        *   **Segmentation:** Global, by `MarketId`, `CadastreDto.municipalityNumber`, `AdViewLocationDto.postalCode`, `RealEstateHomeAdDto.propertyType`.
        *   **Visualization:** Historical trend (line chart), current breakdown (bar chart), geospatial map (choropleth).

2.  **Metric: New Sales Listings Inflow Rate**
    *   **Objective:** Measure the velocity of new supply entering the sales market, a key indicator of seller confidence and market dynamism.
    *   **Method:** `COUNT(DISTINCT Ads.FinnAdId)` where `Ads.FirstSeen` falls within a given time window (e.g., last 24 hours, 7 days, 30 days) AND `MarketId` corresponds to a sales market.
    *   **Data Fields:** `Ads.FinnAdId`, `Ads.FirstSeen`, `Ads.MarketId`.
    *   **Computation Variants:**
        *   **Granularity:** Daily, Weekly, Monthly rolling sums.
        *   **Segmentation:** Global, by `MarketId`, `Municipality`, `PropertyType`.
        *   **Visualization:** Trend over time (line chart), comparison with previous periods (e.g., Year-over-Year change).

3.  **Metric: Sold Listings Outflow Rate**
    *   **Objective:** Measure the velocity of demand absorbing the supply.
    *   **Method:** `COUNT(DISTINCT Ads.FinnAdId)` where a snapshot shows `disposed = true` for the first time within a given time window.
    *   **Data Fields:** `Ads.FinnAdId`, `AdSnapshots.Id` (to identify the change), `RealEstateHomeAdDto.disposed`.
    *   **Computation Variants:**
        *   **Granularity:** Daily, Weekly, Monthly rolling sums.
        *   **Segmentation:** Global, by `MarketId`, `Municipality`, `PropertyType`.
        *   **Visualization:** Trend over time (line chart), often overlaid with the Inflow Rate.

4.  **Metric: Net Sales Inventory Flow**
    *   **Objective:** Determine the sales market's direction: expanding (supply > demand) or contracting (demand > supply).
    *   **Method:** `(New Sales Listings Inflow Rate) - (Sold Listings Outflow Rate)` over the same period.
    *   **Data Fields:** Combination of fields from metrics 1.2 and 1.3.
    *   **Computation Variants:**
        *   **Granularity:** Weekly, Monthly.
        *   **Segmentation:** Global, by `MarketId`, `Municipality`.
        *   **Visualization:** Bar chart showing positive/negative flow over time.

5.  **Metric: Median Time on Market (ToM)**
    *   **Objective:** Provide a standardized measure of sales market velocity.
    *   **Method:** For all properties marked as `disposed = true` in a period, calculate the median duration between the first `AdViewHistoryItemDto.broadcasted` date (where `mode = 'PLAY'`) and the `AdSnapshots.ScrapedAt` date when `disposed` first became `true`.
    *   **Data Fields:** `AdViewMetaDto.history`, `RealEstateHomeAdDto.disposed`, `AdSnapshots.ScrapedAt`.
    *   **Computation Variants:**
        *   **Granularity:** Calculated monthly on the cohort of properties sold that month.
        *   **Segmentation:** By `Municipality`, `PropertyType`, `Price` bracket (e.g., <5M, 5-10M, >10M), and by `AdSummaryDocDto.organisation_name`.
        *   **Visualization:** Trend line for key segments, box plot to show distribution.

6.  **Metric: Stale Sales Inventory Ratio**
    *   **Objective:** Identify the proportion of the sales market that is "stuck," a leading indicator of a downturn or pricing disconnect.
    *   **Method:** `COUNT(Active Sales Ads where current_ToM > 90 days) / COUNT(Total Active Sales Ads)`.
    *   **Data Fields:** `Ads.IsActive`, `AdViewMetaDto.history`.
    *   **Computation Variants:**
        *   **Granularity:** Daily snapshot.
        *   **Segmentation:** By `Municipality`, `PropertyType`, Price bracket.
        *   **Visualization:** Trend line (percentage over time), map highlighting areas with high stale ratios.

7.  **Metric: Market Absorption Rate**
    *   **Objective:** Classic real estate metric to forecast how long the current sales inventory would last at the current sales pace.
    *   **Method:** `(Number of Sold Listings in Month) / (Number of Active Sales Listings at Start of Month)`. The inverse gives "Months of Supply."
    *   **Data Fields:** `Ads.IsActive`, `RealEstateHomeAdDto.disposed`, `AdSnapshots.ScrapedAt`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Trend line showing Months of Supply over time for key markets.

8.  **Metric: Median Price per Square Meter (P/Sqm) Index**
    *   **Objective:** The primary benchmark for tracking property value changes, normalized for size.
    *   **Method:** `Median(DetailedPriceDto.suggestion / SizeDto.primary)` for all active sales listings. P-ROM (`size.primary`) is used as the standard.
    *   **Data Fields:** `DetailedPriceDto.suggestion`, `SizeDto.primary`.
    *   **Computation Variants:**
        *   **Granularity:** Daily, with weekly/monthly smoothing.
        *   **Segmentation:** By `Municipality`, `PostalCode`, `PropertyType`, `ConstructionYear` decade.
        *   **Visualization:** Trend line for key segments, choropleth map for geographic comparison.

9.  **Metric: P/Sqm Distribution (Quartile Analysis)**
    *   **Objective:** Understand the market's price structure and stratification beyond a simple median.
    *   **Method:** For each segment, calculate the 10th, 25th (Q1), 50th (Median), 75th (Q3), and 90th percentile for P/Sqm. The Interquartile Range (IQR = Q3 - Q1) reveals the price spread for "typical" properties.
    *   **Data Fields:** `DetailedPriceDto.suggestion`, `SizeDto.primary`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Box-and-whisker plots per segment, candlestick charts showing the IQR over time.

10. **Metric: Price Volatility Index**
    *   **Objective:** Measure market stability and risk. Highly volatile markets may offer high returns but come with increased risk.
    *   **Method:** Calculate the rolling 30-day standard deviation of the daily Median P/Sqm Index.
    *   **Data Fields:** `DetailedPriceDto.suggestion`, `SizeDto.primary`, `AdSnapshots.ScrapedAt`.
    *   **Computation Variants:**
        *   **Granularity:** Daily rolling calculation.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Trend line comparing volatility across different cities.

11. **Metric: Price Reduction Ratio**
    *   **Objective:** Gauge seller sentiment and negotiating power. A high ratio indicates a buyer's market.
    *   **Method:** `COUNT(Ads with at least one price drop) / COUNT(Total Active Sales Ads)`. A price drop is detected by comparing `price.suggestion` between consecutive snapshots for an ad.
    *   **Data Fields:** `AdSnapshots.PriceSuggestion` (custom field in our snapshot table).
    *   **Computation Variants:**
        *   **Granularity:** Daily snapshot.
        *   **Segmentation:** By `Municipality`, `PropertyType`, `Agency`.
        *   **Visualization:** Trend line of the ratio over time.

12. **Metric: Median Price Reduction Magnitude**
    *   **Objective:** Quantify the typical negotiation margin in the market.
    *   **Method:** For all ads that have had a price drop, calculate the median of `(initial_price - current_price) / initial_price`.
    *   **Data Fields:** `AdSnapshots.PriceSuggestion`.
    *   **Computation Variants:**
        *   **Granularity:** Calculated monthly on the cohort of ads with reductions.
        *   **Segmentation:** By `Municipality`, `PropertyType`, `Agency`.
        *   **Visualization:** Bar chart comparing magnitude across cities or top agencies.

13. **Metric: Price Escalation Ratio**
    *   **Objective:** Identify signals of a "hot" market where sellers feel confident raising prices after listing.
    *   **Method:** `COUNT(Ads with at least one price increase) / COUNT(Total Active Sales Ads)`.
    *   **Data Fields:** `AdSnapshots.PriceSuggestion`.
    *   **Computation Variants:**
        *   **Granularity:** Daily snapshot.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Trend line. A spike is a strong market signal.

14. **Metric: "Kommer for Salg" Pipeline Volume**
    *   **Objective:** Track the "shadow inventory" of properties announced but not yet officially for sale to forecast near-term supply.
    *   **Method:** `COUNT(DISTINCT AdSummaryDocDto.ad_id)` where `labels` array contains `id: "coming_for_sale"`.
    *   **Data Fields:** `AdSummaryDocDto.ad_id`, `AdSummaryDocDto.labels`.
    *   **Computation Variants:**
        *   **Granularity:** Daily snapshot.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Trend line showing the size of the future supply pipeline.

15. **Metric: Agency vs. Private Seller Dominance Ratio**
    *   **Objective:** Understand the professionalism and maturity of different regional sales markets.
    *   **Method:** For each `Municipality`, `COUNT(ads where AdSummaryDocDto.organisation_name IS NOT NULL) / COUNT(ads where AdSummaryDocDto.organisation_name IS NULL)`.
    *   **Data Fields:** `AdSummaryDocDto.organisation_name`, `AdSummaryDocDto.labels` (to confirm "private" flag), `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly snapshot.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Geospatial map or ranked bar chart of municipalities.

16. **Metric: Housing Stock Age Profile**
    *   **Objective:** Understand the age distribution of the housing stock to identify macro-level renovation opportunities.
    *   **Method:** Create a histogram of `RealEstateHomeAdDto.constructionYear` binned by decade.
    *   **Data Fields:** `RealEstateHomeAdDto.constructionYear`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly snapshot.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Histogram/bar chart. Comparing profiles of different cities can be very revealing.

17. **Metric: Property Type Market Share**
    *   **Objective:** Track long-term structural shifts in housing supply (e.g., trend towards apartments vs. houses).
    *   **Method:** `COUNT(ads of PropertyTypeX) / COUNT(Total Sales Ads)` for each region.
    *   **Data Fields:** `RealEstateHomeAdDto.propertyType`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly, Annually.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Stacked area chart showing market share evolution over time.

18. **Metric: New Development Supply Ratio**
    *   **Objective:** Measure the impact of new construction on total market supply.
    *   **Method:** `COUNT(ads where schemaName includes 'development' or 'planned') / COUNT(Total Active Sales Ads)`.
    *   **Data Fields:** `AdViewMetaDto.schemaName`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly snapshot.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Trend line.

19. **Metric: Market Energy Efficiency Profile (Green Index)**
    *   **Objective:** Track the "green transition" of the housing market and quantify the "energy debt" of older housing stock.
    *   **Method:** Generate a distribution chart of `energyLabel.class` (A-G) for all active sales listings. Calculate a market-wide weighted average "Green Index" (e.g., A=7, G=1).
    *   **Data Fields:** `EnergyLabelDto.class`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, `ConstructionYear` decade.
        *   **Visualization:** Stacked bar chart showing the distribution of energy labels, and a trend line for the "Green Index."

20. **Metric: Rental-to-Sales Listing Ratio**
    *   **Objective:** Identify regions where rental demand is structurally high relative to the for-sale market, suggesting strong rental investment potential.
    *   **Method:** `COUNT(active 'realestate-letting' ads) / COUNT(active 'realestate-homes' ads)` for each `Municipality`.
    *   **Data Fields:** `AdViewMetaDto.schemaName`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly snapshot.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Ranked bar chart or geospatial map.

21. **Metric: Sales Process Complexity Index**
    *   **Objective:** Quantify the prevalence of competitive or restrictive sales processes.
    *   **Method:** `COUNT(ads where preemption IS NOT NULL OR electronicBid.bidUrl IS NOT NULL OR housingCooperative IS NOT NULL) / COUNT(Total Ads)`.
    *   **Data Fields:** `RealEstateHomeAdDto.preemption`, `ElectronicBidDto.bidUrl`, `HousingCooperativeDto`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Segmentation:** By `Municipality`, `PropertyType` (e.g., `Andel` properties will have higher complexity).
        *   **Visualization:** Bar chart comparing complexity across segments.

22. **Metric: Failed Sale & Relisting Ratio**
    *   **Objective:** A powerful signal of market weakness or systematic pricing failure.
    *   **Method:** Identify ads with a new `FinnAdId` where `location.streetAddress`, `size.usable`, and `cadastres` match an ad that was active but became inactive (without being marked `disposed`) within the last 90 days.
    *   **Data Fields:** `Ads.FinnAdId`, `Ads.IsActive`, `RealEstateHomeAdDto.disposed`, `AdViewLocationDto.streetAddress`, `SizeDto.usable`, `CadastreDto`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Segmentation:** By `Municipality`, `Agency`. (A high relisting rate for a specific agency is a major red flag).
        *   **Visualization:** Trend line of the ratio.

23. **Metric: Asking Price Realization Proxy**
    *   **Objective:** Estimate how close sellers get to their initial asking price, a proxy for negotiating power.
    *   **Method:** For sold properties, calculate the median of `(final_asking_price / initial_asking_price)`. The `final_asking_price` is the `price.suggestion` from the snapshot just before `disposed` becomes `true`. The `initial_asking_price` is from the first snapshot.
    *   **Data Fields:** `AdSnapshots.PriceSuggestion`, `RealEstateHomeAdDto.disposed`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Segmentation:** By `Municipality`, `Agency`.
        *   **Visualization:** Comparison bar chart.

24. **Metric: Seasonal Fluctuation Index**
    *   **Objective:** Identify and quantify seasonal patterns in market activity and pricing to inform timing decisions.
    *   **Method:** For each month, calculate the average deviation from the annual mean for key metrics (e.g., New Listings Inflow, Median P/Sqm). For example, `(Avg. New Listings in May) / (Avg. Annual Monthly New Listings)`.
    *   **Data Fields:** `Ads.FirstSeen`, `DetailedPriceDto.suggestion`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly index, updated annually based on several years of data.
        *   **Segmentation:** By `Municipality` (e.g., Oslo's seasonality may differ from a ski resort's).
        *   **Visualization:** Line chart showing the index value for each month of the year.

25. **Metric: Ownership Type Dominance**
    *   **Objective:** Understand the underlying cost structures and legal frameworks of regional markets.
    *   **Method:** Calculate the market share of each `ownershipType` (e.g., 'Eier (Selveier)', 'Andel', 'Aksje').
    *   **Data Fields:** `RealEstateHomeAdDto.ownershipType`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Pie chart for a snapshot, or stacked area chart to show trends over time.

26. **Metric: Leased Land Exposure Index**
    *   **Objective:** Quantify the prevalence of leasehold plots (`festetomt`), a significant risk factor for buyers.
    *   **Method:** `COUNT(ads where plot.owned = false) / COUNT(Total Ads)`.
    *   **Data Fields:** `PlotDto.owned`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, `PropertyType` (e.g., common for `Hytte`).
        *   **Visualization:** Map highlighting areas with high exposure to leased land.

27. **Metric: Market-wide Renovation Wave Index**
    *   **Objective:** Identify macro trends in housing stock upgrades.
    *   **Method:** Calculate `COUNT(ads where renovatedYear is in the last 3 years) / COUNT(Total Active Ads)`.
    *   **Data Fields:** `RealEstateHomeAdDto.renovatedYear`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, `ConstructionYear` decade (to see which era of housing is being renovated most).
        *   **Visualization:** Trend line.

28. **Metric: Marketing Feature Adoption Rate**
    *   **Objective:** Track the evolution and sophistication of real estate marketing practices.
    *   **Method:** Calculate the percentage of active ads that include `videoUrl`, have more than 20 `images`, or include `floorplans`.
    *   **Data Fields:** `RealEstateHomeAdDto.videoUrl`, `RealEstateHomeAdDto.images`, `RealEstateHomeAdDto.floorplans`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Agency` (to identify marketing leaders), Price bracket.
        *   **Visualization:** Trend lines for each feature.

29. **Metric: Initial Listing Price Strategy Index**
    *   **Objective:** Detect the prevalence of "lokkepris" (bait pricing) strategies.
    *   **Method:** `Median P/Sqm of Newly Listed Ads` / `Median P/Sqm of Overall Active Market`. A value < 1 suggests new listings are priced lower to generate interest. "Newly Listed" are ads with `FirstSeen` in the last 7 days.
    *   **Data Fields:** `Ads.FirstSeen`, `DetailedPriceDto.suggestion`, `SizeDto.primary`.
    *   **Computation Variants:**
        *   **Granularity:** Weekly.
        *   **Segmentation:** By `Municipality`, `Agency`.
        *   **Visualization:** Trend line. Values consistently below 1 for an agency reveal its core strategy.

30. **Metric: Agency Market Concentration (HHI)**
    *   **Objective:** Measure market competitiveness and identify monopolistic or oligopolistic conditions.
    *   **Method:** Use the Herfindahl-Hirschman Index. For a given market, calculate the market share (by listing count) for each agency. The HHI is the sum of the squares of these market shares. (e.g., `(S_agency1)^2 + (S_agency2)^2 + ...`).
    *   **Data Fields:** `AdSummaryDocDto.organisation_name`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** A table ranking municipalities by their HHI score.

31. **Metric: Rental Furnishing Index**
    *   **Objective:** Understand the target audience and service level of different rental markets.
    *   **Method:** Calculate the market share of each furnishing status: `Møblert`, `Delvis møblert`, `Umøblert`.
    *   **Data Fields:** `AdSummaryDocDto.furnished_state` (from search results), `RealEstateLettingAdDto.furnishing`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, Price bracket.
        *   **Visualization:** Stacked bar chart for each city.

32. **Metric: Cross-Market Lead/Lag Correlation**
    *   **Objective:** Explore advanced, predictive relationships between different markets.
    *   **Method:** Perform time-series correlation analysis. For example, correlate the `New Listings Inflow Rate` for `REALESTATE_HOMES` with a 3-month lag of the `New Listings Inflow Rate` for `BAP_SALE` (Torget) luxury goods (e.g., high-end watches, designer bags). A positive correlation might suggest consumer confidence trends.
    *   **Data Fields:** `Ads.FirstSeen` from different `MarketId`s.
    *   **Computation Variants:**
        *   **Granularity:** Monthly data.
        *   **Segmentation:** National level.
        *   **Visualization:** Scatter plot with a regression line.

### **Real Estate Rental Market**

1.  **Metric: Total Active Rental Inventory**
    *   **Objective:** Establish the baseline supply of available rental properties.
    *   **Method:** `COUNT(DISTINCT Ads.FinnAdId)` where `Ads.IsActive = true` AND `MarketId` corresponds to a rental market.
    *   **Data Fields:** `Ads.FinnAdId`, `Ads.IsActive`, `Ads.MarketId`.
    *   **Computation Variants:**
        *   **Granularity:** Daily snapshot.
        *   **Segmentation:** Global, by `Municipality`, `PostalCode`, `PropertyType`, number of bedrooms.
        *   **Visualization:** Historical trend (line chart), current breakdown (bar chart).

2.  **Metric: New Rental Listings Inflow Rate**
    *   **Objective:** Measure the velocity of new rental properties becoming available, indicating landlord activity and market churn.
    *   **Method:** `COUNT(DISTINCT Ads.FinnAdId)` where `Ads.FirstSeen` is within a given time window AND `MarketId` corresponds to a rental market.
    *   **Data Fields:** `Ads.FinnAdId`, `Ads.FirstSeen`, `Ads.MarketId`.
    *   **Computation Variants:**
        *   **Granularity:** Daily, Weekly, Monthly sums.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Trend line, YoY comparison.

3.  **Metric: Leased Listings Outflow Rate (Proxy)**
    *   **Objective:** Estimate the velocity of rental demand absorbing supply.
    *   **Method:** `COUNT(DISTINCT Ads.FinnAdId)` where `Ads.IsActive` changes from `true` to `false` within a given time window, and `disposed` is not explicitly `true`. This serves as a proxy for the property being leased.
    *   **Data Fields:** `Ads.FinnAdId`, `Ads.IsActive`, `RealEstateLettingAdDto.disposed`.
    *   **Computation Variants:**
        *   **Granularity:** Weekly, Monthly sums.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Trend line, often overlaid with Inflow Rate.

4.  **Metric: Net Rental Inventory Flow**
    *   **Objective:** Determine the rental market's direction and pressure.
    *   **Method:** `(New Rental Listings Inflow Rate) - (Leased Listings Outflow Rate)` over the same period.
    *   **Data Fields:** Combination of fields from metrics 31 and 32.
    *   **Computation Variants:**
        *   **Granularity:** Weekly, Monthly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Bar chart showing positive/negative flow.

5.  **Metric: Median Time to Lease (TTL)**
    *   **Objective:** Measure the speed of the rental market.
    *   **Method:** For all properties leased in a period, calculate the median duration between the first `AdViewHistoryItemDto.broadcasted` date and the `AdSnapshots.ScrapedAt` date when the ad became inactive.
    *   **Data Fields:** `AdViewMetaDto.history`, `Ads.IsActive`, `AdSnapshots.ScrapedAt`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Segmentation:** By `Municipality`, `PropertyType`, Price bracket, `Furnishing` status.
        *   **Visualization:** Trend line for key segments.

6.  **Metric: Stale Rental Inventory Ratio**
    *   **Objective:** Identify the proportion of the rental market that is "stuck," indicating overpricing or low demand.
    *   **Method:** `COUNT(Active Rental Ads where current_TTL > 45 days) / COUNT(Total Active Rental Ads)`.
    *   **Data Fields:** `Ads.IsActive`, `AdViewMetaDto.history`.
    *   **Computation Variants:**
        *   **Granularity:** Daily snapshot.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Map highlighting areas with high stale rental inventory.

7.  **Metric: Median Rent per Square Meter (R/Sqm) Index**
    *   **Objective:** The primary benchmark for tracking rental price changes, normalized for size.
    *   **Method:** `Median(DetailedPriceDto.monthly / SizeDto.usable)` for all active rental listings. `usable` area (BRA) is used as the standard.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `SizeDto.usable`.
    *   **Computation Variants:**
        *   **Granularity:** Daily, with weekly/monthly smoothing.
        *   **Segmentation:** By `Municipality`, `PostalCode`, `PropertyType`, `Bedrooms`.
        *   **Visualization:** Trend line, choropleth map.

8.  **Metric: R/Sqm Distribution (Quartile Analysis)**
    *   **Objective:** Understand the rental market's price stratification.
    *   **Method:** For each segment, calculate the 10th, 25th (Q1), 50th (Median), 75th (Q3), and 90th percentile for R/Sqm.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `SizeDto.usable`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Box-and-whisker plots per segment.

9.  **Metric: Gross Rental Yield Index**
    *   **Objective:** A core investment metric to compare the income-generating potential of different areas.
    *   **Method:** For each `Municipality`, calculate `Median((Median Monthly Rent * 12) / Median Asking Price)`. This requires combining data from both rental and sales markets for the same area and property type.
    *   **Data Fields:** `DetailedPriceDto.monthly` (from letting ads), `DetailedPriceDto.suggestion` (from sales ads).
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Ranked bar chart of municipalities by yield, or a geospatial map.

10. **Metric: Market-wide Tenant Turnover Proxy**
    *   **Objective:** Identify areas or property types with high tenant churn, a key indicator of investment risk.
    *   **Method:** Track unique properties by a composite key (`streetAddress`, `size.usable`, `cadastres`). Calculate the median time between a property being leased (becoming inactive) and being re-listed as available for rent. A short duration indicates high turnover.
    *   **Data Fields:** `AdViewLocationDto.streetAddress`, `SizeDto.usable`, `CadastreDto`, `Ads.IsActive`, `Ads.FirstSeen`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, `PropertyType`, `Agency`. (High turnover for an agency's portfolio is a warning sign).
        *   **Visualization:** Heatmap showing turnover rates.

11. **Metric: Rental Affordability Index (Requires External Data)**
    *   **Objective:** A macro-economic indicator to assess rental market stress and sustainability.
    *   **Method:** `(Median Monthly Rent for a 2-bedroom apartment in Municipality) / (Estimated Median Monthly Net Income in Municipality)`. Requires an external data source for income statistics (e.g., from SSB).
    *   **Data Fields:** `DetailedPriceDto.monthly`, `RealEstateLettingAdDto.bedrooms`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Granularity:** Annually.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Trend line comparing affordability across major cities.

12. **Metric: Furnishing Premium/Discount Analysis**
    *   **Objective:** Quantify the price difference between furnished and unfurnished properties to inform investment strategy.
    *   **Method:** For a given segment (e.g., 2-bedroom apartments in Oslo), calculate `(Median R/Sqm for Furnished) - (Median R/Sqm for Unfurnished)`.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `SizeDto.usable`, `RealEstateLettingAdDto.furnishing` or `AdSummaryDocDto.furnished_state`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Bar chart showing the premium in NOK/sqm.

13. **Metric: Median Deposit-to-Rent Ratio**
    *   **Objective:** Understand the standard security deposit requirements and landlord risk appetite in different markets.
    *   **Method:** `Median(price.deposit / price.monthly)`. Note: `price.deposit` is a string and requires parsing. The typical value is 3.0. Deviations are interesting.
    *   **Data Fields:** `DetailedPriceDto.deposit`, `DetailedPriceDto.monthly`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Table showing the median ratio for different cities.

14. **Metric: "Animals Allowed" Market Share & Price Impact**
    *   **Objective:** Identify the size of the pet-friendly rental market and determine if there's a price premium.
    *   **Method:** 1. `COUNT(ads where animalsAllowed = true) / COUNT(Total Rental Ads)`. 2. `(Median R/Sqm for Pet-Friendly) - (Median R/Sqm for Not Pet-Friendly)`.
    *   **Data Fields:** `RealEstateLettingAdDto.animalsAllowed`, `DetailedPriceDto.monthly`, `SizeDto.usable`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Bar charts comparing market share and price impact.

15. **Metric: Lease Term Profile**
    *   **Objective:** Understand the typical lease durations offered in the market.
    *   **Method:** Parse the `timespan.from` and `timespan.to` fields to calculate the offered lease duration. Create a histogram of these durations (e.g., <1 year, 1 year, 3 years, unlimited).
    *   **Data Fields:** `TimespanDto`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Histogram for different regions.

16. **Metric: Digital Contract Adoption Rate**
    *   **Objective:** Measure the technological sophistication of the rental market.
    *   **Method:** `COUNT(ads where contract.contractUrl IS NOT NULL) / COUNT(Total Rental Ads)`.
    *   **Data Fields:** `ContractDto.contractUrl`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, Agency vs. Private Landlord.
        *   **Visualization:** Trend line.

17. **Metric: All-Inclusive Rent Prevalence**
    *   **Objective:** Identify the proportion of listings where utilities like heating or internet are included, affecting true rental cost.
    *   **Method:** `COUNT(ads where price.includes IS NOT NULL) / COUNT(Total Rental Ads)`. Further analysis can parse the `includes` string for keywords like "Oppvarming", "Internett", "Strøm".
    *   **Data Fields:** `DetailedPriceDto.includes`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Bar chart showing prevalence.

18. **Metric: External Partner (e.g., Qasa) Market Share**
    *   **Objective:** Track the influence of third-party rental management platforms on the market.
    *   **Method:** `COUNT(ads where schemaName = 'realestate-letting-external') / COUNT(Total Rental Ads)`.
    *   **Data Fields:** `AdViewMetaDto.schemaName`, `RealEstateLettingExternalAdDto.externalPartnerName`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Segmentation:** By `Municipality`, Partner Name.
        *   **Visualization:** Trend line showing market share of partners like Qasa.

19. **Metric: Rental Market Concentration (HHI)**
    *   **Objective:** Identify large-scale landlords or agencies dominating regional rental markets.
    *   **Method:** Use Herfindahl-Hirschman Index (HHI) on rental listings, grouped by `organisation_name` or `ownerId`.
    *   **Data Fields:** `AdSummaryDocDto.organisation_name`, `AdViewMetaDto.ownerId`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Table ranking municipalities by their rental market HHI.

20. **Metric: Rental Price Volatility**
    *   **Objective:** Measure the stability of rental prices, a key risk factor for income predictability.
    *   **Method:** Calculate the rolling 30-day standard deviation of the daily Median R/Sqm Index for a given segment.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `SizeDto.usable`, `AdSnapshots.ScrapedAt`.
    *   **Computation Variants:**
        *   **Granularity:** Daily rolling calculation.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Trend line comparing rental price volatility across different cities.

21. **Metric: Price Reduction Ratio (Rental)**
    *   **Objective:** Gauge landlord negotiating power and market softness. A high ratio indicates a renter's market.
    *   **Method:** `COUNT(Rental Ads with at least one price drop) / COUNT(Total Active Rental Ads)`. A price drop is detected by comparing `price.monthly` between consecutive snapshots.
    *   **Data Fields:** `AdSnapshots.MonthlyRent` (custom snapshot field).
    *   **Computation Variants:**
        *   **Granularity:** Daily snapshot.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Trend line of the ratio.

22. **Metric: Median Rental Price Reduction Magnitude**
    *   **Objective:** Quantify the typical discount landlords are willing to give to secure a tenant.
    *   **Method:** For all rental ads with a price drop, calculate the median of `(initial_rent - current_rent) / initial_rent`.
    *   **Data Fields:** `AdSnapshots.MonthlyRent`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly cohort.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Bar chart comparing magnitude across segments.

23. **Metric: Bedroom Count Market Share (Rental)**
    *   **Objective:** Understand the composition of rental supply to match it with demand profiles (e.g., student, family, professional).
    *   **Method:** Calculate the market share for each bedroom count (1, 2, 3, 4+).
    *   **Data Fields:** `RealEstateLettingAdDto.bedrooms`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Pie chart for a snapshot, or stacked area chart for trends over time.

24. **Metric: "Refugees Welcome" Initiative Participation Rate**
    *   **Objective:** A social metric to track landlord participation in humanitarian initiatives.
    *   **Method:** `COUNT(ads where refugeesWelcome = true) / COUNT(Total Rental Ads)`.
    *   **Data Fields:** `RealEstateLettingAdDto.refugeesWelcome`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Trend line.

25. **Metric: Rental Price-to-Size Elasticity**
    *   **Objective:** Determine how much rental prices increase for each additional square meter, identifying potential inefficiencies.
    *   **Method:** Perform a log-log regression where `ln(price.monthly)` is the dependent variable and `ln(size.usable)` is the independent variable. The resulting coefficient is the elasticity. A coefficient of 0.8 means a 10% increase in size only yields an 8% increase in rent.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `SizeDto.usable`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`, `PropertyType`.
        *   **Visualization:** Table showing elasticity coefficients for different segments.

26. **Metric: Rental Market Seasonality**
    *   **Objective:** Identify seasonal patterns in rental demand and supply (e.g., student-driven summer peak).
    *   **Method:** For each month, calculate the average deviation from the annual mean for `New Rental Listings Inflow` and `Median R/Sqm`.
    *   **Data Fields:** `Ads.FirstSeen`, `DetailedPriceDto.monthly`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly index, updated annually.
        *   **Segmentation:** By `Municipality` (especially university towns).
        *   **Visualization:** Line chart showing the index for each month.

27. **Metric: Rental vs. Ownership Cost Ratio**
    *   **Objective:** A classic metric to determine whether it's financially more sensible to rent or buy in a given area.
    *   **Method:** For a comparable property type (e.g., 2-bedroom apartment), calculate `(Median Annual Rent) / (Median Sales Price)`.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `DetailedPriceDto.suggestion`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Trend line. A falling ratio makes buying more attractive.

28. **Metric: Rental Vacancy Rate Proxy**
    *   **Objective:** Estimate the percentage of rental units that are currently unoccupied.
    *   **Method:** `(Total Active Rental Inventory) / (Estimated Total Rental Housing Stock)`. Requires an external data source (e.g., from SSB or an estimate based on property ownership types) for the denominator.
    *   **Data Fields:** `Ads.FinnAdId`, `Ads.IsActive`.
    *   **Computation Variants:**
        *   **Granularity:** Annually.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Trend line.

29. **Metric: Property Type Rental Yield Premium**
    *   **Objective:** Determine which property types (e.g., 'Leilighet', 'Enebolig', 'Rekkehus') offer the best rental yields.
    *   **Method:** Calculate the `Gross Rental Yield Index` for each `PropertyType` within a given `Municipality`.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `DetailedPriceDto.suggestion`, `RealEstateHomeAdDto.propertyType`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Bar chart comparing yields for different property types.

30. **Metric: Energy Label Impact on Rent**
    *   **Objective:** Determine if tenants are willing to pay a premium for properties with better energy efficiency.
    *   **Method:** For a standardized property segment (e.g., 2-bedroom apartments in Oslo), calculate the `Median R/Sqm` for different `energyLabel.class` groups (e.g., A/B vs. F/G).
    *   **Data Fields:** `DetailedPriceDto.monthly`, `SizeDto.usable`, `EnergyLabelDto.class`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `Municipality`.
        *   **Visualization:** Bar chart showing the R/Sqm for each energy label group.

---

### **Category 2: Location-Based Analysis (Geospatial Insights)**

*Goal: To transform raw, point-based data into a rich, layered understanding of geographic areas, enabling direct comparison and identification of regions that align with specific investment or lifestyle objectives. This involves moving beyond simple averages to create composite scores, risk indices, and opportunity heatmaps.*

---

# **Real Estate Sales Market**

1.  **Metric: Geospatial Price-per-Square-Meter (P/Sqm) Heatmap**
    *   **Objective:** To create the fundamental map of real estate value, allowing for instant identification of premium, average, and undervalued neighborhoods. This is the baseline for all other spatial analysis.
    *   **Method:** For each geographic unit (e.g., postal code), calculate `Median(DetailedPriceDto.suggestion / SizeDto.primary)`. Use P-ROM (`size.primary`) as the standard denominator.
    *   **Data Fields:** `DetailedPriceDto.suggestion`, `SizeDto.primary`, `AdViewLocationDto.postalCode`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Granularity:** Calculated on a rolling 90-day window, updated daily.
        *   **Segmentation:** Separate heatmaps for different `PropertyType`s (e.g., `Leilighet` vs. `Enebolig`).
        *   **Visualization:** Choropleth map where postal code areas are colored based on their median P/Sqm.

2.  **Metric: Price Velocity & Acceleration Map**
    *   **Objective:** To map not just the price, but its rate of change. This identifies markets that are rapidly appreciating ("hot") or depreciating ("cold"), providing leading indicators for investment timing.
    *   **Method:** For each postal code, calculate: 1) **Velocity:** The 3-month percentage change in Median P/Sqm. 2) **Acceleration:** The difference between the current 3-month velocity and the previous 3-month velocity (`Velocity_current - Velocity_previous`).
    *   **Data Fields:** `DetailedPriceDto.suggestion`, `SizeDto.primary`, `AdSnapshots.ScrapedAt`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Granularity:** Calculated monthly.
        *   **Segmentation:** By `PropertyType`.
        *   **Visualization:** Dual-layer map. Color indicates velocity (e.g., green for positive, red for negative), while an icon or pattern indicates acceleration (e.g., upward arrow for positive acceleration).

3.  **Metric: Price Volatility Index Map**
    *   **Objective:** To map market risk. High volatility indicates less predictable prices and potentially higher risk for short-term investments or flips.
    *   **Method:** For each postal code, calculate the rolling 90-day standard deviation of the daily Median P/Sqm. Normalize this value into a risk index (e.g., 1-10).
    *   **Data Fields:** `DetailedPriceDto.suggestion`, `SizeDto.primary`, `AdSnapshots.ScrapedAt`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Granularity:** Updated weekly.
        *   **Segmentation:** Can be compared between different `Municipalities`.
        *   **Visualization:** Choropleth map where color intensity represents the volatility index.

4.  **Metric: Sales Transaction Velocity Heatmap**
    *   **Objective:** To map market liquidity and demand intensity. This answers "Where are properties selling the fastest?"
    *   **Method:** For each postal code, calculate the median Time on Market (ToM) for all properties marked `disposed = true` over the last 6 months.
    *   **Data Fields:** `RealEstateHomeAdDto.disposed`, `AdViewMetaDto.history`, `AdSnapshots.ScrapedAt`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Granularity:** Calculated monthly.
        *   **Segmentation:** By `PropertyType` and price bracket.
        *   **Visualization:** Choropleth map where low ToM is colored "hot" (e.g., red) and high ToM is colored "cold" (e.g., blue).

5.  **Metric: Competitive Intensity Score Map**
    *   **Objective:** To identify the most challenging markets for buyers by combining signals of high competition.
    *   **Method:** For each postal code, create a composite score based on normalized values of: 1) Low Median ToM. 2) High Price Escalation Ratio (see Category 1). 3) High prevalence of `electronicBid.bidUrl`. 4) High prevalence of `preemption` clauses.
    *   **Data Fields:** `RealEstateHomeAdDto.disposed`, `AdViewMetaDto.history`, `AdSnapshots.PriceSuggestion`, `ElectronicBidDto.bidUrl`, `RealEstateHomeAdDto.preemption`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Visualization:** Choropleth map highlighting the most competitive "red zones" for buyers.

6.  **Metric: Seller Desperation Index Map**
    *   **Objective:** To identify buyer's markets by mapping signals of seller weakness or overpricing.
    *   **Method:** For each postal code, create a composite score based on: 1) High Stale Inventory Ratio (ToM > global median). 2) High Price Reduction Ratio. 3) High Median Price Reduction Magnitude.
    *   **Data Fields:** `Ads.IsActive`, `AdViewMetaDto.history`, `AdSnapshots.PriceSuggestion`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Visualization:** Choropleth map highlighting areas with high seller desperation, indicating strong negotiating potential for buyers.

7.  **Metric: Development Potential Opportunity Map**
    *   **Objective:** To systematically find investment properties with high potential for extension, subdivision, or new construction, a core project goal.
    *   **Method:** For each postal code, calculate the median `(PlotDto.area / SizeDto.usable)` ratio for all `Enebolig` and `Gårdsbruk/Småbruk` listings. A high ratio indicates a large plot relative to the building.
    *   **Data Fields:** `PlotDto.area`, `SizeDto.usable`, `RealEstateHomeAdDto.propertyType`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Visualization:** A map overlaying points representing individual properties with a high ratio, colored by the ratio's magnitude.

8.  **Metric: Renovation Wave & Flip Potential Map**
    *   **Objective:** To identify neighborhoods ripe for "fix-and-flip" investments by finding areas with old housing stock but rising property values.
    *   **Method:** For each postal code, create a score based on: 1) High Price Velocity (from Metric 2.2). 2) A high proportion of properties with `constructionYear` before a certain threshold (e.g., 1980). 3) A low proportion of properties with a recent `renovatedYear`.
    *   **Data Fields:** `DetailedPriceDto.suggestion`, `AdSnapshots.ScrapedAt`, `RealEstateHomeAdDto.constructionYear`, `RealEstateHomeAdDto.renovatedYear`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Visualization:** Heatmap highlighting areas with the highest "Flip Potential" score.

9.  **Metric: "Småbruk" & Seclusion Density Map**
    *   **Objective:** To specifically address the personal housing goal by mapping where `Småbruk` are most common and meet the "secluded" criterion.
    *   **Method:** Create a map showing the density of listings where `PropertyType` is `Gårdsbruk/Småbruk`. For each listing, calculate a "Seclusion Score" based on `PlotDto.area` and the distance to the nearest POI from `NeighbourhoodProfileDto`.
    *   **Data Fields:** `RealEstateHomeAdDto.propertyType`, `PlotDto.area`, `NeighbourhoodProfileDto.cards`, `AdViewLocationDto.position`.
    *   **Computation Variants:**
        *   **Visualization:** A map with points for each `Småbruk`, color-coded by the Seclusion Score. Use kernel density estimation to show hotspots.

-   **Metric:** "Småbruk" Opportunity Score
    -   **Objective:** Systematically find potential homesteads that meet the criteria of being large and isolated.
    -   **Method:** Create a ranked list of properties where `PropertyType` is `Gårdsbruk/Småbruk`, `plot.area` > 20,000m², and a `Nearest POI Distance` (see below) is > 1km. Score is weighted by plot size and distance.
    -   **Data Fields:** `RealEstateHomeAdDto.propertyType`, `PlotDto.area`, `NeighbourhoodProfileDto.cards`.

10. **Metric: New Development Hotspot Map**
    *   **Objective:** To support the B2B prospecting goal by visualizing where new construction activity is concentrated.
    *   **Method:** Create a density map (heatmap) based on the location of all active ads where `schemaName` is `realestate-development-project`, `realestate-development-single`, or `realestate-planned`.
    *   **Data Fields:** `AdViewMetaDto.schemaName`, `AdViewLocationDto.position`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly snapshot.
        *   **Segmentation:** Can be filtered by project scale (e.g., number of units, if available) or by developer (`organisation_name`).
        *   **Visualization:** Heatmap showing the intensity of new development across the country.

11. **Metric: Regional Housing Stock Age Profile**
    *   **Objective:** To understand and compare the historical development patterns of different cities or regions.
    *   **Method:** For each `Municipality`, create a histogram of `RealEstateHomeAdDto.constructionYear` binned by decade.
    *   **Data Fields:** `RealEstateHomeAdDto.constructionYear`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A small-multiples map, where each municipality is represented by its own histogram, allowing for easy visual comparison.

12. **Metric: "Green" Housing Stock Map**
    *   **Objective:** To map the energy efficiency of the housing stock, identifying areas with high "energy debt" and potential for green investments.
    *   **Method:** For each `Municipality`, calculate the distribution of `EnergyLabel.class` (A-G).
    *   **Data Fields:** `EnergyLabelDto.class`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A map where each municipality is a pie chart showing its energy label distribution.

13. **Metric: Agency vs. Private Seller Dominance Map**
    *   **Objective:** To map the market structure and professionalism across different regions.
    *   **Method:** For each `Municipality`, calculate the ratio: `COUNT(ads where organisation_name IS NOT NULL) / COUNT(Total Ads)`.
    *   **Data Fields:** `AdSummaryDocDto.organisation_name`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Visualization:** Choropleth map showing the percentage of agency-led sales per municipality.

14. **Metric: Ownership Structure Map (`Selveier` vs. `Andel`)**
    *   **Objective:** To map the prevalence of different legal ownership types, which often correlates with urban density and property type (e.g., `Andel` is common for apartments in cities).
    *   **Method:** For each postal code, calculate the market share of each `ownershipType`.
    *   **Data Fields:** `RealEstateHomeAdDto.ownershipType`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Visualization:** A map where each postal code is a pie chart showing the ownership type breakdown.

15. **Metric: Leased Land (`Festetomt`) Risk Exposure Map**
    *   **Objective:** To create a critical risk map for buyers, highlighting areas where leasehold plots are common.
    *   **Method:** For each `Municipality`, calculate the percentage of listings where `PlotDto.owned = false`.
    *   **Data Fields:** `PlotDto.owned`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** Choropleth map where color intensity reflects the percentage of leasehold properties.

16. **Metric: Geospatial "Walk Score" & Amenity Access Map**
    *   **Objective:** To quantify the convenience of a location by creating a custom "Walk Score" based on proximity to key amenities.
    *   **Method:** For each property, use the `NeighbourhoodProfileDto` to calculate a score. Award points for short distances to `Dagligvare`, `Offentlig transport`, `Skole`, etc. Sum the points to create a final score.
    *   **Data Fields:** `NeighbourhoodProfileDto.cards` (specifically `PoiItemDto.distance`), `AdViewLocationDto.position`.
    *   **Computation Variants:**
        *   **Visualization:** A map showing individual properties as points colored by their Walk Score, allowing for fine-grained neighborhood comparison.

17. **Metric: Family-Friendliness Opportunity Map**
    *   **Objective:** To identify neighborhoods that are potentially undervalued but highly suitable for families.
    *   **Method:** Create a composite score for each postal code based on: 1) Proximity to `Barneskole` and `Barnehage`. 2) High prevalence of listings with 3+ bedrooms. 3) Relatively low P/Sqm compared to the city average.
    *   **Data Fields:** `NeighbourhoodProfileDto.cards`, `RealEstateHomeAdDto.bedrooms`, `DetailedPriceDto.suggestion`, `SizeDto.primary`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Visualization:** Heatmap highlighting postal codes with the highest Family-Friendliness Opportunity score.

18. **Metric: Regional Property Type Profile**
    *   **Objective:** To understand the dominant property types in different regions.
    *   **Method:** For each `Municipality`, calculate the market share of each `propertyType` (e.g., `Leilighet`, `Enebolig`, `Rekkehus`, `Tomt`).
    *   **Data Fields:** `RealEstateHomeAdDto.propertyType`, `RealEstatePlotAdDto.propertyType`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A map where each municipality is represented by a donut chart showing its property type mix.

19. **Metric: Failed Sale & Relisting Hotspot Map**
    *   **Objective:** To geospatially identify areas with systemic market weakness or pricing failures.
    *   **Method:** Map the density of "relisted" properties (as defined in Category 1, Metric 22).
    *   **Data Fields:** `Ads.FinnAdId`, `Ads.IsActive`, `RealEstateHomeAdDto.disposed`, `AdViewLocationDto.position`, `SizeDto.usable`.
    *   **Computation Variants:**
        *   **Visualization:** A heatmap showing clusters of relisted properties, which are strong signals of local market distress.

20. **Metric: B2B Agency Dominance Map**
    *   **Objective:** To help our drone business identify which real estate agencies have the strongest geographical footprint in key regions.
    *   **Method:** For each `Municipality`, identify the top 3 agencies by listing volume.
    *   **Data Fields:** `AdSummaryDocDto.organisation_name`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** An interactive map where clicking on a municipality displays a pop-up with the top 3 agencies and their local market share.

21. **Metric: Regulatory Constraint Density Map**
    *   **Objective:** To map areas where development might be more complex due to regulations.
    *   **Method:** Use NLP/keyword matching on the `regulations` text field to search for terms like "SEFRAK", "verneområde" (protected area), "kulturminne" (cultural heritage), "byggegrense" (building line). Map the density of listings containing these terms.
    *   **Data Fields:** `RealEstateHomeAdDto.regulations`, `AdViewLocationDto.position`.
    *   **Computation Variants:**
        *   **Visualization:** A heatmap indicating areas with a high density of regulatory flags.

---

# **Real Estate Rental Market**

1.  **Metric: Geospatial Rent-per-Square-Meter (R/Sqm) Heatmap**
    *   **Objective:** To create the fundamental map of rental value, identifying premium and affordable rental zones.
    *   **Method:** For each postal code, calculate `Median(DetailedPriceDto.monthly / SizeDto.usable)`. Use `usable` area (BRA) as the standard.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `SizeDto.usable`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Granularity:** Rolling 90-day window, updated daily.
        *   **Segmentation:** By `PropertyType` and `Bedrooms`.
        *   **Visualization:** Choropleth map colored by median R/Sqm.

2.  **Metric: Gross Rental Yield Opportunity Map**
    *   **Objective:** To pinpoint the most profitable locations for "buy-to-let" investments. This is a primary investment goal.
    *   **Method:** For each postal code, calculate the Gross Rental Yield: `Median((Median Monthly Rent * 12) / Median Sales Price)` for comparable property types.
    *   **Data Fields:** `DetailedPriceDto.monthly` (from letting), `DetailedPriceDto.suggestion` (from sales), `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Granularity:** Quarterly.
        *   **Segmentation:** By `PropertyType`.
        *   **Visualization:** Choropleth map where high-yield areas are colored green, providing a direct investment targeting tool.

3.  **Metric: Rental Demand & Velocity Heatmap**
    *   **Objective:** To map rental market liquidity, answering "Where are properties being leased the fastest?"
    *   **Method:** For each postal code, calculate the median Time to Lease (TTL) for all properties that became inactive over the last 6 months.
    *   **Data Fields:** `Ads.IsActive`, `AdViewMetaDto.history`, `AdSnapshots.ScrapedAt`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Granularity:** Monthly.
        *   **Segmentation:** By `PropertyType`, `Bedrooms`, `Furnishing` status.
        *   **Visualization:** Choropleth map where low TTL is colored "hot" (high demand).

4.  **Metric: Tenant Turnover Risk Map**
    *   **Objective:** To geospatially identify areas with high tenant churn, a critical risk factor indicating unstable cash flow for landlords.
    *   **Method:** Map the density of properties that are relisted for rent within a short period (e.g., < 12 months), as defined in Category 1.
    *   **Data Fields:** `Ads.FinnAdId`, `Ads.IsActive`, `AdViewLocationDto.position`, `SizeDto.usable`.
    *   **Computation Variants:**
        *   **Visualization:** A heatmap showing clusters of high-turnover properties, directly flagging high-risk investment zones.

5.  **Metric: "Rent vs. Buy" Financial Index Map**
    *   **Objective:** To create a decision-making tool for individuals by mapping whether it's financially more advantageous to rent or buy in a specific area.
    *   **Method:** For each `Municipality`, calculate the ratio: `(Median Annual Rent for a 2-bedroom apartment) / (Median Sales Price for a 2-bedroom apartment)`. A higher ratio makes buying relatively more attractive.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `DetailedPriceDto.suggestion`, `RealEstateLettingAdDto.bedrooms`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A choropleth map where municipalities are colored based on which option (renting or buying) is financially superior.

6.  **Metric: Rental Market Saturation Map**
    *   **Objective:** To identify markets that may be over-supplied with rental properties, indicating high competition for landlords.
    *   **Method:** For each postal code, map the Net Rental Inventory Flow (Metric 1.34). A consistently positive flow indicates supply is outpacing demand.
    *   **Data Fields:** `Ads.FirstSeen`, `Ads.IsActive`, `AdSnapshots.ScrapedAt`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Visualization:** A map where areas with growing rental inventory are flagged, signaling potential downward pressure on future rents.

7.  **Metric: Student Housing Hotspot Identifier**
    *   **Objective:** To find prime locations for student rental investments.
    *   **Method:** Identify postal codes with a high density of: 1) Rental listings with 1-2 bedrooms. 2) Proximity to universities/colleges (requires a static list of campus coordinates). 3) Keywords like "student" or "kollektiv" in the description.
    *   **Data Fields:** `RealEstateLettingAdDto.bedrooms`, `GeneralTextSectionDto.textUnsafe`, `AdViewLocationDto.position`.
    *   **Computation Variants:**
        *   **Visualization:** A heatmap showing the highest concentration of student-centric rental supply.

8.  **Metric: Furnishing Style & Premium Map**
    *   **Objective:** To understand regional rental market norms and quantify the financial return on furnishing a property.
    *   **Method:** 1) Map the market share of `Møblert` vs. `Umøblert` rentals per `Municipality`. 2) For each municipality, calculate the R/Sqm premium: `(Median R/Sqm for Furnished) - (Median R/Sqm for Unfurnished)`.
    *   **Data Fields:** `RealEstateLettingAdDto.furnishing`, `DetailedPriceDto.monthly`, `SizeDto.usable`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A dual map: one showing market share (pie charts) and another showing the price premium (choropleth).

9.  **Metric: Pet-Friendly Rental Desert/Oasis Map**
    *   **Objective:** To help pet owners (a large tenant segment) find suitable areas and to help investors identify an underserved niche.
    *   **Method:** For each postal code, calculate the percentage of rental listings where `animalsAllowed = true`.
    *   **Data Fields:** `RealEstateLettingAdDto.animalsAllowed`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Visualization:** A map showing "Oases" (high percentage of pet-friendly rentals) and "Deserts" (low percentage).

10. **Metric: All-Inclusive Rent Prevalence Map**
    *   **Objective:** To understand the true cost of renting in different areas by mapping where utilities are commonly included.
    *   **Method:** For each `Municipality`, calculate the percentage of listings where the `price.includes` field is not null and contains keywords like "strøm", "oppvarming", or "internett".
    *   **Data Fields:** `DetailedPriceDto.includes`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** Choropleth map indicating the prevalence of all-inclusive rental agreements.

11. **Metric: Rental Market Concentration (HHI) Map**
    *   **Objective:** To map the competitive landscape, identifying areas dominated by a few large landlords versus those with a fragmented market of small landlords.
    *   **Method:** For each `Municipality`, calculate the Herfindahl-Hirschman Index based on the listing volume of each `organisation_name` or `ownerId`.
    *   **Data Fields:** `AdSummaryDocDto.organisation_name`, `AdViewMetaDto.ownerId`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A map coloring municipalities by their HHI score, flagging potentially non-competitive rental markets.

12. **Metric: Commuter Rental Value Map**
    *   **Objective:** To identify areas that offer the best value for commuters, balancing lower rent with reasonable travel times to city centers.
    *   **Method:** For each postal code, create a "Value Score" = `(Travel time to city center POI) * (Median R/Sqm)`. Lower scores are better.
    *   **Data Fields:** `NeighbourhoodProfileDto.cards` (for travel times), `DetailedPriceDto.monthly`, `SizeDto.usable`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Visualization:** A map highlighting the "commuter sweet spots" with the lowest value scores around a major city.

13. **Metric: External Partner (e.g., Qasa) Penetration Map**
    *   **Objective:** To map the geographic footprint and influence of third-party rental management platforms.
    *   **Method:** For each `Municipality`, calculate the market share of ads with `schemaName = 'realestate-letting-external'`.
    *   **Data Fields:** `AdViewMetaDto.schemaName`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** Choropleth map showing the penetration rate of these platforms.

14. **Metric: Lease Term Profile Map**
    *   **Objective:** To visualize regional differences in lease term flexibility, identifying markets dominated by long-term vs. short-term rentals.
    *   **Method:** For each `Municipality`, create a distribution profile of offered lease durations based on the `timespan` DTO.
    *   **Data Fields:** `TimespanDto.from`, `TimespanDto.to`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A map where each municipality is represented by a bar chart showing its lease term distribution (e.g., % <1yr, % 1yr, % 3yr, % unlimited).

15. **Metric: Rental Arbitrage Opportunity Map**
    *   **Objective:** To geospatially identify markets where splitting a larger apartment for co-living (`kollektiv`) is most profitable.
    *   **Method:** For each postal code, calculate an arbitrage score: `(Median R/Sqm for 3-bed) - (Median R/Sqm for 2-bed)`. A large positive difference indicates a premium on extra rooms.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `SizeDto.usable`, `RealEstateLettingAdDto.bedrooms`, `AdViewLocationDto.postalCode`.
    *   **Computation Variants:**
        *   **Visualization:** A heatmap highlighting areas with the highest arbitrage potential.

16. **Metric: Regional Rental Price-to-Size Elasticity Map**
    *   **Objective:** To map how the value of an additional square meter changes across different regions, revealing market inefficiencies.
    *   **Method:** Perform the log-log regression from Category 1 (Metric 50) for each `Municipality`.
    *   **Data Fields:** `DetailedPriceDto.monthly`, `SizeDto.usable`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A map coloring municipalities by their calculated elasticity coefficient.

17. **Metric: Investment Stability Score Map**
    *   **Objective:** To create a single, powerful map for de-risking rental investments.
    *   **Method:** For each `Municipality`, create a composite score based on: 1) High Gross Rental Yield. 2) Low Tenant Turnover Rate. 3) Low Rental Price Volatility. 4) High Rental Demand (Low TTL).
    *   **Data Fields:** Combination of fields from the metrics above.
    *   **Computation Variants:**
        *   **Visualization:** A choropleth map highlighting the most stable, high-potential investment regions.

18. **Metric: Digital Contract Adoption Map**
    *   **Objective:** To map the technological maturity of different regional rental markets.
    *   **Method:** For each `Municipality`, calculate the percentage of rental ads that offer a digital contract (`contract.contractUrl IS NOT NULL`).
    *   **Data Fields:** `ContractDto.contractUrl`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A map showing the adoption rate, distinguishing between modern and traditional rental markets.

19. **Metric: Rental Deposit-to-Rent Ratio Map**
    *   **Objective:** To map regional norms for security deposits, which can be an indicator of landlord risk perception or market standards.
    *   **Method:** For each `Municipality`, calculate the `Median(price.deposit / price.monthly)`.
    *   **Data Fields:** `DetailedPriceDto.deposit`, `DetailedPriceDto.monthly`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A choropleth map showing the typical deposit ratio, highlighting any areas that deviate significantly from the standard (e.g., 3x monthly rent).

20. **Metric: Stale Rental Inventory Profile Map**
    *   **Objective:** To understand *what kind* of properties are struggling to be leased in different areas.
    *   **Method:** For each `Municipality`, analyze the characteristics of the stale inventory (TTL > 45 days). Create a profile showing the distribution of `PropertyType`, `Bedrooms`, and `Furnishing` status for these struggling properties.
    *   **Data Fields:** `Ads.IsActive`, `AdViewMetaDto.history`, `RealEstateLettingAdDto.propertyType`, `RealEstateLettingAdDto.bedrooms`, `RealEstateLettingAdDto.furnishing`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** An interactive map where clicking a municipality reveals the profile of its stale rental inventory.

21. **Metric: Rental Market Seasonality Map**
    *   **Objective:** To visualize how seasonality impacts rental markets differently across regions (e.g., student towns vs. industrial towns).
    *   **Method:** For each `Municipality`, create a seasonal index for `New Rental Listings Inflow`.
    *   **Data Fields:** `Ads.FirstSeen`, `CadastreDto.municipalityNumber`.
    *   **Computation Variants:**
        *   **Visualization:** A map where each municipality is represented by a small line chart showing its unique seasonal pattern throughout the year.

---

## **Category 3: Property-Level Analysis (Valuation & Performance)**

*Goal: To understand the specific attributes that drive a property's value and its performance on the market.*

-   **Metric:** Precise Time on Market (ToM)
    -   **Objective:** Accurately calculate the number of days an ad was active, from first listing to final sale/disposal.
    -   **Method:** For each ad, find the timestamp of the first `history` entry with `mode: 'PLAY'`. Find the timestamp of the last `history` entry where `disposed` becomes `true` or the mode becomes `STOP`. The difference is the ToM.
    -   **Data Fields:** `AdViewMetaDto.history`, `RealEstateHomeAdDto.disposed`.
    
-   Correlate ToM with property characteristics: price point, location, size, property type, and the selling agency.

-   **Metric:** Rental Stability Score (Inverse Turnover Rate)
    -   **Objective:** Identify rental properties with a history of long-term tenants, indicating a safer, more stable investment.
    -   **Method:** Track unique properties (by address/coordinates). If the same property appears in a new `REALESTATE_LETTING` ad within a short timeframe (e.g., < 18 months), flag it as high-turnover. The score is higher for properties with longer intervals between rental listings.
    -   **Data Fields:** `AdViewLocationDto.streetAddress`, `AdViewMetaDto.schemaName`, `AdViewMetaDto.history`.

-   **Metric:** Valuation Driver Analysis
    -   **Objective:** Quantify the price impact of specific property attributes to inform purchasing and renovation decisions.
    -   **Method:** Use multiple regression analysis where the dependent variable is `Price per Square Meter`. Independent variables include `Bedrooms`, `ConstructionYear`, `RenovatedYear`, `EnergyLabel.class`, and binary flags for key `Facilities` (e.g., "Garasje", "Balkong/terrasse"). The resulting coefficients estimate the monetary value of each feature.
    -   **Data Fields:** All pricing, size, and specification fields from `RealEstateHomeAdDto`.

-   **"Success Rate" & Ad Effectiveness:**
    -   Define "success" as an ad being sold/rented within a certain timeframe (e.g., under 30 days).
    -   Analyze which ad attributes correlate with a higher success rate: number of photos, quality of the main title, length and detail of the description, inclusion of a floor plan.

-   **Metric:** Development Potential Score
    -   **Objective:** Systematically identify properties with high potential for extension or new construction.
    -   **Method:** Calculate a score based on `(plot.area / size.usable)`. A high ratio indicates a large plot relative to the existing building. Further weight the score by local zoning information extracted from the `regulations` text.
    -   **Data Fields:** `PlotDto.area`, `SizeDto.usable`, `RealEstateHomeAdDto.regulations`.

-   **Metric:** Sales Process Risk Flags
    -   **Objective:** Automatically flag properties with potentially complicated or competitive sales processes.
    -   **Method:** Create boolean flags in the database for ads where `preemption` text is not null, `electronicBid.bidUrl` exists, or `acquisition.note` contains specific keywords like "budrunde" (bidding round).
    -   **Data Fields:** `RealEstateHomeAdDto.preemption`, `ElectronicBidDto.bidUrl`, `AcquisitionDto.note`.

---

## **Category 4: Actor-Level Analysis (Agency & Seller Behavior)**

*Goal: To model and evaluate the strategies and performance of market actors.*

-   **Metric:** Agency Listing Velocity Score
    -   **Objective:** Identify which agencies are most effective at selling properties quickly, aiding in partner selection.
    -   **Method:** For each agency, calculate their median ToM for a specific property type (e.g., `Leilighet`) in a specific region (e.g., `Oslo`). Compare this to the overall median ToM for that same segment. `Score = RegionalMedianToM / AgencyMedianToM`. A score > 1 means the agency is faster than average.
    -   **Data Fields:** `meta.history` (for ToM), `AdSummaryDocDto.organisation_name`, `AdViewLocationDto.postalCode`, `RealEstateHomeAdDto.propertyType`.

-   **Metric:** Agency Market Share Analysis
    -   **Objective:** Determine which agencies dominate specific market segments (by location, property type, or price bracket).
    -   **Method:** `COUNT(ads for AgencyX in SegmentY) / COUNT(all ads in SegmentY)`.
    -   **Data Fields:** `AdSummaryDocDto.organisation_name`, `AdViewLocationDto.postalCode`, `RealEstateHomeAdDto.propertyType`, `DetailedPriceDto.suggestion`.

-   **Metric:** Agency Pricing Strategy Profile
    -   **Objective:** Reveal an agency's typical negotiation and pricing strategy (e.g., "Price to sell" vs. "Start high").
    -   **Method:**
        1.  **Price Drop Frequency:** `COUNT(price change events in meta.history) / ToM`.
        2.  **Price Drop Magnitude:** `AVG((old_price - new_price) / old_price)` for all price drops.
        3.  Profile agencies based on these two metrics.
    -   **Data Fields:** `meta.history`, `price.suggestion` (from historical snapshots).

-   **Metric:** Ad Content Evolution Tracking
    -   **Objective:** Analyze how agencies modify ads over time to improve their effectiveness.
    -   **Method:** For each ad snapshot, calculate a hash of the `title` and `images` array. Track when these hashes change over the ad's lifecycle. Correlate changes with a subsequent decrease in ToM.
    -   **Data Fields:** `meta.history`, `RealEstateHomeAdDto.title`, `RealEstateHomeAdDto.images`.

-   **Agency Profiling:**
    -   Profile agencies based on the types of properties they specialize in and the price segments they operate in.
    -   Track how many viewings an agency typically schedules and at what intervals (`viewings` array).
    -   Identify agencies associated with high-turnover rental properties to avoid potential partners with poor tenant management.
    -   This helps identify the most suitable agency to partner with for a specific rental property.

---

## **Category 5: Development Project Analysis (B2B & Investment)**

*Goal: To understand the new-build market, its trends, and key players.*

-   **Metric:** Development Project Timeline Analysis
    -   **Objective:** Track the typical duration of development projects to understand market cycles and forecast future housing supply.
    -   **Method:** Calculate the duration between key project milestones: `(phases.sale_start - first_seen_date)`, `(phases.development_start - phases.sale_start)`, `(phases.acquisition - phases.development_start)`.
    -   **Data Fields:** `AdViewMetaDto.history`, `ProjectPhasesDto`.

-   **Metric:** New-Build Feature & Trend Analysis
    -   **Objective:** Identify prevailing trends in new construction to inform investment decisions (i.e., what features are standard in new builds today).
    -   **Method:** Aggregate and analyze the distribution of price, `propertyType`, `bedrooms`, `size.usable`, common `facilities`, and scale of development (e.g., small allotment, condominium, high-rise) for all `realestate-development-project` ads.
    -   **Data Fields:** All fields from `RealEstateDevelopmentProjectAdDto`.

-   **Metric:** B2B Prospect Identification
    -   **Objective:** Systematically generate a list of active developers, construction companies, and real estate agencies for our drone business.
    -   **Method:** Extract and create a unique, ranked list of all `organisation_name` values from ads with `schemaName` of `realestate-development-project` or `realestate-planned`. Rank by number of active projects.
    -   **Data Fields:** `AdSummaryDocDto.organisation_name`, `AdViewMetaDto.schemaName`.

---

## **Category 6: Qualitative & Linguistic Analysis (LLM-Powered Insights)**

*Goal: To extract nuanced, human-centric insights from unstructured text and image data.*

-   **Metric:** Ad Keyword Significance Score
    -   **Objective:** Identify "power words" or phrases in ad descriptions that strongly correlate with faster sales or higher prices.
    -   **Method:** Perform TF-IDF analysis on the `generalText` and `propertyInfo` sections. Use a regression model to find the correlation between the presence of specific high-scoring terms (e.g., "nyoppusset", "panoramautsikt", "uskjenert") and a lower ToM or higher Price/Sqm.
    -   **Data Fields:** `GeneralTextSectionDto.textUnsafe`, `GeneralTextSectionDto.content`.

-   **Metric:** Automated Condition Report Extraction
    -   **Objective:** Structure the unstructured text of the technical condition report (`tilstandsrapport`) often found in the ad description (or in the general description text).
    -   **Method:** Use an LLM to parse the `Standard` section of the `generalText` for patterns like "TG2", "TG3", "Tiltak:", "Kostnadsestimat:", and populate a structured table with these findings for easy review and risk assessment.
    -   **Data Fields:** `GeneralTextSectionDto` where `heading` is `Standard`.

-   **Metric:** Ad Tone Classification
    -   **Objective:** Classify the tone of an ad's description (e.g., "Urgent," "Luxurious," "Family-oriented," "Technical/Factual") to see if tone impacts sales performance.
    -   **Method:** Use a pre-trained sentiment/text-classification model (or an LLM with specific prompting) to classify the ad's text. Correlate the resulting class with ToM.
    -   **Data Fields:** `GeneralTextSectionDto.textUnsafe`, `RealEstateHomeAdDto.title`.

-   **Metric:** Image Quality & Content Score (Future Goal)
    -   **Objective:** Assess whether higher quality, brighter, and more comprehensive photos lead to faster sales.
    -   **Method:** (Future) Use computer vision models or multi-modal AI models like Gemini 2.5 Flash to score images based on brightness, sharpness, and composition. Use object detection to count photos of key rooms (e.g., kitchen, bathroom) and check for the presence of a floor plan. Correlate this score with ToM.
    -   **Data Fields:** URLs from `AdImageDto` and `floorplans` arrays.

---

## **Category 7: Predictive Analytics (Long-Term Goal)**

*Goal: To leverage the historical dataset to build predictive models that forecast market behavior.*

-   **Model:** Time on Market (ToM) Predictor
    -   **Objective:** For a new listing, predict the likely number of days it will stay on the market.
    -   **Features (Inputs):** All normalized property attributes (Price/Sqm, Location, Size, Type, Agency, etc.), and market-level indicators (e.g., current Market Absorption Rate).
    -   **Target (Output):** Predicted ToM in days.

-   **Model:** Optimal Rental Price Predictor
    -   **Objective:** For a given rental property, predict the monthly rent that maximizes income while minimizing vacancy (high occupancy).
    -   **Features (Inputs):** Property attributes, location, historical rental prices in the area, current rental market volume.
    -   **Target (Output):** Predicted `price.monthly`.

-   **Model:** Investment Safety Score
    -   **Objective:** Generate a single, composite score (e.g., 1-100) indicating the relative safety of a rental property investment.
    -   **Method:** A weighted model combining several calculated metrics: historical rental stability in the area (low turnover), stable regional price trends, high rental demand (low ToM for rentals), and positive neighborhood profile scores.
    -   **Features (Inputs):** A combination of the metrics calculated in the preceding categories.
    -   **Target (Output):** A composite safety score.
