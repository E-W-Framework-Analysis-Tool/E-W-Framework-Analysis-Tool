from os import environ


def config():
    try:
        server = 'DESKTOP-1R1DFJM\\SQLEXPRESS01'
        database = 'CEDS-Data-Warehouse-V11-0-0-0'
        # server = environ.get('SSMSServer')
        # database = environ.get('FakerDatabase')
        return server, database
    except KeyError as e:
        print(f"Environment variable not set: {e}")
        return None, None
    except Exception as e:
        print(f"Unexpected error: {e}")
        return None, None


