namespace EwFrameworkAnalysis.Common.Assessors.Ceds;
/// <summary>
/// Defines a SQL-based assessor that profiles a CEDS Data Warehouse for a single
/// E-W Framework data element.
/// <para>
/// Each implementation provides a SQL query that inserts profiling rows into the
/// shared <c>#EWFProfilerResults</c> temp table created by <see cref="CedsDWAssessmentOrchestrator"/>.
/// The orchestrator combines all discovered assessors into a single executable script
/// for the air-gapped assessment workflow (generate → execute in SSMS → import results).
/// </para>
/// <para>
/// See <c>docs/DATA_ASSESSOR_DESIGN.md</c> for the full authoring guide, query structure
/// contract, and characteristic type reference.
/// </para>
/// </summary>
public interface ICedsDWAssessor
{
    /// <summary>
    /// The E-W Framework data element name this assessor profiles. Must match exactly
    /// the corresponding entry in FrameworkReferenceData DataElements — this value is
    /// emitted verbatim as the <c>DataElementName</c> column in every SQL row.
    /// </summary>
    string DataElementName { get; }

    /// <summary>
    /// The SQL statement that inserts this assessor's profiling rows into <c>#EWFProfilerResults</c>.
    /// Must follow the query structure contract in <c>docs/DATA_ASSESSOR_DESIGN.md</c>.
    /// Do not end with a semicolon — the orchestrator adds one.
    /// </summary>
    string Query { get; }

    /// <summary>
    /// Human-readable summary of what is profiled and from which source table(s).
    /// Emitted as a comment above the query in the generated script.
    /// </summary>
    string AssessmentDescription { get; }

    /// <summary>
    /// The earliest CEDS Data Warehouse version this assessor supports.
    /// Use a constant from <see cref="CedsDwVersions"/> (e.g. <c>CedsDwVersions.V13</c>).
    /// The orchestrator excludes this assessor when generating a script for any version
    /// that precedes this value in <see cref="CedsDwVersions.All"/>.
    /// </summary>
    string MinVersion { get; }

    /// <summary>
    /// The latest CEDS Data Warehouse version this assessor supports, or <c>null</c> if
    /// the assessor applies to all versions from <see cref="MinVersion"/> onward.
    /// Use a constant from <see cref="CedsDwVersions"/> (e.g. <c>CedsDwVersions.V13</c>).
    /// The orchestrator excludes this assessor when generating a script for any version
    /// that follows this value in <see cref="CedsDwVersions.All"/>.
    /// </summary>
    string? MaxVersion { get; }
}
