namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles urbanicity data readiness from RDS.BridgeK12StudentEnrollmentPersonAddresses
/// joined to RDS.DimPersonAddresses. Urbanicity is not stored directly in the CEDS Data
/// Warehouse; this assessor checks whether student address records contain sufficient
/// location data (latitude/longitude or postal code) to support external urbanicity
/// derivation via geocoding or Census/NCES locale lookup.
/// </summary>
public class UrbanicityCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Urbanicity";

    public string Query => $@"
WITH AddressBase AS (
    SELECT
        a.Latitude,
        a.Longitude,
        a.AddressPostalCode
    FROM RDS.BridgeK12StudentEnrollmentPersonAddresses b
    JOIN RDS.DimPersonAddresses a
        ON b.PersonAddressId = a.DimPersonAddressId
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM AddressBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM AddressBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN
        (Latitude IS NOT NULL AND Latitude <> '')
        OR (AddressPostalCode IS NOT NULL AND AddressPostalCode <> '')
    THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM AddressBase
";

    public string AssessmentDescription =>
        "Assesses urbanicity data readiness from RDS.BridgeK12StudentEnrollmentPersonAddresses " +
        "joined to RDS.DimPersonAddresses. Urbanicity is not stored directly in the CEDS Data " +
        "Warehouse; populated records are those with a non-null latitude/longitude or postal code, " +
        "indicating that external urbanicity derivation via geocoding or NCES locale lookup is " +
        "feasible. A high completeness rate suggests the data source can support urbanicity-based " +
        "disaggregation.";
}
