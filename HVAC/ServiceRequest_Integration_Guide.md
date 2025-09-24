# ServiceRequest Integration Guide
## HVAC Management System

This guide provides step-by-step instructions for integrating the ServiceRequest functionality into your HVAC Management System.

## Prerequisites

- SQL Server Database Access
- Visual Studio or compatible IDE
- Entity Framework Model Update Access
- Admin access to the HVAC Management System

## Integration Steps

### 1. Database Setup

#### Step 1.1: Create Tables
Execute the following SQL scripts in order:

```sql
-- 1. Create ServiceRequest tables
EXEC sp_executesql N'-- Run the contents of ServiceRequest_Tables.sql'
```

#### Step 1.2: Insert Initial Data
```sql
-- 2. Insert initial data
EXEC sp_executesql N'-- Run the contents of ServiceRequest_InitialData.sql'
```

#### Step 1.3: Add Menu Items
```sql
-- 3. Add menu items
EXEC sp_executesql N'-- Run the contents of ServiceRequest_Menu.sql'
```

### 2. Entity Framework Model Update

#### Step 2.1: Update Model1.edmx
1. Open `HVAC/Models/Model1.edmx` in Visual Studio
2. Right-click in the designer and select "Update Model from Database"
3. Add the following tables:
   - `ServiceRequest`
   - `ServiceType`
   - `ServiceStatus`
   - Updated `Priority` table (with new columns)

#### Step 2.2: Update DbContext
The `HVACEntities` class should automatically include the new entities. Verify that the following properties exist:

```csharp
public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }
public virtual DbSet<ServiceType> ServiceTypes { get; set; }
public virtual DbSet<ServiceStatus> ServiceStatuses { get; set; }
```

### 3. File Integration

#### Step 3.1: Copy Model Files
Copy the following files to their respective locations:

```
HVAC/Models/ServiceRequest.cs
HVAC/Models/ServiceType.cs
HVAC/Models/ServiceStatus.cs (if not exists)
HVAC/Models/ServiceRequestVM.cs
```

#### Step 3.2: Copy Controller
```
HVAC/Controllers/ServiceRequestController.cs
```

#### Step 3.3: Copy Views
Create the following directory structure and copy views:

```
HVAC/Views/ServiceRequest/
├── Index.cshtml
├── Create.cshtml
├── Edit.cshtml
├── Details.cshtml
├── Delete.cshtml
└── _DeleteModal.cshtml
```

### 4. Configuration Updates

#### Step 4.1: Update Web.config (if needed)
The existing Web.config should work with the new functionality. No changes required unless you need specific connection string updates.

#### Step 4.2: Verify Routing
The default routing in `RouteConfig.cs` should handle the ServiceRequest controller automatically.

### 5. Testing

#### Step 5.1: Build the Solution
1. Clean and rebuild the solution
2. Ensure no compilation errors
3. Verify all references are resolved

#### Step 5.2: Test Database Connection
1. Run the application
2. Navigate to ServiceRequest section
3. Verify data loads correctly

#### Step 5.3: Test CRUD Operations
1. **Create**: Test creating new service requests
2. **Read**: Test viewing service request list and details
3. **Update**: Test editing existing service requests
4. **Delete**: Test deleting service requests

### 6. Menu Integration

#### Step 6.1: Verify Menu Items
1. Log in to the system
2. Check that "Service Request Management" appears in the sidebar
3. Verify sub-menu items are visible:
   - Service Requests
   - New Service Request
   - Service Reports

#### Step 6.2: Test Navigation
1. Click on "Service Requests" to view the list
2. Click on "New Service Request" to create a new request
3. Test all navigation links

### 7. User Permissions

#### Step 7.1: Set User Roles
Ensure users have appropriate permissions for ServiceRequest functionality:

- **Admin**: Full access to all ServiceRequest features
- **Manager**: Can view, create, edit, and delete service requests
- **Technician**: Can view and update assigned service requests
- **Viewer**: Can only view service requests

#### Step 7.2: Test Permissions
1. Log in with different user roles
2. Verify appropriate access levels
3. Test permission restrictions

### 8. Performance Optimization

#### Step 8.1: Database Indexes
The SQL scripts include performance indexes. Verify they were created:

```sql
-- Check indexes
SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('ServiceRequest')
```

#### Step 8.2: Caching
The controller includes output caching. Monitor performance and adjust cache durations as needed.

### 9. Troubleshooting

#### Common Issues and Solutions

**Issue**: ServiceRequest menu not visible
- **Solution**: Verify menu SQL script executed successfully
- **Check**: User role permissions

**Issue**: Database connection errors
- **Solution**: Verify connection string in Web.config
- **Check**: Database server accessibility

**Issue**: Entity Framework errors
- **Solution**: Update model from database
- **Check**: Foreign key relationships

**Issue**: View not found errors
- **Solution**: Verify view files are in correct location
- **Check**: Controller action names match view names

### 10. Post-Integration Tasks

#### Step 10.1: User Training
1. Create user documentation
2. Conduct training sessions
3. Provide quick reference guides

#### Step 10.2: Data Migration
If migrating from existing system:
1. Export existing service data
2. Transform data to new format
3. Import using ServiceRequest interface

#### Step 10.3: Backup
1. Backup database before integration
2. Create restore points
3. Document rollback procedures

### 11. Monitoring and Maintenance

#### Step 11.1: Log Monitoring
Monitor application logs for ServiceRequest-related errors:
- Check `App_Data/Logs/` directory
- Monitor database performance
- Track user activity

#### Step 11.2: Regular Maintenance
1. Monitor database growth
2. Clean up old service requests (if needed)
3. Update service types and statuses as required

## Support

For technical support or questions regarding the ServiceRequest integration:

1. Check the application logs
2. Review the technical documentation
3. Contact the development team

## Version History

- **v1.0**: Initial ServiceRequest integration
- **v1.1**: Added menu integration
- **v1.2**: Performance optimizations

---

**Note**: This integration guide assumes you have administrative access to the HVAC Management System and its database. Always backup your system before making any changes.
