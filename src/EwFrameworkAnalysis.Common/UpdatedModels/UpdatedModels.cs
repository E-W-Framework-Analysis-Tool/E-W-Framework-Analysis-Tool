namespace EwFrameworkAnalysis.Common.UpdatedModels;

public class AnalysisProject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Title { get; set; } // optional way to "name" an analysis session
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    public List<DataSource> DataSources { get; set; } = [];
}

// Represents a record of where data is stored, e.g. a custom database, an Ed-Fi ODS API instance, a CEDS DW...
// Need some way to "type" this, either enum within or inheritance (leaning toward enum)
public class DataSource
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DataSourceType DataSourceType { get; set; }

    public List<DataSourceAssessment> Assessments { get; set; } = [];
}

public enum DataSourceType
{
    Custom, // Checklist
    Automated,
    EdFiOdsApi_V73_DS52, // Should versions be locked in to enum?? Probably easiest to do so for now
    CedsDw_v12
}

public class DataSourceAssessment
{
    public string? Name { get; set; }
    public DateTime ConductedAt { get; set; }

    // Data Source data characteristics (e.g. overall population, etc.)
    public List<DataElementAssessment> DataElements { get; set; } = [];
}

public class DataElementAssessment
{
    public required string DataElementName { get; init; }
    public string? Remarks { get; set; }
    public bool? AvailabilityUserOverride { get; set; } // if set, ignore normal processing and take user specified value

    public List<DataCharacteristic> Characteristics { get; set; } = [];

}

public class DataCharacteristic
{
    public required string CharacteristicType { get; set; } // What would this be? Should these be factors into scores? Should they reference another class (not analysis instance specific, but more like a reference)? Checks may look for specific characteristics against specific elements, not all elements will have the same characteristics, some may need to store these in different capacities
    public required object Value { get; set; }
    public string? Remarks { get; set; } // either manually entered or notes from automatic processing
}

public interface IDataSourceAssessor
{
    DataSourceType DataSourceType { get; }

    // implementation will probably have its own sort of "configuration", impl specific

    // run assessment at different "profile levels" (detailed or quick)
    // run assessment for one, many, or all possible checks
    // check results for each can be completed, unable to complete due to error, unsupported
    // default is "unsupported"?
}

public class EdFiApiAssessor
{
    // "configuration" is API base URL, client Id, client secret
}

public class CedsDWAssessor
{
    // "configuration" is SQL query results
}
