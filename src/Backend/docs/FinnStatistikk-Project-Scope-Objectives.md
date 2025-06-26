# FinnStatistikk Project Overview

**FinnStatistikk** is a private, comprehensive market intelligence platform designed to systematically model and analyze the Norwegian real estate market. The platform's core function is to scrape, structure, and store historical advertisement data from Finn.no, transforming it from a transient, public resource into a persistent, private analytical asset.

This platform is a strategic tool intended to provide a significant information advantage for making critical personal and financial decisions. Its insights will directly support three main goals:

1.  **Personal Housing Acquisition:** To identify and evaluate unique properties, specifically `Småbruk` (homesteads) and forest plots, that align with the desire for a forested, semi-isolated family home. The platform will enable deep analysis of property characteristics, location specifics, and value beyond what is available through standard search interfaces.

2.  **Strategic Real Estate Investment:** To build a profitable residential rental portfolio in Norway. The platform is designed to de-risk and optimize every stage of the investment lifecycle by providing data-driven answers to key questions:
    *   **Where to Invest:** Identify the best locations (`kommune`, `bydel`) with the optimal balance of rental demand, price stability, and growth potential.
    *   **What to Buy:** Pinpoint the most profitable property types and characteristics (e.g., specific number of rooms, amenities, energy labels) that command higher rental prices and attract stable, long-term tenants, thereby maximizing investment safety and minimizing turnover.
    *   **How to Add Value:** Systematically find properties with high value-add potential, such as those with opportunities for extension (large plots relative to the existing building) or renovation.
    *   **Who to Partner With:** Evaluate and select the most qualified and successful real estate agencies by analyzing their track record, market strategies, and performance in specific regions and property segments, should we decide to let them manage our properties.

3.  **B2B Business Development:** To identify and track prospect clients for our drone services company. By analyzing ads for new development projects, the platform will systematically extract information on active developers, construction companies, architects, and real estate agencies, creating a valuable, targeted lead generation pipeline.

## Supporting Documentation

This document provides a comprehensive strategic and architectural overview of the FinnStatistikk project. For more granular details on specific components, please refer to the following supporting documents, which may be provided alongside this one:

*   **Database Specification (`FinnStatistikk-Database-Design.md`):** Provides a complete and detailed schema for both the `Engine` and `Archive` databases, including all tables, columns, data types, and relationships.
*   **Analytics Goals Catalogue (`FinnStatistikk-Analytics-Goals-Catalogue.md`):** A draft version of the catalogue of all planned analytical goals, metrics, and qualitative inferences. This document details the specific questions the platform is designed to answer and guides the data collection and statistical processing efforts.
*   **Finn.no API Integration Specification (`FinnStatistikk-FinnApi-Headers.md`):** A detailed technical specification explaining the construction of all required HTTP headers, including the `User-Agent` string and the `FINN-GW-KEY` HMAC signature, for successfully sending API requests to Finn.no.
*   **Finn.no API DTO Definitions (`FinnStatistikk-Api-specs.md`):** Contains the complete C# Data Transfer Object (DTO) definitions that map to the JSON responses from the Finn.no API, serving as the contract for deserialization.
*   **LLM Integration Specification (`FinnStatistikk-LLM-Processing.md`):** Outlines the technical approach for integrating Large Language Models (LLMs) into the data processing pipeline for tasks such as sentiment analysis and structured data extraction from unstructured text.
*   **Android Device Profiles (`android-devices.csv`):** A raw data file containing a list of real-world Android device profiles used by the `UserIdentityManager` to generate believable user fingerprints.

## Technical Overview

The backend is built on a modern, high-performance technology stack, intended for deployment on a personal Linux server.

-   **Primary Framework:** .NET 8 (LTS)
-   **API Framework:** ASP.NET Core 8 Web API
-   **Scraper Implementation:** .NET 8 Worker Service
-   **Primary Language:** C# 12
-   **Databases:** PostgreSQL (hosting two separate logical databases: `FinnStatistikk_Engine` and `FinnStatistikk_Archive`)
-   **Data Access:** Entity Framework Core 8 with a Code-First approach.
-   **HTTP Client:** `IHttpClientFactory` with Polly for resilient, retrying HTTP requests.
-   **Anonymity Layer:** A system-wide VPN combined with `TorSharp` for managing a pool of Tor proxy instances.
-   **Validation:** FluentValidation for robust request model validation.
-   **Containerization:** Docker for containerizing the application services and database.
-   **Deployment Platform:** Self-hosted Linux Server running Docker.
-   **Package Manager:** NuGet

## System Architecture

The FinnStatistikk platform is grounded in modern software engineering principles to ensure the platform is not only powerful on day one but also scalable, maintainable, and operationally sound for years to come. It consists of several cooperating services orchestrated via Docker.

### Architectural Philosophy & Pillars

The entire system is engineered around four uncompromising architectural pillars, which dictate every design decision from the macro-service level down to individual class implementations.

1.  **The "Assembly Line" Philosophy (Decoupling & Specialization):** The system is designed as a decoupled, multi-stage data processing pipeline. Each stage—search discovery, detailed ad scraping, raw data archival, normalization, and statistical processing—is a specialized, independent worker. These workers communicate asynchronously via robust queues. This ensures that a failure or bottleneck in one stage does not cascade and halt the entire system. It also allows for individual components, like the resource-intensive `StatisticsProcessor`, to be scaled and deployed independently on more powerful hardware.

2.  **The Principle of Asynchronous Supremacy (Performance & Scalability):** Every operation that involves waiting for external resources—be it a database call, an HTTP request to an ad source, or a file write—is designed to be fully asynchronous. We will leverage modern .NET patterns like `IHttpClientFactory` with Polly for resilient, retrying, and non-blocking HTTP clients. The system is designed to handle thousands of concurrent operations gracefully, ensuring the scraper remains efficient and the API remains responsive, never allowing a single long-running task to block a thread.

3.  **The Mandate for Clean Abstraction (Maintainability & Extensibility):** A strict Clean Architecture is enforced. The core application logic is completely agnostic of its data sources. It does not know it is talking to "Finn.no"; it will communicate through a well-defined `IAdSourceProvider` interface. This ensures that in the future, supporting a new data source (e.g., another country's property portal) is a matter of implementing a new adapter, not re-engineering the core pipeline. This principle guarantees the system's longevity and adaptability.

4. **Immutability by Default**: For all DTOs and internal data-holding classes, use C# record types. Their value-based equality and non-destructive mutation (with keyword) prevent a wide class of bugs and make state easier to reason about.

5.  **The Doctrine of Operational Realism (Security & Robustness):** The design considers real-world operation from day one. This includes:
    *   **Anonymity:** A robust, two-layer anonymity strategy. A system-wide VPN provides a secure, foundational layer of protection, while a managed pool of `TorSharp` instances provides per-session IP rotation, crucial for simulating a diverse user base.
    *   **Resilience:** Implementing circuit breakers and exponential backoff for all external dependencies to handle transient failures gracefully.
    *   **Observability:** Building in structured, contextual logging to provide clear, traceable diagnostics for every ad processed.
    *   **Configuration:** Designing a strongly-typed, environment-aware configuration system that securely manages secrets and allows for runtime adjustments without redeployment.

6. **Result Pattern for Service Responses**: Instead of throwing exceptions for predictable failures (e.g., "Ad not found," "Validation failed"), application services should return a Result<T> object (from a library like FluentResults). This makes error handling explicit and separates expected outcomes from true, unexpected exceptions.

7. **API Idempotency**: Endpoints that modify state (like the proposed configuration API) should be idempotent. PUT, PATCH, and DELETE requests should be safe to retry without causing unintended side effects.

### High-Level System Overview

To promote separation of concerns, testability, and a multi-marketplace design, the solution will be structured into the following projects. This "plug-in" architecture allows new ad source providers to be developed and added with minimal changes to the core system.

-   **`FinnStatistikk.Domain`:** Contains core enterprise logic and entities (e.g., `Property`, `Ad`, `AdSnapshot`). Has no dependencies on other layers.
-   **`FinnStatistikk.Application`:** Contains application-specific business rules, use cases, and key abstractions, including `IEngineDbContext` and `IArchiveDbContext`.
-   **`FinnStatistikk.Infrastructure`:** Implements interfaces from `Application`. Contains PostgreSQL `DbContext`s, EF Core repositories, and other concrete infrastructure services.
-   **`FinnStatistikk.Contracts.Ipc`:** A shared library defining the data contracts for Inter-Process Communication.
-   **`FinnStatistikk.Contracts.AdSources`:** A shared library defining the core `IAdSourceProvider` interface and its related provider-agnostic DTOs.
-   **`FinnStatistikk.Infrastructure.Provider.Finn`:** A provider-specific project implementing `IAdSourceProvider` for Finn.no. Contains the `FinnClient`, the authentication/signing logic, and all DTOs specific to Finn.no.
-   **`FinnStatistikk.Api`:** The ASP.NET Core project for the API service. It provides read-only data access and administrative endpoints for configuration.
-   **`FinnStatistikk.Scraper`:** The primary .NET Worker Service project. It hosts the core "Assembly Line" workers and dynamically loads all available `IAdSourceProvider` implementations at runtime.
-   **`FinnStatistikk.Statistics.Processor`:** A standalone .NET Console Application for on-demand statistical analysis.

The platform's operational architecture consists of several cooperating services, communicating via IPC and the shared databases:

1.  **Scraper Service (`FinnStatistikk.Scraper`):** Hosts the data ingestion pipeline.
2.  **API Service (`FinnStatistikk.Api`):** Provides read-only data access and administrative endpoints for viewing and editing the platform's operational configuration.
3.  **Statistics Processor (`FinnStatistikk.Statistics.Processor`):** Performs heavy analytical computations.
4.  **Database Service (PostgreSQL):** A single PostgreSQL instance hosting two separate databases: `FinnStatistikk_Engine` and `FinnStatistikk_Archive`. This provides logical separation while simplifying administration.

This decoupled architecture allows the `Scraper` to run 24/7 on a low-power server, while the `StatisticsProcessor` can be run on-demand on a powerful workstation to perform its resource-intensive calculations.

### Inter-Service Communication & State Management

To enable remote configuration and real-time monitoring, a robust IPC mechanism is established between the API and the Scraper.

*   **Mechanism:** A duplex IPC channel, implemented using a Unix Domain Socket on Linux for high performance and security. A "null adapter" will be used on other operating systems (like Windows) during development to allow the services to run without requiring the IPC connection, ensuring cross-platform compatibility.
*   **State Service (`ScraperStateService`):** A singleton service within the `Scraper` process acts as the single source of truth for its operational state. It maintains real-time metrics such as queue depths, worker statuses (`Idle`, `Processing`), active job details, and statistics from the `TorPoolManager`.
*   **IPC Contract (`FinnStatistikk.Contracts.Ipc`):** The API can send commands to the Scraper (e.g., `GetFullState`, `UpdateMarketConfiguration`), and the Scraper can push state updates or critical log events back to the API, enabling a live monitoring dashboard.

### 4. Anonymity & Advanced User Simulation Layer

This layer is the cornerstone of the platform's operational security. It is architected with a clear separation between the generic, provider-agnostic mechanisms for anonymity and the provider-specific strategies for user simulation.

#### 4.1. The Two-Layer Anonymity Strategy

The platform employs a two-layer approach to ensure a high degree of anonymity and operational resilience.

1.  **Layer 1: System-Wide VPN (Foundational Anonymity):** The entire server operates behind a system-level VPN connection. This provides a constant, secure tunnel that masks the server's true IP address from all internet traffic, including the initial connections made to the Tor network. This is the first and most critical line of defense.
2.  **Layer 2: Per-Session Tor Circuits (User Simulation):** On top of the VPN, the application uses a pool of Tor circuits to provide a unique IP address for each simulated user session. This is essential for creating a believable and diverse user footprint, preventing the ad source from linking multiple scraping sessions to a single identity.

This layered strategy ensures that even if a Tor exit node is compromised or blocked, the server's core IP address remains protected by the VPN.

#### 4.2. Generic Anonymity Mechanisms (Provided by the Scraper Service)

The core `Scraper` service provides the foundational tools for the second layer of anonymity, which can be leveraged by any ad source provider.

*   **The `TorPoolManager` Service:** This critical service abstracts away the complexities and potential instability of `TorSharp`, providing a resilient, high-performance pool of anonymous IP circuits.
    *   **Pools:** It maintains two pools of `ManagedTorInstance` objects:
        *   **Active Pool:** A configurable number of instances (e.g., 6) currently assigned to active user sessions.
        *   **Reserve Pool:** A pool of instances that have completed self-testing and are ready for immediate use, ensuring zero delay when a new circuit is needed. The size of this pool will be at least equal to the active pool size.
    *   **Instance Lifecycle:**
        1.  **Creation:** A new `ManagedTorInstance` is created, wrapping a `TorSharpProxy`.
        2.  **Self-Test:** The instance acquires a new Tor circuit and performs a health check:
            *   **IP Uniqueness:** Queries `api.ipify.org` to ensure its public IP is not already in use by another instance in the pool.
            *   **Quality of Service (QoS):** Performs a brief latency and bandwidth test. Connections that fail to meet configurable thresholds (e.g., Ping > 2000ms, Bandwidth < 1 Mbps) are discarded.
        3.  **Ready:** The healthy instance is moved to the Reserve Pool.
        4.  **Activation & Session:** When a new user session begins, an instance is taken from the Reserve pool, associated with the session, and moved to the Active Pool.
        5.  **Discard:** At the end of the user session, the instance and its underlying Tor process are gracefully stopped and discarded. This ensures every new session starts with a fresh, clean circuit.
*   **The `ManagedTorInstance` Robustness Wrapper:** This class is the key to handling the potential instability of `TorSharp`, designed to treat it as an untrusted external process.
    *   **State Machine:** Manages the state (`Initializing`, `SelfTesting`, `ReadyInReserve`, `Active`, `Faulted`, `Stopped`).
    *   **"Black Box" Buffered Logging:** The wrapper subscribes to the `TorSharpProxy.OutputDataReceived` and `ErrorDataReceived` events. All output from the underlying `tor.exe` process is captured into a rolling in-memory buffer (e.g., last 200 lines).
    *   **Proactive Failure Detection & Reporting:** If the underlying process crashes (detected via `Process.Exited`) or `TorSharp` throws an unhandled exception (e.g., `FileNotFoundException` during `FetchAsync` or `SEHException` during startup), the wrapper immediately transitions to a `Faulted` state. It then logs a single, detailed critical error message containing the exception details *and the entire contents of its log buffer*. This provides a complete "black box" recording of the events leading up to the failure, enabling precise debugging without spamming logs during normal operation.

#### 4.3. Provider-Specific Simulation Strategies

The *strategy* for user simulation is highly dependent on the target marketplace. Each `IAdSourceProvider` implementation is responsible for managing its own session and identity logic.

*   **The Finn.no Implementation**
    The strategy implemented within `FinnStatistikk.Infrastructure.Provider.Finn` is designed to mimic believable human behavior and manage a resilient, anonymous network footprint.
    *   **`FinnUserIdentityManager` Service:** Manages a pool of 1,000+ simulated user identities persisted in the `Engine.UserIdentities` table.
        *   **Rotation Strategy:** When a new user session is initiated, the manager provides the identity that has been inactive the longest, ensuring a broad and slow rotation.
        *   **Failure & Retirement Logic:** Each identity tracks a local `ConsecutiveFailureCount` and a persisted `TotalFailureCount`.
            *   If a request fails, `ConsecutiveFailureCount` is incremented.
            *   On a successful request, it is reset to `0`.
            *   If `ConsecutiveFailureCount` reaches a configurable threshold (e.g., 3), the identity is marked as `IsActive = false` for a cool-down period.
            *   After the cool-down, it is retried. If it fails again, the cycle repeats. If the `TotalFailureCount` exceeds a lifetime threshold (e.g., 3), the identity is permanently retired and replaced with a newly generated one.
    *   **`DeviceProfileManager` & `FinnAppVersionManager` Services:** These services provide the building blocks for creating believable user fingerprints.
        *   **Device Profiles:** A static catalogue of ~80 real-world Android device profiles (Model, Brand, OS Version, Build ID) will be loaded into the `Engine.DeviceProfiles` table from a seed CSV file.
        *   **App Versions:** A background worker, `FinnAppVersionSyncWorker`, will periodically scrape `apkpure.net` using AngleSharp to fetch the latest Finn.no `VersionName` and `VersionCode`. These are stored in the `Engine.FinnAppVersions` table with a `DiscoveredDate`.
        *   **Identity Provisioning:** When a new `FinnUserIdentity` is created, it is assigned a random `DeviceProfile` and one of the most recent `FinnAppVersion`s, simulating a gradual rollout of app updates across the user base via a dedicated algorithm.
    *   **Night-time Simulation (Humanization):** To avoid a robotic, 24/7 scraping pattern, the system will operate with distinct day/night profiles, managed via the central configuration and editable via the API. A `TimeOfDayService` will determine the current profile.
        *   **Reduced Activity:** During configured night hours (e.g., 23:30 - 08:00), the `SearchDiscoveryWorker` will increase its delay between runs to a much larger value (e.g., `NighttimeSearchRunInterval: 3 hours` (instead of e.g., 6 seconds +/- 50% during day)).
        *   **Increased Delays:** The `HttpRequestExecutorWorker` will use a separate, longer delay profile between individual requests during night hours (e.g., `NighttimeInterRequestDelay: 25 seconds +/- 50%`) to simulate reduced user activity.

### 5. The Provider-Agnostic HTTP Request Pipeline

To centralize all external communication and enforce operational policies, a dedicated, prioritized request execution pipeline is established.

*   **Worker:** `HttpRequestExecutorWorker`
*   **Responsibility:** To be the *sole* component responsible for making outbound HTTP requests. It orchestrates user simulation, signing, anonymity, and resilience.
*   **Input Queue:** `HttpRequestQueue`. This is a `PriorityBlockingCollection<HttpRequestJob>`.
*   **Job Object:** `HttpRequestJob { Guid SessionId, int Priority, string ProviderName, string Url, HttpMethod Method, string Payload, ResponseType ExpectedResponseType }`.
*   **Process:**
    1.  Maintains a dictionary of active user sessions. Each session has a context object containing a `UserIdentity`, a `ManagedTorInstance`, and a configurable timer for its lifespan (e.g., 5 minutes +/- 50%).
    2.  Continuously dequeues the highest-priority `HttpRequestJob`.
    3.  Retrieves or creates a user session for the job. Implements a random delay between requests *for that session* based on the current `TimeOfDayService` profile (day/night) without blocking other sessions.
    4.  Constructs an `HttpRequestMessage`.
    5.  Sends the request through the `IHttpClientFactory` client configured for the specific `ProviderName`.
    6.  The handler chain (see Section 7) automatically applies resilience policies and provider-specific authentication.
    7.  On success/failure, updates the provider-specific identity's failure counters, if applicable.
    8.  Places the `ProcessedHttpResponse` onto the central `HttpResponseQueue`.

### 6. The "Assembly Line": Data Ingestion & Processing Pipeline

The data collection workflow is a series of specialized workers that communicate via the centralized request pipeline and dedicated processing queues. It is designed to be fully resilient, ensuring that no work is lost across application restarts.

**Principle of Queue Persistence & Re-hydration:** To guarantee data integrity and ensure no work is lost across application restarts or crashes, the system adheres to a strict queueing protocol. For any processing stage where the input represents a committed state change (e.g., a raw ad has been saved and is ready for normalization), the work item is first persisted to the database with a "Pending" status. Only then is it added to the corresponding in-memory queue for immediate processing. At application startup, each worker performs a re-hydration routine: it queries the database for any items left in a "Pending" state from the previous run and enqueues them, ensuring that all outstanding work is automatically resumed.

#### Stage 1: Configuration Synchronization

*   **Worker:** `ProviderConfigurationSyncWorker`
*   **Trigger:** On application startup and then on a 24-hour timer.
*   **Generic Process:**
    1.  Iterates through all registered `IAdSourceProvider` implementations loaded at runtime.
    2.  For each provider, it calls the `SyncConfigurationAsync()` method. This method acts as a black box to the worker; the provider is solely responsible for fetching its necessary metadata (e.g., market lists, categories) and persisting it to the `Engine` database.

*   **The Finn.no Implementation:**
    *   The `FinnAdSourceProvider`'s implementation of `SyncConfigurationAsync` triggers two separate `HttpRequestJob`s: one for the Finn.no `/api/` endpoint (to get the market list) and another background job (`FinnAppVersionSyncWorker`) to scrape the latest app version.
    *   A corresponding `MarketListResponseProcessor` (a small, internal part of this worker) listens to the `HttpResponseQueue` for its job completion.
    *   On receiving the response, it deserializes the market list and synchronizes it with the `Engine.Markets` table, preserving the `IsScrapingEnabled` flag and `LastScrapedAdTimestamp` timestamp.

#### Stage 2: Ad Discovery

*   **Worker:** `SearchDiscoveryWorker`
*   **Trigger:** Self-scheduling. The worker executes its full logic for all providers, and upon completion, it schedules the next run after a configurable delay determined by the `TimeOfDayService` (e.g., 60 minutes during the day, 3 hours at night).
*   **Generic Process:**
    1.  Iterates through all registered `IAdSourceProvider` implementations.
    2.  Calls the provider's `GetEnabledMarketsAsync()` method to retrieve the list of markets that are configured for scraping *for that provider*.
    3.  For each enabled market, it calls the provider's `DiscoverAdsAsync(market)` method. The provider is responsible for its own logic for paginating through search results and identifying which ads need to be scraped in detail.
    4.  The provider's implementation of `DiscoverAdsAsync` is expected to create and place one or more `ScrapeRequest` messages onto the central `AdDetailScrapeQueue`. Each `ScrapeRequest` contains the `ProviderName` and a source-specific `AdIdentifier`.

*   **The Finn.no Implementation:**
    *   The `FinnAdSourceProvider`'s `DiscoverAdsAsync` method implements the timestamp-based efficiency check. It retrieves the `LastScrapedAdTimestamp` for the market from the database to use as a stop condition.
    *   It begins paginating through the market's search results, creating and queuing `HttpRequestJob`s for each page.
    *   A `SearchResponseProcessor` listens for the responses. For each `FinnAdSummaryDocDto`, it checks if `ad.timestamp <= LastScrapedAdTimestamp`. If so, it stops paginating.
    *   Otherwise, it checks if the ad is new or updated and places a `ScrapeRequest("Finn", ad.Id)` onto the `AdDetailScrapeQueue`.
    *   After pagination, it updates the `LastScrapedAdTimestamp` for the market.

#### Stage 3: Raw Data Archival

*   **Worker:** `DetailScrapingWorker`
*   **Trigger:** Dequeues `ScrapeRequest` messages from the `AdDetailScrapeQueue`.
*   **Generic Process:**
    1.  Dequeues a `ScrapeRequest` message.
    2.  Using the `ProviderName` from the message, it dynamically resolves the correct `IAdSourceProvider` instance.
    3.  It calls the provider's `FetchAdDetailsAsync(request.AdIdentifier)` method.
    4.  The provider is responsible for making one or more HTTP requests and returning a collection of generic `RawAdPayload` objects. Each payload contains the raw data (e.g., as a JSON string) and metadata about its type (e.g., "AdView", "Profile").
    5.  The worker persists these payloads to the `Archive.RawAdSnapshots` table, associating them with the correct source provider and setting the `ProcessingStatus` to `Queued`.
    6.  A `ProcessingRequest(RawSnapshotId)` message is placed onto the `NormalizationQueue`.

*   **The Finn.no Implementation:**
    *   The `FinnAdSourceProvider`'s `FetchAdDetailsAsync` method creates two `HttpRequestJob`s: one for `/adview/{adId}` and one for `/getProfile?adId={adId}`.
    *   An internal response processor waits for both JSON payloads before creating a single `RawAdSnapshots` record containing both the `AdViewJson` and `ProfileJson`.

#### Stage 4: Normalization & Persistence

*   **Worker:** `NormalizationProcessorWorker`
*   **Trigger:** Dequeues `ProcessingRequest` messages from the `NormalizationQueue`.
*   **Responsibility:** To transform the raw data from the `Archive` into the structured, relational format of the `Engine` database. This is the most complex worker.
*   **Startup Behavior (Resilience):** At application startup, this worker performs a re-hydration task. It queries the `Archive.RawAdSnapshots` table for all records where `ProcessingStatus = Queued` and places their `RawSnapshotId` into the `NormalizationQueue` before starting its regular processing loop. This ensures that any unprocessed ads from a previous, interrupted run are automatically picked up.
*   **Generic Process:**
    1.  Dequeues a `ProcessingRequest(RawSnapshotId)` message.
    2.  Fetches the corresponding record from `Archive.RawAdSnapshots`.
    3.  Dynamically resolves the correct `IAdSourceProvider` using the provider name associated with the raw snapshot.
    4.  Calls the provider's `NormalizeAndPersistAsync(rawSnapshotRecord)` method, delegating the entire complex normalization logic to the provider-specific implementation.
    5.  The provider is responsible for returning a success/fail status and a list of media URLs to be downloaded.
    6.  **Media Queuing:** For each image URL returned by the provider, it places a `DownloadRequest(Url, LocalPath)` message onto the `MediaDownloadQueue`.
    7.  **LLM Queuing:** Places an `LlmEnrichmentRequest(SnapshotId)` onto the `LlmQueue`.
    8.  **Status Update:** Updates the `ProcessingStatus` in the `Archive.RawAdSnapshots` table to `Processed` (1) or `Failed` (2) and logs any errors.

*   **The Finn.no Implementation:**
    *   The `FinnAdSourceProvider`'s implementation of `NormalizeAndPersistAsync` contains the detailed business logic:
    *   **Deserialization:** Deserializes the `AdViewJson` and `ProfileJson` into their respective DTO models. The `JsonSerializer` will be configured with custom converters (like the `ProfileCardConverter`) to handle polymorphism.
    *   **Property Unification:** Invokes the `PropertyUnificationService`. This service uses a combination of `CadastreDetails` and a fuzzy match on `StreetAddress` and `PostalCode` to determine if this ad belongs to an existing `PropertyId` in the `Engine` database. If not, a new `Property` record is created.
    *   **Data Normalization & Upsert:**
        *   Creates or updates the central `Engine.Ads` record.
        *   Creates a new `Engine.AdSnapshots` record.
        *   `PostProcessingStatus` is initialized to `0` (None).
        *   For every categorical field (e.g., `OrganisationName`, `PropertyType`, `Facilities`), it performs a "Get or Create" operation against the corresponding lookup table (`Agencies`, `PropertyTypes`, `Facilities`) to get the integer foreign key.
        *   Populates the correct polymorphic detail table (`RealEstateHomeDetails`, `RealEstateProjectDetails`, etc.) with the structured data.
        *   Populates all supporting tables (`Media` (URLs only), `CadastreDetails`, `NeighbourhoodPois`, etc.).
    *   **Output:** Returns a success flag and a list of all image and floorplan URLs to the generic worker.

#### Stage 5: Supporting Tasks (Media & AI)

*   **Worker:** `MediaDownloadWorker`
    *   **Responsibility:** To download image assets asynchronously without blocking the main processing pipeline.
    *   **Startup Behavior (Resilience):** To ensure no media downloads are lost, this worker will rely on a periodic cleanup job. The `NormalizeAndPersistAsync` step creates `Media` records with a placeholder `LocalPath`. A separate, low-priority background worker will periodically scan the `Media` table for entries with a null `LocalPath`, queueing them for download. This avoids a large startup query and distributes the resilience check over time.
    *   **Process:** Dequeues `DownloadRequest` messages and downloads the image from the `Url` to a configured file storage system. Upon success, it updates the `LocalPath` field in the `Engine.Media` table and sets the `ImagesDownloaded` bit in the `PostProcessingStatus` field for the corresponding snapshot.

*   **Worker:** `LlmEnrichmentWorker`
    *   **Responsibility:** To perform AI-based data extraction and analysis on the normalized data.
    *   **Startup Behavior (Resilience):** To ensure all required AI enrichment tasks are completed, this worker queries the `Engine.AdSnapshots` table at startup. It identifies all snapshots where the `LlmEnriched` bit in the `PostProcessingStatus` field is not set and queues an `LlmEnrichmentRequest` for each. This ensures that any snapshots that were not processed in a previous run are automatically scheduled for enrichment.
    *   **Process:** Dequeues `LlmEnrichmentRequest` messages. It constructs a provider-specific prompt by concatenating relevant text fields (`Title`, `SummaryText`, `PropertyInfo`, etc.) from the `Engine` database, sends it to an LLM service, and persists the structured JSON response back into dedicated columns or tables in the `Engine` database. Sets the `LlmEnriched` bit in the `PostProcessingStatus` field.

### 7. The Provider-Agnostic HTTP Interaction Layer

This layer is the highly specialized component responsible for all external communication. It is designed to be configurable on a per-provider basis.

*   **Abstraction (`FinnStatistikk.Contracts.AdSources` layer):**
    *   The `IAdSourceProvider` interface defines a method: `void ConfigureHttpClient(IHttpClientBuilder builder)`. This allows each provider to inject its own specific behaviors into the HTTP pipeline.

*   **Configuration (`Scraper` layer):**
    *   In `Startup.cs`, the application iterates through all registered `IAdSourceProvider` implementations.
    *   For each provider, it registers a named `HttpClient` using `AddHttpClient(provider.Name, ...)`.
    *   Within the registration, it applies generic `DelegatingHandler`s first (like `PollyRetryHandler` and a `TorProxyHandler` responsible for routing the request through a local Tor proxy managed by `TorPoolManager`).
    *   Then, it calls `provider.ConfigureHttpClient(httpClientBuilder)`, allowing the provider to add its own specific handlers.

*   **The Finn.no Implementation:**
    *   The `FinnAdSourceProvider`'s `ConfigureHttpClient` method adds the `FinnHttpSigningHandler`.
    *   **`FinnHttpSigningHandler`:** This is the final handler in the chain for Finn.no requests. Before the request is sent, it performs three critical functions:
        1.  Retrieves the session context, which includes the active `FinnUserIdentity` and its associated `DeviceProfile` and `FinnAppVersion`.
        2.  Constructs the dynamic `User-Agent` and request headers according to the logic defined in Section 4.
        3.  Calculates the required `FINN-GW-KEY` HMAC-SHA512 signature and injects it, along with all other required headers sourced from the session context and static configuration.

#### 7.1. Finn.no API Request Headers Construction

The construction of valid and dynamic HTTP request headers is paramount to the project's success. It is the primary mechanism by which the scraper emulates a legitimate mobile application and avoids detection. Every request sent to the Finn.no API **must** include the following headers, constructed dynamically for each session.

**Canonical List of Required Headers:**

| Header | Example Value | Source |
| :--- | :--- | :--- |
| `User-Agent` | `FinnApp_And/250616-85b30270 (...)` | Session (Device Profile) |
| `FINN-Device-Info` | `Android, mobile` | Static |
| `VersionCode` | `1004576694` | Session (Device Profile) |
| `Session-Id` | `uuid-for-this-session` | Session (Generated) |
| `Visitor-Id` | `persistent-uuid-for-identity` | Session (User Identity) |
| `FINN-App-Installation-Id` | `persistent-uuid-for-identity` | Session (User Identity) |
| `Ab-Test-Device-Id` | `persistent-uuid-for-identity`| Session (User Identity) |
| `x-nmp-os-version` | `14` | Session (Device Profile, field `AndroidVersion`) |
| `x-nmp-device` | `SM-G998B` | Session (Device Profile, field `DeviceModel` ) |
| `x-nmp-app-version-name` | `250616-85b30270` | (same as version name) |
| `x-nmp-app-build-number`| `1004576694` | (same as version code) |
| `CMP-Advertising` | `1` | Static |
| `CMP-Analytics` | `1` | Static |
| `CMP-Marketing` | `1` | Static |
| `CMP-Personalisation` | `1` | Static |
| `x-nmp-os-name` | `Android` | Static |
| `x-nmp-app-brand` | `finn` | Static |
| `x-finn-apps-adinput-version-name` | `viewings` | Static |
| `X-FINN-API-Version` | `5` | Static |
| `Feature-Toggles` | `(long list of flags)`| Static |
| `X-FINN-API-Feature` | `7,9,13,15,16...` | Static |
| `Build-Type` | `release` | Static |

**Note on Static Headers:** The `Feature-Toggles`, and `X-FINN-API-Feature` headers will be sourced from a static configuration file (`finn_headers.json`). While they are static per-request, they can be updated via configuration changes without a full deployment if future analysis reveals that they need to be refreshed.

#### 7.2. User-Agent Construction Logic

The `User-Agent` string is dynamically constructed for each session using a precise format reverse-engineered from the Finn.no Android application. The format is:

```
%s/%s (Linux; U; Android %s; %s; %s Build/%s) %s(UA spoofed for tracking) %s
```

Example: `FinnApp_And/250616-85b30270 (Linux; U; Android 9; nb_no; Redmi Note 8 Build/PKQ1.190616.001) FINNNativeApp(UA spoofed for tracking) FinnApp_And`

The `FinnHttpSigningHandler` populates this string using data from the active session's `UserIdentity`, `DeviceProfile`, and `FinnAppVersion`. For example:

-   `%s` (1st): "FinnApp_And" (Static)
-   `%s` (2nd): `FinnAppVersion.VersionName` (e.g., "250616-85b30270")
-   `%s` (3rd): `DeviceProfile.AndroidVersion` (e.g., "14")
-   `%s` (4th): "nb_no" (Static Locale)
-   `%s` (5th): `DeviceProfile.DeviceModel` (e.g., "SM-G998B")
-   `%s` (6th): `DeviceProfile.BuildID` (e.g., "UQ1A.240205.004")
-   `%s` (7th): "FINNNativeApp" (Static)
-   `%s` (8th): "FinnApp_And" (Static)

This dynamic construction, tied to a diverse and realistic pool of device profiles and app versions, is critical for blending in with legitimate traffic.

### 8. The Ad Platform API

The API is the clean, public-facing gateway to the platform's data and configuration.

*   **Framework:** ASP.NET Core 8 Web API.
*   **Primary Role:** To serve read-only data from the `FinnStatistikk_Engine` database and to provide a secure interface for editing the system's operational configuration.
*   **Key Endpoint Categories:**
    *   **Entity Endpoints:** `GET /api/v1/ads`, `GET /api/v1/properties/{propertyId}`. Provide access to the core normalized data with filtering, sorting, and pagination.
    *   **Statistics Endpoints:** The specific endpoints will be defined once the statistics data model is well-defined. They will serve the pre-calculated, aggregated data generated by the `StatisticsProcessor`. These endpoints will be designed to be extremely fast as they query simple, flat tables.
    *   **Configuration Endpoints:** `PUT /api/v1/config/markets/{marketId}`, `POST /api/v1/config/system`. Provide secure endpoints for authorized users to dynamically update the system's operational parameters, such as enabling/disabling market scraping or adjusting delay timers.

### 9. Core Technology & Implementation Details

*   **Data Access:** Entity Framework Core 8 will be used with a Code-First approach. The `Engine` and `Archive` databases will be managed by separate `DbContext`s (`EngineDbContext`, `ArchiveDbContext`) to maintain logical separation.
*   **Ad Source Abstraction:** All interactions with external marketplaces are routed through the `IAdSourceProvider` interface, ensuring the core application remains decoupled and extensible.

### 10. Operational Excellence & Deployment

*   **System-Level Anonymity (VPN):**
    1.  **Deployment:** Install the official Proton VPN Linux client on the host server.
    2.  **Configuration:**
        *   Configure the client to launch on system boot and auto-connect.
        *   **Enable the Permanent Kill Switch.** This is the most critical step to ensure the server's real IP is never exposed.
        *   For the scraper's sessions, configure the client to connect to a **Secure Core** server. The small performance overhead is an acceptable trade-off for the vastly increased security.
    3.  **Operation:** The `FinnStatistikk` services will run on top of this system-wide VPN connection. No code changes are needed in the application, as it will transparently and automatically benefit from the VPN's protection.

*   **Containerization (`docker-compose.yml`):** The entire platform will be orchestrated via Docker Compose for consistent development and easy deployment. The services will include:
    *   `ad-scraper`: The .NET Worker Service.
    *   `ad-api`: The ASP.NET Core API.
    *   `postgres-db`: A single PostgreSQL container hosting both the `Engine` and `Archive` databases.
    *   `pgadmin`: (Optional) A web UI for database management.
    The `StatisticsProcessor` is run as a `docker run` command, not as a long-running service.
*   **Configuration & Secrets Management:**
    *   Configuration is loaded from `appsettings.json`, `appsettings.{Environment}.json`, and environment variables.
    *   For local development, sensitive data will be stored in .NET User Secrets.
    *   For production deployment, secrets will be passed as environment variables to the Docker containers.
*   **Logging & Observability:**
    *   **Structured Logging:** Implemented using **Serilog**, configured to output JSON.
    *   **Traceability IDs:** To ensure comprehensive traceability, every log message will be enriched with two key identifiers:
        *   **`TraceId`:** A `Guid` generated at the start of a high-level workflow (e.g., a `SearchDiscoveryWorker` run for a specific market). This ID is propagated through all subsequent jobs and queues spawned by that workflow, allowing for complete tracing of a batch operation.
        *   **`SourceAdId`:** When processing a specific advertisement, its unique ID from the source marketplace (e.g., the `FinnAdId`) will be attached to the log scope. This allows for filtering all log entries related to a single ad's journey through the pipeline.

*   **Testing Strategy:**
    *   **Unit Tests (xUnit, Moq):** For testing business logic in the `Domain` and `Application` layers in isolation.
    *   **Integration Tests (Testcontainers):** For testing the `Infrastructure` layer, spinning up temporary PostgreSQL Docker containers to validate EF Core queries and repository logic.
    *   **End-to-End Tests:** A limited set of tests that run the entire scraper pipeline against mocked ad source API responses to ensure the workers and queues interact correctly.

### 11. The `StatisticsProcessor` Module

This is a **separate .NET Console Application**, not a worker inside the scraper service. This design choice is deliberate to isolate its heavy computational load.

*   **Trigger:** Manually executed or triggered by a cron job on a schedule (e.g., nightly).
*   **Responsibility:** To execute the analytical queries defined in Chapter A and populate dedicated statistics tables.
*   **Process:**
    1.  Connects directly to the `FinnStatistikk_Engine` database.
    2.  **In-Memory Caching:** On startup, it loads the entirety of small lookup tables (e.g., `Locations`, `PropertyTypes`, `Agencies`) into in-memory `Dictionary<int, string>` objects for extremely fast lookups during processing.
    3.  Executes a series of complex SQL queries or LINQ statements to calculate metrics like Median P/Sqm, Time on Market, Agency Performance Scores, etc., for various segments.
    4.  Writes the aggregated results to dedicated tables (e.g., `DailyMarketStats`, `AgencyPerformanceMetrics`) in a dedicated statistics database. This pre-calculation is crucial for API performance.

---

## Project Roadmap & Milestones

*This section it outdated and needs to be updated.*

### Phase 1: Foundation & Core Scraping
*Goal: Establish a functional backend capable of scraping, signing, storing, and serving data for a single market.*

-   **Milestone 1.1: Project Initialization & Setup**
    -   [ ] Create a new Git repository.
    -   [ ] Initialize the .NET 8 solution with the Clean Architecture structure (`Domain`, `Application`, `Infrastructure`, `Api`, `Scraper.Worker`).
    -   [ ] Set up basic dependency injection and structured logging (e.g., Serilog).

-   **Milestone 1.2: Port & Validate Finn API Client**
    -   [ ] Port the legacy `FinnService` logic for HMAC-SHA512 signing into a new service within the `Infrastructure` project.
    -   [ ] Port the `HttpRequestHeaderInjector` `DelegatingHandler`.
    -   [ ] Implement the `UserIdentity` rotation logic.
    -   [ ] Create a simple console app or test to validate that a signed request to a Finn.no endpoint (e.g., search) returns a `200 OK` response.

-   **Milestone 1.3: Database & Core Models**
    -   [ ] Set up a local PostgreSQL instance using Docker for development.
    -   [ ] Define the core EF Core entities as outlined in the `Database Design` section (`Ad`, `AdSnapshot`, `RealEstateDetails`, etc.).
    -   [ ] Create the initial EF Core `DbContext` and generate the first database migration.

-   **Milestone 1.4: Implement the Scraper Worker Service**
    -   [ ] Build the worker services for scheduling, executing, and processing requests using the queue-based architecture.
    -   [ ] Implement the logic in `HttpResponseProcessorWorker` to parse a search result, extract ads, and persist them to the database (initially for `REALESTATE_HOMES`).
    -   [ ] Implement logic to handle pagination (i.e., if a search result has multiple pages, queue requests for all subsequent pages).

-   **Milestone 1.5: Basic Data API**
    -   [ ] Create a read-only RESTful endpoint in the `Api` project: `GET /api/v1/ads?market=...`.
    -   [ ] This endpoint should query the database and return the list of scraped ads in a simple JSON format.

### Phase 2: Expanding Scope & Features
*Goal: Add support for more markets and enhance the API's querying capabilities.*

-   **Milestone 2.1: Add Support for Småbruk & Plots**
    -   [ ] **[API]** Research Finn.no to identify the correct market/filters for `Småbruk` (likely within `REALESTATE_HOMES` or `REALESTATE_LEISURE_SALE` with a specific filter parameter) and forest plots (`REALESTATE_LEISURE_PLOTS`).
    -   [ ] **[API]** Update the scraper configuration and processing logic to handle these new categories and persist their specific data points.

-   **Milestone 2.2: Add Support for Consumer Goods (`Torget`)**
    -   [ ] **[API]** Add `BAP_SALE` (Torget) to the scraper configuration.
    -   [ ] **[API]** Implement the data processing and persistence logic for consumer goods, mapping to the `ConsumerGoodDetails` table.

-   **Milestone 2.3: Advanced API Filtering**
    -   [ ] **[API]** Enhance the `GET /api/v1/ads` endpoint to support robust server-side filtering by price range, area size, location, and other relevant ad properties.
    -   [ ] **[API]** Add sorting capabilities to the endpoint (e.g., sort by price, date).
    -   [ ] **[API]** Create a new backend endpoint `GET /api/v1/ads/{finnId}/history` to return all historical snapshots for an ad.

### Phase 3: Deployment & Production Hardening
*Goal: Deploy the application to a live server and ensure it runs reliably.*

-   **Milestone 3.1: Containerization**
    -   [ ] Create a `Dockerfile` for the ASP.NET Core API.
    -   [ ] Create a `Dockerfile` for the Scraper Worker service.
    -   [ ] Create a `docker-compose.yml` file to orchestrate the API, Scraper, and PostgreSQL containers.

-   **Milestone 3.2: CI/CD & Deployment**
    -   [ ] Set up a basic CI/CD pipeline (e.g., using GitHub Actions) to build Docker images on push to the `main` branch.
    -   [ ] Write deployment scripts to pull and run the new images on the Linux server.

-   **Milestone 3.3: Monitoring & Maintenance**
    -   [ ] Configure structured logging to output JSON to files or a logging service.
    -   [ ] Implement basic health check endpoints in the API and Scraper.
    -   [ ] Set up a database backup strategy.