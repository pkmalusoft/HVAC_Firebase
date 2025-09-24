-- ServiceRequest Integration SQL Scripts
-- HVAC Management System
-- Created: 2024

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
-- Update Priority Table (Add new columns if not exist)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'PriorityDescription')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [PriorityDescription] [nvarchar](500) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'PriorityLevel')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [PriorityLevel] [int] NOT NULL DEFAULT(1)
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'PriorityColor')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [PriorityColor] [nvarchar](20) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [IsActive] [bit] NOT NULL DEFAULT(1)
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [CreatedBy] [int] NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'CreatedDate')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [CreatedDate] [datetime] NULL DEFAULT(GETDATE())
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [UpdatedBy] [int] NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'UpdatedDate')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [UpdatedDate] [datetime] NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'IsDeleted')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [IsDeleted] [bit] NOT NULL DEFAULT(0)
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'DeletedBy')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [DeletedBy] [int] NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Priority]') AND name = 'DeletedDate')
BEGIN
    ALTER TABLE [dbo].[Priority] ADD [DeletedDate] [datetime] NULL
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
-- Create Foreign Key Constraints
-- =============================================

-- ServiceRequest -> CustomerMaster
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_CustomerMaster]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceRequest]'))
BEGIN
    ALTER TABLE [dbo].[ServiceRequest] 
    ADD CONSTRAINT [FK_ServiceRequest_CustomerMaster] 
    FOREIGN KEY([CustomerID]) REFERENCES [dbo].[CustomerMaster] ([CustomerID])
END
GO

-- ServiceRequest -> Equipment
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_Equipment]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceRequest]'))
BEGIN
    ALTER TABLE [dbo].[ServiceRequest] 
    ADD CONSTRAINT [FK_ServiceRequest_Equipment] 
    FOREIGN KEY([EquipmentID]) REFERENCES [dbo].[Equipment] ([ID])
END
GO

-- ServiceRequest -> Priority
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_Priority]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceRequest]'))
BEGIN
    ALTER TABLE [dbo].[ServiceRequest] 
    ADD CONSTRAINT [FK_ServiceRequest_Priority] 
    FOREIGN KEY([PriorityID]) REFERENCES [dbo].[Priority] ([PriorityID])
END
GO

-- ServiceRequest -> ServiceStatus
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_ServiceStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceRequest]'))
BEGIN
    ALTER TABLE [dbo].[ServiceRequest] 
    ADD CONSTRAINT [FK_ServiceRequest_ServiceStatus] 
    FOREIGN KEY([ServiceStatusID]) REFERENCES [dbo].[ServiceStatus] ([ServiceStatusID])
END
GO

-- ServiceRequest -> ServiceType
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_ServiceType]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceRequest]'))
BEGIN
    ALTER TABLE [dbo].[ServiceRequest] 
    ADD CONSTRAINT [FK_ServiceRequest_ServiceType] 
    FOREIGN KEY([ServiceTypeID]) REFERENCES [dbo].[ServiceType] ([ServiceTypeID])
END
GO

-- ServiceRequest -> UserRegistration (CreatedBy)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_UserRegistration_CreatedBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceRequest]'))
BEGIN
    ALTER TABLE [dbo].[ServiceRequest] 
    ADD CONSTRAINT [FK_ServiceRequest_UserRegistration_CreatedBy] 
    FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserRegistration] ([UserID])
END
GO

-- ServiceRequest -> UserRegistration (UpdatedBy)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_UserRegistration_UpdatedBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceRequest]'))
BEGIN
    ALTER TABLE [dbo].[ServiceRequest] 
    ADD CONSTRAINT [FK_ServiceRequest_UserRegistration_UpdatedBy] 
    FOREIGN KEY([UpdatedBy]) REFERENCES [dbo].[UserRegistration] ([UserID])
END
GO

-- ServiceRequest -> BranchMaster
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_BranchMaster]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceRequest]'))
BEGIN
    ALTER TABLE [dbo].[ServiceRequest] 
    ADD CONSTRAINT [FK_ServiceRequest_BranchMaster] 
    FOREIGN KEY([BranchID]) REFERENCES [dbo].[BranchMaster] ([BranchID])
END
GO

-- ServiceRequest -> AcFinancialYear
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_AcFinancialYear]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceRequest]'))
BEGIN
    ALTER TABLE [dbo].[ServiceRequest] 
    ADD CONSTRAINT [FK_ServiceRequest_AcFinancialYear] 
    FOREIGN KEY([AcFinancialYearID]) REFERENCES [dbo].[AcFinancialYear] ([AcFinancialYearID])
END
GO

-- =============================================
-- Create Indexes for Performance
-- =============================================

-- ServiceRequest indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServiceRequest]') AND name = 'IX_ServiceRequest_CustomerID')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ServiceRequest_CustomerID] ON [dbo].[ServiceRequest] ([CustomerID])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServiceRequest]') AND name = 'IX_ServiceRequest_EquipmentID')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ServiceRequest_EquipmentID] ON [dbo].[ServiceRequest] ([EquipmentID])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServiceRequest]') AND name = 'IX_ServiceRequest_PriorityID')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ServiceRequest_PriorityID] ON [dbo].[ServiceRequest] ([PriorityID])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServiceRequest]') AND name = 'IX_ServiceRequest_ServiceStatusID')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ServiceRequest_ServiceStatusID] ON [dbo].[ServiceRequest] ([ServiceStatusID])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServiceRequest]') AND name = 'IX_ServiceRequest_ServiceTypeID')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ServiceRequest_ServiceTypeID] ON [dbo].[ServiceRequest] ([ServiceTypeID])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServiceRequest]') AND name = 'IX_ServiceRequest_CreationDate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ServiceRequest_CreationDate] ON [dbo].[ServiceRequest] ([CreationDate])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServiceRequest]') AND name = 'IX_ServiceRequest_ServiceRequestNo')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_ServiceRequest_ServiceRequestNo] ON [dbo].[ServiceRequest] ([ServiceRequestNo])
END
GO

-- =============================================
-- Create Unique Constraints
-- =============================================

-- ServiceRequestNo should be unique
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServiceRequest]') AND name = 'UQ_ServiceRequest_ServiceRequestNo')
BEGIN
    ALTER TABLE [dbo].[ServiceRequest] 
    ADD CONSTRAINT [UQ_ServiceRequest_ServiceRequestNo] UNIQUE ([ServiceRequestNo])
END
GO

PRINT 'ServiceRequest tables created successfully!'
