-- Bootstrap Ed-Fi Admin database with default API client
USE EdFi_Admin;
GO

PRINT 'Bootstrapping Admin database...';

DECLARE @VendorId INT;
DECLARE @ApplicationId INT;

-- Create Vendor
INSERT INTO dbo.Vendors (VendorName) VALUES ('Test Vendor');
SELECT @VendorId = VendorId FROM dbo.Vendors WHERE VendorName = 'Test Vendor';

-- Create Vendor Namespace Prefix
INSERT INTO dbo.VendorNamespacePrefixes (NamespacePrefix, Vendor_VendorId)
VALUES ('uri://ed-fi.org', @VendorId);

-- Create Application
INSERT INTO dbo.Applications (ApplicationName, OperationalContextUri, Vendor_VendorId, ClaimSetName)
VALUES ('Test Application', 'uri://ed-fi.org', @VendorId, 'Ed-Fi Sandbox');

SELECT @ApplicationId = ApplicationId FROM dbo.Applications 
WHERE ApplicationName = 'Test Application' AND Vendor_VendorId = @VendorId;

-- Create Education Organization
INSERT INTO dbo.ApplicationEducationOrganizations (EducationOrganizationId, Application_ApplicationId)
VALUES (255901001, @ApplicationId);

-- Create API Client
INSERT INTO dbo.ApiClients ([Key], [Secret], [Name], IsApproved, UseSandbox, SandboxType, SecretIsHashed, Application_ApplicationId)
VALUES ('RvcohKz9zHI4', 'E1676E88-4D3B-4E4E-B7B7-7C3F8E5D2A9C', 'Default API Client', 1, 0, 0, 0, @ApplicationId);

-- Link API Client to Education Organizations
INSERT INTO dbo.ApiClientApplicationEducationOrganizations (ApiClient_ApiClientId, ApplicationEducationOrganization_ApplicationEducationOrganizationId)
SELECT ac.ApiClientId, aeo.ApplicationEducationOrganizationId
FROM dbo.ApiClients ac
CROSS JOIN dbo.ApplicationEducationOrganizations aeo
INNER JOIN dbo.Applications a ON aeo.Application_ApplicationId = a.ApplicationId
WHERE ac.[Key] = 'RvcohKz9zHI4' AND a.ApplicationName = 'Test Application';

PRINT 'Bootstrap completed';
GO
