# Introduction 
This folder defines process for generating a sample dataset for the CEDS Data Warehouse (DWH) using the Faker Python library. The purpose is to create realistic test data that supports development and validation of one or more mapped indicators within the CEDS framework.

# Folder Content

## 1. Config: 

### 1.1. config:
Contains configuration logic to retrieve database connection parameters.

## 2. Scripts: 

### 2.1. Load_Base_Dimension_Tables:
Contains scripts to load dimension tables, which serve as foundational data sources for fact tables.

### 2.2. Load_Fact_Tables:
Includes scripts for loading fact tables, ensuring referential integrity with related dimension tables.

## 3. Tests:

### 3.1. Test.py:
Contains scripts to perform data validation and quality assurance checks on loaded datasets.

### 3.2. BaseAssertions.py:
Contains scripts to perform data count match for base tables loaded datasets.

### 3.3. Assertions.py:
Contains scripts to perform data validation and quality assurance checks on loaded datasets.

## 4. AnalysisQuery:
Contains SQL queries used to create derived characteristics and assess data readiness per indicator.

# Getting Started

**Software Dependencies:**
1. SQL Server Instance with CEDS-Data-Warehouse-V11-0-0-0 deployed.
2. SQL Server Instance with CEDS-Elements-V11.0.0.0 deployed.
3. Load Junk Dimension Tables 
4. Create TestResults Table (DDL script under Tests folder)
5. Python (recommended version: 3.10 or higher).
6. Install dependencies using:
    pip install -r requirements.txt


**How to Run Scripts to Load and Test Sample Data in CEDS DWH**
1. Load Sample Data into the CEDS Data Warehouse
python -m Scripts.Load_Fact_Tables

2. Run QA Tests on the Loaded Data
python -m Tests.Test

3. Verify Test Results in SQL Server
    Select * from RDS.TestResults;