Of course. Based on your inputs and our established architectural principles, I have drafted a new chapter dedicated to formalizing the role of Large Language and Multi-Modal Models within the FinnStatistikk platform.

This chapter outlines a comprehensive strategy and the corresponding technical architecture to integrate advanced AI capabilities, transforming the platform from a data aggregator into a sophisticated inference engine.

---

### **D. The LLM Enrichment & Analysis Layer**

This chapter details the architecture of the AI-powered enrichment layer of the **FinnStatistikk** platform. While the core system excels at capturing and structuring explicit data, a vast amount of high-value information remains latent within unstructured text and images. This layer is designed to systematically extract this latent data, applying human-like intuition at machine scale to unlock deeper, more nuanced insights into the real estate market.

#### **1. Strategic Role & Core Objectives**

The primary objective of the LLM Enrichment Layer is to **transmute unstructured data into structured, queryable assets**. It acts as a cognitive extension to our data pipeline, tasked with reading, understanding, and evaluating property advertisements in their entirety. Its strategic goals are threefold:

1.  **Data Completion (Filling the Gaps):** To systematically extract critical data points that are frequently mentioned in descriptive text but are missing from the structured fields of the ad (e.g., the specific type of heating system, the number of floors in a building, the presence of specific materials or construction techniques). This enriches our core dataset, making quantitative analysis more accurate and complete.

2.  **Qualitative Signal Extraction (Reading Between the Lines):** To move beyond simple facts and capture subjective and qualitative signals. This includes classifying the sentiment and tone of an ad's description, identifying potential "red flags" or "selling points" in the text, and assessing the overall quality and style of a property based on its visual representation. These signals are crucial for de-risking investments and understanding market psychology.

3.  **Multi-Modal Validation & Analysis (Connecting Text and Vision):** To leverage multi-modal AI to analyze images in conjunction with text. This allows the system to validate claims made in the description (e.g., verifying the presence of a "panoramic view"), assess the condition of a property from its photos, and score the quality of the marketing material itself, which is a key indicator of an agency's professionalism.

By achieving these objectives, this layer directly supports the project's core mission of building a superior market intelligence platform.

#### **2. Architectural Integration & The LLM Workflow**

The LLM Enrichment Layer is designed as a specialized, fully-decoupled component of the "Assembly Line" architecture. It is implemented via the `LlmEnrichmentWorker`, which operates asynchronously and in parallel to the main data ingestion pipeline, ensuring that its long-running, resource-intensive tasks do not impact the core scraping process.

The end-to-end workflow is as follows:

1.  **Queuing:** After the `NormalizationProcessorWorker` successfully processes a raw ad and persists the structured data to the `Engine` database, it concludes by placing a lightweight `LlmEnrichmentRequest` message onto a dedicated `LlmQueue`. The message contains only the `SnapshotId`.

2.  **Dequeuing & Data Aggregation:** The `LlmEnrichmentWorker` dequeues a request. Its first action is to connect to the `Engine` database and retrieve all data associated with the given `SnapshotId`. It aggregates all available text fields (`Title`, `SummaryText`, `GeneralTextJson`, `PropertyInfoJson`, `RegulationsText`, etc.) into a single, cohesive text block. This provides the LLM with the maximum possible context.

3.  **Dynamic Prompt Construction:** The worker selects the appropriate "Prompt Template" based on the ad's `SchemaName`. It then dynamically constructs the final prompt, injecting the aggregated text and a targeted JSON "questionnaire."

4.  **LLM API Call:** The worker sends the fully constructed prompt to the configured LLM service API (e.g., Google's Gemini API).

5.  **Response Parsing & Validation:** Upon receiving a response, the worker parses the resulting JSON. It validates the structure and checks the model-provided `confidence` score for each field against a configured threshold (e.g., `0.95`).

6.  **Persistence:** The validated, structured data from the LLM is persisted into the `LlmEnrichmentResults` table in the `Engine` database, linked directly to the original `SnapshotId`.

7.  **Status Update:** The worker sets the `LlmEnriched` bit in the `AdSnapshots.PostProcessingStatus` field, marking the task as complete for that snapshot.

#### **3. The Dynamic Prompt Engineering Framework**

To ensure flexibility, maintainability, and precision, the interaction with the LLM is governed by a dynamic prompt engineering framework. This framework allows us to evolve our analytical questions without re-deploying the application.

*   **Prompt Templates:** The core of the framework is a set of version-controlled prompt templates stored in the `Engine.PromptTemplates` table. Each template is a JSON object tailored to a specific `SchemaName` (e.g., one for `realestate-home`, another for `realestate-development-project`).

*   **Template Structure:** A typical prompt template has three parts:
    1.  **`SystemMessage`:** Instructs the LLM on its role, persona, and output format. (e.g., "You are an expert real estate analyst. Your task is to analyze the following property advertisement text and answer the questions in the provided JSON structure. For each answer, provide your confidence on a scale of 0.0 to 1.0.").
    2.  **`UserMessageTemplate`:** A template for the user prompt, containing placeholders for the data. (e.g., "Analyze this ad: ```{{AD_TEXT}}```. Fill out this JSON structure: ```{{JSON_QUESTIONNAIRE}}```").
    3.  **`QuestionnaireTemplate`:** A JSON structure defining the questions to be answered. This template contains all *possible* questions for that schema type.

*   **Dynamic Questionnaire Generation:** This is a critical efficiency feature. Before sending the prompt, the `LlmEnrichmentWorker` compares the `QuestionnaireTemplate` with the data already present in the `Engine` database for that `SnapshotId`. It **removes** any questions from the JSON questionnaire for which a value already exists, instructing the LLM to only focus on extracting missing information. This dramatically reduces token usage and improves response accuracy.

*   **Example `QuestionnaireTemplate` for `realestate-home`:**
    ```json
    {
      "heatingSource": {
        "value": null,
        "confidence": null,
        "question": "What is the primary heating source? (e.g., 'Fjernvarme', 'Varmepumpe', 'Elektrisk')"
      },
      "buildingFloors": {
        "value": null,
        "confidence": null,
        "question": "How many floors does the entire building have?"
      },
      "sentiment": {
        "value": null,
        "confidence": null,
        "question": "Classify the overall tone of the ad text. (Options: 'Neutral/Factual', 'Luxurious/Premium', 'Family-Friendly', 'Urgent/High-Demand')"
      },
      "conditionFlags": {
        "value": [],
        "confidence": null,
        "question": "List any keywords or phrases that indicate potential defects or need for renovation (e.g., 'oppussingsobjekt', 'TG3', 'behov for vedlikehold')."
      }
    }
    ```

#### **4. Multi-Modal Analysis: Integrating Vision**

To unlock insights from images, the framework is designed to seamlessly integrate multi-modal models (like Gemini 2.5 Flash).

*   **Workflow Integration:** The `LlmEnrichmentWorker`'s process is extended. After aggregating text, it queries the `Media` table for all locally downloaded images associated with the `SnapshotId`.
*   **Prompt Construction:** The worker constructs a multi-modal prompt payload. It reads the image files, converts them to base64 strings, and embeds them in the API request alongside the text and the JSON questionnaire. The prompt template for a multi-modal request will include image-specific questions.
*   **Vision-Specific Analytical Goals:**
    *   **Image Quality Score:** "Rate the overall quality of the provided images (lighting, clarity, composition) on a scale of 1 to 10."
    *   **Room Identification:** "For each image, identify the type of room shown (e.g., 'Kitchen', 'Bathroom', 'Bedroom', 'Facade', 'Living Room')."
    *   **Amenity & Feature Verification:** "Does any image visually confirm the presence of a balcony, fireplace, or panoramic view?"
    *   **Style & Condition Assessment:** "Based on the images, classify the interior design style ('Modern', 'Classic', 'Scandinavian', 'Needs Renovation') and identify any visible signs of wear, damage, or outdated fixtures."
    *   **Floor Plan Analysis:** "If an image of a floor plan is provided, validate its room count against the ad's text and extract the total area if visible."

#### **5. Data Persistence & Schema Integration**

The data generated by the LLM is valuable and requires a dedicated, structured home in the `Engine` database.

*   **`PromptTemplates` Table:**
    *   `PromptTemplateId` (Integer, PK)
    *   `SchemaNameId` (Integer, FK to `SchemaNames.SchemaNameId`)
    *   `Version` (Integer)
    *   `ModelTarget` (Text): e.g., "gemini-1.5-pro", "gemini-2.5-flash-multimodal"
    *   `TemplateJson` (JSONB): The full prompt template.
    *   `IsActive` (Boolean)

*   **`LlmEnrichmentResults` Table:**
    *   **Purpose:** Stores the structured output of text-based analysis for a single snapshot.
    *   **Columns:**
        *   `SnapshotId` (BigInt, PK, FK): The snapshot this result belongs to.
        *   `PromptTemplateId` (Integer, FK): The exact template version used.
        *   `ExecutionTimestamp` (Timestamp (UTC)): When the analysis was run.
        *   `CostInMicroCents` (Integer): The cost of the API call, for budget tracking.
        *   `ExtractedHeatingSource` (Text, Nullable).
        *   `ExtractedBuildingFloors` (Integer, Nullable).
        *   `SentimentClassification` (Text, Nullable).
        *   `ConditionFlagsJson` (JSONB): A JSON array of extracted "red flag" keywords.
        *   `GeneratedSummary` (Text, Nullable): An AI-generated summary of the ad.
        *   `RawResponseJson` (JSONB): The complete, unmodified JSON response from the LLM for debugging and future reprocessing.

*   **`LlmImageAnalysisResults` Table:**
    *   **Purpose:** Stores the structured output of image-based analysis.
    *   **Columns:**
        *   `ImageAnalysisId` (BigInt, PK).
        *   `MediaId` (BigInt, FK to `Media.MediaId`): The specific image that was analyzed.
        *   `SnapshotId` (BigInt, FK): The parent snapshot.
        *   `OverallQualityScore` (Decimal(3, 2), Nullable): e.g., 8.5/10.0.
        *   `IdentifiedRoomType` (Text, Nullable).
        *   `DetectedFeaturesJson` (JSONB): e.g., `["Balcony", "Fireplace"]`.
        *   `AssessedStyle` (Text, Nullable).
        *   `AssessedConditionNotes` (Text, Nullable).

#### **6. Operational Considerations & Best Practices**

Deploying LLMs in a production system requires careful operational planning to manage costs, ensure consistency, and handle failures.

*   **Cost Management:** The `LlmEnrichmentWorker` will be designed with strict cost controls. Every API call will calculate the token usage and convert it to a monetary value, which is stored in the `CostInMicroCents` field. A central configuration will define a monthly budget, and the system will automatically pause the LLM worker if the budget is exceeded.
*   **Versioning & Reprocessing:** The `PromptTemplates` table includes a `Version` number. This is critical. When we improve a prompt, we will create a new version and mark it as active. The `LlmEnrichmentWorker` will always use the latest active version for new snapshots. Crucially, the API will expose an administrative endpoint to **re-queue a batch of old snapshots for reprocessing** with a specific, newer prompt version. This allows us to continuously improve the quality of our historical data as our AI capabilities evolve.
*   **Batching & Throughput:** To maximize efficiency and respect API rate limits, the `LlmEnrichmentWorker` will dequeue a batch of requests (e.g., 10) from the `LlmQueue` and execute the calls to the LLM API in parallel using `Task.WhenAll`.
*   **Error Handling & Fallbacks:** If an LLM API call fails or returns malformed JSON, the worker will log the error along with the `SnapshotId` and the raw response payload. It will retry the request using an exponential backoff strategy. If it fails repeatedly, the snapshot will be marked with a specific error state for manual review, preventing a single problematic ad from halting the entire enrichment pipeline.