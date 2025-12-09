from faker import Faker
import random
import pandas as pd
import urllib
from sqlalchemy import create_engine, text
from sqlalchemy.exc import SQLAlchemyError
from Config.config import create_sql_alchemy_engine

def load_base_tables(num_seas=1,num_leas=5,num_schools=20, num_people=100):
        engine = None 

        try:
            SEED = 1
            random.seed(SEED)
            fake = Faker()
            fake.seed_instance(SEED)

            # Number of rows
            num_seas = num_seas
            num_leas = num_leas
            num_schools = num_schools

            # Generate DimSeas
            state_lookup = {
                "01": ("Alabama", "AL")
                # "02": ("Alaska", "AK"),
                # "04": ("Arizona", "AZ"),
                # "05": ("Arkansas", "AR")
            }

            dim_seas = []
            for i, (state_code, (state_name, state_abbr)) in enumerate(state_lookup.items(), start=1):
                state_code, (state_name, state_abbr) = random.choice(list(state_lookup.items()))
                dim_seas.append({
                    "SeaOrganizationName": state_name,
                    "SeaOrganizationIdentifierSea":fake.uuid4()[:8],
                    "StateAnsiCode": state_code,
                    "StateAbbreviationCode":state_abbr,
                    "StateAbbreviationDescription":state_name,
                    "MailingAddressCity":fake.city(),
                    "MailingAddressPostalCode":fake.postcode(),
                    "MailingAddressStateAbbreviation":state_abbr,
                    "MailingAddressStreetNumberAndName":fake.street_address(),
                    "PhysicalAddressCity": fake.city(),
                    "PhysicalAddressPostalCode": fake.postcode(),
                    "PhysicalAddressStateAbbreviation":  state_abbr,
                    "PhysicalAddressStreetNumberAndName": fake.street_address(),
                    "TelephoneNumber":  fake.phone_number(),
                    "WebSiteAddress": fake.url(),
                    "MailingAddressApartmentRoomOrSuiteNumber":random.choice([None, f"Apt {random.randint(1,999)}"]),
                    "PhysicalAddressApartmentRoomOrSuiteNumber":random.choice([None, f"Suite {random.randint(1,500)}"]),
                    "MailingAddressCountyAnsiCodeCode": str(random.randint(100,999)),
                    "PhysicalAddressCountyAnsiCodeCode":  str(random.randint(100,999)),
                    "RecordStartDateTime":fake.date_this_decade(),
                    "RecordEndDateTime":None
                })

            
            LEATypeCode ={
                    "RegularNotInSupervisoryUnion":"Regular public school district that is NOT a component of a supervisory union",
                    "RegularInSupervisoryUnion":"Regular public school district that is a component of a supervisory union",
                    "SupervisoryUnion":"Supervisory Union",
                    "SpecializedPublicSchoolDistrict":"Specialized Public School District",
                    "ServiceAgency":"Service Agency",
                    "StateOperatedAgency":"State Operated Agency",
                    "FederalOperatedAgency":"Federal Operated Agency",
                    "IndependentCharterDistrict": "Independent Charter District",
                    "Other":"Other Local Education Agencies"

            }

            # Generate DimLeas
            dim_leas = []
            for i in range(1, num_leas + 1):
                sea = random.choice(dim_seas)  # assign LEA to a SEA/state
                
                lea_type_key = random.choice(list(LEATypeCode.keys()))  # pick one key
                lea_type_desc = LEATypeCode[lea_type_key]               # get the matching description
                
                dim_leas.append({
                    "IeuOrganizationName": f"{sea['SeaOrganizationName']} Education Unit",
                    "IeuOrganizationIdentifierSea": fake.unique.bothify(text="IEU-#####??"),  # unique alphanumeric
                    "StateAnsiCode": sea["StateAnsiCode"],
                    "StateAbbreviationCode": sea["StateAbbreviationCode"],
                    "StateAbbreviationDescription": sea["StateAbbreviationDescription"],
                    "SeaOrganizationName": sea["SeaOrganizationName"],
                    "SeaOrganizationIdentifierSea": sea["SeaOrganizationIdentifierSea"],
                    "LeaOrganizationName": f"{fake.city()} Public Schools",
                    "LeaIdentifierNces": fake.unique.random_number(digits=7),
                    "LeaIdentifierSea": fake.unique.bothify(text="LEA-#####"),
                    "PriorLeaIdentifierSea": None,
                    "LeaSupervisoryUnionIdentificationNumber": fake.bothify(text="???"),
                    "ReportedFederally": random.choice([0,1]),
                    "LeaTypeCode": lea_type_key,
                    "LeaTypeDescription": lea_type_desc,
                    "LeaTypeEdFactsCode": None,
                    "MailingAddressStreetNumberAndName": fake.street_address(),
                    "MailingAddressApartmentRoomOrSuiteNumber": random.choice([None,f"Apt {random.randint(1,999)}"]),
                    "MailingAddressCity": fake.city(),
                    "MailingAddressPostalCode": fake.postcode(),
                    "MailingAddressStateAbbreviation": sea["StateAbbreviationCode"],
                    "MailingAddressCountyAnsiCodeCode": str(random.randint(100,999)),
                    "PhysicalAddressStreetNumberAndName": fake.street_address(),
                    "PhysicalAddressApartmentRoomOrSuiteNumber": random.choice([None,f"Suite {random.randint(1,500)}"]),
                    "PhysicalAddressCity": fake.city(),
                    "PhysicalAddressPostalCode": fake.postcode(),
                    "PhysicalAddressStateAbbreviation": sea["StateAbbreviationCode"],
                    "PhysicalAddressCountyAnsiCodeCode": str(random.randint(100,999)),
                    "Longitude": round(random.uniform(-125, -65), 6),
                    "Latitude": round(random.uniform(25, 50), 6),
                    "TelephoneNumber": fake.phone_number(),
                    "WebSiteAddress": fake.url(),
                    "OutOfStateIndicator": random.choice([0,1]),
                    "LeaOperationalStatus": random.choice(["Open","Closed","New","Added","FutureAgency","Reopened"]),
                    "LeaOperationalStatusEdFactsCode": None,
                    "OperationalStatusEffectiveDate": fake.date_this_decade(),
                    "CharterLeaStatus": random.choice([0,1]),
                    "ReconstitutedStatus": random.choice([0,1]),
                    "McKinneyVentoSubgrantRecipient": random.choice([0,1]),
                    ##"NameOfInstitution": f"{fake.city()} Education Institution",
                    "RecordStartDateTime": fake.date_this_decade(),
                    "RecordEndDateTime": None
                })


            # Lookup for School Types
            SchoolTypeCode = {
                "RegularSchool": "A school providing a general program of regular academic subjects",
                "SpecialEducationSchool": "A school that focuses primarily on serving students with disabilities",
                "VocationalSchool": "A school that focuses on vocational/technical training",
                "AlternativeSchool": "A school that provides nontraditional education, often for at-risk students",
                "CharterSchool": "A publicly funded school that operates under a charter",
                "Other": "Other school types"
            }

            AdministrativeFundingControl = {
                "Public":  "Public School",
                "Private": "Private School",
                "Other": "Other"  
            }

            # Generate DimK12Schools
            dim_schools = []
            for i in range(1, num_schools + 1):
                lea = random.choice(dim_leas)  # randomly assign schools to LEAs
                school_type_key = random.choice(list(SchoolTypeCode.keys()))
                school_type_desc = SchoolTypeCode[school_type_key]
                AdministrativeFundingControl_key = random.choice(list(AdministrativeFundingControl.keys()))
                AdministrativeFundingControl_desc = AdministrativeFundingControl[AdministrativeFundingControl_key]

                dim_schools.append({
                    "LeaOrganizationName": lea["LeaOrganizationName"],
                    "LeaIdentifierNces": lea["LeaIdentifierNces"],
                    "LeaIdentifierSea": lea["LeaIdentifierSea"],
                    "NameOfInstitution": f"{fake.city()} {random.choice(['Elementary','Middle','High'])} School",
                    "SchoolIdentifierNces": fake.unique.random_number(digits=8),
                    "SchoolIdentifierSea": fake.unique.bothify(text="SCH-#####"),
                    "SeaOrganizationName": lea["SeaOrganizationName"],
                    "SeaOrganizationIdentifierSea": lea["SeaOrganizationIdentifierSea"],
                    "StateAnsiCode": lea["StateAnsiCode"],
                    "StateAbbreviationCode": lea["StateAbbreviationCode"],
                    "StateAbbreviationDescription": lea["StateAbbreviationDescription"],
                    "PriorLeaIdentifierSea": None,
                    "PriorSchoolIdentifierSea": None,
                    "CharterSchoolIndicator": random.choice([0, 1]),
                    "CharterSchoolContractIdNumber": fake.bothify(text="CSC-#####"),
                    "CharterSchoolContractApprovalDate": fake.date_this_decade(),
                    "CharterSchoolContractRenewalDate": fake.date_between(start_date="-1y", end_date="today"),
                    "ReportedFederally": random.choice([0, 1]),
                    "LeaTypeCode": lea["LeaTypeCode"],
                    "LeaTypeDescription": lea["LeaTypeDescription"],
                    "LeaTypeEdFactsCode": lea["LeaTypeEdFactsCode"],
                    "SchoolTypeCode": school_type_key,
                    "SchoolTypeDescription": school_type_desc,
                    "SchoolTypeEdFactsCode": None,
                    "MailingAddressCity": fake.city(),
                    "MailingAddressPostalCode": fake.postcode(),
                    "MailingAddressStateAbbreviation": lea["StateAbbreviationCode"],
                    "MailingAddressStreetNumberAndName": fake.street_address(),
                    "PhysicalAddressCity": fake.city(),
                    "PhysicalAddressPostalCode": fake.postcode(),
                    "PhysicalAddressStateAbbreviation": lea["StateAbbreviationCode"],
                    "PhysicalAddressStreetNumberAndName": fake.street_address(),
                    "TelephoneNumber": fake.phone_number(),
                    "WebSiteAddress": fake.url(),
                    "OutOfStateIndicator": random.choice([0, 1]),
                    "RecordStartDateTime": fake.date_this_decade(),
                    "RecordEndDateTime": None,
                    "SchoolOperationalStatus": random.choice(["Open", "Closed", "Inactive"]),
                    "SchoolOperationalStatusEdFactsCode": None,
                    "CharterSchoolStatus": random.choice([0, 1]),
                    "ReconstitutedStatus": random.choice([0, 1]),
                    "MailingAddressApartmentRoomOrSuiteNumber": random.choice([None, f"Apt {random.randint(1, 999)}"]),
                    "PhysicalAddressApartmentRoomOrSuiteNumber": random.choice([None, f"Suite {random.randint(1, 500)}"]),
                    "IeuOrganizationName": lea["IeuOrganizationName"],
                    "IeuOrganizationIdentifierSea": lea["IeuOrganizationIdentifierSea"],
                    "MailingAddressCountyAnsiCodeCode": str(random.randint(100, 999)),
                    "PhysicalAddressCountyAnsiCodeCode": str(random.randint(100, 999)),
                    "Longitude": round(random.uniform(-125, -65), 6),
                    "Latitude": round(random.uniform(25, 50), 6),
                    "SchoolOperationalStatusEffectiveDate": fake.date_this_decade(),
                    "AdministrativeFundingControlCode": AdministrativeFundingControl_key,
                    "AdministrativeFundingControlDescription": AdministrativeFundingControl_desc
                })


            ##assign roles 
            roles = [
                # "ELChild",
                "K12Student",
                # "PsStudent",
                # "AeStudent",
                # "WorkforceProgramParticipant",
                # "ELStaff",
                 "K12Staff",
                # "PsStaff"
            ]

            dim_people = []
            for i in range(1, num_people + 1):
                # Randomly pick ONE role
                role = random.choice(roles)
                
                # Random start/end dates
                start_date = fake.date_between(start_date="-10y", end_date="-1y")
                end_date = None if random.random() > 0.8 else fake.date_between(start_date=start_date, end_date="-1y")
                
                dim_people.append({
                    "FirstName": fake.first_name(),
                    "MiddleName": fake.first_name() if random.random() > 0.5 else None,
                    "LastOrSurname": fake.last_name(),
                    "BirthDate": fake.date_of_birth(minimum_age=5, maximum_age=21),  
                    "ELChildChildIdentifierState": None,
                    "K12StudentStudentIdentifierState": None,
                    "K12StudentStudentIdentifierDistrict": None,
                    "K12StudentStudentIdentifierNationalMigrant": None,
                    "PsStudentStudentIdentifierState": None,
                    "AeStudentStudentIdentifierState": None,
                    "WorkforceProgramParticipantPersonIdentifierState": None,
                    "ELStaffStaffMemberIdentifierState": None,
                    "K12StaffStaffMemberIdentifierState": None,
                    "K12StaffStaffMemberIdentifierDistrict": None,
                    "PsStaffStaffMemberIdentifierState": None,
                    "PersonIdentifierDriversLicense": None,
                    "IsActiveELChild": 0,
                    "IsActiveK12Student": random.choice([0,1]),
                    "IsActivePsStudent": 0,
                    "IsActiveAeStudent": 0,
                    "IsActiveWorkforceProgramParticipant": 0,
                    "IsActiveELStaff": 0,
                    "IsActiveK12Staff": random.choice([0,1]),
                    "IsActivePsStaff": 0,
                    "RecordStartDateTime": start_date,
                    "RecordEndDateTime": end_date,
                    "ElectronicMailAddressHome": fake.email(),
                    "ElectronicMailAddressOrganizational": fake.company_email(),
                    "ElectronicMailAddressWork": fake.email(),
                    "TelephoneNumberFax": fake.phone_number(),
                    "TelephoneNumberHome": fake.phone_number(),
                    "TelephoneNumberMobile": fake.phone_number(),
                    "TelephoneNumberWork": fake.phone_number(),
                    "PersonalTitleOrPrefix": None,
                    "PositionTitle": None
                })
                
                # # Assign identifiers/status depending on role commenting for now
                if role == "ELChild":
                    dim_people[-1]["IsActiveELChild"] = 1
                    dim_people[-1]["ELChildChildIdentifierState"] = fake.uuid4()[:10]
                elif role == "K12Student":
                    dim_people[-1]["IsActiveK12Student"] = 1
                    dim_people[-1]["K12StudentStudentIdentifierState"] = fake.uuid4()[:10]
                    dim_people[-1]["K12StudentStudentIdentifierDistrict"] = fake.random_number(digits=8, fix_len=True)
                    dim_people[-1]["K12StudentStudentIdentifierNationalMigrant"] = fake.uuid4()[:10]
                elif role == "PsStudent":
                    dim_people[-1]["IsActivePsStudent"] = 1
                    dim_people[-1]["PsStudentStudentIdentifierState"] = fake.uuid4()[:10]
                elif role == "AeStudent":
                    dim_people[-1]["IsActiveAeStudent"] = 1
                    dim_people[-1]["AeStudentStudentIdentifierState"] = fake.uuid4()[:10]
                elif role == "WorkforceProgramParticipant":
                    dim_people[-1]["IsActiveWorkforceProgramParticipant"] = 1
                    dim_people[-1]["WorkforceProgramParticipantPersonIdentifierState"] = fake.uuid4()[:10]
                elif role == "ELStaff":
                    dim_people[-1]["IsActiveELStaff"] = 1
                    dim_people[-1]["ELStaffStaffMemberIdentifierState"] = fake.uuid4()[:10]
                    dim_people[-1]["PositionTitle"] = random.choice(["Teacher", "Assistant", "Specialist"])
                elif role == "K12Staff":
                    dim_people[-1]["IsActiveK12Staff"] = 1
                    dim_people[-1]["K12StaffStaffMemberIdentifierState"] = fake.uuid4()[:10]
                    dim_people[-1]["K12StaffStaffMemberIdentifierDistrict"] = fake.random_number(digits=7, fix_len=True)
                    dim_people[-1]["PositionTitle"] = random.choice(["Teacher", "Administrator", "Counselor"])
                 
                elif role == "PsStaff":
                    dim_people[-1]["IsActivePsStaff"] = 1
                    dim_people[-1]["PsStaffStaffMemberIdentifierState"] = fake.uuid4()[:10]
                    dim_people[-1]["PositionTitle"] = random.choice(["Professor", "Lecturer", "Coordinator"])

                # Assign driver license only for staff
                if role.endswith("Staff"):
                    dim_people[-1]["PersonIdentifierDriversLicense"] = fake.bothify(text="??######")
                
           
            dim_people_current = []
            for  person in dim_people:
                dim_people_current.append({
                    "FirstName": person['FirstName'],
                    "MiddleName": person['MiddleName'],
                    "LastOrSurname": person['LastOrSurname'],
                    "ELChildChildIdentifierState": None,
                    "K12StudentStudentIdentifierState": person['K12StudentStudentIdentifierState'],
                    "K12StudentStudentIdentifierDistrict": person['K12StudentStudentIdentifierDistrict'],
                    "K12StudentStudentIdentifierNationalMigrant": None,
                    "PsStudentStudentIdentifierState": None,
                    "AeStudentStudentIdentifierState": None,
                    "WorkforceProgramParticipantPersonIdentifierState": None,
                    "ELStaffStaffMemberIdentifierState": None,
                    "K12StaffStaffMemberIdentifierState": person['K12StaffStaffMemberIdentifierState'],
                    "K12StaffStaffMemberIdentifierDistrict": person['K12StaffStaffMemberIdentifierDistrict'],
                    "PsStaffStaffMemberIdentifierState": None,
                    "PersonIdentifierDriversLicense":  person['PersonIdentifierDriversLicense'],
                    "PersonIdentifierSSN": None,
                    "PersonIdentifierState":None,
                    "StudentIdentifierState": None,
                    "IsActiveELChild": 0,
                    "IsActiveK12Student": person['IsActiveK12Student'],
                    "IsActivePsStudent": 0,
                    "IsActiveAeStudent": 0,
                    "IsActiveWorkforceProgramParticipant": 0,
                    "IsActiveELStaff": 0,
                    "IsActiveK12Staff": person['IsActiveK12Staff'],
                    "IsActivePsStaff": 0,
                    "ElectronicMailAddressHome": random.choice([person['ElectronicMailAddressHome'],fake.email()]),
                    "ElectronicMailAddressOrganizational": random.choice([person['ElectronicMailAddressOrganizational'],fake.company_email()]),
                    "ElectronicMailAddressWork": random.choice([person['ElectronicMailAddressWork'],fake.email()]),
                    "TelephoneNumberFax": random.choice([person['TelephoneNumberFax'],fake.phone_number()]),
                    "TelephoneNumberHome": random.choice([person['TelephoneNumberHome'],fake.phone_number()]),
                    "TelephoneNumberMobile": random.choice([person['TelephoneNumberMobile'],fake.phone_number()]),
                    "TelephoneNumberWork": random.choice([person['TelephoneNumberWork'],fake.phone_number()]),
                    "PersonalTitleOrPrefix": random.choice([None, "Mr.", "Ms.", "Mrs.", "Dr."]),
                    "PositionTitle": person['PositionTitle'],
                    "GenerationCodeOrSuffix": random.choice([None, "Jr.", "Sr.", "III"]),
                    "HighestLevelOfEducationCompletedCode":  None,
                    "HighestLevelOfEducationCompletedDescription": None
                })

            

            subjects = { "13371": "Arts","13372":"English","01166": "Mathematics","00560": "Reading"} 
            assessment_types = {"AchievementTest":"Achievement test","AptitudeTest": "Aptitude Test","Benchmark": "Benchmark"}
            assessments = []
            for i, subject_key in enumerate(subjects.keys(), start=1):
                        for assessment_type_key in assessment_types.keys():
                            subject_desc = subjects[subject_key]
                            assessment_type_desc = assessment_types[assessment_type_key]
                            assessments.append({
                                "AssessmentIdentifierState": 1000+i,
                                "AssessmentFamilyShortName": subject_desc[:3].capitalize(),
                                "AssessmentTitle": f"{subject_desc} Assessment",
                                "AssessmentShortName": f"{subject_desc}",
                                "AssessmentTypeCode": assessment_type_key,
                                "AssessmentTypeDescription": assessment_type_desc,
                                "AssessmentTypeEdFactsCode": -1,
                                "AssessmentAcademicSubjectCode": subject_key,
                                "AssessmentAcademicSubjectDescription": subject_desc,
                                "AssessmentAcademicSubjectEdFactsCode": -1,
                                "AssessmentTypeAdministeredCode": -1,
                                "AssessmentTypeAdministeredDescription": -1,
                                "AssessmentTypeAdministeredEdFactsCode": -1,
                                "AssessmentTypeAdministeredToEnglishLearnersCode": -1,
                                "AssessmentTypeAdministeredToEnglishLearnersDescription": -1,
                                "AssessmentTypeAdministeredToEnglishLearnersEdFactsCode": -1
                            })


            # Convert to DataFrames
            df_seas = pd.DataFrame(dim_seas)
            df_leas = pd.DataFrame(dim_leas)
            df_schools = pd.DataFrame(dim_schools)
            df_people = pd.DataFrame(dim_people)
            dim_people_current = pd.DataFrame(dim_people_current)
            df_assessments = pd.DataFrame(assessments)

            # Create engine
            engine = create_sql_alchemy_engine() 

            # Delete existing rows 
            with engine.begin() as conn:  # auto-commit transaction
                conn.execute(text(                
                "DELETE FROM RDS.FactK12StudentEnrollments;DBCC CHECKIDENT ('RDS.FactK12StudentEnrollments', RESEED, 0);" \
                "DELETE FROM RDS.FactK12StudentAssessments;DBCC CHECKIDENT ('RDS.FactK12StudentAssessments', RESEED, 0);" \
                "DELETE FROM RDS.FactK12StudentDisciplines;DBCC CHECKIDENT ('RDS.FactK12StudentDisciplines', RESEED, 0);" \
                "DELETE FROM RDS.FactK12StudentAttendanceRates;DBCC CHECKIDENT ('RDS.FactK12StudentAttendanceRates', RESEED, 0);"\
                "DELETE FROM RDS.DimSeas;DBCC CHECKIDENT ('RDS.DimSeas', RESEED, 0);" \
                "DELETE FROM RDS.DimLeas;DBCC CHECKIDENT ('RDS.DimLeas', RESEED, 0);" \
                "DELETE FROM RDS.DimK12Schools;DBCC CHECKIDENT ('RDS.DimK12Schools', RESEED, 0);" \
                "DELETE FROM RDS.DimPeople;DBCC CHECKIDENT ('RDS.DimPeople', RESEED, 0);" \
                "DELETE FROM RDS.DimPeople_current;DBCC CHECKIDENT ('RDS.DimPeople_current', RESEED, 0);" \
                "DELETE FROM RDS.DimAssessments;DBCC CHECKIDENT ('RDS.DimAssessments', RESEED, 0);"\
                "IF NOT EXISTS (SELECT 1 FROM RDS.DimDataCollections WHERE DimDataCollectionId = -1) BEGIN SET IDENTITY_INSERT RDS.DimDataCollections ON " \
                "INSERT INTO RDS.DimDataCollections  (DimDataCollectionId, DataCollectionName) VALUES (-1, 'None') SET IDENTITY_INSERT RDS.DimDataCollections off END;"\
                "IF NOT EXISTS (SELECT 1 FROM RDS.DimEducationOrganizationNetworks WHERE DimEducationOrganizationNetworkId = -1) BEGIN SET IDENTITY_INSERT RDS.DimEducationOrganizationNetworks ON " \
                "INSERT INTO RDS.DimEducationOrganizationNetworks (DimEducationOrganizationNetworkId, OrganizationIdentifierSea,OrganizationTypeCode,OrganizationTypeDescription,"\
			    "OrganizationName,RecordStartDateTime) VALUES (-1, 'None','None','None','None',-1) SET IDENTITY_INSERT RDS.DimEducationOrganizationNetworks off END;"\
                "IF NOT EXISTS (SELECT 1 FROM RDS.DimIeus WHERE DimIeuId = -1) BEGIN SET IDENTITY_INSERT RDS.DimIeus ON " \
                "INSERT INTO RDS.DimIeus (DimIeuId,OutOfStateIndicator,RecordStartDateTime) VALUES (-1,-1,-1) SET IDENTITY_INSERT RDS.DimIeus off END;" \
                "IF NOT EXISTS (SELECT 1 FROM RDS.DimCompetencyDefinitions WHERE DimCompetencyDefinitionId = -1) " \
                "BEGIN SET IDENTITY_INSERT RDS.DimCompetencyDefinitions ON INSERT INTO RDS.DimCompetencyDefinitions" \
                " (DimCompetencyDefinitionId,CompetencyDefinitionValidStartDate) VALUES (-1,-1) SET IDENTITY_INSERT RDS.DimCompetencyDefinitions off END;" \
                "IF NOT EXISTS (SELECT 1 FROM RDS.DimAssessmentPerformanceLevels WHERE DimAssessmentPerformanceLevelId = -1) " \
                "BEGIN SET IDENTITY_INSERT RDS.DimAssessmentPerformanceLevels ON INSERT INTO RDS.DimAssessmentPerformanceLevels"\
                " (DimAssessmentPerformanceLevelId,AssessmentPerformanceLevelIdentifier,AssessmentPerformanceLevelLabel,AssessmentPerformanceLevelScoreMetric," \
                "AssessmentPerformanceLevelLowerCutScore,AssessmentPerformanceLevelUpperCutScore) VALUES (-1,'','','','','') SET IDENTITY_INSERT RDS.DimAssessmentPerformanceLevels off END;"\
                "IF NOT EXISTS (SELECT 1 FROM RDS.DimTitleIStatuses WHERE DimTitleIStatusId = -1) BEGIN SET IDENTITY_INSERT RDS.DimTitleIStatuses ON " \
                "INSERT INTO RDS.DimTitleIStatuses (DimTitleIStatusId) VALUES (-1) SET IDENTITY_INSERT RDS.DimTitleIStatuses off END;"\
                "DELETE FROM RDS.DimAssessments;DBCC CHECKIDENT ('RDS.DimAssessments', RESEED, 0);" \
                "IF NOT EXISTS (SELECT 1 FROM RDS.DimAssessmentSubtests WHERE DimAssessmentSubtestId = -1) BEGIN SET IDENTITY_INSERT RDS.DimAssessmentSubtests ON " \
                "INSERT INTO RDS.DimAssessmentSubtests (DimAssessmentSubtestId,AssessmentAcademicSubjectCode,AssessmentAcademicSubjectDescription," \
                "AssessmentSubtestIdentifierInternal,AssessmentSubtestTitle,AssessmentSubtestAbbreviation,AssessmentSubtestDescription," \
                "AssessmentSubtestVersion,AssessmentLevelForWhichDesigned,AssessmentEarlyLearningDevelopmentalDomain,AssessmentSubtestMinimumValue," \
                "AssessmentSubtestMaximumValue,AssessmentSubtestScaleOptimalValue,AssessmentContentStandardType,AssessmentPurpose," \
                "AssessmentSubtestRules,AssessmentFormSubtestTier,AssessmentFormSubtestContainerOnly) " \
                "VALUES (-1,'','','','','','','','','','','','','','','','','') SET IDENTITY_INSERT RDS.DimAssessmentSubtests off END;" \
                "IF NOT EXISTS (SELECT 1 FROM RDS.DimAssessmentAdministrations WHERE DimAssessmentAdministrationId = -1) " \
                "BEGIN SET IDENTITY_INSERT RDS.DimAssessmentAdministrations ON INSERT INTO RDS.DimAssessmentAdministrations (DimAssessmentAdministrationId) " \
                "VALUES (-1) SET IDENTITY_INSERT RDS.DimAssessmentAdministrations off END; " \
                "IF NOT EXISTS (SELECT 1 FROM RDS.DimAssessmentParticipationSessions WHERE DimAssessmentParticipationSessionId = -1) " \
                "BEGIN SET IDENTITY_INSERT RDS.DimAssessmentParticipationSessions ON INSERT INTO RDS.DimAssessmentParticipationSessions " \
				"(DimAssessmentParticipationSessionId,AssessmentSessionSpecialCircumstanceTypeCode,AssessmentSessionSpecialCircumstanceTypeDescription) " \
                "VALUES (-1,'MISSING','MISSING') SET IDENTITY_INSERT RDS.DimAssessmentParticipationSessions off END;"))
            
            df_seas.to_sql("DimSeas", engine, if_exists="append", index=False, schema="RDS")
            df_leas.to_sql("DimLeas", engine, if_exists="append", index=False, schema="RDS")
            df_schools.to_sql("DimK12Schools", engine, if_exists="append", index=False, schema="RDS")
            df_people.to_sql("DimPeople", engine, if_exists="append", index=False, schema="RDS")
            dim_people_current.to_sql("DimPeople_Current", engine, if_exists="append", index=False, schema="RDS")
            df_assessments.to_sql("DimAssessments", engine, if_exists="append", index=False, schema="RDS")
            return True

        except SQLAlchemyError as e:
            return "Error occurred:{}".format(e)

        finally:
            # Dispose engine to close all connections
            if engine:
                engine.dispose()
                print("SQL connection closed.")