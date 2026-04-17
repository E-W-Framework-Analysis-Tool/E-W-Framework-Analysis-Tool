# Data Assessor Design

This document describes the design and implementation patterns for data assessors in the E-W Framework Analysis Tool.
Assessors are the core mechanism for evaluating data readiness across different data sources.

## Overview

This tool uses a common data model where all assessors, regardless of implementation type, produce Data Element
Assessments containing standardized Data Characteristics. This allows downstream scoring algorithms to consistently
evaluate data readiness across API-based, query-based, and manual assessment methods.

### Key Principles

- **Data Element Level:** Each assessor focuses on a single data element (e.g., "Student Age", "Suspensions and
  Expulsions (K-12)"). The data element name must match exactly the corresponding entry in FrameworkReferenceData
  DataElements — it is not a CEDS element name, it is the application's own framework reference.
- **Standardized Output:** All assessors produce the same core data characteristics structure
- **Automatic Discovery:** C#-based assessors are automatically discovered and executed via reflection
- **Flexible Implementation:** Support for both real-time API assessment and air-gapped SQL-based assessment

## Assessor Types

### API-Based Assessors (Ed-Fi)

API-based assessors connect directly to Ed-Fi ODS APIs to assess data in real-time.

Each assessor should implement the [`IEdFiAssessor`](../src/EwFrameworkAnalysis.Common/Assessors/EdFi/IEdFiAssessor.cs)
C# interface.

Key features of the API-based assessors:

- Direct API interaction via authenticated HttpClient
- Real-time progress reporting through `AssessorContext`
- Automatic discovery via reflection — any class implementing `IEdFiAssessor` is automatically registered and executed
- Common patterns available via [`EdFiApiPatterns`](../src/EwFrameworkAnalysis.Common/Assessors/EdFi/EdFiApiPatterns.cs)
  helper methods

### Query-Based Assessors (CEDS Data Warehouse)

Query-based assessors profile a CEDS Data Warehouse for a single E-W Framework data element. Because the tool runs
in the browser without direct database access, queries are bundled into a single executable script that users run
in their own environment and import results back into the tool (air-gapped workflow).

Each assessor implements the
[`ICedsDWAssessor`](../src/EwFrameworkAnalysis.Common/Assessors/Ceds/ICedsDWAssessor.cs) interface. The
[`CedsDWAssessmentOrchestrator`](../src/EwFrameworkAnalysis.Common/Services/CedsDWAssessmentOrchestrator.cs)
discovers all implementations via reflection and combines them into a single script.

Key features:

- SQL queries designed for CEDS Data Warehouse schema
- Air-gapped assessment workflow: generate script → execute in SSMS → import CSV results
- Automatic discovery via reflection — any class implementing `ICedsDWAssessor` is included
- All assessor queries insert into a shared `#EWFProfilerResults` temp table; a final `SELECT` returns the combined result set

#### Query Structure Contract

Each assessor's `Query` property is emitted verbatim into the generated script. The orchestrator:

1. Creates the shared `#EWFProfilerResults` temp table
2. Appends each assessor's `Query` followed by a `;`
3. Selects all rows from `#EWFProfilerResults` at the end

Because of this, every `Query` must:

- **Insert its own rows** into `#EWFProfilerResults` using `INSERT INTO #EWFProfilerResults`
- **Not end with a semicolon** — the orchestrator adds one
- **Not reference or create `#EWFProfilerResults`** — the orchestrator owns the table definition

For queries **without a CTE**:

```sql
INSERT INTO #EWFProfilerResults
SELECT ...
UNION ALL
SELECT ...
```

For queries **with a CTE**, `WITH` must open the statement and `INSERT INTO #EWFProfilerResults` must follow the CTE
definition, before the `SELECT`:

```sql
WITH MyCte AS (
    SELECT ... FROM RDS.SomeTable
)
INSERT INTO #EWFProfilerResults
SELECT ... FROM MyCte
UNION ALL
SELECT ... FROM MyCte
```

Do **not** add a leading semicolon before `WITH` — the orchestrator ensures clean statement boundaries.

#### Required Column Structure

Every `SELECT` in a CEDS assessor query must return exactly these five columns in this order:

| Column               | Type     | Notes                                                                          |
| -------------------- | -------- | ------------------------------------------------------------------------------ |
| `DataElementName`    | NVARCHAR | Must exactly match the assessor's `DataElementName` property                   |
| `CharacteristicType` | NVARCHAR | One of the valid types listed in [Data Characteristics](#data-characteristics) |
| `Value`              | NVARCHAR | Always cast: `CAST(... AS NVARCHAR(MAX))`                                      |
| `SubItemLabel`       | NVARCHAR | NULL or a type-specific label (see each characteristic below)                  |
| `Remarks`            | NVARCHAR | Optional context, or NULL                                                      |

The `DataElementName` value must be emitted via C# string interpolation using the property itself — never
hardcoded as a separate literal — so that refactoring the property updates the SQL automatically:

```csharp
public string DataElementName => "Suspensions and Expulsions (K-12)";

public string Query => $@"
INSERT INTO #EWFProfilerResults
SELECT
    '{DataElementName}' AS DataElementName,
    ...
";
```

#### Query Organization

**CTE usage:** When multiple characteristics can be derived from the same underlying data, compute shared values
once in a CTE. Each characteristic's `SELECT` block then reads from the CTE rather than re-querying the base table.

**UNION ALL structure:** All characteristic rows are combined using `UNION ALL`. Each block should be preceded by
a comment identifying what it produces:

```sql
-- RecordCount
SELECT ...
UNION ALL
-- Completeness - TotalRecords
SELECT ...
UNION ALL
-- Completeness - PopulatedRecords
SELECT ...
```

**No trailing UNION ALL:** The final `SELECT` block must not be followed by `UNION ALL`.

**Ordering:** Related rows for a single characteristic must be kept together and not interleaved with other
characteristics.

#### Suggested Characteristics by Element Type

| Element Type                                                        | Suggested Characteristics                                            |
| ------------------------------------------------------------------- | -------------------------------------------------------------------- |
| **Numeric** (age, grade level, score, count)                        | `RecordCount`, `Completeness`, `NumericalRange`                      |
| **Categorical** (race/ethnicity, program type, disability category) | `RecordCount`, `Completeness`, `Distribution`                        |
| **Boolean / Flag** (enrolled, active, indicator)                    | `RecordCount`, `Completeness`, `Distribution` (`'True'` / `'False'`) |
| **Date** (birth date, enrollment date, exit date)                   | `RecordCount`, `Completeness`                                        |
| **Identifier / Text** (student ID, name)                            | `RecordCount`, `Completeness`                                        |

Example: [`StudentAgeCedsDWAssessor`](../src/EwFrameworkAnalysis.Common/Assessors/Ceds/StudentAgeCedsDWAssessor.cs)

---

## Data Characteristics

All assessors must produce one or more of these standardized data characteristics:

### RecordCount

Simple count of records matching the data element criteria. Produces exactly 1 row.

**Query output:**

```sql
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS SubItemLabel,
    NULL AS Remarks
```

**C# output:**

```csharp
new RecordCount(totalStudents)
```

### ReportedAvailability

Subjective, user-driven judgment about data availability. Used in manual assessments only. Produces exactly 1 row.

**Values:** `Available`, `PartiallyAvailable`, `NotAvailable`, `InsufficientData`

**C# output:**

```csharp
new ReportedAvailability(AvailabilityJudgment.Available)
```

### NumericalRange

Minimum and maximum values for a numeric data element. Produces exactly 2 rows.

**Query output:**

```sql
-- NumericalRange - Minimum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'      AS CharacteristicType,
    CAST(MinValue AS NVARCHAR(MAX)) AS Value,
    'Minimum'           AS SubItemLabel,
    NULL                AS Remarks
UNION ALL
-- NumericalRange - Maximum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'      AS CharacteristicType,
    CAST(MaxValue AS NVARCHAR(MAX)) AS Value,
    'Maximum'           AS SubItemLabel,
    NULL                AS Remarks
```

**C# output:**

```csharp
new NumericalRange(minValue, maxValue, "Age Range")
```

### Completeness

Measures data completeness for a specific field by comparing populated records to total records. Produces exactly
2 rows.

**Query output:**

```sql
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(TotalCount AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL AS Remarks
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN SomeColumn IS NOT NULL AND SomeColumn <> '' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL AS Remarks
```

> **Populated records:** Count rows where the relevant field has a meaningful value. For text fields, exclude both NULLs and empty strings (`IS NOT NULL AND <> ''`). For nullable non-text fields, `COUNT(col)` (which excludes NULLs) is sufficient. When the data element name covers multiple fields (e.g., "identifier or title"), use a single `CASE` combining them with `OR` rather than producing separate `Completeness` blocks per field.

**C# output:**

```csharp
new Completeness(totalRecords, populatedRecords, "BirthDate")
```

> **Note:** Constructor parameter order is `(totalRecords, populatedRecords, attributeName)`. The characteristic
> automatically calculates a `Percentage` property.

### Distribution

Counts grouped by categorical values (e.g., race/ethnicity, grade levels, program types). Produces 1 row per
category.

**Query output:**

```sql
-- Distribution - Hispanic or Latino
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'Hispanic or Latino' AS SubItemLabel,
    NULL                AS Remarks
UNION ALL
-- Distribution - White
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'White'             AS SubItemLabel,
    NULL                AS Remarks
-- ... one block per category
```

**C# output:**

```csharp
new Distribution(
    new Dictionary<string, int>
    {
        ["Hispanic or Latino"] = 1250,
        ["White"] = 3400,
        ["Black or African American"] = 890,
        // ... other categories
    },
    "Race/Ethnicity"
)
```
