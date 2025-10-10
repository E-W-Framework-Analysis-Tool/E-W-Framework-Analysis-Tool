from datetime import datetime
import pandas as pd
import urllib
from sqlalchemy import create_engine, text
from sqlalchemy.exc import SQLAlchemyError
from Config.config import *

def log_result(results, table, test, column, status, details, log_time):
    results.append({
        "Table_Name": table,
        "Test": test,
        "Column_Name": column,
        "Status": status,
        "Details": details,
        "Log_DateTime": log_time
    })

def BaseAssertion():
    engine = None
    results = []
    Log_Datetime = datetime.now()

    try:
        # --- DB Connection ---
        server, database = config()
        params = urllib.parse.quote_plus(
            f"Driver={{ODBC Driver 17 for SQL Server}};"
            f"Server={server};Database={database};Trusted_Connection=yes;"
        )
        engine = create_engine(f"mssql+pyodbc:///?odbc_connect={params}")
        with engine.begin() as conn:
            dim_people = pd.read_sql_query("SELECT * FROM RDS.DimPeople;", conn)
            dim_seas = pd.read_sql_query("SELECT * FROM RDS.DimSeas;", conn)
            dim_leas = pd.read_sql_query("SELECT * FROM RDS.DimLeas;", conn)
            dim_schools = pd.read_sql_query("SELECT * FROM RDS.DimK12Schools;", conn)

         # --- Count Tests ---
        for name, df, expected in [
                ("DimPeople", dim_people, 100),
                ("DimSeas", dim_seas, 1),
                ("DimLeas", dim_leas, 5),
                ("DimK12Schools", dim_schools, 20)            ]:
                try:
                    assert len(df) == expected, f"Expected {expected} rows but got {len(df)}"
                    log_result(results, name, "RowCount", "-", "Passed", "Row count matches", Log_Datetime)
                except AssertionError as e:
                    log_result(results, name, "RowCount", "-", "Failed", str(e), Log_Datetime)

    except SQLAlchemyError as e:
        log_result(results, '', "Database Connection", "-", "Error", str(e), Log_Datetime)
    except Exception as e:
        log_result(results, '', "Unexpected Error", "-", "Error", str(e), Log_Datetime)
    finally:
        if engine:
            engine.dispose()

    return results


