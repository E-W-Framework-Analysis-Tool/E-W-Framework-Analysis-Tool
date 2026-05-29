# E-W Framework Analysis Tool

The **E-W Framework Analysis Tool** is an open-source application designed to help education agencies, districts, and
their partners evaluate their data readiness to answer key questions from the
[Education-to-Workforce Indicator Framework (E-W Framework)](https://educationtoworkforce.org). It supports states and
LEAs in identifying priority indicators, assessing gaps, and planning improvements in data infrastructure.

## Key Features

- **Data Readiness Scoring** – Score your current data coverage against E-W Framework Essential Questions and
  Indicators.
- **Manual and Automated Inventory** – Use high-level checklists or connect to systems via Ed-Fi APIs or CEDS-compatible
  data stores.
- **LLM-Assisted Entry** – Generate a structured data source assessment from any schema, data dictionary, or API spec
  using an LLM of your choice — no special integration required.
- **Granular Feedback** – Identify specific data elements missing for full coverage of each Indicator.
- **Secure Workflows** – Analyze data in secure environments via air-gapped SQL profiling or direct browser-to-API
  interactions.
- **Standards Mapping Overview** – Explore how Ed-Fi and CEDS data standards map to the Framework's data elements.
- **Example Visualizations** – Browse embedded Power BI dashboards built for Essential Questions 12, 18, and 19 to see
  what analysis looks like end-to-end.
- **Guided Walkthrough** – A built-in tour highlights key areas of the application for new users.
- **Reporting** – Generate detailed, accessible reports to support planning and communication.
- **Benchmarks and Collaboration (Coming Soon)** – Compare anonymized scores with other users to identify areas of
  shared focus or opportunity.

## Who Is This For?

- State education agencies (SEAs) building or expanding their P20W+ systems
- Local education agencies (LEAs) evaluating their P12+ infrastructure
- **Statewide Longitudinal Data System (SLDS) teams** planning cross-agency data integration across early childhood,
  K–12, higher education, labor, and other sectors
- Education-focused vendors and researchers working with public education data systems

A common use case is **data roadmapping for multi-agency SLDS projects**. The tool helps teams identify which Framework
questions are within reach given current data assets, surface high-value near-term wins (such as adding a single
dimension to an existing warehouse), and make the case for longer-term investments like onboarding a new data system
that would round out coverage for important cross-sector questions.

## The E-W Framework and This Tool

The [Education-to-Workforce Indicator Framework](https://educationtoworkforce.org) defines a set of Essential Questions
and Indicators for understanding how students progress from education into the workforce. Each Indicator comes with
recommended metrics and associated data needs, but the Framework itself does not prescribe specific data elements or
data standards.

This tool takes those recommended metrics and breaks them down into concrete, named **data elements** — discrete pieces
of information such as "Age," "Race/Ethnicity," or "Suspensions and Expulsions (K-12)" — that can be mapped to real data
systems. Each data element is then associated with:

- The **Indicators** it supports, so a gap in a single element can be traced to its impact on Framework coverage
- **Data standard assessors** (Ed-Fi API assessors and CEDS Data Warehouse queries) that know how to find and profile
  that element in a given system
- A **scoring model** that translates data availability and quality into a coverage score for each Indicator and
  Essential Question

This mapping layer is what allows the tool to go from "we have an Ed-Fi ODS" or "we have a CEDS data warehouse" to "here
is your coverage score for each Essential Question, and here are the specific gaps holding you back."

### Data Sources

Beyond direct system integrations, the tool incorporates two additional data sources:

**Education Commission of the States (ECS) Inventory** — ECS maintains a state-by-state inventory of data elements that
states have reported as available for research and exploration. The tool incorporates this inventory so that agencies
can see how their state's publicly reported availability compares to what the Framework requires, and to identify
elements that may be present but underreported.

**LLM-Assisted Manual Entry** — For data systems that don't have an Ed-Fi API or CEDS-compatible warehouse, the tool
supports a guided manual entry workflow. Users copy a ready-made prompt (pre-loaded with all Framework data elements and
their descriptions) into any LLM of their choice, along with a SQL schema, data dictionary, API specification, or other
reference material. The LLM produces a structured project file that can be imported directly into the tool. This turns
manual data inventory from a from-scratch task into a QA and review exercise — and works in environments where automated
profiling isn't possible, including against private LLM endpoints where source schemas never leave your network.

## Application Pages

The tool is organized around several pages, each serving a distinct purpose in the assessment workflow:

- **Home** – Overview of the Framework, how to use the tool, and links to get started.
- **Dashboard** – Project management hub. Load, save, and share your project file; manage data sources; and track
  overall coverage progress.
- **Data Source Details** – Deep-dive into a single data source. Run assessments (Ed-Fi, CEDS, ECS, manual, or
  LLM-assisted), review element-level results, and make corrections.
- **Analysis** – Coverage reports mapped to the Framework's 20 Essential Questions, individual Indicators, and
  Disaggregates. Identifies gaps and surfaces near-term opportunities.
- **Mapping Overview** – A reference view showing how Ed-Fi and CEDS data standards align to the Framework's data
  elements. Useful for understanding coverage before running a full assessment.
- **Visualizations** – Embedded Power BI dashboards demonstrating what end-to-end analysis looks like for Essential
  Questions 12, 18, and 19. These dashboards are also available as a separate open-source repository and are built on
  the CEDS Data Warehouse schema. A setup guide is included for agencies that want to connect them to their own data.
- **Guided Walkthrough** – A built-in "Take a Tour" feature that highlights key UI elements and walks new users through
  the application's core workflow.

## Architecture and Data Privacy

The tool is a **client-side Blazor WebAssembly application**. All processing occurs in the user's browser — there is no
application server, no database, and no telemetry. User data never leaves the browser unless the user explicitly exports
it as a file.

**Project data** is stored in browser `localStorage` as a convenience cache and can be saved to and loaded from a local
JSON file at any time. The JSON file contains only metadata and scoring results — not source data records.

**Ed-Fi API integration** is optional. When used, API credentials are entered by the user and exist only in browser
memory for the duration of the session. The WASM runtime makes API calls directly from the browser to the configured
Ed-Fi ODS endpoint. No credentials are transmitted to any third-party server. The tool can be hosted on an internal
network to connect to a private Ed-Fi API without any public internet access.

**CEDS Data Warehouse integration** uses an air-gapped workflow. The tool generates a SQL script that users review and
run in their own SQL Server environment (e.g., SSMS). The script produces aggregate metadata — record counts, ranges,
and distributions — not individual records. Users review the CSV output for any concerns before importing it into the
tool. Once imported, the data is processed in the browser and is not transmitted anywhere.

See [/docs/technical-overview.md](./docs/technical-overview.md) for a detailed description of the architecture and data
handling practices, suitable for IT security review.

## Quick Start

Prerequisites:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS recommended)

```bash
# Install root-level dev dependencies (Prettier, markdownlint)
npm install

# Install UI CSS build dependencies (Tailwind CSS, DaisyUI)
cd EwFrameworkAnalysis.UI
npm install
cd ..

# Build and run the application
./eng/build-solution.ps1
```

> Full setup and contribution workflow details are in [CONTRIBUTING.md](./CONTRIBUTING.md).

## Documentation

Technical and development-focused documentation can be found in this repository under [/docs/](./docs/).

| Document                                                        | Description                                                                                                                    |
| --------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| [Architecture and Security Brief](./docs/technical-overview.md) | Technical overview of the application architecture, data flows, and security considerations — intended for IT security review. |
| [Code Standards](./docs/code-standards.md)                      | Coding conventions and standards for contributors.                                                                             |
| [Data Assessor Design](./docs/data-assessor-design.md)          | Design and implementation patterns for data assessors.                                                                         |
| [Deployment and CI/CD](./docs/deployment.md)                    | CI/CD pipeline and deployment configuration.                                                                                   |

## UI Utilities

The repository includes a separate utilities project used to maintain the Framework reference data — the data element
dictionary, indicator mappings, essential question definitions, and disaggregates — that the main application is built
on top of.

The Framework Manager (`/framework-manager`) is a browser-based tool that allows maintainers to:

- Browse and edit data elements, including names, categories, sectors, and scoring rule assignments
- Manage the mappings between data elements and their associated Indicators and Disaggregates
- Review and update Essential Question to Indicator relationships
- Run diagnostics to surface orphaned elements, broken references, and empty mappings
- Track review status and notes for each item as mappings are verified or updated
- Export finalized mappings as ready-to-compile C# source, which is then checked in and compiled into the main
  application

Working state and review notes are persisted in browser `localStorage` during an editing session. Project data merging
is handled natively through the main application's Import function.

## Contributing

We welcome contributions! Please review our [CONTRIBUTING.md](./CONTRIBUTING.md) guide for our workflow, branch naming,
and pull request practices.

## License

This project will be released under an open-source license (to be finalized). All contributions are expected to follow
the applicable licensing terms once published.
