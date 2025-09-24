-- Simple ServiceRequest Tables Creation Script
-- HVAC Management System

-- =============================================
-- Create ServiceType Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ServiceType' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[ServiceType](
        [ServiceTypeID] [int] IDENTITY(1,1) NOT NULL,
        [ServiceTypeName] [nvarchar](100) NOT NULL,
        [ServiceTypeDescription] [nvarchar](500) NULL,
        [IsActive] [bit] NOT NULL DEFAULT(1),
        [CreatedBy] [int] NULL,
        [CreatedDate] [datetime] NULL DEFAULT(GETDATE()),
        [UpdatedBy] [int] NULL,
        [UpdatedDate] [datetime] NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT(0),
        [DeletedBy] [int] NULL,
        [DeletedDate] [datetime] NULL,
        CONSTRAINT [PK_ServiceType] PRIMARY KEY CLUSTERED ([ServiceTypeID] ASC)
    )
END
GO

-- =============================================
-- Create ServiceStatus Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ServiceStatus' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[ServiceStatus](
        [ServiceStatusID] [int] IDENTITY(1,1) NOT NULL,
        [ServiceStatusName] [nvarchar](100) NOT NULL,
        [ServiceStatusDescription] [nvarchar](500) NULL,
        [StatusOrder] [int] NOT NULL DEFAULT(1),
        [StatusColor] [nvarchar](20) NULL,
        [IsActive] [bit] NOT NULL DEFAULT(1),
        [IsClosed] [bit] NOT NULL DEFAULT(0),
        [CreatedBy] [int] NULL,
        [CreatedDate] [datetime] NULL DEFAULT(GETDATE()),
        [UpdatedBy] [int] NULL,
        [UpdatedDate] [datetime] NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT(0),
        [DeletedBy] [int] NULL,
        [DeletedDate] [datetime] NULL,
        CONSTRAINT [PK_ServiceStatus] PRIMARY KEY CLUSTERED ([ServiceStatusID] ASC)
    )
END
GO

-- =============================================
-- Create ServiceRequest Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ServiceRequest' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[ServiceRequest](
        [ServiceRequestID] [int] IDENTITY(1,1) NOT NULL,
        [ServiceRequestNo] [nvarchar](50) NOT NULL,
        [CustomerID] [int] NOT NULL,
        [EquipmentID] [int] NOT NULL,
        [PriorityID] [int] NOT NULL,
        [ServiceStatusID] [int] NOT NULL,
        [ServiceTypeID] [int] NOT NULL,
        [ServiceDescription] [nvarchar](1000) NOT NULL,
        [CreationDate] [datetime] NOT NULL DEFAULT(GETDATE()),
        [CreatedBy] [int] NOT NULL,
        [ScheduledDate] [datetime] NULL,
        [CompletedDate] [datetime] NULL,
        [AssignedTo] [nvarchar](100) NULL,
        [ResolutionNotes] [nvarchar](2000) NULL,
        [EstimatedCost] [decimal](18, 2) NULL,
        [ActualCost] [decimal](18, 2) NULL,
        [ContactPerson] [nvarchar](100) NULL,
        [ContactPhone] [nvarchar](20) NULL,
        [ContactEmail] [nvarchar](100) NULL,
        [Location] [nvarchar](200) NULL,
        [Remarks] [nvarchar](1000) NULL,
        [UpdatedBy] [int] NULL,
        [UpdatedDate] [datetime] NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT(0),
        [DeletedBy] [int] NULL,
        [DeletedDate] [datetime] NULL,
        [BranchID] [int] NULL,
        [AcFinancialYearID] [int] NULL,
        CONSTRAINT [PK_ServiceRequest] PRIMARY KEY CLUSTERED ([ServiceRequestID] ASC)
    )
END
GO

-- =============================================
-- Insert Initial Data
-- =============================================

-- ServiceType data
IF NOT EXISTS (SELECT 1 FROM ServiceType WHERE ServiceTypeName = 'Service')
BEGIN
    INSERT INTO [dbo].[ServiceType] ([ServiceTypeName], [ServiceTypeDescription], [IsActive], [CreatedBy], [CreatedDate])
    VALUES ('Service', 'Regular maintenance and repair services', 1, 1, GETDATE())
END
GO

IF NOT EXISTS (SELECT 1 FROM ServiceType WHERE ServiceTypeName = 'AMC')
BEGIN
    INSERT INTO [dbo].[ServiceType] ([ServiceTypeName], [ServiceTypeDescription], [IsActive], [CreatedBy], [CreatedDate])
    VALUES ('AMC', 'Annual Maintenance Contract services', 1, 1, GETDATE())
END
GO

-- ServiceStatus data
IF NOT EXISTS (SELECT 1 FROM ServiceStatus WHERE ServiceStatusName = 'Pending')
BEGIN
    INSERT INTO [dbo].[ServiceStatus] ([ServiceStatusName], [ServiceStatusDescription], [StatusOrder], [StatusColor], [IsActive], [IsClosed], [CreatedBy], [CreatedDate])
    VALUES ('Pending', 'Service request is pending assignment', 1, '#ffc107', 1, 0, 1, GETDATE())
END
GO

IF NOT EXISTS (SELECT 1 FROM ServiceStatus WHERE ServiceStatusName = 'Work In Progress')
BEGIN
    INSERT INTO [dbo].[ServiceStatus] ([ServiceStatusName], [ServiceStatusDescription], [StatusOrder], [StatusColor], [IsActive], [IsClosed], [CreatedBy], [CreatedDate])
    VALUES ('Work In Progress', 'Service work is currently in progress', 2, '#007bff', 1, 0, 1, GETDATE())
END
GO

IF NOT EXISTS (SELECT 1 FROM ServiceStatus WHERE ServiceStatusName = 'Waiting for parts')
BEGIN
    INSERT INTO [dbo].[ServiceStatus] ([ServiceStatusName], [ServiceStatusDescription], [StatusOrder], [StatusColor], [IsActive], [IsClosed], [CreatedBy], [CreatedDate])
    VALUES ('Waiting for parts', 'Service is waiting for required parts', 3, '#17a2b8', 1, 0, 1, GETDATE())
END
GO

IF NOT EXISTS (SELECT 1 FROM ServiceStatus WHERE ServiceStatusName = 'Closed')
BEGIN
    INSERT INTO [dbo].[ServiceStatus] ([ServiceStatusName], [ServiceStatusDescription], [StatusOrder], [StatusColor], [IsActive], [IsClosed], [CreatedBy], [CreatedDate])
    VALUES ('Closed', 'Service request has been completed and closed', 4, '#28a745', 1, 1, 1, GETDATE())
END
GO

PRINT 'ServiceRequest tables and data created successfully!'
