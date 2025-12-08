from datetime import datetime
import pandas as pd
import urllib
from sqlalchemy import create_engine, text
from sqlalchemy.exc import SQLAlchemyError
from Config.config import *
from Tests.Assertions import *  
from Tests.BaseAssertions import *  


try:
    # Create engine
    engine = create_sql_alchemy_engine() 



    # --- Run assertions for all tables ---
    all_results = []
    all_results.extend(BaseAssertion())

    all_results.extend(Assertion("RDS.FactK12StudentEnrollments", ["K12StudentId", "K12SchoolId", "SchoolYearId", "EntryGradeLevelId"]))
    all_results.extend(Assertion("RDS.FactK12StudentAttendanceRates", ["K12StudentId","SchoolYearId","StudentAttendanceRate"]))
    all_results.extend(Assertion("RDS.FactK12StudentDisciplines", ["K12StudentId","SchoolYearId"]))
    all_results.extend(Assertion("RDS.FactK12StudentAssessments", ["K12StudentId","AssessmentId","SchoolYearId"]))

    # --- Insert results into RDS.TestResults ---
    with engine.begin() as conn:
        for result in all_results:
            conn.execute(
                text("""
                    INSERT INTO RDS.TestResults([Table_Name], [Test], [Column_Name], [Status], [Details],[Log_DateTime])
                    VALUES (:Table_Name, :Test, :Column_Name, :Status, :Details, :Log_DateTime)
                """),
                {
                    "Table_Name": result["Table_Name"],
                    "Test": result["Test"],
                    "Column_Name": result["Column_Name"],
                    "Status": result["Status"],
                    "Details": result["Details"],
                    "Log_DateTime": result['Log_DateTime']
                }
            )

        print("Results successfully loaded")
        

except SQLAlchemyError as e:
    print(f"SQLAlchemy error: {e}")
except Exception as e:
    print(f"Unexpected error: {e}")
finally:
    if engine:
        engine.dispose()
        print("SQL connection closed")
