
-- with total as (
--     select
--         count(1) as Total_count
--     from
--         RDS.FactK12StudentEnrollments f
-- ),
-- cond as (
--     select
--         count(1) as conditional_count
--     from
--         RDS.FactK12StudentEnrollments f
--         join RDS.DimGradeLevels g on f.EntryGradeLevelId = g.DimGradeLevelId
--         join RDS.DimSchoolYears y on f.SchoolYearId = y.DimSchoolYearId
--         join RDS.DimDates entry_dt on entry_dt.DimDateId = f.EnrollmentEntryDateId
--         join RDS.DimDates exit_dt on exit_dt.DimDateId = f.EnrollmentExitDateId
--         join RDS.FactK12StudentAttendanceRates a on f.K12StudentId = a.K12StudentId
--         and f.SchoolYearId = a.SchoolYearId
--         join RDS.FactK12StudentDisciplines d on f.K12StudentId = d.K12StudentId
--         and f.SchoolYearId = d.SchoolYearId
--         join RDS.DimDates discp_dt on discp_dt.DimDateId = d.DisciplinaryActionStartDateId
--         join RDS.DimDisciplineStatuses discp on discp.DimDisciplineStatusId = d.DisciplineStatusId
--         join RDS.FactK12StudentAssessments assess on f.K12StudentId = assess.K12StudentId
--         and f.SchoolYearId = assess.SchoolYearId
--         join RDS.DimAssessments da on da.DimAssessmentId = assess.AssessmentId ---join RDS.DimassessmentResults ar 
--     where
--         discp_dt.DateValue between entry_dt.DateValue
--         and exit_dt.DateValue
--         and g.GradeLevelCode in ('01', '02') --Percentage of students in grades 1 and 2
--         and StudentAttendanceRate >= 90.0 --with an attendance rate of 90 percent or higher
--         -- meeting grade-level math and reading benchmarks
--         and da.AssessmentAcademicSubjectCode in ('01166', '00560')
--         and da.AssessmentTypeCode = 'Benchmark'
--         and assess.AssessmentResultScoreValueScaleScore is not null --Assessment Administration Start Date ≥ July 1 of the school year being reported
--         --Assessment Administration Finish Date ≤ June 30 of the school year being reported
--         --Assessment Score Metric Type = 00493 (Grade equivalent or grade-level indicator) Not present in target fact table
--         and (
--             d.DisciplinaryActionStartDateId = -1
--             OR (
--                 d.DisciplinaryActionStartDateId is not null
--                 AND discp.DisciplinaryActionTakenCode in (
--                     '03086',
--                     '03087',
--                     '03099',
--                     '03100',
--                     '03154',
--                     '03155',
--                     '03102',
--                     '03101'
--                 )
--             )
--         ) --no in- or out-of-school suspensions or expulsions
-- )
-- select
--     CAST(
--         c.conditional_count * 100.0 / t.Total_count as Decimal(5, 2)
--     ) as PercentageStudents
-- from
--     Total t,
--     Cond c

select
	'Number of Students (Grade 1 and 2)' AS DataElementName,
    'RecordCount' AS CharacteristicType,
	CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS Remarks
    from
        RDS.FactK12StudentEnrollments f
        join RDS.DimGradeLevels g on f.EntryGradeLevelId = g.DimGradeLevelId
    where
         g.GradeLevelCode in ('01', '02') --students in grades 1 and 2
    union
	
    select
	'Total Students All Grades (K-12)' AS DataElementName,
    'RecordCount' AS CharacteristicType,
	CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS Remarks
    from
        RDS.FactK12StudentEnrollments f
        join RDS.DimGradeLevels g on f.EntryGradeLevelId = g.DimGradeLevelId;

---Attendance rate--
    select
	'Attendance rate of 90 percent or higher (Grades 1 and 2)' AS DataElementName,
    'RecordCount' AS CharacteristicType,
	CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS Remarks
    from
        RDS.FactK12StudentEnrollments f
        join RDS.DimGradeLevels g on f.EntryGradeLevelId = g.DimGradeLevelId
        join RDS.DimSchoolYears y on f.SchoolYearId = y.DimSchoolYearId
        join RDS.DimDates entry_dt on entry_dt.DimDateId = f.EnrollmentEntryDateId
        join RDS.DimDates exit_dt on exit_dt.DimDateId = f.EnrollmentExitDateId
        join RDS.FactK12StudentAttendanceRates a on f.K12StudentId = a.K12StudentId
        and f.SchoolYearId = a.SchoolYearId
        join RDS.FactK12StudentDisciplines d on f.K12StudentId = d.K12StudentId
        and f.SchoolYearId = d.SchoolYearId
        join RDS.DimDates discp_dt on discp_dt.DimDateId = d.DisciplinaryActionStartDateId
    where
        discp_dt.DateValue between entry_dt.DateValue
        and exit_dt.DateValue
        and g.GradeLevelCode in ('01', '02') --Percentage of students in grades 1 and 2
        and StudentAttendanceRate >= 90.0 --with an attendance rate of 90 percent or higher
    union
	
    select
	'Attendance rate of 90 percent or higher (K-12)' AS DataElementName,
    'RecordCount' AS CharacteristicType,
	CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS Remarks
    from
        RDS.FactK12StudentEnrollments f
        join RDS.DimGradeLevels g on f.EntryGradeLevelId = g.DimGradeLevelId
        join RDS.DimSchoolYears y on f.SchoolYearId = y.DimSchoolYearId
        join RDS.DimDates entry_dt on entry_dt.DimDateId = f.EnrollmentEntryDateId
        join RDS.DimDates exit_dt on exit_dt.DimDateId = f.EnrollmentExitDateId
        join RDS.FactK12StudentAttendanceRates a on f.K12StudentId = a.K12StudentId
        and f.SchoolYearId = a.SchoolYearId
        join RDS.FactK12StudentDisciplines d on f.K12StudentId = d.K12StudentId
        and f.SchoolYearId = d.SchoolYearId
        join RDS.DimDates discp_dt on discp_dt.DimDateId = d.DisciplinaryActionStartDateId
    where
        discp_dt.DateValue between entry_dt.DateValue
        and exit_dt.DateValue
        and StudentAttendanceRate >= 90.0; --with an attendance rate of 90 percent or higher
  

  
  
  --Chronic Absenteeism Rate---
    select
	'Chronic Absenteeism Rate (Grades 1 and 2)' AS DataElementName,
    'Percentage' AS CharacteristicType,
	 CAST(
    100.0 * SUM(CASE WHEN a.StudentAttendanceRate < 90 THEN 1 ELSE 0 END) / (select COUNT(1)  from rds.FactK12StudentEnrollments)
    AS NVARCHAR(MAX)
  ) AS Value,
    NULL AS Remarks
    from
        RDS.FactK12StudentEnrollments f
        join RDS.DimGradeLevels g on f.EntryGradeLevelId = g.DimGradeLevelId
        join RDS.DimSchoolYears y on f.SchoolYearId = y.DimSchoolYearId
        join RDS.DimDates entry_dt on entry_dt.DimDateId = f.EnrollmentEntryDateId
        join RDS.DimDates exit_dt on exit_dt.DimDateId = f.EnrollmentExitDateId
        join RDS.FactK12StudentAttendanceRates a on f.K12StudentId = a.K12StudentId
        and f.SchoolYearId = a.SchoolYearId
        join RDS.FactK12StudentDisciplines d on f.K12StudentId = d.K12StudentId
        and f.SchoolYearId = d.SchoolYearId
        join RDS.DimDates discp_dt on discp_dt.DimDateId = d.DisciplinaryActionStartDateId
    where
        discp_dt.DateValue between entry_dt.DateValue
        and exit_dt.DateValue
        and g.GradeLevelCode in ('01', '02') --Percentage of students in grades 1 and 2
        and StudentAttendanceRate < 90.0 --with an attendance rate of 90 percent or higher
  union
  SELECT
  'Chronic Absenteeism Rate (K-12)' AS DataElementName,
  'Percentage' AS CharacteristicType,
  CAST(
    100.0 * SUM(CASE WHEN a.StudentAttendanceRate < 90 THEN 1 ELSE 0 END) / (select COUNT(1)  from rds.FactK12StudentEnrollments)
    AS NVARCHAR(MAX)
  ) AS Value,
  NULL AS Remarks
FROM RDS.FactK12StudentEnrollments f
        join RDS.DimGradeLevels g on f.EntryGradeLevelId = g.DimGradeLevelId
        join RDS.DimSchoolYears y on f.SchoolYearId = y.DimSchoolYearId
        join RDS.DimDates entry_dt on entry_dt.DimDateId = f.EnrollmentEntryDateId
        join RDS.DimDates exit_dt on exit_dt.DimDateId = f.EnrollmentExitDateId
        join RDS.FactK12StudentAttendanceRates a on f.K12StudentId = a.K12StudentId
        and f.SchoolYearId = a.SchoolYearId
		and StudentAttendanceRate < 90.0;

		
--Students meet Assessment Benchmark--
			select  'Students meet Assessment Benchmark (Grades 1 and 2)' AS DataElementName,
		'RecordCount' AS CharacteristicType,  
		count(1) AS Value,
		NULL AS Remarks from rds.FactK12StudentAssessments ase  
		join  RDS.FactK12StudentEnrollments f on f.K12StudentId = ase.K12StudentId
        join RDS.DimAssessments da on da.DimAssessmentId = ase.AssessmentId
		join RDS.DimGradeLevels g on f.EntryGradeLevelId = g.DimGradeLevelId
		where  ase.AssessmentResultScoreValueScaleScore is not null
			and da.AssessmentTypeCode = 'Benchmark' 
			and da.AssessmentAcademicSubjectCode in ('01166', '00560')
			and g.GradeLevelCode in ('01', '02')
union
		select  
        'Students meet Assessment Benchmark (K-12)' AS DataElementName,
		'RecordCount' AS CharacteristicType,  
		count(1) AS Value,
		NULL AS Remarks
		from rds.FactK12StudentAssessments ase  
		join  RDS.FactK12StudentEnrollments f on f.K12StudentId = ase.K12StudentId
        join RDS.DimAssessments da on da.DimAssessmentId = ase.AssessmentId
		join RDS.DimGradeLevels g on f.EntryGradeLevelId = g.DimGradeLevelId
		where  ase.AssessmentResultScoreValueScaleScore is not null
			and da.AssessmentTypeCode = 'Benchmark' 
			and da.AssessmentAcademicSubjectCode in ('01166', '00560')

--Suspensions and Expulsions---
SELECT
    'Suspensions and Expulsions (Grades 1 and 2)' AS DataElementName,
    'RecordCount' AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS Remarks
FROM RDS.FactK12StudentDisciplines fact
JOIN RDS.DimDisciplineStatuses discStat ON fact.DisciplineStatusId = discStat.DimDisciplineStatusId
JOIN RDS.DimGradeLevels gradeLevel ON fact.GradeLevelId = gradeLevel.DimGradeLevelId
WHERE discStat.DisciplinaryActionTakenCode IN 
	('03086' -- Expulsion with services
	,'03087' -- Expulsion without services
	,'03099' -- Suspension after school
	,'03100' -- Suspension, in-school
	,'03154' -- Suspension, out-of-school, greater than 10 consecutive days
	,'03155' -- Suspension, out-of-school, separate days culminating to more than 10 school days
	,'03101' -- Suspension, out-of-school, with services
	,'03102' -- Suspension, out-of-school, without services
	)
AND gradeLevel.GradeLevelCode IN ('01', '02')
union
SELECT
    'Suspensions and Expulsions (K-12)' AS DataElementName,
    'RecordCount' AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS Remarks
FROM RDS.FactK12StudentDisciplines fact
JOIN RDS.DimDisciplineStatuses discStat ON fact.DisciplineStatusId = discStat.DimDisciplineStatusId
WHERE discStat.DisciplinaryActionTakenCode IN 
	('03086' -- Expulsion with services
	,'03087' -- Expulsion without services
	,'03099' -- Suspension after school
	,'03100' -- Suspension, in-school
	,'03154' -- Suspension, out-of-school, greater than 10 consecutive days
	,'03155' -- Suspension, out-of-school, separate days culminating to more than 10 school days
	,'03101' -- Suspension, out-of-school, with services
	,'03102' -- Suspension, out-of-school, without services
	)

