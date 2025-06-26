# FinnStatistikk DTO Discovery Tool: Project Overview

This document provides a complete overview of the **FinnStatistikk DTO Discovery Tool**, a standalone .NET console application. The purpose of this document is to serve as a self-contained starting point for understanding the tool's architecture, objectives, and implementation.

## 1. Project Context & Objectives

This tool is a specialized utility built for a larger project called **`FinnStatistikk`**. The main project's goal is to scrape advertisement data from the Norwegian marketplace **Finn.no** to build a private database for market analysis. The main project consists of:
1.  A backend **API** (`ASP.NET Core`) to serve the collected data.
2.  A frontend UI (`Vue.js`) to visualize the data.
3.  A **Scraper Service** (`.NET Worker`) that continuously polls Finn.no.

The Discovery Tool was created to solve a critical prerequisite for the Scraper Service: ensuring we have a complete and accurate set of C# DTO models that can deserialize any JSON response from Finn.no's private mobile API without data loss or exceptions.

### Core Objectives

The tool acts as an intelligent "structural diffing" engine, comparing live API responses against a registry of known object structures. Its specific goals are:

*   **Discovering Unknown Schemas:** Systematically identify new values for the `meta.schemaName` field in the Ad View API (`/adview/{adId}`). Each new schema represents an entirely new ad structure that requires a new C# DTO model.
*   **Mapping Unknown Sub-structures:** Find and log the concrete data structures for properties that are initially unknown or ambiguous (e.g., the `extras` field in `AdSummaryDocDto`).
*   **Identifying New Properties:** Detect when Finn.no adds new properties to existing, known JSON objects, which might represent new features or data points we want to capture.
*   **Generating AI-Friendly Output:** Produce a concise, context-rich log file (`discovery_log.md`) that can be used as a prompt for an AI assistant to help generate or update C# DTOs efficiently.

### Explicit Non-Goals

This tool will **not** track or report on new "enum-like" string values (e.g., for `main_search_key`, `flags`, `type`, etc.). This task is the responsibility of the main `FinnStatistikk` application's dynamic registry system. The Discovery Tool's focus is exclusively on **structural discovery**.

## 2. Technical Overview

*   **Platform:** .NET 8
*   **Application Type:** Console Application
*   **Primary Language:** C# 12
*   **Key Libraries:** `System.Text.Json` for JSON parsing and manipulation, `Microsoft.Extensions.Hosting` for dependency injection and configuration, `Polly` for resilient HTTP retries.
*   **API Interaction:** The tool is self-contained and implements its own `HttpClient` configuration for making signed requests to the Finn.no API.

### Finn.no API Authentication

The tool successfully implements the reverse-engineered authentication mechanism of the Finn.no mobile app:
1.  **User Identity Rotation:** It maintains a pool of virtual Android device identities and rotates them to mimic real users.
2.  **HMAC-SHA512 Signature:** For each request, it constructs a "secret string" from the HTTP method, path, query, and a specific gateway header.
3.  **Key Deobfuscation:** It deobfuscates a hardcoded key from the mobile app using a character-shifting algorithm (`ROT13`-like).
4.  **Signature Calculation:** It signs the secret string with the deobfuscated key using HMAC-SHA512 and Base64 encodes the result.
5.  **Header Injection:** A custom `DelegatingHandler` automatically injects the signature into the `FINN-GW-KEY` header, along with ~15 other required headers (User-Agent, Session-Id, etc.).

## 3. System Architecture & Core Concepts

The tool is built around three main components:

1.  **The Discovery Registry (`structural_schema.json`):** A single JSON file that acts as the tool's persistent "memory." It maps every unique JSON path (e.g., `$.docs[*].price`) to the set of property names observed at that path. It also contains a list of all discovered `schemaName` values. This file is intended to be committed to source control.

2.  **The Discovery Engine:** The core logic that traverses incoming `JsonNode` responses and compares their structure against the in-memory version of the Discovery Registry. It is responsible for identifying discrepancies and is resilient to duplicate property keys in the source JSON.

3.  **The Discovery Logger:** A service responsible for writing new discoveries to a human-readable Markdown file (`discovery_log.md`) in a structured, AI-friendly format.

### Detailed Workflow

1.  **Load Registry:** At startup, the tool loads the on-disk `structural_schema.json` into an in-memory `Dictionary<string, HashSet<string>>`.
2.  **Begin Scraping:** It iterates through a configured list of markets (defined by their `SearchKey`, e.g., `SEARCH_ID_REALESTATE_HOMES`) and pages from `appsettings.json`.
3.  **Fetch Data:** It uses a configured delay between every single HTTP request. For each ad found in a search result, it fetches both the **Search Result Summary** and the **Full Ad View** JSON from Finn.no's API gateway (`appsgw.finn.no`).
4.  **Parse and Traverse:** Each JSON response is parsed into a dynamic `JsonNode`. The Discovery Engine then recursively traverses the entire JSON tree.
5.  **Compare and Identify:** At each JSON object, the engine compares the object's property keys against the known properties for that specific JSON path in the in-memory registry.
6.  **Log and Update:** If a new schema or property is found, the discovery is logged to `discovery_log.md`. The new structure is **immediately added to the in-memory registry**. This ensures the same discovery is not logged multiple times within the same run.
7.  **Save Registry:** After the run is complete (or cancelled), the updated in-memory registry is written back to the on-disk `structural_schema.json` file.

### The Discovery Process in Detail

*   **`NEW_SCHEMA_NAME`:** When processing an ad view, it checks `$.meta.schemaName`. If the value is not in the registry's `schemas` list, it's a new ad type. The tool logs the new schema name and the *entire* `ad` JSON object as the contextual snippet.

*   **`NEW_STRUCTURE`:** This covers finding new properties in known objects and mapping unknown structures. As the engine traverses the JSON, if it encounters an object at a path (e.g., `$.docs[0].extras[0]`) whose property keys are not fully contained in the registry for that path, it logs a discovery. The contextual snippet is the parent object containing the new structure/property.

## 4. Key Data Formats (Input/Output)

### Input: `DiscoveryRegistry/structural_schema.json`

A JSON file containing all known schemas and the properties for each known JSON path.

```json
{
  "schemas": [
    "realestate-home",
    "realestate-development-project",
    "recommerce-sell"
  ],
  "paths": {
    "$.docs[*]": [
      "type", "id", "ad_id", "heading", "location", "image"
    ],
    "$.ad": [
      "title", "plot", "size", "price", "location"
    ]
  }
}
```

### Output: `discovery_log.md`

An append-only Markdown file logging all new discoveries.

**Example Log Entry: New Schema Name**
```markdown
---
- **Timestamp**: `2023-10-27T14:30:15Z`
- **Discovery Type**: `NEW_SCHEMA_NAME`
- **JSON Path**: `$.meta.schemaName`
- **Discovered Value**: `"realestate-leisure-plot"`
- **Source URL**: `https://appsgw.finn.no/adview/321456789`
- **Contextual JSON Snippet ($.ad)**:
\`\`\`json
{
  "title": "Idyllisk tomt med strandlinje på Sørlandet",
  "plot": { "area": 1250, "owned": true },
  "price": { "suggestion": 2500000, "total": 2550000 },
  "location": { "postalCode": "4640", "postalName": "Søgne" }
}
\`\`\`

---
```

**Example Log Entry: New Structure (e.g., for `extras`)**
```markdown
---
- **Timestamp**: `2023-10-27T14:35:02Z`
- **Discovery Type**: `NEW_STRUCTURE`
- **JSON Path**: `$.docs[0].extras`
- **Source URL**: `https://appsgw.finn.no/search/SEARCH_ID_BAP_SALE?page=1`
- **Contextual JSON Snippet ($.docs[0].extras)**:
\`\`\`json
[
    {
        "id": 2,
        "label": "Produkttype",
        "values": ["Stasjonær PC"]
    }
]
\`\`\`
```


# Current Task

I have successfully ran a first batch. I would like you to help me integrate the new discoveries into our DTO models.

There are a lot of discoveries, so I will send the data progressively:
- I will send `structural_schema.json` is full since it is compact;
- I will send `discovery_log.md` in segments because it is large, and you will process each part by integrating the required additions into our DTO;
- I will send all our DTO definitions.

Make sure to *comment* each property. Include *examples* when it makes sense. For example, it is particularly important to have comments AND examples on properties that are not immediately obvious: `Type`, `Unit`, `SearchKey`, etc. However, it is NOT necessary to have example on self-explanatory properties like `DisplayName`, `MinValue`, `Longitude`, etc. If the property is an enum or enum-analog type (eg. there is a discrete number of choices that are not obvious), and you have found a new value, add it to the list of examples. On enum properties, the examples should list all the known values. Make sure that the reader can easily understand what each record, class or property represents without prior context. This will be essential when writing the main app.

Don't forget to mark properties as optional where applicable (which should be most of the time since we are typically "discovering" these properties progressively).

Please write the `# Finn.no API DTO Models` section *IN FULL*, and only the `# Finn.no API DTO Models`. I will copy your output to my clipboard, and diff against the existing version so that I can integrate the changes into document.

It is ESSENTIAL that you follow this instruction otherwise I will not be able to diff the files and integrate your work, which will have been for nothing.

# structural_schema.json



# discovery_log.md





