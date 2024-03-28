-- Creating logins for each user
CREATE LOGIN AppUserLog WITH PASSWORD = 'AppUserWIUT'
CREATE LOGIN DataImporterLog WITH PASSWORD = 'DataImportWIUT'
CREATE LOGIN DataExporterLog WITH PASSWORD = 'DataExportWIUT'

USE Belissimo
GO

-- Creating database users
CREATE USER AppUser FOR LOGIN AppUserLog
CREATE USER DataImporter FOR LOGIN DataImporterLog
CREATE USER DataExporter FOR LOGIN DataExporterLog
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Employees TO AppUser

GRANT INSERT ON dbo.Employees TO DataImporter

GRANT SELECT ON dbo.Employees TO DataExporter
