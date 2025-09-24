-- ServiceRequest Integration Verification Script
-- HVAC Management System
-- This script verifies that the ServiceRequest integration was successful

-- =============================================
-- Check Table Existence
-- =============================================
PRINT 'Checking table existence...'

IF EXISTS (SELECT * FROM sysobjects WHERE name='ServiceRequest' AND xtype='U')
    PRINT '✓ ServiceRequest table exists'
ELSE
    PRINT '✗ ServiceRequest table missing'

IF EXISTS (SELECT * FROM sysobjects WHERE name='ServiceType' AND xtype='U')
    PRINT '✓ ServiceType table exists'
ELSE
    PRINT '✗ ServiceType table missing'

IF EXISTS (SELECT * FROM sysobjects WHERE name='ServiceStatus' AND xtype='U')
    PRINT '✓ ServiceStatus table exists'
ELSE
    PRINT '✗ ServiceStatus table missing'

-- =============================================
-- Check Data Existence
-- =============================================
PRINT ''
PRINT 'Checking initial data...'

-- Check ServiceType data
DECLARE @ServiceTypeCount INT = (SELECT COUNT(*) FROM ServiceType)
IF @ServiceTypeCount > 0
    PRINT '✓ ServiceType data exists (' + CAST(@ServiceTypeCount AS VARCHAR) + ' records)'
ELSE
    PRINT '✗ ServiceType data missing'

-- Check ServiceStatus data
DECLARE @ServiceStatusCount INT = (SELECT COUNT(*) FROM ServiceStatus)
IF @ServiceStatusCount > 0
    PRINT '✓ ServiceStatus data exists (' + CAST(@ServiceStatusCount AS VARCHAR) + ' records)'
ELSE
    PRINT '✗ ServiceStatus data missing'

-- Check Priority data (updated)
DECLARE @PriorityCount INT = (SELECT COUNT(*) FROM Priority WHERE IsActive = 1)
IF @PriorityCount > 0
    PRINT '✓ Priority data exists (' + CAST(@PriorityCount AS VARCHAR) + ' active records)'
ELSE
    PRINT '✗ Priority data missing or not active'

-- =============================================
-- Check Foreign Key Constraints
-- =============================================
PRINT ''
PRINT 'Checking foreign key constraints...'

-- Check ServiceRequest foreign keys
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_CustomerMaster]'))
    PRINT '✓ ServiceRequest -> CustomerMaster FK exists'
ELSE
    PRINT '✗ ServiceRequest -> CustomerMaster FK missing'

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_Equipment]'))
    PRINT '✓ ServiceRequest -> Equipment FK exists'
ELSE
    PRINT '✗ ServiceRequest -> Equipment FK missing'

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_Priority]'))
    PRINT '✓ ServiceRequest -> Priority FK exists'
ELSE
    PRINT '✗ ServiceRequest -> Priority FK missing'

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_ServiceStatus]'))
    PRINT '✓ ServiceRequest -> ServiceStatus FK exists'
ELSE
    PRINT '✗ ServiceRequest -> ServiceStatus FK missing'

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_ServiceType]'))
    PRINT '✓ ServiceRequest -> ServiceType FK exists'
ELSE
    PRINT '✗ ServiceRequest -> ServiceType FK missing'

-- =============================================
-- Check Indexes
-- =============================================
PRINT ''
PRINT 'Checking performance indexes...'

DECLARE @IndexCount INT = (SELECT COUNT(*) FROM sys.indexes WHERE object_id = OBJECT_ID('ServiceRequest') AND name LIKE 'IX_ServiceRequest_%')
IF @IndexCount > 0
    PRINT '✓ ServiceRequest indexes exist (' + CAST(@IndexCount AS VARCHAR) + ' indexes)'
ELSE
    PRINT '✗ ServiceRequest indexes missing'

-- =============================================
-- Check Menu Items
-- =============================================
PRINT ''
PRINT 'Checking menu items...'

IF EXISTS (SELECT * FROM Menu WHERE Title = 'Service Request Management')
    PRINT '✓ Service Request Management menu exists'
ELSE
    PRINT '✗ Service Request Management menu missing'

IF EXISTS (SELECT * FROM Menu WHERE Title = 'Service Requests' AND Link = '/ServiceRequest')
    PRINT '✓ Service Requests menu item exists'
ELSE
    PRINT '✗ Service Requests menu item missing'

IF EXISTS (SELECT * FROM Menu WHERE Title = 'New Service Request' AND Link = '/ServiceRequest/Create')
    PRINT '✓ New Service Request menu item exists'
ELSE
    PRINT '✗ New Service Request menu item missing'

-- =============================================
-- Test Data Integrity
-- =============================================
PRINT ''
PRINT 'Testing data integrity...'

-- Test ServiceType data
IF EXISTS (SELECT * FROM ServiceType WHERE ServiceTypeName = 'Service' AND IsActive = 1)
    PRINT '✓ Service type data is correct'
ELSE
    PRINT '✗ Service type data is incorrect'

IF EXISTS (SELECT * FROM ServiceType WHERE ServiceTypeName = 'AMC' AND IsActive = 1)
    PRINT '✓ AMC type data is correct'
ELSE
    PRINT '✗ AMC type data is incorrect'

-- Test ServiceStatus data
IF EXISTS (SELECT * FROM ServiceStatus WHERE ServiceStatusName = 'Pending' AND IsActive = 1)
    PRINT '✓ Pending status data is correct'
ELSE
    PRINT '✗ Pending status data is incorrect'

IF EXISTS (SELECT * FROM ServiceStatus WHERE ServiceStatusName = 'Work In Progress' AND IsActive = 1)
    PRINT '✓ Work In Progress status data is correct'
ELSE
    PRINT '✗ Work In Progress status data is incorrect'

IF EXISTS (SELECT * FROM ServiceStatus WHERE ServiceStatusName = 'Waiting for parts' AND IsActive = 1)
    PRINT '✓ Waiting for parts status data is correct'
ELSE
    PRINT '✗ Waiting for parts status data is incorrect'

IF EXISTS (SELECT * FROM ServiceStatus WHERE ServiceStatusName = 'Closed' AND IsActive = 1)
    PRINT '✓ Closed status data is correct'
ELSE
    PRINT '✗ Closed status data is incorrect'

-- Test Priority data
IF EXISTS (SELECT * FROM Priority WHERE PriorityName = 'High' AND IsActive = 1 AND PriorityLevel = 1)
    PRINT '✓ High priority data is correct'
ELSE
    PRINT '✗ High priority data is incorrect'

IF EXISTS (SELECT * FROM Priority WHERE PriorityName = 'Medium' AND IsActive = 1 AND PriorityLevel = 2)
    PRINT '✓ Medium priority data is correct'
ELSE
    PRINT '✗ Medium priority data is incorrect'

IF EXISTS (SELECT * FROM Priority WHERE PriorityName = 'Low' AND IsActive = 1 AND PriorityLevel = 3)
    PRINT '✓ Low priority data is correct'
ELSE
    PRINT '✗ Low priority data is incorrect'

-- =============================================
-- Summary
-- =============================================
PRINT ''
PRINT '==============================================='
PRINT 'ServiceRequest Integration Verification Summary'
PRINT '==============================================='

DECLARE @TotalChecks INT = 0
DECLARE @PassedChecks INT = 0

-- Count checks (simplified)
SET @TotalChecks = 20 -- Approximate number of checks
SET @PassedChecks = 
    (CASE WHEN EXISTS (SELECT * FROM sysobjects WHERE name='ServiceRequest' AND xtype='U') THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM sysobjects WHERE name='ServiceType' AND xtype='U') THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM sysobjects WHERE name='ServiceStatus' AND xtype='U') THEN 1 ELSE 0 END) +
    (CASE WHEN @ServiceTypeCount > 0 THEN 1 ELSE 0 END) +
    (CASE WHEN @ServiceStatusCount > 0 THEN 1 ELSE 0 END) +
    (CASE WHEN @PriorityCount > 0 THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_CustomerMaster]')) THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_Equipment]')) THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_Priority]')) THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_ServiceStatus]')) THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceRequest_ServiceType]')) THEN 1 ELSE 0 END) +
    (CASE WHEN @IndexCount > 0 THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM Menu WHERE Title = 'Service Request Management') THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM Menu WHERE Title = 'Service Requests' AND Link = '/ServiceRequest') THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM Menu WHERE Title = 'New Service Request' AND Link = '/ServiceRequest/Create') THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM ServiceType WHERE ServiceTypeName = 'Service' AND IsActive = 1) THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM ServiceType WHERE ServiceTypeName = 'AMC' AND IsActive = 1) THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM ServiceStatus WHERE ServiceStatusName = 'Pending' AND IsActive = 1) THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM ServiceStatus WHERE ServiceStatusName = 'Work In Progress' AND IsActive = 1) THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM ServiceStatus WHERE ServiceStatusName = 'Waiting for parts' AND IsActive = 1) THEN 1 ELSE 0 END) +
    (CASE WHEN EXISTS (SELECT * FROM ServiceStatus WHERE ServiceStatusName = 'Closed' AND IsActive = 1) THEN 1 ELSE 0 END)

PRINT 'Total Checks: ' + CAST(@TotalChecks AS VARCHAR)
PRINT 'Passed Checks: ' + CAST(@PassedChecks AS VARCHAR)
PRINT 'Success Rate: ' + CAST(ROUND((@PassedChecks * 100.0 / @TotalChecks), 2) AS VARCHAR) + '%'

IF @PassedChecks = @TotalChecks
    PRINT '✓ ServiceRequest integration is COMPLETE and SUCCESSFUL!'
ELSE
    PRINT '⚠ ServiceRequest integration has some issues. Please review the failed checks above.'

PRINT '==============================================='
