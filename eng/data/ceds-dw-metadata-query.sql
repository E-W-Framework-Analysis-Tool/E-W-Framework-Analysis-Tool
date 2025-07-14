-- Query to extract CEDS Data Warehouse schema information with extended properties
SELECT 
    s.name AS schema_name,
    t.name AS table_name,
    c.name AS column_name,
    ty.name AS column_data_type,
    CASE 
        WHEN ty.name IN ('varchar', 'nvarchar', 'char', 'nchar') 
        THEN ty.name + '(' + CASE WHEN c.max_length = -1 THEN 'MAX' ELSE CAST(c.max_length AS VARCHAR(10)) END + ')'
        WHEN ty.name IN ('decimal', 'numeric') 
        THEN ty.name + '(' + CAST(c.precision AS VARCHAR(10)) + ',' + CAST(c.scale AS VARCHAR(10)) + ')'
        ELSE ty.name
    END AS full_data_type,
    ep_global.value AS CEDS_GlobalId,
    ep_element.value AS CEDS_Element
FROM 
    sys.schemas s
    INNER JOIN sys.tables t ON s.schema_id = t.schema_id
    INNER JOIN sys.columns c ON t.object_id = c.object_id
    INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
    LEFT JOIN sys.extended_properties ep_global ON 
        ep_global.major_id = c.object_id 
        AND ep_global.minor_id = c.column_id 
        AND ep_global.name = 'CEDS_GlobalId'
    LEFT JOIN sys.extended_properties ep_element ON 
        ep_element.major_id = c.object_id 
        AND ep_element.minor_id = c.column_id 
        AND ep_element.name = 'CEDS_Element'
WHERE 
    s.name = 'RDS'  -- Focus on RDS schema
    AND t.type = 'U'  -- Only user tables (not views, etc.)
ORDER BY 
    s.name,
    t.name,
    c.column_id;
