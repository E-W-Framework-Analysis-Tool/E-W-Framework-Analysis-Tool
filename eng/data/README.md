# CEDS Connection Analysis

This directory contains analysis files for mapping CEDS Connections to CEDS Data Warehouse schema, processed using the `ceds-connection-parser-notebook.dib` Polyglot Notebook.

## Primary Outputs

- **[`ew-connection-to-ceds-dw-mapping.csv`](ew-connection-to-ceds-dw-mapping.csv)** - Maps indicator elements to Global IDs to Data Warehouse column paths. Shows which CEDS Connection elements have corresponding columns in the DW.

- **[`ew-connection-elements-unique-by-path.csv`](ew-connection-elements-unique-by-path.csv)** - Unique CEDS paths compiled across all CEDS Connections for E-W indicators. Elements are unique by CEDS Data Model ID (not Global ID), so the same concept may appear multiple times for different domains (e.g., Birthdate for EL Child, K12 Student, PS Student).

## Reference Files

- **[`ceds-dw-metadata-query.sql`](ceds-dw-metadata-query.sql)** - Manually crafted SQL query to extract metadata from CEDS Data Warehouse v11

- **[`ceds-dw-metadata-v11-results.csv`](ceds-dw-metadata-v11-results.csv)** - Results from running the metadata query above. Includes Global ID mappings for DW columns.

- **[`ceds-dw-v11-ddl-cleaned.sql`](ceds-dw-v11-ddl-cleaned.sql)** - CEDS Data Warehouse v11 DDL from the official CEDS DW repository, cleaned to remove hardcoded database names

- **[`ceds-elements-v13-globalcrosswalk.csv`](ceds-elements-v13-globalcrosswalk.csv)** - Crosswalk derived from CEDS Elements v13 workbook to map Data Model IDs to Global IDs

- **[`ew-connection-to-ceds-dw-mapping-distinct.csv`](ew-connection-to-ceds-dw-mapping-distinct.csv)** - Distinct version of the main mapping file, deduplicated by Global ID

## Analysis Notebook

- **[`ceds-connection-parser-notebook.dib`](ceds-connection-parser-notebook.dib)** - Polyglot Notebook containing C# code to parse CEDS Connection reports and generate the mapping analysis