-- =============================================================================
-- Seed Social-Emotional Learning (SEL) student assessment data into Ed-Fi ODS
-- =============================================================================
-- Target: EdFi_Ods_GrandBend (localhost,14333)
-- Usage:  sqlcmd -S localhost,14333 -U sa -P 'P@ssw0rd123' -d EdFi_Ods_GrandBend -i seed-sel-assessment-data.sql
--
-- Creates 5 SEL assessment instruments and 50 student assessment records with
-- performance levels and score results. All SEL records use the 'SEL-' prefix
-- for easy identification and cleanup.
--
-- Idempotent: safe to re-run (deletes previous SEL data first).
-- =============================================================================

USE EdFi_Ods_GrandBend;
GO

-- ---------------------------------------------------------------------------
-- Clean up previous runs
-- ---------------------------------------------------------------------------
PRINT 'Cleaning up previous SEL assessment data...';

DELETE FROM edfi.StudentAssessmentScoreResult
WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org';

DELETE FROM edfi.StudentAssessmentPerformanceLevel
WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org';

DELETE FROM edfi.StudentAssessment
WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org';

DELETE FROM edfi.Assessment
WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org';
GO

-- ---------------------------------------------------------------------------
-- Descriptor ID constants (verified from edfi.Descriptor in GrandBend)
-- ---------------------------------------------------------------------------
-- Grade Levels
--   Kindergarten          = 2899
--   Third grade           = 2902
--   Fifth grade           = 2903
--   Eighth grade          = 2908
--   Ninth grade           = 2907
--   Tenth grade           = 2909
--   Eleventh grade        = 2910
--   Twelfth grade         = 2913
--
-- Performance Levels
--   Advanced              = 1285
--   Proficient            = 1291
--   Basic                 = 1282
--   Below Basic           = 1283
--   Well Below Basic      = 1284
--
-- Assessment Reporting Method
--   Scale score           = 754
--
-- Result Datatype
--   Integer               = 2062
--
-- Assessment Category
--   Early Learning - Social and emotional development = 3237
--   Benchmark test        = 3258
--   Developmental observation = 3250

-- ---------------------------------------------------------------------------
-- 1. Assessment definitions
-- ---------------------------------------------------------------------------
PRINT 'Inserting SEL assessment definitions...';

INSERT INTO edfi.Assessment
    (AssessmentIdentifier, Namespace, AssessmentTitle, AssessmentFamily, AssessmentCategoryDescriptorId)
VALUES
    ('SEL-ASQ-SE2',  'uri://ed-fi.org', 'ASQ:SE-2 Social-Emotional Screening',              'Social-Emotional Learning', 3237),
    ('SEL-DECA',     'uri://ed-fi.org', 'DECA Devereux Early Childhood Assessment',          'Social-Emotional Learning', 3250),
    ('SEL-SAEBRS',   'uri://ed-fi.org', 'SAEBRS Social-Academic-Emotional Behavior Screener','Social-Emotional Learning', 3258),
    ('SEL-SSIS',     'uri://ed-fi.org', 'SSIS Social Skills Improvement System',             'Social-Emotional Learning', 3258),
    ('SEL-CORE',     'uri://ed-fi.org', 'CORE Districts SEL Survey',                         'Social-Emotional Learning', 3258);
GO

-- ---------------------------------------------------------------------------
-- 2. Student Assessment records (50 rows)
-- ---------------------------------------------------------------------------
PRINT 'Inserting 50 student assessment records...';

INSERT INTO edfi.StudentAssessment
    (AssessmentIdentifier, Namespace, StudentAssessmentIdentifier, StudentUSI, AdministrationDate, WhenAssessedGradeLevelDescriptorId)
VALUES
    -- ASQ:SE-2 — PK/Kindergarten screening (10 records, StudentUSI 1-10)
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-001',  1, '2025-09-15', 2899),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-002',  2, '2025-09-15', 2899),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-003',  3, '2025-09-16', 2899),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-004',  4, '2025-09-16', 2899),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-005',  5, '2025-09-17', 2899),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-006',  6, '2025-09-17', 2899),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-007',  7, '2025-09-18', 2899),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-008',  8, '2025-09-18', 2899),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-009',  9, '2025-09-19', 2899),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-010', 10, '2025-09-19', 2899),

    -- DECA — PK/Kindergarten social-emotional (10 records, StudentUSI 11-20)
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-011', 11, '2025-10-01', 2899),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-012', 12, '2025-10-01', 2899),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-013', 13, '2025-10-02', 2899),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-014', 14, '2025-10-02', 2899),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-015', 15, '2025-10-03', 2899),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-016', 16, '2025-10-03', 2899),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-017', 17, '2025-10-04', 2899),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-018', 18, '2025-10-04', 2899),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-019', 19, '2025-10-05', 2899),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-020', 20, '2025-10-05', 2899),

    -- SAEBRS — K-12 behavior screener (10 records, StudentUSI 21-30, mixed grade levels)
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-021', 21, '2025-10-10', 2902),  -- Third grade
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-022', 22, '2025-10-10', 2902),  -- Third grade
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-023', 23, '2025-10-11', 2903),  -- Fifth grade
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-024', 24, '2025-10-11', 2903),  -- Fifth grade
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-025', 25, '2025-10-12', 2908),  -- Eighth grade
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-026', 26, '2025-10-12', 2908),  -- Eighth grade
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-027', 27, '2025-10-13', 2907),  -- Ninth grade
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-028', 28, '2025-10-13', 2907),  -- Ninth grade
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-029', 29, '2025-10-14', 2909),  -- Tenth grade
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-030', 30, '2025-10-14', 2909),  -- Tenth grade

    -- SSIS — K-12 social skills (10 records, StudentUSI 31-40, mixed grade levels)
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-031', 31, '2025-10-20', 2899),  -- Kindergarten
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-032', 32, '2025-10-20', 2899),  -- Kindergarten
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-033', 33, '2025-10-21', 2903),  -- Fifth grade
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-034', 34, '2025-10-21', 2903),  -- Fifth grade
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-035', 35, '2025-10-22', 2908),  -- Eighth grade
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-036', 36, '2025-10-22', 2908),  -- Eighth grade
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-037', 37, '2025-10-23', 2910),  -- Eleventh grade
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-038', 38, '2025-10-23', 2910),  -- Eleventh grade
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-039', 39, '2025-10-24', 2913),  -- Twelfth grade
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-040', 40, '2025-10-24', 2913),  -- Twelfth grade

    -- CORE SEL Survey — K-12 self-report (10 records, StudentUSI 41-50, mixed grade levels)
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-041', 41, '2025-11-01', 2902),  -- Third grade
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-042', 42, '2025-11-01', 2903),  -- Fifth grade
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-043', 43, '2025-11-02', 2908),  -- Eighth grade
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-044', 44, '2025-11-02', 2907),  -- Ninth grade
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-045', 45, '2025-11-03', 2909),  -- Tenth grade
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-046', 46, '2025-11-03', 2910),  -- Eleventh grade
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-047', 47, '2025-11-04', 2913),  -- Twelfth grade
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-048', 48, '2025-11-04', 2899),  -- Kindergarten
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-049', 49, '2025-11-05', 2902),  -- Third grade
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-050', 50, '2025-11-05', 2903);  -- Fifth grade
GO

-- ---------------------------------------------------------------------------
-- 3. Performance Levels (~60% of records = 30 rows)
--    Distributed across the 5 performance level categories
-- ---------------------------------------------------------------------------
PRINT 'Inserting performance levels for 30 student assessments...';

INSERT INTO edfi.StudentAssessmentPerformanceLevel
    (AssessmentIdentifier, Namespace, StudentAssessmentIdentifier, StudentUSI, AssessmentReportingMethodDescriptorId, PerformanceLevelDescriptorId)
VALUES
    -- ASQ:SE-2: 6 of 10 have performance levels
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-001',  1, 754, 1291),  -- Proficient
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-002',  2, 754, 1285),  -- Advanced
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-003',  3, 754, 1291),  -- Proficient
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-004',  4, 754, 1282),  -- Basic
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-005',  5, 754, 1283),  -- Below Basic
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-006',  6, 754, 1284),  -- Well Below Basic

    -- DECA: 6 of 10 have performance levels
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-011', 11, 754, 1285),  -- Advanced
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-012', 12, 754, 1291),  -- Proficient
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-013', 13, 754, 1291),  -- Proficient
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-014', 14, 754, 1282),  -- Basic
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-015', 15, 754, 1283),  -- Below Basic
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-016', 16, 754, 1285),  -- Advanced

    -- SAEBRS: 6 of 10 have performance levels
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-021', 21, 754, 1291),  -- Proficient
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-022', 22, 754, 1282),  -- Basic
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-023', 23, 754, 1285),  -- Advanced
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-024', 24, 754, 1291),  -- Proficient
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-025', 25, 754, 1283),  -- Below Basic
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-026', 26, 754, 1284),  -- Well Below Basic

    -- SSIS: 6 of 10 have performance levels
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-031', 31, 754, 1285),  -- Advanced
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-032', 32, 754, 1291),  -- Proficient
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-033', 33, 754, 1282),  -- Basic
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-034', 34, 754, 1291),  -- Proficient
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-035', 35, 754, 1283),  -- Below Basic
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-036', 36, 754, 1285),  -- Advanced

    -- CORE: 6 of 10 have performance levels
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-041', 41, 754, 1291),  -- Proficient
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-042', 42, 754, 1285),  -- Advanced
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-043', 43, 754, 1282),  -- Basic
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-044', 44, 754, 1291),  -- Proficient
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-045', 45, 754, 1283),  -- Below Basic
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-046', 46, 754, 1284);  -- Well Below Basic
GO

-- ---------------------------------------------------------------------------
-- 4. Score Results (~70% of records = 35 rows)
--    Numeric scale scores varying by performance level
-- ---------------------------------------------------------------------------
PRINT 'Inserting score results for 35 student assessments...';

INSERT INTO edfi.StudentAssessmentScoreResult
    (AssessmentIdentifier, Namespace, StudentAssessmentIdentifier, StudentUSI, AssessmentReportingMethodDescriptorId, Result, ResultDatatypeTypeDescriptorId)
VALUES
    -- ASQ:SE-2: 7 of 10 have scores
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-001',  1, 754, '92',  2062),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-002',  2, 754, '97',  2062),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-003',  3, 754, '85',  2062),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-004',  4, 754, '68',  2062),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-005',  5, 754, '45',  2062),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-006',  6, 754, '28',  2062),
    ('SEL-ASQ-SE2', 'uri://ed-fi.org', 'SEL-SA-007',  7, 754, '73',  2062),

    -- DECA: 7 of 10 have scores
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-011', 11, 754, '95',  2062),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-012', 12, 754, '82',  2062),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-013', 13, 754, '88',  2062),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-014', 14, 754, '62',  2062),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-015', 15, 754, '41',  2062),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-016', 16, 754, '96',  2062),
    ('SEL-DECA', 'uri://ed-fi.org', 'SEL-SA-017', 17, 754, '77',  2062),

    -- SAEBRS: 7 of 10 have scores
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-021', 21, 754, '84',  2062),
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-022', 22, 754, '65',  2062),
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-023', 23, 754, '93',  2062),
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-024', 24, 754, '87',  2062),
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-025', 25, 754, '42',  2062),
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-026', 26, 754, '31',  2062),
    ('SEL-SAEBRS', 'uri://ed-fi.org', 'SEL-SA-027', 27, 754, '76',  2062),

    -- SSIS: 7 of 10 have scores
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-031', 31, 754, '98',  2062),
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-032', 32, 754, '86',  2062),
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-033', 33, 754, '69',  2062),
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-034', 34, 754, '83',  2062),
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-035', 35, 754, '47',  2062),
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-036', 36, 754, '91',  2062),
    ('SEL-SSIS', 'uri://ed-fi.org', 'SEL-SA-037', 37, 754, '74',  2062),

    -- CORE: 7 of 10 have scores
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-041', 41, 754, '88',  2062),
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-042', 42, 754, '94',  2062),
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-043', 43, 754, '63',  2062),
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-044', 44, 754, '81',  2062),
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-045', 45, 754, '44',  2062),
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-046', 46, 754, '33',  2062),
    ('SEL-CORE', 'uri://ed-fi.org', 'SEL-SA-047', 47, 754, '79',  2062);
GO

-- ---------------------------------------------------------------------------
-- 5. Verify inserted data
-- ---------------------------------------------------------------------------
PRINT '';
PRINT '=== SEL Assessment Data Summary ===';

SELECT 'Assessments' AS Entity, COUNT(*) AS [Count]
FROM edfi.Assessment
WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org'
UNION ALL
SELECT 'StudentAssessments', COUNT(*)
FROM edfi.StudentAssessment
WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org'
UNION ALL
SELECT 'PerformanceLevels', COUNT(*)
FROM edfi.StudentAssessmentPerformanceLevel
WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org'
UNION ALL
SELECT 'ScoreResults', COUNT(*)
FROM edfi.StudentAssessmentScoreResult
WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org';

PRINT '';
PRINT '--- Grade Level Distribution ---';
SELECT d.CodeValue AS GradeLevel, COUNT(*) AS [Count]
FROM edfi.StudentAssessment sa
JOIN edfi.Descriptor d ON sa.WhenAssessedGradeLevelDescriptorId = d.DescriptorId
WHERE sa.AssessmentIdentifier LIKE 'SEL-%' AND sa.Namespace = 'uri://ed-fi.org'
GROUP BY d.CodeValue
ORDER BY COUNT(*) DESC;

PRINT '';
PRINT '--- Performance Level Distribution ---';
SELECT d.CodeValue AS PerformanceLevel, COUNT(*) AS [Count]
FROM edfi.StudentAssessmentPerformanceLevel sap
JOIN edfi.Descriptor d ON sap.PerformanceLevelDescriptorId = d.DescriptorId
WHERE sap.AssessmentIdentifier LIKE 'SEL-%' AND sap.Namespace = 'uri://ed-fi.org'
GROUP BY d.CodeValue
ORDER BY COUNT(*) DESC;

PRINT '';
PRINT '--- Score Completeness ---';
SELECT
    COUNT(*) AS TotalAssessments,
    (SELECT COUNT(*) FROM edfi.StudentAssessmentScoreResult
     WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org') AS WithScores,
    CAST(
        (SELECT COUNT(*) FROM edfi.StudentAssessmentScoreResult
         WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org') * 100.0 / COUNT(*)
    AS DECIMAL(5,1)) AS [Completeness%]
FROM edfi.StudentAssessment
WHERE AssessmentIdentifier LIKE 'SEL-%' AND Namespace = 'uri://ed-fi.org';

PRINT '';
PRINT 'Done. SEL assessment data seeded successfully.';
GO
