from os import environ
import urllib
from sqlalchemy import create_engine
import os


def get_config():
    config = dict()
    try:
        config['SERVER'] = environ.get('SQL_SERVER')
        config['DATABASE'] = environ.get('SQL_DATABASE')
        config['PORT'] = environ.get('SQL_SERVER_PORT', '1433')
        config['UID'] = environ.get('SQL_USER', '')
        config['PWD'] = environ.get('SQL_USER_PWD', '')
        config['ODBC_DRIVER_VER'] = environ.get('ODBC_DRIVER_VER', '17')

        return config
    except KeyError as e:
        print(f"Environment variable not set: {e}")
        return {}
    except Exception as e:
        print(f"Unexpected error: {e}")
        return {}

def create_sql_alchemy_engine():
    try:
        config = get_config()

        odbc_connection_string = []
        odbc_connection_string.append('DRIVER={ODBC Driver '+config['ODBC_DRIVER_VER']+' for SQL Server};')
        odbc_connection_string.append(f'SERVER={config['SERVER']},{config['PORT']};')
        odbc_connection_string.append(f'DATABASE={config['DATABASE']};')


        if config['UID'] != '':
            odbc_connection_string.append(f'UID={config['UID']};')

        if config['PWD'] != '':
            odbc_connection_string.append(f'PWD={config['PWD']};')

        # Set to yes if you don't have a trusted certificate
        odbc_connection_string.append('TrustServerCertificate=yes;')
            
        # URL-encode the connection string parameters
        params = urllib.parse.quote_plus("".join(odbc_connection_string))
        
        # Create engine
        engine = create_engine(f"mssql+pyodbc:///?odbc_connect={params}")
        return engine
    except Exception as e:
        print('Error creating SQL Alchemy Engine.')
        print('Error', e)
        print('Exiting...')
        os._exit(1)


