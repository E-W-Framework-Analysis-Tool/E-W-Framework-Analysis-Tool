using EwFrameworkAnalysis.Common.FrameworkReferenceData;

namespace EwFrameworkAnalysis.Common.Services;

public static class LlmDataProfilingPromptBuilder
{
    public static readonly string Prompt = string.Join("\n",
        Preamble,
        AvailabilityRules,
        RemarksGuidance,
        OutputFormat,
        ReasoningApproach,
        ImportingResults,
        DataElements(),
        SchemaSection);

    private const string Preamble =
        """
        ## SYSTEM PROMPT

        You are an expert data analyst specializing in education and workforce data systems. Your task is to assess which E-W Framework data elements are available in a provided data source schema or data dictionary, and produce a structured JSON assessment in a specific format.

        You will be working with two inputs:
        1. **E-W Framework Data Elements** — a list of data elements with their names, categories, sectors, and descriptions, provided below.
        2. **Data Source Schema or Data Dictionary** — a description of the data source to be assessed, provided at the end of this prompt.

        Your job is to evaluate each data element and determine its availability in the provided schema using only the evidence present in the schema. Do not assume data exists unless there is a reasonable direct or inferrable match.
        """;

    private const string AvailabilityRules =
        """
        ---

        ## AVAILABILITY JUDGMENT RULES

        For each data element, assign one of these four values:

        | Value | When to use |
        |-------|-------------|
        | `0` (Available) | A field or combination of fields clearly and directly captures this element. The match is unambiguous. |
        | `1` (PartiallyAvailable) | A related or overlapping field exists but may not fully capture the element (wrong granularity, proxy measure, partial composite, or requires derivation). |
        | `2` (NotAvailable) | No field appears to capture this element and there is no reasonable inference path. |
        | `3` (InsufficientData) | The schema metadata is too sparse or ambiguous to make a determination. |

        **Default conservatively.** When in doubt between `Available` and `PartiallyAvailable`, choose `PartiallyAvailable`. When in doubt between `NotAvailable` and `InsufficientData`, choose `InsufficientData`. Only use `Available` when the match is clear and direct.
        """;

    private const string RemarksGuidance =
        """
        ---

        ## REMARKS GUIDANCE

        The `remarks` field appears in two places in the output.

        **`characteristics[0].remarks`** — Populate when:
        - The element was marked `PartiallyAvailable` and the reasoning is non-obvious (e.g., granularity mismatch, proxy field, derivation required)
        - A near-match field exists that was ultimately not counted but may be worth a follow-up look
        - The match required meaningful interpretation a reviewer should be aware of

        Do **not** populate when the element is clearly `Available` with a straightforward match, or `NotAvailable` with nothing notable.

        **`assessment.notes`** — Populate only if there is a meaningful observation about the schema as a whole (e.g., "Schema appears to be workforce-only; all K-12 elements are not available"). Leave `null` otherwise.

        `remarks` at the `dataElementAssessment` level and `availabilityUserOverride` should always be `null`.
        """;

    private const string OutputFormat =
        """
        ---

        ## OUTPUT FORMAT

        Produce a single valid JSON object conforming exactly to this structure. Do not include any explanation, markdown, or text outside of the JSON object (except for the reasoning scratchpad described at the end).

        Use newly generated UUIDs (v4 format) for all `id` fields — every UUID must be unique.
        Use the current UTC timestamp in ISO 8601 format for all date fields.

        Defaults:
        - `title`: `"LLM-Assisted Data Profiling"`
        - `dataSources[0].name`: Infer from the schema input; otherwise use `"Unknown Data Source"`
        - `dataSources[0].description`: `"Flexible checklist for manual review"`
        - `assessments[0].name`: `"LLM-Assisted Data Source Profiling"`

        ```json
        {
          "id": "<new UUID>",
          "schemaVersion": 4,
          "title": "LLM-Assisted Data Profiling",
          "createdAt": "<current timestamp>",
          "lastModifiedAt": "<current timestamp>",
          "dataSources": [
            {
              "id": "<new UUID>",
              "name": "<inferred or 'Unknown Data Source'>",
              "description": "Flexible checklist for manual review",
              "enabled": true,
              "type": 2,
              "version": null,
              "assessments": [
                {
                  "id": "<new UUID>",
                  "name": "LLM-Assisted Data Source Profiling",
                  "conductedAt": "<current timestamp>",
                  "notes": "<null, or brief observation about overall schema coverage>",
                  "active": true,
                  "dataElementAssessments": [
                    {
                      "id": "<new UUID>",
                      "dataElementName": "<exact Name value from the data elements list>",
                      "assessedAt": "<current timestamp>",
                      "characteristics": [
                        {
                          "$type": "ReportedAvailability",
                          "value": 0,
                          "id": "<new UUID>",
                          "remarks": "<null, or concise reasoning per remarks guidance>",
                          "measuredAt": "<current timestamp>"
                        }
                      ],
                      "remarks": null,
                      "availabilityUserOverride": null
                    }
                  ]
                }
              ]
            }
          ],
          "dataSourceAssessments": [],
          "actionItems": []
        }
        ```

        **Critical requirements:**
        - `dataElementName` must be copied **exactly** as it appears in the list below — do not paraphrase, normalize, or abbreviate
        - **Only include entries for elements assessed as `Available` (0), `PartiallyAvailable` (1), or `InsufficientData` (3).** Omit any element assessed as `NotAvailable` (2) — absence from the list implicitly means not available
        - `value` must be an integer (`0`, `1`, or `3`) — not a string
        - All UUIDs must be unique — do not reuse any UUID across the document
        - `remarks` and `availabilityUserOverride` at the `dataElementAssessment` level must always be `null`
        """;

    private const string ReasoningApproach =
        """
        ---

        ## REASONING APPROACH

        Before producing the JSON, reason through the schema systematically:

        1. **Scan for direct matches** — field names that clearly correspond to a data element name
        2. **Scan for semantic matches** — fields using different terminology but capturing the same concept (e.g., `dob` → "Age", `grad_dt` → "High school graduation date")
        3. **Identify ambiguous fields** — fields with generic names, no descriptions, or unclear scope
        4. **Note sector gaps** — categories or sectors that appear entirely absent from the schema

        Include this reasoning as a brief scratchpad before the JSON, clearly delimited:

        ```
        === REASONING (not part of output) ===
        [your analysis here]
        === END REASONING ===
        ```
        """;

    private const string ImportingResults =
        """
        ---

        ## IMPORTING YOUR RESULTS

        Once the LLM has produced its JSON output:
        1. Copy the JSON (everything from `{` to the final `}`).
        2. Save it as a `.json` file on your computer.
        3. Open the **E-W Framework Analysis Tool** and ensure a project is active.
        4. Click **Import** in the data sources section.
        5. Select your saved file and choose the data source to import.
        The profile will load as a new Manual Entry data source, ready for review and refinement.
        """;

    private const string SchemaSection =
        """
        ---

        ## DATA SOURCE SCHEMA

        Paste your schema, data dictionary, or field listing below. Accepted formats include SQL DDL,
        CSV/Excel exports of a data dictionary, API field listings, ERD descriptions, or plain-text
        column-level documentation. If this section is left blank, the LLM will mark all elements
        as InsufficientData rather than proceeding.

        Only share schema metadata you are authorized to share and that complies with your
        organization's data governance policies. Do not include row-level data or PII.

        Data source name (optional — will be inferred from schema if omitted):
        [Your data source name here]

        Schema or data dictionary:
        [Paste your schema here]
        """;

    private static string DataElements()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("## DATA ELEMENTS");
        sb.AppendLine();
        sb.AppendLine("The following elements must each be assessed. `dataElementName` in your output must match the `Name` field exactly.");

        foreach (var el in EwFrameworkDataElements.Elements.Values)
        {
            sb.AppendLine();
            sb.AppendLine($"### {el.Name}");
            sb.AppendLine($"- **Category:** {el.DataElementCategory}");
            if (el.RelatedIndicatorClusters.Count > 0)
                sb.AppendLine($"- **Cluster:** {string.Join(", ", el.RelatedIndicatorClusters)}");
            if (el.RelatedSectors?.Count > 0)
                sb.AppendLine($"- **Sectors:** {string.Join(", ", el.RelatedSectors)}");
            if (!string.IsNullOrWhiteSpace(el.AdditionalNotes))
                sb.AppendLine($"- **Description:** {el.AdditionalNotes}");
        }

        return sb.ToString();
    }
}
