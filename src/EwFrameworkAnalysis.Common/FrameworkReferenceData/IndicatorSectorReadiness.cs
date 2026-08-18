using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

/// <summary>
/// Readiness for adoption is ascribed per sector, not per indicator — the same
/// indicator can be well-established for one sector and emerging for another.
/// </summary>
public class IndicatorSectorReadiness
{
    public required Sector Sector { get; init; }
    public required IndicatorReadiness Readiness { get; init; }
}
