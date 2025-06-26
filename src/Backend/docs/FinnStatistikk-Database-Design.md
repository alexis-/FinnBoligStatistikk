# Database Design & Data to Capture

This chapter defines the core data architecture for the FinnStatistikk platform. It is architected not merely to store data, but to model the intricate dynamics of the Norwegian real estate market. Every table, column, and relationship is a deliberate choice, engineered to fulfill the specific analytical objectives outlined in Chapter A, from macro-level trend analysis to granular, AI-driven property assessment. The design adheres to a set of uncompromising principles to ensure performance, scalability, and analytical integrity.

## **1. Data Architecture Philosophy & Core Principles**

The data platform is built upon a foundation of three core architectural principles, designed to balance historical fidelity, query performance, and long-term maintainability.

1.  **The Two-Database Model: `Archive` and `Engine`**
    To support both perfect data preservation and high-speed analytics, the system is logically separated into two distinct databases hosted on a single PostgreSQL instance:
    *   **The `FinnStatistikk_Archive` Database:** This database serves a single, critical purpose: to act as a complete, immutable, and time-series ledger of all raw data scraped from an ad source. Its primary component is the raw JSON response for every ad snapshot. This "write-once, read-rarely" archive is our insurance policy; it guarantees that we can always reprocess historical data to populate new analytical fields or correct past processing errors, and it provides a rich, unstructured corpus for future LLM analysis. Its design prioritizes data integrity and completeness over query performance.
    *   **The `FinnStatistikk_Engine` Database:** This is the high-performance analytical heart of the platform. All data within this database is structured, normalized, cleansed, and indexed for rapid querying and aggregation. It is the single source of truth for the API, the frontend, and the statistics processing module. Its design prioritizes query speed, data consistency, and efficient storage.

2.  **The Property-Centric Model: Tracking Assets Through Time**
    The platform's primary entity is not the advertisement, but the **physical property** itself. An ad is merely a transient representation of a property at a point in time. Our model reflects this by introducing a central `Properties` table. This allows us to:
    *   **Group Multiple Ads:** Link different ads (e.g., a rental ad followed by a sales ad, or a failed sale that gets relisted with a new `FinnAdId`) to the same underlying physical asset.
    *   **Track True History:** Analyze the complete lifecycle of a property across different transactions and ad campaigns.
    *   **Calculate Key Metrics:** Accurately compute critical metrics like tenant turnover rates and relisting frequencies.
    An algorithmic process, utilizing a combination of address, cadastral information, and GPS coordinates, will be responsible for linking new ads to existing properties.

3.  **Aggressive & Pragmatic Normalization**
    To ensure data integrity and minimize storage footprint, the `Engine` database employs aggressive normalization for repetitive data. All categorical text data (e.g., agency names, locations, property types, facilities) is stored in dedicated lookup tables, with main tables referencing them via efficient integer foreign keys. This reduces redundancy, speeds up grouping and filtering operations, and provides a single point of update for these values.

## **2. Database & Schema Definitions: `FinnStatistikk_Engine`**

The following schema represents the structured, high-performance `Engine` database. All monetary values are stored as `BigInt` in Norwegian Krone (NOK), converted at the time of processing.

---
### **Group 1: Configuration & Metadata Tables**

*Rationale: These tables store dynamic configuration data, some scraped from ad sources and some generated locally, allowing the application to adapt and operate robustly at runtime.*

*   **`Markets`**
    *   **Purpose:** To store the hierarchy of all available markets and submarkets, allowing scraping to be configured dynamically.
    *   **Columns:**
        *   `MarketId` (Integer, PK): Unique internal ID.
        *   `ParentMarketId` (Integer, FK to `Markets.MarketId`, Nullable): For hierarchical structure (e.g., `realestate-homes` is a submarket of `realestate`).
        *   `MarketIdentifier` (Text, Unique): The API's market identifier (e.g., "realestate-homes"). From `MarketDto.search-id`.
        *   `SearchKey` (Text, Unique): The constant key used in API calls (e.g., "SEARCH_ID_REALESTATE_HOMES"). From `MarketDto.search-key`.
        *   `Label` (Text): The human-readable name (e.g., "Boliger til salgs"). From `MarketDto.label`.
        *   `IsScrapingEnabled` (Boolean, Default: `false`): A flag for administrators to enable/disable scraping for this market.
        *   `LastScrapedAdTimestamp` (Timestamp (UTC), Nullable): `timestamp` of the newest ad found during the last successful scrape.

*   **`DeviceProfiles`**
    *   **Purpose:** A static catalogue of real-world Android device profiles used to generate believable user fingerprints. Seeded from a CSV file.
    *   **Columns:**
        *   `DeviceProfileId` (Integer, PK).
        *   `DeviceModel` (Text): (e.g., "SM-G998B").
        *   `Brand` (Text): (e.g., "samsung").
        *   `AndroidVersion` (Text): (e.g., "14").
        *   `BuildId` (Text): (e.g., "UQ1A.240205.004").

*   **`FinnAppVersions`**
    *   **Purpose:** To store a historical record of Finn.no application versions, allowing the scraper to mimic a gradual update cycle.
    *   **Columns:**
        *   `FinnAppVersionId` (Integer, PK).
        *   `VersionName` (Text, Unique): The user-facing version string (e.g., "250616-85b30270").
        *   `VersionCode` (BigInt, Unique): The internal version code.
        *   `DiscoveredDate` (Date).

*   **`UserIdentities`**
    *   **Purpose:** To manage a large pool of simulated user identities for anonymous scraping.
    *   **Columns:**
        *   `UserIdentityId` (Integer, PK).
        *   `VisitorId` (GUID, Unique): The persistent UUID used for this identity.
        *   `DeviceProfileId` (Integer, FK to `DeviceProfiles.DeviceProfileId`).
        *   `FinnAppVersionId` (Integer, FK to `FinnAppVersions.FinnAppVersionId`).
        *   `IsActive` (Boolean, Default: `true`): Whether the identity is available for use.
        *   `LastUsedTimestamp` (Timestamp (UTC), Nullable).
        *   `TotalFailureCount` (Integer, Default: `0`): The lifetime number of consecutive failures for this identity.

---
### **Group 2: Core Entity Tables**

*Rationale: These are the central tables that model the primary entities of the real estate ecosystem: the physical property, the advertisement, and its historical snapshots.*

*   **`Properties`**
    *   **Purpose:** Represents a unique, physical real estate asset, acting as an anchor for all associated ads over time.
    *   **Columns:**
        *   `PropertyId` (Integer, PK): Unique internal ID.
        *   `BestGuessLocationId` (Integer, FK to `Locations.LocationId`): A reference to the most accurate known location for this property.
        *   `FirstSeenTimestamp` (Timestamp (UTC)): When the first ad for this property was ever recorded.
        *   `LastSeenTimestamp` (Timestamp (UTC)): When the most recent ad for this property was last seen.

*   **`Ads`**
    *   **Purpose:** Represents a single, unique Finn.no advertisement (`FinnAdId`).
    *   **Columns:**
        *   `AdId` (Integer, PK): Unique internal ID.
        *   `PropertyId` (Integer, FK to `Properties.PropertyId`): Links the ad to the underlying physical property.
        *   `FinnAdId` (BigInt, Unique): The `ad_id` from Finn.no.
        *   `MarketId` (Integer, FK to `Markets.MarketId`): The market this ad belongs to.
        *   `OwnerId` (BigInt, Nullable): The advertiser's owner ID from `AdViewMetaDto.ownerId`.
        *   `OwnerUrn` (Text, Nullable): The owner's unique resource name from `AdViewMetaDto.ownerUrn`.
        *   `ExternalAdId` (Text, Nullable): The advertiser's external ID.
        *   `AdvertiserRef` (Text, Nullable): The advertiser's internal reference number.
        *   `FirstSeenTimestamp` (Timestamp (UTC)): When this specific ad was first scraped.
        *   `LastSeenTimestamp` (Timestamp (UTC)): When this ad was last seen in an active state.
        *   `IsActive` (Boolean): The current status of the ad.

*   **`AdSnapshots`**
    *   **Purpose:** Records the state of an ad at a specific point in time. This is the core of our historical data.
    *   **Columns:**
        *   `SnapshotId` (BigInt, PK): Unique internal ID for the snapshot.
        *   `AdId` (Integer, FK to `Ads.AdId`): Links the snapshot to a specific ad.
        *   `ScrapedTimestamp` (Timestamp (UTC)): The exact time this snapshot was taken.
        *   `AdEditedTimestamp` (Timestamp (UTC), Nullable): The `edited` timestamp from `AdViewMetaDto`.
        *   `Version` (Text): The ad version string from `meta.version` (e.g., "56.4").
        *   `Mode` (Text): The ad mode from `meta.mode` (e.g., "PLAY", "STOP").
        *   `Title` (Text): The ad's heading/title at the time of the scrape.
        *   `SchemaNameId` (Integer, FK to `SchemaNames.SchemaNameId`): The ad type identifier (e.g., "realestate-home").
        *   `IsDisposed` (Boolean, Nullable): Whether the ad was marked as sold/leased. From `disposed` field.
        *   `RawAdViewJsonHash` (Text): A hash of the raw AdView JSON to detect changes.
        *   `RawProfileJsonHash` (Text): A hash of the raw Neighbourhood Profile JSON.
        *   `PostProcessingStatus` (SmallInt, Default: `0`): A bitmask field to track the status of various post-processing tasks. The flags are additive (e.g., a value of `3` means both ImagesDownloaded and LlmEnriched are complete).
            *   `1`: `ImagesDownloaded`
            *   `2`: `LlmEnriched`

---
### **Group 3: Lookup Tables (Normalization)**

*Rationale: These tables store deduplicated, categorical data to reduce database size and improve query performance.*

*   **`Agencies`**
    *   **Purpose:** A unique registry of all real estate agencies and other organizations.
    *   **Columns:**
        *   `AgencyId` (Integer, PK): Unique internal ID.
        *   `Name` (Text, Unique): The name of the agency (e.g., "DNB Eiendom AS"). From `organisation_name`.
        *   `LogoUrl` (Text, Nullable): URL to the agency's logo.

*   **`Contacts`**
    *   **Purpose:** A unique registry of all contact details (emails, phone numbers).
    *   **Columns:**
        *   `ContactId` (Integer, PK).
        *   `Email` (Text, Nullable, Unique).
        *   `PhoneNumber` (Text, Nullable, Unique).

*   **`SnapshotContacts` (Join Table)**
    *   **Purpose:** A many-to-many link between snapshots and contacts.
    *   **Columns:**
        *   `SnapshotId` (BigInt, PK, FK).
        *   `ContactId` (Integer, PK, FK).
        *   `ContactType` (Text): (e.g., 'SellerEmail', 'MainContactPhone').

*   **`HousingCooperatives`**
    *   **Purpose:** A unique registry of all housing cooperatives (`borettslag`).
    *   **Columns:**
        *   `HousingCooperativeId` (Integer, PK).
        *   `Name` (Text).
        *   `OrganisationNumber` (Text, Unique).

*   **`Locations`**
    *   **Purpose:** A unique registry of all geographic locations.
    *   **Columns:**
        *   `LocationId` (Integer, PK): Unique internal ID.
        *   `StreetAddress` (Text, Nullable).
        *   `PostalCode` (Text, Nullable).
        *   `City` (Text): The postal name (e.g., "Oslo").
        *   `Coordinates` (Geospatial Point, Nullable): `(Latitude, Longitude)`.

*   **`PropertyTypes`**
    *   **Purpose:** A registry of all distinct property types.
    *   **Columns:**
        *   `PropertyTypeId` (Integer, PK): Unique internal ID.
        *   `Name` (Text, Unique): (e.g., "Leilighet", "Enebolig", "Gårdsbruk/Småbruk").

*   **`AdSnapshotPropertyTypes` (Join Table)**
    *   **Purpose:** A many-to-many link between snapshots and property types, accommodating ads with multiple types (e.g., development projects).
    *   **Columns:**
        *   `SnapshotId` (BigInt, PK, FK).
        *   `PropertyTypeId` (Integer, PK, FK).

*   **`OwnershipTypes`**
    *   **Purpose:** A registry of all distinct ownership types.
    *   **Columns:**
        *   `OwnershipTypeId` (Integer, PK): Unique internal ID.
        *   `Name` (Text, Unique): (e.g., "Eier (Selveier)", "Andel", "Aksje").

*   **`Facilities`**
    *   **Purpose:** A registry of all distinct facilities.
    *   **Columns:**
        *   `FacilityId` (Integer, PK): Unique internal ID.
        *   `Name` (Text, Unique): (e.g., "Garasje/P-plass", "Balkong/terrasse").

*   **`AdSnapshotFacilities` (Join Table)**
    *   **Purpose:** A many-to-many link between snapshots and facilities.
    *   **Columns:**
        *   `SnapshotId` (BigInt, PK, FK).
        *   `FacilityId` (Integer, PK, FK).

*   **`SchemaNames`**
    *   **Purpose:** A registry of all distinct ad schema names.
    *   **Columns:**
        *   `SchemaNameId` (Integer, PK): Unique internal ID.
        *   `Name` (Text, Unique): (e.g., "realestate-home", "recommerce-sell").

*   **`EnergyLabelColors`**
    *   **Purpose:** A registry of all distinct energy label colors.
    *   **Columns:**
        *   `EnergyLabelColorId` (Integer, PK): Unique internal ID.
        *   `Name` (Text, Unique): (e.g., "GREEN", "RED").

*   **`FurnishingStatuses`**
    *   **Purpose:** A registry of all distinct furnishing statuses for rental ads.
    *   **Columns:**
        *   `FurnishingStatusId` (Integer, PK): Unique internal ID.
        *   `Name` (Text, Unique): (e.g., "Møblert", "Umøblert", "Delvis møblert").

---
### **Group 4: Polymorphic Detail Tables**

*Rationale: These tables store the specific, structured data for each major ad category, avoiding a single monolithic table with many null columns. To maintain a simple and powerful query model, commercial properties are merged into the main residential detail tables (`RealEstateHomeDetails`, `RealEstateLettingDetails`), as the data overlap is significant. This simplifies cross-segment analysis at a negligible storage cost.*

*   **`RealEstateHomeDetails`**
    *   **Applies to:** `realestate-home`, `realestate-development-single`, `realestate-leisure-sale`, `realestate-business-sale`.
    *   **Columns:**
        *   `SnapshotId` (BigInt, PK, FK): Links to the parent snapshot.
        *   **Core Specs:**
            *   `OwnershipTypeId` (Integer, FK).
            *   `ConstructionYear` (Integer, Nullable).
            *   `RenovatedYear` (Integer, Nullable).
            *   `Floor` (Integer, Nullable).
            *   `NumberOfFloors` (Integer, Nullable).
            *   `NumberOfRooms` (Integer, Nullable).
            *   `NumberOfBedrooms` (Integer, Nullable).
            *   `ParkingSpots` (Integer, Nullable).
            *   `IsForLetting` (Boolean): From `LettingUnit`.
            *   `AcquisitionFrom` (Timestamp (UTC)).
        *   **Business-Specific Specs:**
            *   `RentalIncome` (BigInt, Nullable): Annual rental income for commercial properties.
            *   `OfficeSpaces` (Integer, Nullable): Number of separate office spaces.
        *   **Pricing (all in NOK):**
            *   `PriceSuggestion` (BigInt, Nullable): Asking price.
            *   `PriceTotal` (BigInt, Nullable): Total price including costs.
            *   `PriceEstimatedValue` (BigInt, Nullable).
            *   `PriceSharedCost` (BigInt, Nullable): `felleskostnader`.
            *   `PriceTaxValue` (BigInt, Nullable): `formuesverdi`.
            *   `PriceMunicipalFees` (BigInt, Nullable): `kommunale avgifter`.
            *   `CollectiveDebt` (BigInt, Nullable): `andel fellesgjeld`.
            *   `CollectiveAssets` (BigInt, Nullable): `andel fellesformue`.
            *   `EstateTax` (BigInt, Nullable): `eiendomsskatt`.
            *   `LoanFare` (BigInt, Nullable).
            *   `LoanValue` (BigInt, Nullable).
            *   `HasSharedCostHedge` (Boolean, Nullable).
            *   `ElectronicBidUrl` (Text, Nullable).
            *   `ElectronicBidInfoUrl` (Text, Nullable).
        *   **Sizing & Plot:**
            *   `AreaGross` (Integer, Nullable): BTA.
            *   `AreaUsable` (Integer, Nullable): BRA.
            *   `AreaPrimary` (Integer, Nullable): P-ROM.
            *   `AreaUsableInternal` (Integer, Nullable): BRA-i.
            *   `AreaUsableExternal` (Integer, Nullable): BRA-e.
            *   `AreaUsableBuiltIn` (Integer, Nullable): BRA-b.
            *   `AreaOpen` (Integer, Nullable): TBA.
            *   `PlotArea` (BigInt, Nullable).
            *   `AreaFrom` (Integer, Nullable).
            *   `AreaTo` (Integer, Nullable).
            *   `IsPlotOwned` (Boolean, Nullable).
            *   `PlotLeaseholdPoint` (Boolean, Nullable).
            *   `PlotLeaseholdFee` (BigInt, Nullable).
            *   `PlotLeaseholdYear` (Integer, Nullable).
        *   **Energy, Status & Sale Process:**
            *   `EnergyLabelClass` (Character(1), Nullable): (e.g., 'A', 'G').
            *   `EnergyLabelColorId` (Integer, FK to `EnergyLabelColors.EnergyLabelColorId`, Nullable).
            *   `HasChangeOfOwnershipInsurance` (Boolean, Nullable).
            *   `HousingCooperativeId` (Integer, FK to `HousingCooperatives.HousingCooperativeId`, Nullable).
        *   **URLs & Media:**
            *   `VideoUrl` (Text, Nullable).
            *   `VirtualViewingUrl` (Text, Nullable).
            *   `ProspectusViewUrl` (Text, Nullable).
            *   `ProspectusDownloadUrl` (Text, Nullable).
        *   **Unstructured Text (for LLM Analysis):**
            *   `GeneralTextJson` (JSONB, Nullable): Stores the `generalText` array.
            *   `PropertyInfoJson` (JSONB, Nullable): Stores the `propertyInfo` array.
            *   `SummaryText` (Text, Nullable).
            *   `RegulationsText` (Text, Nullable).
            *   `PreemptionText` (Text, Nullable).
            *   `AcquisitionNote` (Text, Nullable).
            *   `SituationUnsafe` (Text, Nullable).
            *   `AccessUnsafe` (Text, Nullable).
            *   `ContentUnsafe` (Text, Nullable): General content, primarily for business properties.
            *   `PlotConditionUnsafe` (Text, Nullable).
            *   `SharedCostIncludesUnsafe` (Text, Nullable).
            *   `PriceSalesCostIncludesUnsafe` (Text, Nullable).
            *   `SizeDescriptionUnsafe` (Text, Nullable).

*   **`RealEstateLettingDetails`**
    *   **Applies to:** `realestate-letting`, `realestate-letting-external`, `realestate-business-letting`.
    *   **Columns:**
        *   `SnapshotId` (BigInt, PK, FK).
        *   **Core Specs:**
            *   `NumberOfBedrooms` (Integer, Nullable).
            *   `Floor` (Integer, Nullable).
            *   `FurnishingStatusId` (Integer, FK to `FurnishingStatuses.FurnishingStatusId`).
            *   `AnimalsAllowed` (Boolean, Nullable).
            *   `RefugeesWelcome` (Boolean, Nullable).
            *   `IncludedInRentText` (Text, Nullable).
            *   `AcquisitionFrom` (Timestamp (UTC)).
        *   **Business-Specific Specs:**
            *   `OfficeSpaces` (Integer, Nullable).
            *   `ParkingSpots` (Integer, Nullable).
            *   `RenovatedYear` (Integer, Nullable).
        *   **Pricing (all in NOK):**
            *   `MonthlyRent` (Integer, Nullable).
            *   `Deposit` (Integer, Nullable).
        *   **Sizing:**
            *   `AreaPrimary` (Integer, Nullable): P-ROM.
            *   `AreaUsableInternal` (Integer, Nullable): BRA-i.
            *   `AreaUsableExternal` (Integer, Nullable): BRA-e.
            *   `AreaUsableBuiltIn` (Integer, Nullable): BRA-b.
            *   `AreaOpen` (Integer, Nullable): TBA.
            *   `AreaFrom` (Integer, Nullable).
            *   `AreaTo` (Integer, Nullable).
        *   **Lease Info:**
            *   `LeaseFromDate` (Date, Nullable).
            *   `LeaseToDate` (Date, Nullable).
            *   `DigitalContractUrl` (Text, Nullable).
        *   **Energy & Status:**
            *   `EnergyLabelClass` (Character(1), Nullable).
            *   `EnergyLabelColorId` (Integer, FK to `EnergyLabelColors.EnergyLabelColorId`, Nullable).
        *   **Unstructured Text (for LLM Analysis):**
            *   `GeneralTextJson` (JSONB, Nullable): Stores the `generalText` array.
            *   `PropertyInfoJson` (JSONB, Nullable): Stores the `propertyInfo` array.
            *   `AccessUnsafe` (Text, Nullable).
            *   `ConditionUnsafe` (Text, Nullable).
            *   `SituationUnsafe` (Text, Nullable).

*   **`RealEstateProjectDetails`**
    *   **Applies to:** `realestate-development-project`, `realestate-planned`.
    *   **Columns:**
        *   `SnapshotId` (BigInt, PK, FK).
        *   **Core Specs:**
            *   `OwnershipTypeId` (Integer, FK).
            *   `BedroomsFrom` (Integer, Nullable).
            *   `BedroomsTo` (Integer, Nullable).
            *   `AreaFrom` (Integer, Nullable).
            *   `AreaTo` (Integer, Nullable).
            *   `IsLeisure` (Boolean, Nullable).
        *   **Project Specs:**
            *   `ProjectName` (Text).
            *   `PhaseCurrent` (Text, Nullable).
            *   `PhasePlanningText` (Text, Nullable).
            *   `PhaseSaleStartText` (Text, Nullable).
            *   `PhaseDevelopmentStartText` (Text, Nullable).
        *   **Unstructured Text (for LLM Analysis):**
            *   `PropertyInfoJson` (JSONB, Nullable).
            *   `PhaseAcquisitionText` (Text, Nullable).
        *   `ExternalUrl` (Text, Nullable).

*   **`RealEstatePlotDetails`**
    *   **Applies to:** `realestate-plot`, `realestate-leisure-plot`, `realestate-business-plot`.
    *   **Columns:**
        *   `SnapshotId` (BigInt, PK, FK).
        *   **Pricing (all in NOK):**
            *   `PriceSuggestion` (BigInt, Nullable).
            *   `PriceTotal` (BigInt, Nullable).
            *   `PriceTaxValue` (BigInt, Nullable).
            *   `ElectronicBidUrl` (Text, Nullable).
            *   `ElectronicBidInfoUrl` (Text, Nullable).
        *   **Plot Specs:**
            *   `PlotArea` (BigInt, Nullable).
            *   `IsPlotOwned` (Boolean, Nullable).
            *   `PlotLeaseholdPoint` (Boolean, Nullable).
            *   `PlotLeaseholdFee` (BigInt, Nullable).
            *   `PlotLeaseholdYear` (Integer, Nullable).
        *   **Unstructured Text (for LLM Analysis):**
            *   `GeneralTextJson` (JSONB, Nullable).
            *   `PropertyInfoJson` (JSONB, Nullable).
            *   `SituationUnsafe` (Text, Nullable).
            *   `AccessUnsafe` (Text, Nullable).
            *   `PlotConditionUnsafe` (Text, Nullable).
            *   `RegulationsText` (Text, Nullable).
            *   `PriceSalesCostIncludesUnsafe` (Text, Nullable).

*   **`BapDetails`** (for Torget)
    *   **Applies to:** `recommerce-sell`.
    *   **Columns:**
        *   `SnapshotId` (BigInt, PK, FK).
        *   `Price` (BigInt, Nullable).
        *   `Condition` (Text).
        *   `CategoryPath` (Text): A flattened string of the category hierarchy (e.g., "Elektronikk > Data > Harddisk").
        *   `Description` (Text).

---
### **Group 5: Supporting Data Tables**

*Rationale: These tables capture structured, relational data associated with a snapshot, such as media, viewing times, and neighborhood information.*

*   **`Media`**
    *   **Purpose:** Stores references to all downloaded media assets.
    *   **Columns:**
        *   `MediaId` (BigInt, PK).
        *   `SnapshotId` (BigInt, FK).
        *   `MediaType` (Text): ('Image', 'Floorplan', 'Document', 'Logo').
        *   `OriginalUrl` (Text, Unique): The source URL from Finn.no.
        *   `LocalPath` (Text): The relative path in our local storage.
        *   `Description` (Text, Nullable).
        *   `Width` (Integer, Nullable).
        *   `Height` (Integer, Nullable).

*   **`MoreInfoLinks`**
    *   **Purpose:** Stores all external "more info" links associated with an ad snapshot.
    *   **Columns:**
        *   `MoreInfoLinkId` (BigInt, PK).
        *   `SnapshotId` (BigInt, FK).
        *   `Title` (Text): The display text for the link (e.g., "Gi bud").
        *   `Uri` (Text): The target URL.

*   **`AdHistoryEvents`**
    *   **Purpose:** Stores the granular version history of an ad, critical for ToM calculations.
    *   **Columns:**
        *   `HistoryEventId` (BigInt, PK).
        *   `AdId` (Integer, FK).
        *   `BroadcastedTimestamp` (Timestamp (UTC)): The `broadcasted` time.
        *   `Version` (Text).
        *   `Mode` (Text): ('PLAY', 'STOP').

*   **`AdViewingEvents`**
    *   **Purpose:** Stores all scheduled viewing times for an ad.
    *   **Columns:**
        *   `ViewingEventId` (BigInt, PK).
        *   `SnapshotId` (BigInt, FK).
        *   `StartTimestamp` (Timestamp (UTC), Nullable).
        *   `EndTimestamp` (Timestamp (UTC), Nullable).
        *   `Note` (Text, Nullable).

*   **`CadastreDetails`**
    *   **Purpose:** Stores all cadastral (matrikkel) identifiers associated with a snapshot. An ad can have multiple.
    *   **Columns:**
        *   `CadastreId` (BigInt, PK).
        *   `SnapshotId` (BigInt, FK to `AdSnapshots.SnapshotId`).
        *   `MunicipalityNumber` (Integer).
        *   `LandNumber` (Integer, Nullable).
        *   `TitleNumber` (Integer, Nullable).
        *   `SectionNumber` (Integer, Nullable).
        *   `LeaseholdNumber` (Integer, Nullable).
        *   `ApartmentNumber` (Text, Nullable).

*   **`NeighbourhoodPois`** (Points of Interest)
    *   **Purpose:** Stores the structured neighborhood data.
    *   **Columns:**
        *   `PoiId` (BigInt, PK).
        *   `SnapshotId` (BigInt, FK).
        *   `Category` (Text): The card title (e.g., "Offentlig transport", "Skoler").
        *   `PoiType` (Text): The specific POI name (e.g., "Buss", "Barneskole").
        *   `DistanceText` (Text): The human-readable distance (e.g., "7 min").
        *   `TravelMethod` (Text): ('walk', 'drive').

## **3. Database & Schema: `FinnStatistikk_Archive`**

The archive database is intentionally simple, designed for high-throughput writes and data fidelity.

*   **`RawAdSnapshots`**
    *   **Purpose:** To store the complete, unmodified JSON payloads for every successful scrape.
    *   **Columns:**
        *   `RawSnapshotId` (BigInt, PK).
        *   `FinnAdId` (BigInt).
        *   `ScrapedTimestamp` (Timestamp (UTC)).
        *   `SchemaName` (Text).
        *   `AdViewJson` (JSONB, Nullable): The full JSON from `/adview/{adId}`.
        *   `ProfileJson` (JSONB, Nullable): The full JSON from `/getProfile`.
        *   `ProcessingStatus` (SmallInt): An enum-like integer representing the processing state (e.g., 0='Queued', 1='Processed', 2='Failed').