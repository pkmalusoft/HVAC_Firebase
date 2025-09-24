-- ServiceRequest Menu Integration Script
-- HVAC Management System
-- Created: 2024

-- =============================================
-- Add ServiceRequest Menu Items
-- =============================================

-- Check if ServiceRequest menu already exists
IF NOT EXISTS (SELECT 1 FROM Menu WHERE Title = 'Service Request Management')
BEGIN
    -- Insert parent menu for Service Request Management
    INSERT INTO [dbo].[Menu] (
        [Title], [Link], [ParentID], [Ordering], [SubLevel], [RoleID], 
        [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsActive], 
        [imgclass], [PermissionRequired], [MenuOrder], [IsAccountMenu], [MenuDescription]
    )
    VALUES (
        'Service Request Management', '#', 0, 100, 0, '1,2,3,4,5', 
        '1', GETDATE(), '1', GETDATE(), 1, 
        'mdi mdi-wrench', 1, 100, 0, 'Manage service requests and maintenance'
    )
END
GO

-- Get the ServiceRequest parent menu ID
DECLARE @ServiceRequestParentID INT = (SELECT MenuID FROM Menu WHERE Title = 'Service Request Management')

-- Add Service Request List menu
IF NOT EXISTS (SELECT 1 FROM Menu WHERE Title = 'Service Requests' AND ParentID = @ServiceRequestParentID)
BEGIN
    INSERT INTO [dbo].[Menu] (
        [Title], [Link], [ParentID], [Ordering], [SubLevel], [RoleID], 
        [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsActive], 
        [imgclass], [PermissionRequired], [MenuOrder], [IsAccountMenu], [MenuDescription]
    )
    VALUES (
        'Service Requests', '/ServiceRequest', @ServiceRequestParentID, 1, 1, '1,2,3,4,5', 
        '1', GETDATE(), '1', GETDATE(), 1, 
        'mdi mdi-format-list-bulleted', 1, 1, 0, 'View and manage service requests'
    )
END
GO

-- Add Create Service Request menu
IF NOT EXISTS (SELECT 1 FROM Menu WHERE Title = 'New Service Request' AND ParentID = @ServiceRequestParentID)
BEGIN
    INSERT INTO [dbo].[Menu] (
        [Title], [Link], [ParentID], [Ordering], [SubLevel], [RoleID], 
        [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsActive], 
        [imgclass], [PermissionRequired], [MenuOrder], [IsAccountMenu], [MenuDescription]
    )
    VALUES (
        'New Service Request', '/ServiceRequest/Create', @ServiceRequestParentID, 2, 1, '1,2,3,4,5', 
        '1', GETDATE(), '1', GETDATE(), 1, 
        'mdi mdi-plus-circle', 1, 2, 0, 'Create new service request'
    )
END
GO

-- Add Service Request Reports menu
IF NOT EXISTS (SELECT 1 FROM Menu WHERE Title = 'Service Reports' AND ParentID = @ServiceRequestParentID)
BEGIN
    INSERT INTO [dbo].[Menu] (
        [Title], [Link], [ParentID], [Ordering], [SubLevel], [RoleID], 
        [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsActive], 
        [imgclass], [PermissionRequired], [MenuOrder], [IsAccountMenu], [MenuDescription]
    )
    VALUES (
        'Service Reports', '/ServiceRequest/Reports', @ServiceRequestParentID, 3, 1, '1,2,3,4,5', 
        '1', GETDATE(), '1', GETDATE(), 1, 
        'mdi mdi-chart-line', 1, 3, 0, 'View service request reports and analytics'
    )
END
GO

-- =============================================
-- Add ServiceRequest to Quick Menus (Optional)
-- =============================================
-- Uncomment if you want to add ServiceRequest to quick access menu

/*
-- Add ServiceRequest to Quick Menus section
IF NOT EXISTS (SELECT 1 FROM Menu WHERE Title = 'Quick Service Request' AND ParentID = 0)
BEGIN
    INSERT INTO [dbo].[Menu] (
        [Title], [Link], [ParentID], [Ordering], [SubLevel], [RoleID], 
        [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsActive], 
        [imgclass], [PermissionRequired], [MenuOrder], [IsAccountMenu], [MenuDescription]
    )
    VALUES (
        'Quick Service Request', '/ServiceRequest/Create', 0, 999, 0, '1,2,3,4,5', 
        '1', GETDATE(), '1', GETDATE(), 1, 
        'mdi mdi-wrench', 0, 999, 0, 'Quick access to create service request'
    )
END
GO
*/

-- =============================================
-- Update Menu Ordering (Optional)
-- =============================================
-- Update ordering to ensure ServiceRequest appears in the right position
-- Adjust the Ordering values based on your existing menu structure

UPDATE [dbo].[Menu] 
SET [Ordering] = 50 
WHERE [Title] = 'Service Request Management' AND [ParentID] = 0
GO

PRINT 'ServiceRequest menu items added successfully!'
