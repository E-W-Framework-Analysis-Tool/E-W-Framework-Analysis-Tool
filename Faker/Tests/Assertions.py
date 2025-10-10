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

def Assertion(table_name, column_names):
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

        # --- Load tables ---
        with engine.begin() as conn:
            query = text(f"SELECT {', '.join([f'[{c}]' for c in column_names])} FROM {table_name};")
            df = pd.read_sql_query(query, conn)

        # --- Row count test ---
        try:
            assert not df.empty, f"Table '{table_name}' is empty"
            log_result(results, table_name, "NotEmpty", "-", "Passed", "Table not empty", Log_Datetime)
        except AssertionError as e:
            log_result(results, table_name, "NotEmpty", "-", "Failed", str(e), Log_Datetime)

        # --- Completeness test ---
        for col in column_names:
            try:
                assert df[col].notnull().all(), f"Nulls found in {col}"
                log_result(results, table_name, "Completeness", col, "Passed", "No NULLs", Log_Datetime)
            except AssertionError as e:
                log_result(results, table_name, "Completeness", col, "Failed", str(e), Log_Datetime)

        # --- Uniqueness test ---
        try:
            assert not df.duplicated(subset=column_names).any(), f"Duplicates found on {', '.join(column_names)}"
            log_result(results, table_name, "Uniqueness", ", ".join(column_names), "Passed", "No duplicates", Log_Datetime)
        except AssertionError as e:
            log_result(results, table_name, "Uniqueness", ", ".join(column_names), "Failed", str(e), Log_Datetime)

    except SQLAlchemyError as e:
        log_result(results, table_name, "Database Connection", "-", "Error", str(e), Log_Datetime)
    except Exception as e:
        log_result(results, table_name, "Unexpected Error", "-", "Error", str(e), Log_Datetime)
    finally:
        if engine:
            engine.dispose()

    return results
