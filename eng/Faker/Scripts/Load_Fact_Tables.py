from datetime import datetime, timedelta
from faker import Faker
import random
import pandas as pd
import urllib
from sqlalchemy import create_engine, text
from sqlalchemy.exc import SQLAlchemyError
from Scripts.Load_Base_Dimension_Tables import *
from Config.config import *

def main():
    engine = None 
    try :
        base_load = load_base_tables(num_seas=1,num_leas=5,num_schools=20, num_people=100) 
        
        if base_load == True:
            print("Base Table Data successfully loaded into SQL Server!")

            SEED = 2
            random.seed(SEED)
            fake = Faker()
            fake.seed_instance(SEED)
            fact_enrollments = []
            fact_assessments = []
            fact_attendance = []
            fact_discipline = []
            num_facts = 1000

            # Create engine
            engine = create_sql_alchemy_engine() 

            # Delete existing rows 
            with engine.begin() as conn:  # auto-commit transaction
                    dim_school_years = pd.read_sql_query("Select DimSchoolYearId, SchoolYear from RDS.DimSchoolYears;", conn) 
                    dim_dates = pd.read_sql_query("Select DimDateId, DateValue from RDS.DimDates;", conn) 
                    dim_grades = pd.read_sql_query("Select DimGradeLevelId, GradeLevelCode from RDS.DimGradeLevels;", conn)
                    dim_people = pd.read_sql_query("Select * from RDS.DimPeople;", conn)
                    dim_seas = pd.read_sql_query("Select * from RDS.DimSeas;", conn)
                    dim_leas = pd.read_sql_query("Select * from RDS.DimLeas;", conn)
                    dim_schools = pd.read_sql_query("Select * from RDS.DimK12Schools;", conn)
                    dim_assessments = pd.read_sql_query("Select * from RDS.DimAssessments;",conn)



            # Map surrogate keys
            # SchoolYear surrogate
            school_year_map = dict(zip(dim_school_years["SchoolYear"], dim_school_years["DimSchoolYearId"]))
            dim_people["SchoolYear"] = dim_people["RecordStartDateTime"].apply(lambda x: x.year)
            dim_people["SchoolYearEnd"] = dim_people["RecordEndDateTime"].apply(lambda x: x.year if pd.notnull(x) else datetime.now().year )

            #dim_people["SchoolYearId"] = dim_people["RecordStartDateTime"].apply(lambda x: x.year).map(school_year_map)

            # Enrollment entry/exit dates surrogate
            date_map = dict(zip(dim_dates["DateValue"], dim_dates["DimDateId"]))

            # GradeLevels code surrogate
            grade_map = dict(zip(
            dim_grades["GradeLevelCode"].apply(lambda x: int(x) if x.isdigit() else x),
            dim_grades["DimGradeLevelId"]
            ))


            grades = list(range(1, 13))  # 1-12

            for index, student in dim_people.iterrows():
                school_years = list(range(student['SchoolYear'],  student['SchoolYear'] + 1 if student['SchoolYearEnd']==student['SchoolYear'] else student['SchoolYearEnd']+1)) #  (student['SchoolYearEnd'] or student['SchoolYear'] + 1)))

                #num_enrollments = random.randint(1, min(len(grades), len(school_years)))
                num_enrollments =  len(school_years)

                # Pick non-repeating grades
                selected_grades = random.sample(grades, num_enrollments)
                selected_grades.sort()

                # Pick school years for enrollments
                selected_years = random.sample(school_years, num_enrollments)
                selected_years.sort()
                K12DemographicId = random.randint(1,3)

                for i, (entry_grade, school_year) in enumerate(zip(selected_grades, selected_years)):
                    school = dim_schools.sample(1).iloc[0]
                    lea = dim_leas.loc[dim_leas["LeaIdentifierSea"] == school["LeaIdentifierSea"]].iloc[0]
                    sea = dim_seas.loc[dim_seas["SeaOrganizationIdentifierSea"] == school["SeaOrganizationIdentifierSea"]].iloc[0]

                    start_dt = student["RecordStartDateTime"]
                    end_dt = student["RecordEndDateTime"] 
                    if pd.isna(end_dt):
                        end_dt = pd.Timestamp("today")
                    else:
                        end_dt = pd.Timestamp(end_dt)
                    entry_date = pd.Timestamp(fake.date_between(start_date=start_dt, end_date=end_dt))
                    exit_date = entry_date + pd.Timedelta(days=random.randint(30, 365*2)) if random.random() > 0.5 else None
                    if exit_date and exit_date > end_dt:
                        exit_date = end_dt

                    # Exit grade logic
                    if i + 1 < len(selected_grades):
                        exit_grade = selected_grades[i + 1]
                    else:
                        exit_grade = entry_grade

                    # Status helper
                    def status_dates():
                        start = fake.date_between(start_date=start_dt, end_date=end_dt)
                        end = fake.date_between(start_date=start_dt, end_date=end_dt)
                        return (start, end) if start <= end else (end, start)

                    econ_start, econ_end = status_dates()
                    el_start, el_end = status_dates()
                    hom_start, hom_end = status_dates()
                                
                    fact_enrollments.append({
                            "SchoolYearId": school_year_map[school_year],#student["SchoolYearId"]
                            "CountDateId": -1,
                            "DataCollectionId": -1,
                            "SeaId": sea["DimSeaId"],
                            "IeuId": -1,
                            "K12StudentId": student["DimPersonId"],
                            "LeaAccountabilityId": lea["DimLeaID"],
                            "LeaAttendanceId": lea["DimLeaID"],
                            "LeaFundingId": lea["DimLeaID"],
                            "LeaGraduationID": lea["DimLeaID"],
                            "LeaIndividualizedEducationProgramId": lea["DimLeaID"],
                            "K12SchoolId": school["DimK12SchoolId"],
                            "EducationOrganizationNetworkId": -1,
                            "CohortGraduationYearId": -1,
                            "CohortYearId": -1,
                            "CteStatusId": -1,
                            "EntryGradeLevelId": grade_map.get(entry_grade),
                            "ExitGradeLevelId": grade_map.get(min(entry_grade + 1, 12)  ),
                            "EnrollmentEntryDateId": -1 if date_map.get(entry_date) is None else date_map.get(entry_date),
                            "EnrollmentExitDateId": -1 if date_map.get(exit_date) is None else date_map.get(exit_date),
                            "EnglishLearnerStatusId": 1 if el_start else 0,
                            "K12EnrollmentStatusId": random.randint(1,1632960),
                            "K12DemographicId": K12DemographicId,
                            "IdeaStatusId": -1,
                            "HomelessnessStatusId": 1 if hom_start else 0,
                            "EconomicallyDisadvantagedStatusId": 1 if econ_start else 0,
                            "FosterCareStatusId": random.choice([-1,1,2]),
                            "ImmigrantStatusId": random.choice([-1,1,2]),
                            "LanguageHomeId": random.choice([-1,1,2]),
                            "LanguageNativeId": random.choice([-1,1,2]),
                            "MigrantStatusId": random.choice([-1,1,2]),
                            "MilitaryStatusId": random.choice([-1,1,2]),
                            "NOrDStatusId": random.choice([-1,1,2]),
                            "PrimaryDisabilityTypeId": random.randint(1,14),
                            "SecondaryDisabilityTypeId": random.randint(1,14),
                            "ProjectedGraduationDateId": random.randint(2023,2030),
                            "StatusStartDateEconomicallyDisadvantagedId": -1 if date_map.get(econ_start) is None else date_map.get(econ_start) ,
                            "StatusEndDateEconomicallyDisadvantagedId": -1 if date_map.get(econ_end) is None else date_map.get(econ_end),
                            "StatusStartDateEnglishLearnerId": -1 if date_map.get(el_start) is None else date_map.get(el_start),
                            "StatusEndDateEnglishLearnerId": -1 if  date_map.get(el_end) is None else date_map.get(el_end),
                            "StatusStartDateHomelessnessId":-1 if date_map.get(hom_start) is None else date_map.get(hom_start),
                            "StatusEndDateHomelessnessId":-1 if date_map.get(hom_end) is None else date_map.get(hom_end),
                            "StatusStartDateIdeaId": -1,
                            "StatusEndDateIdeaId": -1,
                            "StatusStartDateMigrantId": -1,
                            "StatusEndDateMigrantId": -1,
                            "StatusStartDateMilitaryConnectedStudentId": -1,
                            "StatusEndDateMilitaryConnectedStudentId": -1,
                            "StatusStartDatePerkinsEnglishLearnerId": -1,
                            "StatusEndDatePerkinsEnglishLearnerId": -1,
                            "StatusStartDateTitleIIIImmigrantId": -1,
                            "StatusEndDateTitleIIIImmigrantId": -1,
                            "TitleIIIStatusId": random.choice([-1,1]),
                            "FullTimeEquivalency": round(random.uniform(0.5,1.0),2),
                            "StudentCount": 1,
                            "ResponsibleSchoolTypeId": random.choice([1,2,3]),
                            "LeaMembershipResidentId": lea["DimLeaID"]
                        })
                    
                    assessment = dim_assessments.sample(1).iloc[0]

                    # Scores - some tests won’t use all metrics, so leave many as None
                    raw_score = random.randint(20, 100)
                    scale_score = raw_score * 10 + random.randint(-15, 15)
                    percentile = random.randint(1, 99)
                    t_score = random.randint(20, 80)
                    z_score = round(random.uniform(-3, 3), 2)

                    fact_assessments.append({
                        "SchoolYearId": school_year_map[school_year],
                        "CountDateId": -1,
                        "FactTypeId": -1,
                        "SeaId": sea["DimSeaId"],
                        "IeuId": -1,
                        "LeaId": lea["DimLeaID"],
                        "K12SchoolId": school["DimK12SchoolId"],
                        "K12StudentId": student["DimPersonId"],
                        "AssessmentId": assessment["DimAssessmentId"],
                        "AssessmentSubtestId": -1,
                        "AssessmentAdministrationId": -1,
                        "AssessmentRegistrationId": -1,
                        "AssessmentParticipationSessionId": -1,
                        "AssessmentPerformanceLevelId": -1,
                        "CompetencyDefinitionId": -1,
                        "CteStatusId": -1,
                        "GradeLevelWhenAssessedId": grade_map.get(entry_grade),
                        "IdeaStatusId": -1,
                        "K12DemographicId": K12DemographicId,
                        "NOrDStatusId": random.choice([-1,1,2]),
                        "TitleIIIStatusId": random.choice([-1,1]),
                        "AssessmentCount": 1,
                        "AssessmentResultScoreValueRawScore": raw_score,
                        "AssessmentResultScoreValueScaleScore": scale_score,
                        "AssessmentResultScoreValuePercentile": percentile,
                        "AssessmentResultScoreValueTScore": t_score,
                        "AssessmentResultScoreValueZScore": z_score,
                        "AssessmentResultScoreValueACTScore": random.choice([None, random.randint(15, 36)]),
                        "AssessmentResultScoreValueSATScore": random.choice([None, random.randint(400, 1600)]),
                    })


                    # Random dates for incident
                    discipline_start_date = pd.Timestamp(fake.date_between(start_date=start_dt, end_date=  end_dt))
                    duration_days = random.randint(1, 10)
                    discipline_end_date = entry_date + pd.Timedelta(days=random.randint(30, 365*2)) if random.random() > 0.5 else None
                    IncidentStatusId = random.choice([1, 2,3,-1]) 

                    fact_discipline.append({
                        "SchoolYearId": school_year_map[school_year],
                        "FactTypeId": -1,
                        "DataCollectionId": -1,
                        "SeaId": sea['DimSeaId'],
                        "IeuId": -1,
                        "LeaId": lea["DimLeaID"],
                        "K12SchoolId":school["DimK12SchoolId"],
                        "K12StudentId": student["DimPersonId"],
                        "AgeId": random.randint(6, 18),
                        "CteStatusId": -1,
                        "DisabilityStatusId": -1,
                        "DisciplinaryActionStartDateId": -1 if date_map.get(discipline_start_date) is None else date_map.get(discipline_start_date),
                        "DisciplinaryActionEndDateId": -1 if date_map.get(discipline_end_date) is None else date_map.get(discipline_end_date),
                        "DisciplineStatusId": random.randint(1, 5075),
                        "EconomicallyDisadvantagedStatusId": random.randint(1, 44),
                        "EnglishLearnerStatusId": random.randint(1, 8),
                        "FirearmId": random.randint(1, 4),
                        "FirearmDisciplineStatusId": -1,
                        "FosterCareStatusId": random.choice([-1,1,2]),
                        "GradeLevelId": grade_map.get(entry_grade),
                        "HomelessnessStatusId": 1 if hom_start else 0,
                        "IdeaStatusId": -1,
                        "ImmigrantStatusId": random.choice([-1,1,2]),
                        "IncidentIdentifier": f"INC-{10000 + i}",
                        "IncidentStatusId": IncidentStatusId,
                        "IncidentDateId": date_map.get((discipline_start_date - pd.Timedelta(days=1))) if IncidentStatusId!=-1 else -1 ,
                        "K12DemographicId": K12DemographicId,
                        "MigrantStatusId": -1,
                        "MilitaryStatusId": -1,
                        "NOrDStatusId": -1,
                        "RaceId": random.randint(1, 8),
                        "PrimaryDisabilityTypeId": random.randint(1,14),
                        "SecondaryDisabilityTypeId": random.randint(1,14),
                        "TitleIStatusId": -1,
                        "TitleIIIStatusId": random.choice([-1,1]),
                        "DurationOfDisciplinaryAction": duration_days,
                        "DisciplineCount": 1
                    })

                    fact_attendance.append({
                        "SchoolYearId":school_year_map[school_year],
                        "FactTypeId":-1,
                        "SeaId":sea["DimSeaId"],
                        "LeaId":lea["DimLeaID"],
                        "K12SchoolId":school["DimK12SchoolId"],
                        "K12StudentId":student["DimPersonId"],
                        "AttendanceId":-1,
                        "K12DemographicId":K12DemographicId,
                        "StudentAttendanceRate":round(fake.pyfloat(min_value=89, max_value=100, right_digits=2), 2)
                    })

            # Convert to DataFrames
            df_fact_enrollments = pd.DataFrame(fact_enrollments)
            df_fact_assessments = pd.DataFrame(fact_assessments)
            df_fact_discipline = pd.DataFrame(fact_discipline)
            df_fact_attendance = pd.DataFrame(fact_attendance)
        
            # Write tables (replace with schema if needed, e.g., schema="dbo")
            df_fact_enrollments.to_sql("FactK12StudentEnrollments", engine, if_exists="append", index=False, schema="RDS")
            df_fact_assessments.to_sql("FactK12StudentAssessments", engine, if_exists="append", index=False, schema="RDS")
            df_fact_discipline.to_sql("FactK12StudentDisciplines", engine, if_exists="append", index=False, schema="RDS")
            df_fact_attendance.to_sql("FactK12StudentAttendanceRates", engine, if_exists="append", index=False, schema="RDS")


            #print("Data successfully loaded into SQL Server!")
            return {True, "Data successfully loaded into SQL Server!"}

        else:
            #print(base_load)
            return {False, base_load}
        
    except SQLAlchemyError as e:
        #print(f"SQLAlchemy error: {e}")
        return {False,f"SQLAlchemy error: {e}"}
    except Exception as e:
        #print(f"Unexpected error: {e}")
        return {False,f"Unexpected error: {e}"}
    finally:
        if engine:
            engine.dispose()
            print("SQL connection closed")
            


if __name__ == '__main__':
    output = main()
    print (output)
