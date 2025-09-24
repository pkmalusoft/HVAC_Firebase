# HVAC Management System - Database Setup

## Table of Contents
1. [Database Overview](#database-overview)
2. [Prerequisites](#prerequisites)
3. [Database Installation](#database-installation)
4. [Schema Creation](#schema-creation)
5. [Initial Data Population](#initial-data-population)
6. [Security Configuration](#security-configuration)
7. [Performance Optimization](#performance-optimization)
8. [Backup and Recovery](#backup-and-recovery)
9. [Maintenance Procedures](#maintenance-procedures)
10. [Troubleshooting](#troubleshooting)

## Database Overview

The HVAC Management System uses Microsoft SQL Server as its primary database, storing all business data including enquiries, quotations, purchase orders, inventory, and financial transactions.

### Database Characteristics
- **Database Name**: HVAC_DB
- **Compatibility Level**: SQL Server 2019 (140)
- **Recovery Model**: Simple
- **Collation**: SQL_Latin1_General_CP1_CI_AS
- **Initial Size**: 1 GB
- **Growth Rate**: 10% per month

## Prerequisites

### Software Requirements
- Microsoft SQL Server 2019 or later
- SQL Server Management Studio (SSMS) 18.x or later
- Windows Server 2016 or later
- .NET Framework 4.8

### Hardware Requirements
- **CPU**: 4 cores minimum, 8 cores recommended
- **RAM**: 8 GB minimum, 16 GB recommended
- **Storage**: 100 GB free space minimum
- **Network**: 1 Gbps connection

### Permissions Required
- SQL Server System Administrator
- Windows Administrator
- Database Creator role

## Database Installation

### 1. Create Database

```sql
-- Create the main database
CREATE DATABASE [HVAC_DB]
ON 
( NAME = 'HVAC_DB',
  FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\HVAC_DB.mdf',
  SIZE = 100MB,
  MAXSIZE = 10GB,
  FILEGROWTH = 10MB )
LOG ON 
( NAME = 'HVAC_DB_Log',
  FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\HVAC_DB_Log.ldf',
  SIZE = 10MB,
  MAXSIZE = 1GB,
  FILEGROWTH = 10% );

-- Set database options
ALTER DATABASE [HVAC_DB] SET COMPATIBILITY_LEVEL = 140;
ALTER DATABASE [HVAC_DB] SET RECOVERY SIMPLE;
ALTER DATABASE [HVAC_DB] SET AUTO_SHRINK OFF;
ALTER DATABASE [HVAC_DB] SET AUTO_CREATE_STATISTICS ON;
ALTER DATABASE [HVAC_DB] SET AUTO_UPDATE_STATISTICS ON;
```

### 2. Configure Database Settings

```sql
-- Enable full-text search
USE [HVAC_DB];
EXEC sp_fulltext_database 'enable';

-- Configure database options
ALTER DATABASE [HVAC_DB] SET PAGE_VERIFY CHECKSUM;
ALTER DATABASE [HVAC_DB] SET ALLOW_SNAPSHOT_ISOLATION ON;
ALTER DATABASE [HVAC_DB] SET READ_COMMITTED_SNAPSHOT ON;
```

## Schema Creation

### 1. Core Tables

```sql
-- Company Master Table
CREATE TABLE [dbo].[AcCompany] (
    [AcCompanyID] INT IDENTITY(1,1) NOT NULL,
    [AcCompany1] NVARCHAR(100) NOT NULL,
    [Address1] NVARCHAR(200) NULL,
    [Address2] NVARCHAR(200) NULL,
    [Address3] NVARCHAR(200) NULL,
    [Phone] NVARCHAR(20) NULL,
    [EMail] NVARCHAR(100) NULL,
    [KeyPerson] NVARCHAR(100) NULL,
    [AcceptSystem] BIT NULL DEFAULT(0),
    [CountryName] NVARCHAR(100) NULL,
    [CityName] NVARCHAR(100) NULL,
    [LocationName] NVARCHAR(100) NULL,
    [CreatedDate] DATETIME NULL DEFAULT(GETDATE()),
    [ModifiedDate] DATETIME NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_AcCompany] PRIMARY KEY CLUSTERED ([AcCompanyID] ASC)
);

-- Branch Master Table
CREATE TABLE [dbo].[BranchMaster] (
    [BranchID] INT IDENTITY(1,1) NOT NULL,
    [AcCompanyID] INT NOT NULL,
    [BranchName] NVARCHAR(100) NOT NULL,
    [Address] NVARCHAR(200) NULL,
    [Phone] NVARCHAR(20) NULL,
    [Email] NVARCHAR(100) NULL,
    [IsActive] BIT NULL DEFAULT(1),
    [CreatedDate] DATETIME NULL DEFAULT(GETDATE()),
    [ModifiedDate] DATETIME NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_BranchMaster] PRIMARY KEY CLUSTERED ([BranchID] ASC),
    CONSTRAINT [FK_BranchMaster_AcCompany] FOREIGN KEY ([AcCompanyID]) REFERENCES [dbo].[AcCompany] ([AcCompanyID])
);

-- User Registration Table
CREATE TABLE [dbo].[UserRegistration] (
    [UserID] INT IDENTITY(1,1) NOT NULL,
    [AcCompanyID] INT NOT NULL,
    [UserName] NVARCHAR(50) NOT NULL,
    [Password] NVARCHAR(255) NOT NULL,
    [FirstName] NVARCHAR(50) NULL,
    [LastName] NVARCHAR(50) NULL,
    [Email] NVARCHAR(100) NULL,
    [Phone] NVARCHAR(20) NULL,
    [RoleID] INT NOT NULL,
    [IsActive] BIT NULL DEFAULT(1),
    [LastLogin] DATETIME NULL,
    [CreatedDate] DATETIME NULL DEFAULT(GETDATE()),
    [ModifiedDate] DATETIME NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_UserRegistration] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_UserRegistration_AcCompany] FOREIGN KEY ([AcCompanyID]) REFERENCES [dbo].[AcCompany] ([AcCompanyID]),
    CONSTRAINT [UQ_UserRegistration_UserName] UNIQUE ([UserName])
);
```

### 2. Business Tables

```sql
-- Customer Master Table
CREATE TABLE [dbo].[CustomerMaster] (
    [CustomerID] INT IDENTITY(1,1) NOT NULL,
    [CustomerName] NVARCHAR(100) NOT NULL,
    [ContactPerson] NVARCHAR(100) NULL,
    [Address] NVARCHAR(200) NULL,
    [City] NVARCHAR(50) NULL,
    [State] NVARCHAR(50) NULL,
    [Country] NVARCHAR(50) NULL,
    [Phone] NVARCHAR(20) NULL,
    [Email] NVARCHAR(100) NULL,
    [GSTNumber] NVARCHAR(20) NULL,
    [IsActive] BIT NULL DEFAULT(1),
    [CreatedDate] DATETIME NULL DEFAULT(GETDATE()),
    [ModifiedDate] DATETIME NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_CustomerMaster] PRIMARY KEY CLUSTERED ([CustomerID] ASC)
);

-- Enquiry Table
CREATE TABLE [dbo].[Enquiry] (
    [EnquiryID] INT IDENTITY(1,1) NOT NULL,
    [BranchID] INT NOT NULL,
    [CustomerID] INT NOT NULL,
    [EnquiryNo] NVARCHAR(50) NOT NULL,
    [EnquiryDate] DATETIME NOT NULL,
    [ProjectName] NVARCHAR(200) NOT NULL,
    [ProjectDescription] NVARCHAR(MAX) NULL,
    [ProjectNumber] NVARCHAR(50) NULL,
    [CityID] INT NULL,
    [CountryID] INT NULL,
    [DueDate] DATETIME NULL,
    [PriorityID] INT NULL,
    [BuildingTypeID] INT NULL,
    [EnquiryTypeID] INT NULL,
    [StatusID] INT NULL,
    [CreatedBy] INT NULL,
    [CreatedDate] DATETIME NULL DEFAULT(GETDATE()),
    [ModifiedBy] INT NULL,
    [ModifiedDate] DATETIME NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_Enquiry] PRIMARY KEY CLUSTERED ([EnquiryID] ASC),
    CONSTRAINT [FK_Enquiry_BranchMaster] FOREIGN KEY ([BranchID]) REFERENCES [dbo].[BranchMaster] ([BranchID]),
    CONSTRAINT [FK_Enquiry_CustomerMaster] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[CustomerMaster] ([CustomerID])
);

-- Quotation Table
CREATE TABLE [dbo].[Quotation] (
    [QuotationID] INT IDENTITY(1,1) NOT NULL,
    [EnquiryID] INT NOT NULL,
    [BranchID] INT NOT NULL,
    [QuotationNo] NVARCHAR(50) NOT NULL,
    [QuotationDate] DATETIME NOT NULL,
    [QuotationValue] DECIMAL(18,2) NULL,
    [DiscountAmount] DECIMAL(18,2) NULL,
    [DiscountPercent] DECIMAL(5,2) NULL,
    [VATPercent] DECIMAL(5,2) NULL,
    [VATAmount] DECIMAL(18,2) NULL,
    [NetAmount] DECIMAL(18,2) NULL,
    [QuotationStatusID] INT NULL,
    [Validity] INT NULL,
    [DeliveryTerms] NVARCHAR(MAX) NULL,
    [PaymentTerms] NVARCHAR(MAX) NULL,
    [TermsandConditions] NVARCHAR(MAX) NULL,
    [CreatedBy] INT NULL,
    [CreatedDate] DATETIME NULL DEFAULT(GETDATE()),
    [ModifiedBy] INT NULL,
    [ModifiedDate] DATETIME NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_Quotation] PRIMARY KEY CLUSTERED ([QuotationID] ASC),
    CONSTRAINT [FK_Quotation_Enquiry] FOREIGN KEY ([EnquiryID]) REFERENCES [dbo].[Enquiry] ([EnquiryID]),
    CONSTRAINT [FK_Quotation_BranchMaster] FOREIGN KEY ([BranchID]) REFERENCES [dbo].[BranchMaster] ([BranchID])
);
```

### 3. Indexes Creation

```sql
-- Create indexes for performance
CREATE NONCLUSTERED INDEX [IX_Enquiry_EnquiryNo] ON [dbo].[Enquiry] ([EnquiryNo]);
CREATE NONCLUSTERED INDEX [IX_Enquiry_CustomerID] ON [dbo].[Enquiry] ([CustomerID]);
CREATE NONCLUSTERED INDEX [IX_Enquiry_EnquiryDate] ON [dbo].[Enquiry] ([EnquiryDate]);
CREATE NONCLUSTERED INDEX [IX_Quotation_QuotationNo] ON [dbo].[Quotation] ([QuotationNo]);
CREATE NONCLUSTERED INDEX [IX_Quotation_QuotationDate] ON [dbo].[Quotation] ([QuotationDate]);
CREATE NONCLUSTERED INDEX [IX_UserRegistration_UserName] ON [dbo].[UserRegistration] ([UserName]);
CREATE NONCLUSTERED INDEX [IX_CustomerMaster_CustomerName] ON [dbo].[CustomerMaster] ([CustomerName]);
```

## Initial Data Population

### 1. Master Data

```sql
-- Insert default company
INSERT INTO [dbo].[AcCompany] ([AcCompany1], [Address1], [Phone], [EMail], [KeyPerson], [AcceptSystem])
VALUES ('HVAC Management Company', '123 Business Street', '555-0123', 'info@hvac.com', 'John Smith', 1);

-- Insert default branch
INSERT INTO [dbo].[BranchMaster] ([AcCompanyID], [BranchName], [Address], [Phone], [Email])
VALUES (1, 'Main Branch', '123 Business Street', '555-0123', 'main@hvac.com');

-- Insert default roles
INSERT INTO [dbo].[RoleMaster] ([RoleName], [RoleDescription], [IsActive])
VALUES 
    ('Administrator', 'Full system access', 1),
    ('Manager', 'Management level access', 1),
    ('Sales Executive', 'Sales and enquiry management', 1),
    ('Purchase Manager', 'Purchase order management', 1),
    ('Project Manager', 'Project management', 1),
    ('Accountant', 'Financial management', 1);

-- Insert default user
INSERT INTO [dbo].[UserRegistration] ([AcCompanyID], [UserName], [Password], [FirstName], [LastName], [Email], [RoleID])
VALUES (1, 'admin', 'hashed_password_here', 'Admin', 'User', 'admin@hvac.com', 1);
```

### 2. Lookup Data

```sql
-- Insert enquiry statuses
INSERT INTO [dbo].[EnquiryStatus] ([StatusName], [IsActive])
VALUES 
    ('New', 1),
    ('In Progress', 1),
    ('Quoted', 1),
    ('Approved', 1),
    ('Rejected', 1),
    ('On Hold', 1);

-- Insert quotation statuses
INSERT INTO [dbo].[QuotationStatus] ([Status], [IsActive])
VALUES 
    ('Draft', 1),
    ('Sent', 1),
    ('Approved', 1),
    ('Rejected', 1),
    ('Expired', 1);

-- Insert priority levels
INSERT INTO [dbo].[PriorityMaster] ([PriorityName], [IsActive])
VALUES 
    ('Low', 1),
    ('Medium', 1),
    ('High', 1),
    ('Urgent', 1);
```

## Security Configuration

### 1. Create Application User

```sql
-- Create login for application
CREATE LOGIN [HVAC_App] WITH PASSWORD = 'SecurePassword123!';

-- Create database user
USE [HVAC_DB];
CREATE USER [HVAC_App] FOR LOGIN [HVAC_App];

-- Grant necessary permissions
ALTER ROLE [db_datareader] ADD MEMBER [HVAC_App];
ALTER ROLE [db_datawriter] ADD MEMBER [HVAC_App];
ALTER ROLE [db_ddladmin] ADD MEMBER [HVAC_App];

-- Grant specific permissions
GRANT EXECUTE ON SCHEMA::dbo TO [HVAC_App];
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [HVAC_App];
```

### 2. Row-Level Security

```sql
-- Create security policy for branch-based access
CREATE SCHEMA [Security];
GO

CREATE FUNCTION [Security].[fn_securitypredicate](@BranchID INT)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN SELECT 1 AS fn_securitypredicate_result
WHERE @BranchID = CONVERT(INT, SESSION_CONTEXT(N'BranchID'));
GO

CREATE SECURITY POLICY [Security].[BranchAccessPolicy]
ADD FILTER PREDICATE [Security].[fn_securitypredicate]([BranchID]) ON [dbo].[Enquiry],
ADD FILTER PREDICATE [Security].[fn_securitypredicate]([BranchID]) ON [dbo].[Quotation];
GO
```

## Performance Optimization

### 1. Database Maintenance Plan

```sql
-- Create maintenance plan
USE [msdb];
GO

EXEC dbo.sp_add_maintenance_plan
    @plan_name = N'HVAC_DB_Maintenance',
    @description = N'Maintenance plan for HVAC database',
    @schedule_name = N'HVAC_DB_Maintenance_Schedule';
GO

-- Add maintenance tasks
EXEC dbo.sp_add_maintenance_plan_task
    @plan_name = N'HVAC_DB_Maintenance',
    @task_name = N'HVAC_DB_Backup',
    @subplan_name = N'Backup',
    @command = N'BACKUP DATABASE [HVAC_DB] TO DISK = ''C:\Backup\HVAC_DB.bak''';
GO
```

### 2. Statistics Update

```sql
-- Update statistics
USE [HVAC_DB];
GO

EXEC sp_updatestats;
GO

-- Create statistics for frequently queried columns
CREATE STATISTICS [stat_Enquiry_EnquiryDate] ON [dbo].[Enquiry] ([EnquiryDate]);
CREATE STATISTICS [stat_Quotation_QuotationDate] ON [dbo].[Quotation] ([QuotationDate]);
GO
```

## Backup and Recovery

### 1. Full Backup

```sql
-- Create full backup
BACKUP DATABASE [HVAC_DB] 
TO DISK = 'C:\Backup\HVAC_DB_Full.bak'
WITH FORMAT, INIT, COMPRESSION;
GO
```

### 2. Differential Backup

```sql
-- Create differential backup
BACKUP DATABASE [HVAC_DB] 
TO DISK = 'C:\Backup\HVAC_DB_Diff.bak'
WITH DIFFERENTIAL, COMPRESSION;
GO
```

### 3. Transaction Log Backup

```sql
-- Create transaction log backup
BACKUP LOG [HVAC_DB] 
TO DISK = 'C:\Backup\HVAC_DB_Log.trn'
WITH COMPRESSION;
GO
```

### 4. Restore Procedures

```sql
-- Restore from full backup
RESTORE DATABASE [HVAC_DB] 
FROM DISK = 'C:\Backup\HVAC_DB_Full.bak'
WITH REPLACE, RECOVERY;
GO

-- Restore with point-in-time recovery
RESTORE DATABASE [HVAC_DB] 
FROM DISK = 'C:\Backup\HVAC_DB_Full.bak'
WITH NORECOVERY;
GO

RESTORE LOG [HVAC_DB] 
FROM DISK = 'C:\Backup\HVAC_DB_Log.trn'
WITH STOPAT = '2024-09-24 10:30:00', RECOVERY;
GO
```

## Maintenance Procedures

### 1. Daily Maintenance

```sql
-- Daily maintenance script
USE [HVAC_DB];
GO

-- Update statistics
EXEC sp_updatestats;

-- Check database integrity
DBCC CHECKDB([HVAC_DB]) WITH NO_INFOMSGS;

-- Clean up old log entries
DELETE FROM [dbo].[AuditLog] 
WHERE [CreatedDate] < DATEADD(day, -90, GETDATE());
GO
```

### 2. Weekly Maintenance

```sql
-- Weekly maintenance script
USE [HVAC_DB];
GO

-- Rebuild indexes
ALTER INDEX ALL ON [dbo].[Enquiry] REBUILD;
ALTER INDEX ALL ON [dbo].[Quotation] REBUILD;

-- Update statistics
EXEC sp_updatestats;

-- Check for unused indexes
SELECT 
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s ON i.object_id = s.object_id AND i.index_id = s.index_id
WHERE i.name IS NOT NULL
ORDER BY s.user_seeks + s.user_scans + s.user_lookups;
GO
```

### 3. Monthly Maintenance

```sql
-- Monthly maintenance script
USE [HVAC_DB];
GO

-- Archive old data
-- Move old enquiries to archive table
INSERT INTO [dbo].[EnquiryArchive]
SELECT * FROM [dbo].[Enquiry] 
WHERE [EnquiryDate] < DATEADD(year, -2, GETDATE());

DELETE FROM [dbo].[Enquiry] 
WHERE [EnquiryDate] < DATEADD(year, -2, GETDATE());

-- Update database statistics
EXEC sp_updatestats;

-- Check database growth
SELECT 
    name,
    size * 8 / 1024 AS SizeMB,
    max_size * 8 / 1024 AS MaxSizeMB
FROM sys.database_files;
GO
```

## Troubleshooting

### Common Database Issues

#### 1. Connection Issues
```sql
-- Check SQL Server services
SELECT name, status FROM sys.dm_server_services;

-- Check database status
SELECT name, state_desc FROM sys.databases WHERE name = 'HVAC_DB';

-- Test connection
SELECT @@VERSION, DB_NAME(), GETDATE();
```

#### 2. Performance Issues
```sql
-- Check blocking processes
SELECT 
    session_id,
    blocking_session_id,
    wait_type,
    wait_time,
    wait_resource
FROM sys.dm_exec_requests
WHERE blocking_session_id <> 0;

-- Check index usage
SELECT 
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s ON i.object_id = s.object_id AND i.index_id = s.index_id
WHERE i.name IS NOT NULL;
```

#### 3. Space Issues
```sql
-- Check database size
SELECT 
    name,
    size * 8 / 1024 AS SizeMB,
    max_size * 8 / 1024 AS MaxSizeMB,
    growth * 8 / 1024 AS GrowthMB
FROM sys.database_files;

-- Check table sizes
SELECT 
    t.name AS TableName,
    s.name AS SchemaName,
    p.rows AS RowCounts,
    SUM(a.total_pages) * 8 AS TotalSpaceKB,
    SUM(a.used_pages) * 8 AS UsedSpaceKB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
LEFT OUTER JOIN sys.schemas s ON t.schema_id = s.schema_id
GROUP BY t.name, s.name, p.rows
ORDER BY TotalSpaceKB DESC;
```

### Database Health Check Script

```sql
-- Comprehensive database health check
USE [HVAC_DB];
GO

-- Check database integrity
DBCC CHECKDB([HVAC_DB]) WITH NO_INFOMSGS;

-- Check index fragmentation
SELECT 
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.avg_fragmentation_in_percent,
    ips.page_count
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'DETAILED') ips
INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 10;

-- Check missing indexes
SELECT 
    migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) AS improvement_measure,
    'CREATE INDEX [IX_' + OBJECT_NAME(migs.object_id) + '_' + ISNULL(mid.equality_columns, '') + '] ON ' + mid.statement + ' (' + ISNULL(mid.equality_columns, '') + ')' AS create_index_statement
FROM sys.dm_db_missing_index_groups mig
INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
WHERE migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) > 10
ORDER BY improvement_measure DESC;
GO
```

---

*This database setup guide provides comprehensive instructions for installing, configuring, and maintaining the HVAC Management System database.*
