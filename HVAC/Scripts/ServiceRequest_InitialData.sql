-- ServiceRequest Initial Data Script
-- HVAC Management System
-- Created: 2024

-- =============================================
-- Insert ServiceType Data
-- =============================================
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

-- =============================================
-- Insert ServiceStatus Data
-- =============================================
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

-- =============================================
-- Update Priority Data (if not exists)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Priority WHERE PriorityName = 'High')
BEGIN
    INSERT INTO [dbo].[Priority] ([PriorityName], [PriorityDescription], [PriorityLevel], [PriorityColor], [IsActive], [CreatedBy], [CreatedDate])
    VALUES ('High', 'High priority service request - urgent attention required', 1, '#dc3545', 1, 1, GETDATE())
END
ELSE
BEGIN
    -- Update existing High priority with new columns
    UPDATE [dbo].[Priority] 
    SET [PriorityDescription] = 'High priority service request - urgent attention required',
        [PriorityLevel] = 1,
        [PriorityColor] = '#dc3545',
        [IsActive] = 1
    WHERE [PriorityName] = 'High'
END
GO

IF NOT EXISTS (SELECT 1 FROM Priority WHERE PriorityName = 'Medium')
BEGIN
    INSERT INTO [dbo].[Priority] ([PriorityName], [PriorityDescription], [PriorityLevel], [PriorityColor], [IsActive], [CreatedBy], [CreatedDate])
    VALUES ('Medium', 'Medium priority service request - normal processing', 2, '#ffc107', 1, 1, GETDATE())
END
ELSE
BEGIN
    -- Update existing Medium priority with new columns
    UPDATE [dbo].[Priority] 
    SET [PriorityDescription] = 'Medium priority service request - normal processing',
        [PriorityLevel] = 2,
        [PriorityColor] = '#ffc107',
        [IsActive] = 1
    WHERE [PriorityName] = 'Medium'
END
GO

IF NOT EXISTS (SELECT 1 FROM Priority WHERE PriorityName = 'Low')
BEGIN
    INSERT INTO [dbo].[Priority] ([PriorityName], [PriorityDescription], [PriorityLevel], [PriorityColor], [IsActive], [CreatedBy], [CreatedDate])
    VALUES ('Low', 'Low priority service request - can be scheduled later', 3, '#28a745', 1, 1, GETDATE())
END
ELSE
BEGIN
    -- Update existing Low priority with new columns
    UPDATE [dbo].[Priority] 
    SET [PriorityDescription] = 'Low priority service request - can be scheduled later',
        [PriorityLevel] = 3,
        [PriorityColor] = '#28a745',
        [IsActive] = 1
    WHERE [PriorityName] = 'Low'
END
GO

-- =============================================
-- Create Sample Service Requests (Optional)
-- =============================================
-- Uncomment the following section if you want to create sample data

/*
-- Get sample customer and equipment IDs
DECLARE @SampleCustomerID INT = (SELECT TOP 1 CustomerID FROM CustomerMaster WHERE StatusActive = 1)
DECLARE @SampleEquipmentID INT = (SELECT TOP 1 ID FROM Equipment)
DECLARE @SamplePriorityID INT = (SELECT TOP 1 PriorityID FROM Priority WHERE IsActive = 1)
DECLARE @SampleStatusID INT = (SELECT TOP 1 ServiceStatusID FROM ServiceStatus WHERE IsActive = 1)
DECLARE @SampleServiceTypeID INT = (SELECT TOP 1 ServiceTypeID FROM ServiceType WHERE IsActive = 1)
DECLARE @SampleUserID INT = (SELECT TOP 1 UserID FROM UserRegistration)
DECLARE @SampleBranchID INT = (SELECT TOP 1 BranchID FROM BranchMaster)
DECLARE @SampleYearID INT = (SELECT TOP 1 AcFinancialYearID FROM AcFinancialYear WHERE IsActive = 1)

IF @SampleCustomerID IS NOT NULL AND @SampleEquipmentID IS NOT NULL
BEGIN
    -- Insert sample service requests
    INSERT INTO [dbo].[ServiceRequest] (
        [ServiceRequestNo], [CustomerID], [EquipmentID], [PriorityID], [ServiceStatusID], [ServiceTypeID],
        [ServiceDescription], [CreationDate], [CreatedBy], [ContactPerson], [ContactPhone], [ContactEmail],
        [Location], [BranchID], [AcFinancialYearID]
    )
    VALUES 
    ('SR2024001', @SampleCustomerID, @SampleEquipmentID, @SamplePriorityID, @SampleStatusID, @SampleServiceTypeID,
     'Regular maintenance service required for HVAC unit', GETDATE(), @SampleUserID, 'John Doe', '1234567890', 'john@example.com',
     'Main Office Building', @SampleBranchID, @SampleYearID),
    
    ('SR2024002', @SampleCustomerID, @SampleEquipmentID, @SamplePriorityID, @SampleStatusID, @SampleServiceTypeID,
     'Emergency repair needed for cooling system', GETDATE(), @SampleUserID, 'Jane Smith', '0987654321', 'jane@example.com',
     'Warehouse Facility', @SampleBranchID, @SampleYearID)
END
*/

PRINT 'ServiceRequest initial data inserted successfully!'
