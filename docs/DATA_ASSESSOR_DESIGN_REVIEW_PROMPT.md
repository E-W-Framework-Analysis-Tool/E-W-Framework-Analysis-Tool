# LLM-Assisted Assessor Review

This section is written as LLM instructions. Provide the `DataElementName` and `Query` along with this
document. The LLM should verify the query, enrich it with missing characteristics, and produce a corrected
C# class.

**Schema boundary:** Do not invent table or column names not already present in the submitted query. Any
suggested characteristic that requires schema not present in the query must use a placeholder and a
`-- NOTE:` comment — regardless of confidence. If you see `-- NOTE:` items in the output, locate the
relevant schema and resubmit with it included in the prompt to resolve them.

---

## Review Checklist

1. **DataElementName match** — Exactly matches the `DataElementName` property (case, spacing, punctuation),
   emitted via C# string interpolation (`'{DataElementName}'`), never as a separate hardcoded literal.
2. **Column structure** — Every `SELECT` returns all five columns in order: `DataElementName`,
   `CharacteristicType`, `Value`, `SubItemLabel`, `Remarks`.
3. **CAST usage** — Every `Value` is `CAST(... AS NVARCHAR(MAX))`.
4. **CharacteristicType validity** — Every type is one of: `RecordCount`, `ReportedAvailability`,
   `IntegerRange`, `Completeness`, `Distribution`.
5. **Row counts** — `IntegerRange` and `Completeness` produce exactly 2 rows each; `RecordCount` and
   `ReportedAvailability` produce exactly 1; `Distribution` produces 1 per category.
6. **SubItemLabel correctness** — `'Minimum'`/`'Maximum'` for `IntegerRange`; `'TotalRecords'`/
   `'PopulatedRecords'` for `Completeness`; category name for `Distribution`; `NULL` otherwise.
7. **CTE usage** — A CTE is used when multiple characteristics share the same source data.
8. **UNION ALL structure** — Blocks are separated by `UNION ALL` with identifying comments; no trailing
   `UNION ALL`.
9. **Insert structure** — Follows the [Query Structure Contract](#query-structure-contract) for its type
   (CTE vs. non-CTE); no leading semicolon before `WITH`; no trailing semicolon.
10. **Reasonableness** — The query actually measures what the data element name implies. Flag wrong tables,
    wrong fields, or wrong logic.
11. **Characteristic completeness** — Missing suggested characteristics per the
    [Suggested Characteristics by Element Type](#suggested-characteristics-by-element-type) table are added
    if possible, or flagged with `-- NOTE:` if schema is uncertain.

---

## Confidence Levels

- **Apply directly** — Unambiguous structural issues, and suggested characteristics implementable from
  tables and columns already present in the submitted query.
- **Add with `-- NOTE:`** — Anything requiring tables or columns not present in the submitted query.

---

## C# Output Format

```csharp
namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// [Brief description of what is being profiled and from which table(s)]
/// </summary>
public class [ClassName]CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "[Exact data element name]";
    public string Query => $@"
[Corrected and enriched SQL query]
";
    public string AssessmentDescription =>
        "[Human-readable summary of what characteristics are assessed and from which source]";
}
```

**Class naming:** Strip non-alphanumeric characters from `DataElementName`, PascalCase each word, append
`CedsDWAssessor`. Example: `"Student Age"` → `StudentAgeCedsDWAssessor`.

Respond with: (1) a checklist summary of issues found and characteristics added or flagged, and (2) the
corrected C# class.