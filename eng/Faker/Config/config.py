from os import environ


def config():
    try:
        server = environ.get('SSMSServer')
        database = environ.get('CEDS_DATABASE')
        return server, database
    except KeyError as e:
        print(f"Environment variable not set: {e}")
        return None, None
    except Exception as e:
        print(f"Unexpected error: {e}")
        return None, None


