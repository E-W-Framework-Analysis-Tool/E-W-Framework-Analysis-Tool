# Data Assessor Design

This document describes the design and implementation patterns for data assessors in the E-W Framework Analysis Tool.
Assessors are the core mechanism for evaluating data readiness across different data sources.

## Overview

This tool uses a common data model where all assessors, regardless of implementation type, produce Data Element
Assessments containing standardized Data Characteristics. This allows downstream scoring algorithms to consistently
evaluate data readiness across API-based, query-based, and manual assessment methods.

### Key Principles

- **Data Element Level:** Each assessor focuses on a single data element (e.g., "Student Age", "Suspensions and
  Expulsions (K-12)")
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
- Automatic discovery via reflection—any class implementing `IEdFiAssessor` is automatically registered and executed
- Common patterns available via [`EdFiApiPatterns`](../src/EwFrameworkAnalysis.Common/Assessors/EdFi/EdFiApiPatterns.cs)
  helper methods

### Query-Based Assessors (CEDS Data Warehouse)

Query-based assessors generate SQL queries that users execute in their secure environments and import results back into
the tool.

These queries are added to the application as implementations of the
[`ICedsDWAssessor`](../src/EwFrameworkAnalysis.Common/Assessors/Ceds/ICedsDWAssessor.cs) interface. Refer to existing
examples for recommendations on how to format these (e.g. use exact `DataElementName` in the query via string
interpolation).

Key features of the query-based assessors:

- SQL queries designed for CEDS Data Warehouse
- Air-gapped assessment workflow (export query > execute > import results)
- Automatic discovery and query generation via
  [`CedsDWAssessmentOrchestrator`](../src/EwFrameworkAnalysis.Common/Services/CedsDWAssessmentOrchestrator.cs)
- All discovered queries are combined with `UNION ALL` into a single executable script

### Required Column Structure

The most critical aspect of each individual query is that it produces exactly the same columns as the rest.

```sql
SELECT
    'Data Element Name' AS DataElementName,
    'RecordCount' AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS SubItemLabel,
    NULL AS Remarks
```

All queries must return exactly these columns in this order:

1. **DataElementName** (NVARCHAR): The name of the data element being assessed
2. **CharacteristicType** (NVARCHAR): One of: `RecordCount`, `ReportedAvailability`, `IntegerRange`, `Completeness`,
   `Distribution`
3. **Value** (NVARCHAR): The measured value as a string (use `CAST(... AS NVARCHAR(MAX))`)
4. **SubItemLabel** (NVARCHAR): Label for sub-items (NULL for simple characteristics, may be `Minimum` and `Maximum` for
   ranges, for example, refer to specific characteristic documentation)
5. **Remarks** (NVARCHAR): Optional notes or context (can be NULL)

Example: [`StudentAgeCedsDWAssessor`](../src/EwFrameworkAnalysis.Common/Assessors/Ceds/StudentAgeCedsDWAssessor.cs)

## Data Characteristics

All assessors must produce one or more of these standardized data characteristics:

### RecordCount

Simple count of records matching the data element criteria.

**Query Output**:

```sql
SELECT
    'Data Element Name' AS DataElementName,
    'RecordCount' AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS SubItemLabel,
    NULL AS Remarks
```

**C# Output**:

```csharp
new RecordCount(totalStudents)
```

### ReportedAvailability

Subjective and user-driven judgment about data availability, used in manual assessments.

**Values**: `Available`, `PartiallyAvailable`, `NotAvailable`, `InsufficientData`

**C# Output**:

```csharp
new ReportedAvailability(AvailabilityJudgment.Available)
```

### IntegerRange

Minimum and maximum values for numeric data elements (e.g., age ranges, grade levels).

**Query Output** (requires three rows):

```sql
-- Minimum
SELECT
    'Data Element Name' AS DataElementName,
    'IntegerRange' AS CharacteristicType,
    CAST(MinValue AS NVARCHAR(MAX)) AS Value,
    'Minimum' AS SubItemLabel,
    NULL AS Remarks

UNION ALL

-- Maximum
SELECT
    'Data Element Name' AS DataElementName,
    'IntegerRange' AS CharacteristicType,
    CAST(MaxValue AS NVARCHAR(MAX)) AS Value,
    'Maximum' AS SubItemLabel,
    NULL AS Remarks
```

**C# Output**:

```csharp
new IntegerRange(minValue, maxValue, "Age Range")
```

### Completeness

Measures data completeness for a specific field by comparing populated records to total records.

**Query Output** (requires three rows):

```sql
-- Total Records
SELECT
    'Data Element Name' AS DataElementName,
    'Completeness' AS CharacteristicType,
    CAST(TotalCount AS NVARCHAR(MAX)) AS Value,
    'TotalRecords' AS SubItemLabel,
    NULL AS Remarks

UNION ALL

-- Populated Records
SELECT
    'Data Element Name' AS DataElementName,
    'Completeness' AS CharacteristicType,
    CAST(PopulatedCount AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords' AS SubItemLabel,
    NULL AS Remarks
```

**C# Output**:

```csharp
new Completeness(totalRecords, populatedRecords, "BirthDate")
```

> **Note**: The constructor parameter order is `(totalRecords, populatedRecords, attributeName)`. The characteristic
> automatically calculates a `Percentage` property.

### Distribution

Counts grouped by categorical values (e.g., race/ethnicity categories, grade levels, program enrollment). Includes a
label and dictionary of counts.

**Query Output** (requires one row for label + one row per category):

```sql
-- Category counts (one row per category)
SELECT
    'Data Element Name' AS DataElementName,
    'Distribution' AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'Hispanic or Latino' AS SubItemLabel,  -- Category name
    NULL AS Remarks

UNION ALL

SELECT
    'Data Element Name' AS DataElementName,
    'Distribution' AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'White' AS SubItemLabel,
    NULL AS Remarks
-- ... repeat for each category
```

**C# Output**:

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
