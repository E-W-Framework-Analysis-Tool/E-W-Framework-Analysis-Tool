using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles job title / position type for active K-12 Teacher and Educator staff
/// from RDS.FactK12StaffEmployments, joined to RDS.DimK12StaffCategories and RDS.DimPeople.
/// Assesses record count, completeness of PositionTitle, and distribution across
/// K12StaffClassificationCode values.
/// </summary>
public class StaffPositionTypeCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Staff position type";

    public string Query => $@"
WITH StaffBase AS (
    SELECT
        p.PositionTitle,
        b.K12StaffClassificationCode
    FROM RDS.FactK12StaffEmployments f
    JOIN RDS.DimK12StaffCategories b ON b.DimK12StaffCategoryId = f.K12StaffCategoryId
    JOIN RDS.DimPeople p             ON f.K12StaffId = p.DimPersonId
    WHERE p.IsActiveK12Staff = 1
      AND (
              b.K12StaffClassificationCode LIKE '%Teacher%'
           OR b.K12StaffClassificationCode LIKE '%Educator%'
          )
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM StaffBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM StaffBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                                          AS DataElementName,
    'Completeness'                                                               AS CharacteristicType,
    CAST(COUNT(CASE WHEN PositionTitle IS NOT NULL AND PositionTitle <> ''
                    THEN 1 END) AS NVARCHAR(MAX))                                AS Value,
    'PopulatedRecords'                                                           AS SubItemLabel,
    NULL                                                                         AS Remarks
FROM StaffBase
UNION ALL
-- Distribution - by K12StaffClassificationCode
SELECT
    '{DataElementName}'                      AS DataElementName,
    'Distribution'                           AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))          AS Value,
    K12StaffClassificationCode               AS SubItemLabel,
    NULL                                     AS Remarks
FROM StaffBase
GROUP BY K12StaffClassificationCode";

    public string AssessmentDescription =>
        "Assesses job title or position type for active K-12 Teacher and Educator staff. " +
        "Reports total record count, completeness of the PositionTitle field, and distribution " +
        "of records across K12StaffClassificationCode categories.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
