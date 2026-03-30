-- =============================================================================
-- Seed SEL Survey data for: Teacher reports of social-emotional development
-- =============================================================================
-- Target: EdFi_Ods_GrandBend (localhost,14333)
-- Usage:  sqlcmd -S localhost,14333 -U sa -P 'P@ssw0rd123' -d EdFi_Ods_GrandBend -i seed-sel-survey-data.sql
--
-- Seeds survey definitions and survey responses into the Ed-Fi Survey API tables
-- so that TeacherReportsSocialEmotionalEdFiAssessor returns valid record counts.
--
-- The assessor matches surveys whose surveyCategoryDescriptor CodeValue contains
-- any of: teacher, sel, social, emotional, casel, deca, saebrs, panorama, secondstep.
--
-- This script creates:
--   - 3 custom SurveyCategoryDescriptor values (SEL-specific)
--   - 5 survey definitions across different SEL frameworks
--   - 75 survey responses linked to existing students and staff
--
-- Idempotent: safe to re-run (deletes previous SEL survey data first).
-- =============================================================================

USE EdFi_Ods_GrandBend;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- ---------------------------------------------------------------------------
-- Clean up previous runs
-- ---------------------------------------------------------------------------
PRINT 'Cleaning up previous SEL survey data...';

DELETE FROM edfi.SurveyResponse
WHERE Namespace = 'uri://ewftool.org/Survey' AND SurveyIdentifier LIKE 'SEL-SURV-%';

DELETE FROM edfi.Survey
WHERE Namespace = 'uri://ewftool.org/Survey' AND SurveyIdentifier LIKE 'SEL-SURV-%';

-- Clean up custom descriptors (leave standard ones intact)
DELETE FROM edfi.SurveyCategoryDescriptor
WHERE SurveyCategoryDescriptorId IN (
    SELECT DescriptorId FROM edfi.Descriptor
    WHERE Namespace = 'uri://ewftool.org/SurveyCategoryDescriptor'
);

DELETE FROM edfi.Descriptor
WHERE Namespace = 'uri://ewftool.org/SurveyCategoryDescriptor';
GO

-- ---------------------------------------------------------------------------
-- Existing descriptor reference (verified from GrandBend):
--   Teacher = 2345 (uri://ed-fi.org/SurveyCategoryDescriptor#Teacher)
--
-- We also create 3 custom SEL-specific descriptors to exercise the assessor's
-- keyword matching against common district-level SEL descriptor patterns.
-- ---------------------------------------------------------------------------
PRINT 'Creating custom SEL survey category descriptors...';

-- Insert custom descriptors into the base Descriptor table (DescriptorId is IDENTITY)
INSERT INTO edfi.Descriptor (Namespace, CodeValue, ShortDescription, Description)
VALUES
    ('uri://ewftool.org/SurveyCategoryDescriptor', 'SocialEmotionalLearning',
     'Social-Emotional Learning', 'Surveys measuring social-emotional learning competencies'),
    ('uri://ewftool.org/SurveyCategoryDescriptor', 'SocialEmotionalDevelopment',
     'Social-Emotional Development', 'Early childhood social-emotional development surveys'),
    ('uri://ewftool.org/SurveyCategoryDescriptor', 'SchoolClimate',
     'School Climate', 'School climate surveys with embedded SEL components');

-- Register them as SurveyCategoryDescriptor subtypes
INSERT INTO edfi.SurveyCategoryDescriptor (SurveyCategoryDescriptorId)
SELECT DescriptorId FROM edfi.Descriptor
WHERE Namespace = 'uri://ewftool.org/SurveyCategoryDescriptor';
GO

-- ---------------------------------------------------------------------------
-- Capture descriptor IDs into variables for use in survey inserts
-- ---------------------------------------------------------------------------
DECLARE @TeacherDescId INT = 2345;  -- Standard Ed-Fi "Teacher" category

DECLARE @SelDescId INT;
SELECT @SelDescId = DescriptorId FROM edfi.Descriptor
WHERE Namespace = 'uri://ewftool.org/SurveyCategoryDescriptor' AND CodeValue = 'SocialEmotionalLearning';

DECLARE @SedDescId INT;
SELECT @SedDescId = DescriptorId FROM edfi.Descriptor
WHERE Namespace = 'uri://ewftool.org/SurveyCategoryDescriptor' AND CodeValue = 'SocialEmotionalDevelopment';

DECLARE @ClimateDescId INT;
SELECT @ClimateDescId = DescriptorId FROM edfi.Descriptor
WHERE Namespace = 'uri://ewftool.org/SurveyCategoryDescriptor' AND CodeValue = 'SchoolClimate';

PRINT 'Descriptor IDs:';
PRINT '  Teacher (standard):              ' + CAST(@TeacherDescId AS VARCHAR);
PRINT '  SocialEmotionalLearning:         ' + CAST(@SelDescId AS VARCHAR);
PRINT '  SocialEmotionalDevelopment:       ' + CAST(@SedDescId AS VARCHAR);
PRINT '  SchoolClimate:                   ' + CAST(@ClimateDescId AS VARCHAR);

-- ---------------------------------------------------------------------------
-- 1. Survey definitions — 5 surveys representing common SEL frameworks
-- ---------------------------------------------------------------------------
PRINT '';
PRINT 'Inserting SEL survey definitions...';

INSERT INTO edfi.Survey
    (Namespace, SurveyIdentifier, SurveyTitle, SchoolYear, SurveyCategoryDescriptorId, NumberAdministered)
VALUES
    -- CASEL-aligned teacher SEL rating (matches "Teacher" keyword)
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL',
     'CASEL Teacher SEL Rating Form', 2025, @TeacherDescId, 120),

    -- DECA early childhood assessment (matches "SocialEmotionalDevelopment" keyword)
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA',
     'DECA-P2 Preschool Social-Emotional Assessment', 2025, @SedDescId, 80),

    -- SAEBRS behavior screener (matches "SocialEmotionalLearning" keyword)
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS',
     'SAEBRS Teacher Rating - Social-Emotional Behavior', 2025, @SelDescId, 200),

    -- Panorama SEL survey (matches "SocialEmotionalLearning" keyword)
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA',
     'Panorama Social-Emotional Learning Survey', 2025, @SelDescId, 350),

    -- EDSCLS school climate survey (matches "SchoolClimate" keyword)
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS',
     'EDSCLS School Climate and SEL Survey', 2025, @ClimateDescId, 150);

-- ---------------------------------------------------------------------------
-- 2. Survey responses — 75 total, distributed across the 5 surveys
--    Mix of: student-linked (teacher reporting on student) and staff responses
-- ---------------------------------------------------------------------------
PRINT 'Inserting 75 survey responses...';

-- CASEL Teacher SEL Rating: 15 responses (teacher rates individual students)
-- StudentUSI 1-15, StaffUSI 1 (the teacher completing the form)
INSERT INTO edfi.SurveyResponse
    (Namespace, SurveyIdentifier, SurveyResponseIdentifier, ResponseDate, StudentUSI, StaffUSI)
VALUES
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-001', '2025-10-01', 1, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-002', '2025-10-01', 2, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-003', '2025-10-01', 3, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-004', '2025-10-02', 4, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-005', '2025-10-02', 5, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-006', '2025-10-02', 6, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-007', '2025-10-03', 7, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-008', '2025-10-03', 8, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-009', '2025-10-03', 9, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-010', '2025-10-04', 10, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-011', '2025-10-04', 11, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-012', '2025-10-04', 12, 1),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-013', '2025-10-05', 13, 2),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-014', '2025-10-05', 14, 2),
    ('uri://ewftool.org/Survey', 'SEL-SURV-CASEL', 'SEL-SR-015', '2025-10-05', 15, 2);

-- DECA Preschool: 15 responses (early childhood teacher reports)
-- StudentUSI 16-30, StaffUSI 3
INSERT INTO edfi.SurveyResponse
    (Namespace, SurveyIdentifier, SurveyResponseIdentifier, ResponseDate, StudentUSI, StaffUSI)
VALUES
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-016', '2025-09-15', 16, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-017', '2025-09-15', 17, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-018', '2025-09-15', 18, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-019', '2025-09-16', 19, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-020', '2025-09-16', 20, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-021', '2025-09-16', 21, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-022', '2025-09-17', 22, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-023', '2025-09-17', 23, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-024', '2025-09-17', 24, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-025', '2025-09-18', 25, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-026', '2025-09-18', 26, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-027', '2025-09-18', 27, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-028', '2025-09-19', 28, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-029', '2025-09-19', 29, 3),
    ('uri://ewftool.org/Survey', 'SEL-SURV-DECA', 'SEL-SR-030', '2025-09-19', 30, 3);

-- SAEBRS Behavior Screener: 20 responses (K-12 teacher-completed)
-- StudentUSI 31-50, StaffUSI 4 and 5
INSERT INTO edfi.SurveyResponse
    (Namespace, SurveyIdentifier, SurveyResponseIdentifier, ResponseDate, StudentUSI, StaffUSI)
VALUES
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-031', '2025-10-10', 31, 4),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-032', '2025-10-10', 32, 4),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-033', '2025-10-10', 33, 4),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-034', '2025-10-11', 34, 4),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-035', '2025-10-11', 35, 4),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-036', '2025-10-11', 36, 4),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-037', '2025-10-12', 37, 4),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-038', '2025-10-12', 38, 4),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-039', '2025-10-12', 39, 4),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-040', '2025-10-13', 40, 4),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-041', '2025-10-13', 41, 5),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-042', '2025-10-13', 42, 5),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-043', '2025-10-14', 43, 5),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-044', '2025-10-14', 44, 5),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-045', '2025-10-14', 45, 5),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-046', '2025-10-15', 46, 5),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-047', '2025-10-15', 47, 5),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-048', '2025-10-15', 48, 5),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-049', '2025-10-16', 49, 5),
    ('uri://ewftool.org/Survey', 'SEL-SURV-SAEBRS', 'SEL-SR-050', '2025-10-16', 50, 5);

-- Panorama SEL Survey: 15 responses (student self-report, linked to students)
-- StudentUSI 51-65, no StaffUSI (student self-report)
INSERT INTO edfi.SurveyResponse
    (Namespace, SurveyIdentifier, SurveyResponseIdentifier, ResponseDate, StudentUSI)
VALUES
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-051', '2025-11-01', 51),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-052', '2025-11-01', 52),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-053', '2025-11-01', 53),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-054', '2025-11-02', 54),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-055', '2025-11-02', 55),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-056', '2025-11-02', 56),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-057', '2025-11-03', 57),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-058', '2025-11-03', 58),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-059', '2025-11-03', 59),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-060', '2025-11-04', 60),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-061', '2025-11-04', 61),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-062', '2025-11-04', 62),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-063', '2025-11-05', 63),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-064', '2025-11-05', 64),
    ('uri://ewftool.org/Survey', 'SEL-SURV-PANORAMA', 'SEL-SR-065', '2025-11-05', 65);

-- EDSCLS School Climate: 10 responses (mixed — some with student ref, some without)
-- 7 with StudentUSI, 3 anonymous (no student or staff link)
INSERT INTO edfi.SurveyResponse
    (Namespace, SurveyIdentifier, SurveyResponseIdentifier, ResponseDate, StudentUSI)
VALUES
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS', 'SEL-SR-066', '2025-11-10', 66),
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS', 'SEL-SR-067', '2025-11-10', 67),
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS', 'SEL-SR-068', '2025-11-10', 68),
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS', 'SEL-SR-069', '2025-11-11', 69),
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS', 'SEL-SR-070', '2025-11-11', 70),
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS', 'SEL-SR-071', '2025-11-11', 71),
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS', 'SEL-SR-072', '2025-11-12', 72);

INSERT INTO edfi.SurveyResponse
    (Namespace, SurveyIdentifier, SurveyResponseIdentifier, ResponseDate, FullName)
VALUES
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS', 'SEL-SR-073', '2025-11-12', 'Anonymous Respondent 1'),
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS', 'SEL-SR-074', '2025-11-13', 'Anonymous Respondent 2'),
    ('uri://ewftool.org/Survey', 'SEL-SURV-EDSCLS', 'SEL-SR-075', '2025-11-13', 'Anonymous Respondent 3');
GO

-- ---------------------------------------------------------------------------
-- 3. Verify inserted data
-- ---------------------------------------------------------------------------
PRINT '';
PRINT '=== SEL Survey Data Summary ===';

SELECT 'Custom Descriptors' AS Entity, COUNT(*) AS [Count]
FROM edfi.Descriptor WHERE Namespace = 'uri://ewftool.org/SurveyCategoryDescriptor'
UNION ALL
SELECT 'Surveys', COUNT(*)
FROM edfi.Survey
WHERE Namespace = 'uri://ewftool.org/Survey' AND SurveyIdentifier LIKE 'SEL-SURV-%'
UNION ALL
SELECT 'Survey Responses (total)', COUNT(*)
FROM edfi.SurveyResponse
WHERE Namespace = 'uri://ewftool.org/Survey' AND SurveyIdentifier LIKE 'SEL-SURV-%'
UNION ALL
SELECT 'Responses with Student Ref', COUNT(*)
FROM edfi.SurveyResponse
WHERE Namespace = 'uri://ewftool.org/Survey' AND SurveyIdentifier LIKE 'SEL-SURV-%'
  AND StudentUSI IS NOT NULL;

PRINT '';
PRINT '--- Responses per Survey ---';
SELECT s.SurveyTitle, d.CodeValue AS Category, COUNT(sr.SurveyResponseIdentifier) AS Responses
FROM edfi.Survey s
JOIN edfi.SurveyResponse sr
    ON s.Namespace = sr.Namespace AND s.SurveyIdentifier = sr.SurveyIdentifier
LEFT JOIN edfi.Descriptor d ON s.SurveyCategoryDescriptorId = d.DescriptorId
WHERE s.Namespace = 'uri://ewftool.org/Survey' AND s.SurveyIdentifier LIKE 'SEL-SURV-%'
GROUP BY s.SurveyTitle, d.CodeValue
ORDER BY Responses DESC;

PRINT '';
PRINT '--- Expected Assessor Output ---';
PRINT 'RecordCount:                75 (total responses across all matched surveys)';
PRINT 'Student Reference complete: 72 of 75 (96.0%)';
PRINT 'Survey Category dist:       Teacher=15, SocialEmotionalLearning=35, SocialEmotionalDevelopment=15, SchoolClimate=10';
PRINT '';
PRINT 'Done. SEL survey data seeded successfully.';
GO
