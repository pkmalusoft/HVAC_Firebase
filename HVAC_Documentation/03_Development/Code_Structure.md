# HVAC Management System - Code Structure

## Table of Contents
1. [Project Overview](#project-overview)
2. [Solution Structure](#solution-structure)
3. [Controller Architecture](#controller-architecture)
4. [Model Architecture](#model-architecture)
5. [Data Access Layer](#data-access-layer)
6. [View Architecture](#view-architecture)
7. [Common Components](#common-components)
8. [Configuration Files](#configuration-files)
9. [Dependencies](#dependencies)
10. [Code Organization](#code-organization)

## Project Overview

The HVAC Management System follows the Model-View-Controller (MVC) architectural pattern, providing clear separation of concerns and maintainable code structure.

### Architecture Principles
- **Separation of Concerns**: Clear boundaries between presentation, business logic, and data access
- **Single Responsibility**: Each class has a single, well-defined purpose
- **Dependency Injection**: Loose coupling between components
- **Repository Pattern**: Abstracted data access layer
- **Service Layer**: Business logic encapsulation

## Solution Structure

```
HVAC_Management_System/
├── HVAC/                          # Main application project
│   ├── Controllers/               # MVC Controllers
│   ├── Models/                    # Data models and ViewModels
│   ├── Views/                     # Razor views
│   ├── DAL/                       # Data Access Layer
│   ├── Common/                    # Common utilities
│   ├── Content/                   # Static content (CSS, JS, Images)
│   ├── Scripts/                   # JavaScript files
│   ├── App_Start/                 # Application startup configuration
│   ├── Properties/                # Project properties
│   ├── Reports/                   # Crystal Reports
│   ├── UploadDocuments/           # File upload directory
│   ├── Web.config                 # Application configuration
│   └── Global.asax               # Application events
├── HVAC_Documentation/            # Technical documentation
└── HVAC.sln                      # Visual Studio solution file
```

## Controller Architecture

### Controller Organization

```
Controllers/
├── HomeController.cs              # Home page and dashboard
├── LoginController.cs             # Authentication
├── DashboardController.cs         # Dashboard analytics
├── EnquiryController.cs           # Enquiry management
├── QuotationController.cs         # Quotation management
├── PurchaseOrderController.cs     # Purchase order management
├── CustomerMasterController.cs    # Customer management
├── SupplierController.cs          # Supplier management
├── CompanyMasterController.cs     # Company management
├── UserRegistrationController.cs  # User management
├── ReportsController.cs           # Report generation
├── ErrorsController.cs            # Error handling
└── SessionExpireAttribute.cs      # Session management
```

### Controller Base Structure

```csharp
[SessionExpireFilter]
public class ExampleController : Controller
{
    private HVACEntities db = new HVACEntities();
    
    // GET: Example
    public ActionResult Index()
    {
        // Controller logic
    }
    
    // POST: Example
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ExampleModel model)
    {
        // Create logic
    }
}
```

### Controller Responsibilities

| Controller | Primary Responsibilities |
|------------|-------------------------|
| **HomeController** | Dashboard, file uploads, trial management |
| **LoginController** | User authentication, session management |
| **EnquiryController** | Enquiry CRUD, project management |
| **QuotationController** | Quotation generation, pricing |
| **PurchaseOrderController** | Purchase order management |
| **CustomerMasterController** | Customer data management |
| **SupplierController** | Supplier data management |
| **ReportsController** | Report generation and export |

## Model Architecture

### Model Organization

```
Models/
├── Entity Models/                 # Entity Framework models
│   ├── AcCompany.cs
│   ├── BranchMaster.cs
│   ├── CustomerMaster.cs
│   ├── Enquiry.cs
│   ├── Quotation.cs
│   └── UserRegistration.cs
├── ViewModels/                    # Presentation models
│   ├── EnquiryVM.cs
│   ├── QuotationVM.cs
│   ├── DashboardVM.cs
│   └── CompanyVM.cs
├── Common Models/                 # Shared models
│   ├── CommonFunctions.cs
│   ├── SessionExpireFilterAttribute.cs
│   └── LoggingHelper.cs
└── Search Models/                 # Search and filter models
    ├── EnquirySearch.cs
    ├── QuotationSearch.cs
    └── PurchaseOrderSearch.cs
```

### Model Categories

#### 1. Entity Models
```csharp
// Example Entity Model
public partial class Enquiry
{
    public int EnquiryID { get; set; }
    public int BranchID { get; set; }
    public int CustomerID { get; set; }
    public string EnquiryNo { get; set; }
    public DateTime EnquiryDate { get; set; }
    public string ProjectName { get; set; }
    
    // Navigation properties
    public virtual BranchMaster BranchMaster { get; set; }
    public virtual CustomerMaster CustomerMaster { get; set; }
}
```

#### 2. ViewModels
```csharp
// Example ViewModel
public class EnquiryVM
{
    public int EnquiryID { get; set; }
    public string EnquiryNo { get; set; }
    public DateTime EnquiryDate { get; set; }
    public string ProjectName { get; set; }
    public string CustomerName { get; set; }
    public string StatusName { get; set; }
    
    // Additional properties for UI
    public List<EnquiryEquipmentVM> EquipmentDetails { get; set; }
    public List<EnquiryClientVM> ClientDetails { get; set; }
}
```

#### 3. Search Models
```csharp
// Example Search Model
public class EnquirySearch
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string EnquiryNo { get; set; }
    public int CustomerID { get; set; }
    public int StatusID { get; set; }
    
    public List<EnquiryVM> Details { get; set; }
}
```

## Data Access Layer

### DAL Organization

```
DAL/
├── GeneralDAO.cs                  # General database operations
├── EnquiryDAO.cs                  # Enquiry-specific operations
├── QuotationDAO.cs                # Quotation-specific operations
├── PurchaseOrderDAO.cs            # Purchase order operations
├── CustomerDAO.cs                 # Customer operations
├── SupplierDAO.cs                 # Supplier operations
├── DashboardDAO.cs                # Dashboard data operations
├── ReportsDAO.cs                  # Report data operations
├── MasterDAO.cs                   # Master data operations
├── LocationDAO.cs                 # Location operations
├── EmailDAO.cs                    # Email operations
├── DocumentDAO.cs                 # Document operations
├── AccountsDAO.cs                 # Accounting operations
├── CreditNoteDAO.cs               # Credit note operations
├── ReceiptDAO.cs                  # Receipt operations
└── HVACReportsDAO.cs              # HVAC-specific reports
```

### DAO Pattern Implementation

```csharp
// Example DAO Class
public static class EnquiryDAO
{
    public static List<EnquiryVM> EnquiryList(DateTime fromDate, DateTime toDate, 
        string enquiryNo, int yearid, int employeeId, int roleID)
    {
        using (SqlConnection connection = new SqlConnection(CommonFunctions.GetConnectionString))
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = connection;
            cmd.CommandText = "SP_GetEnquiryList";
            cmd.CommandType = CommandType.StoredProcedure;
            
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@EnquiryNo", enquiryNo);
            cmd.Parameters.AddWithValue("@YearID", yearid);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
            cmd.Parameters.AddWithValue("@RoleID", roleID);
            
            // Execute query and return results
        }
    }
}
```

### Data Access Patterns

| Pattern | Usage | Example |
|---------|-------|---------|
| **Repository** | Data access abstraction | `EnquiryDAO.GetEnquiryList()` |
| **Unit of Work** | Transaction management | `db.SaveChanges()` |
| **Connection Management** | Resource disposal | `using (SqlConnection)` |
| **Parameterized Queries** | SQL injection prevention | `cmd.Parameters.AddWithValue()` |

## View Architecture

### View Organization

```
Views/
├── Shared/                        # Shared views
│   ├── _Layout.cshtml             # Main layout
│   ├── _LoginPartial.cshtml       # Login partial
│   └── Error.cshtml               # Error page
├── Home/                          # Home views
│   ├── Index.cshtml
│   └── Home.cshtml
├── Enquiry/                       # Enquiry views
│   ├── Index.cshtml
│   ├── Create.cshtml
│   ├── Edit.cshtml
│   └── Details.cshtml
├── Quotation/                     # Quotation views
│   ├── Index.cshtml
│   ├── Create.cshtml
│   ├── Print.cshtml
│   └── WarrantyDetailList.cshtml
└── [Other Controllers]/           # Controller-specific views
```

### View Structure

```html
<!-- Example View Structure -->
@model HVAC.Models.EnquiryVM
@{
    ViewBag.Title = "Enquiry Management";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

<div class="container-fluid">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4 class="card-title">@ViewBag.Title</h4>
                </div>
                <div class="card-body">
                    <!-- View content -->
                </div>
            </div>
        </div>
    </div>
</div>
```

## Common Components

### Common Utilities

```
Common/
├── LoggingHelper.cs               # Logging utilities
├── CommonFunctions.cs             # Common functions
├── EmailHelper.cs                 # Email utilities
├── FileHelper.cs                  # File operations
├── ValidationHelper.cs            # Validation utilities
└── SecurityHelper.cs              # Security utilities
```

### Common Functions Example

```csharp
public static class CommonFunctions
{
    public static string GetConnectionString
    {
        get { return ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString; }
    }
    
    public static DateTime GetCurrentDateTime()
    {
        return DateTime.Now;
    }
    
    public static int ParseInt(string value)
    {
        int result;
        return int.TryParse(value, out result) ? result : 0;
    }
}
```

## Configuration Files

### Web.config Structure

```xml
<configuration>
  <configSections>
    <!-- Configuration sections -->
  </configSections>
  
  <appSettings>
    <!-- Application settings -->
  </appSettings>
  
  <connectionStrings>
    <!-- Database connections -->
  </connectionStrings>
  
  <system.web>
    <!-- Web configuration -->
  </system.web>
  
  <system.webServer>
    <!-- IIS configuration -->
  </system.webServer>
</configuration>
```

### App_Start Configuration

```
App_Start/
├── BundleConfig.cs                # CSS/JS bundling
├── FilterConfig.cs                # Global filters
├── RouteConfig.cs                 # URL routing
└── AttributeRouting.cs            # Attribute routing
```

## Dependencies

### NuGet Packages

```xml
<packages>
  <package id="EntityFramework" version="6.4.4" targetFramework="net48" />
  <package id="Microsoft.AspNet.Mvc" version="5.2.9" targetFramework="net48" />
  <package id="Microsoft.AspNet.Razor" version="3.2.9" targetFramework="net48" />
  <package id="Microsoft.AspNet.WebPages" version="3.2.9" targetFramework="net48" />
  <package id="Newtonsoft.Json" version="13.0.3" targetFramework="net48" />
  <package id="CrystalDecisions.CrystalReports.Engine" version="13.0.4000" targetFramework="net48" />
  <package id="AWSSDK.S3" version="3.7.0.1" targetFramework="net48" />
</packages>
```

### External Dependencies

| Component | Purpose | Version |
|-----------|---------|---------|
| **Entity Framework** | ORM | 6.4.4 |
| **Crystal Reports** | Reporting | 13.0.4000 |
| **AWS SDK** | Cloud storage | 3.7.0.1 |
| **Newtonsoft.Json** | JSON serialization | 13.0.3 |
| **AttributeRouting** | URL routing | 3.5.6 |

## Code Organization

### Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| **Classes** | PascalCase | `EnquiryController` |
| **Methods** | PascalCase | `GetEnquiryList()` |
| **Properties** | PascalCase | `EnquiryID` |
| **Variables** | camelCase | `enquiryList` |
| **Constants** | UPPER_CASE | `MAX_FILE_SIZE` |
| **Files** | PascalCase | `EnquiryController.cs` |

### Code Structure Standards

```csharp
// 1. Using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// 2. Namespace
namespace HVAC.Controllers
{
    // 3. Class declaration
    [SessionExpireFilter]
    public class ExampleController : Controller
    {
        // 4. Private fields
        private HVACEntities db = new HVACEntities();
        
        // 5. Public properties
        public string PropertyName { get; set; }
        
        // 6. Constructors
        public ExampleController()
        {
            // Constructor logic
        }
        
        // 7. Public methods
        public ActionResult Index()
        {
            // Method logic
        }
        
        // 8. Private methods
        private void HelperMethod()
        {
            // Helper logic
        }
    }
}
```

### File Organization

```
HVAC/
├── Controllers/                   # 15 controller files
├── Models/                        # 150+ model files
├── Views/                         # 376 view files
├── DAL/                           # 15 DAO files
├── Common/                        # 5 utility files
├── Content/                       # Static content
├── Scripts/                       # JavaScript files
├── App_Start/                     # 4 configuration files
├── Properties/                    # Project properties
├── Reports/                       # 11 Crystal Reports
├── UploadDocuments/               # File storage
├── Web.config                     # Main configuration
└── Global.asax                    # Application events
```

---

*This code structure documentation provides a comprehensive overview of the HVAC Management System's codebase organization and architecture.*
