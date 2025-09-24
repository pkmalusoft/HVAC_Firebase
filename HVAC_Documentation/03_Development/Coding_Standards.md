# HVAC Management System - Coding Standards

## Table of Contents
1. [Overview](#overview)
2. [General Principles](#general-principles)
3. [Naming Conventions](#naming-conventions)
4. [Code Organization](#code-organization)
5. [C# Coding Standards](#c-coding-standards)
6. [ASP.NET MVC Standards](#aspnet-mvc-standards)
7. [Database Standards](#database-standards)
8. [JavaScript Standards](#javascript-standards)
9. [CSS Standards](#css-standards)
10. [Documentation Standards](#documentation-standards)
11. [Error Handling](#error-handling)
12. [Security Standards](#security-standards)
13. [Performance Standards](#performance-standards)
14. [Testing Standards](#testing-standards)

## Overview

This document establishes coding standards and best practices for the HVAC Management System development team. These standards ensure code consistency, maintainability, and quality across the entire application.

### Purpose
- Ensure consistent code style across the team
- Improve code readability and maintainability
- Reduce bugs and technical debt
- Facilitate code reviews and collaboration
- Enhance application performance and security

## General Principles

### 1. Code Quality Principles
- **Readability**: Code should be self-documenting and easy to understand
- **Maintainability**: Code should be easy to modify and extend
- **Testability**: Code should be designed for easy testing
- **Performance**: Code should be optimized for performance
- **Security**: Code should follow security best practices

### 2. SOLID Principles
- **S**ingle Responsibility Principle: Each class should have one reason to change
- **O**pen/Closed Principle: Open for extension, closed for modification
- **L**iskov Substitution Principle: Derived classes must be substitutable for base classes
- **I**nterface Segregation Principle: Clients should not depend on interfaces they don't use
- **D**ependency Inversion Principle: Depend on abstractions, not concretions

### 3. DRY Principle
- **D**on't **R**epeat **Y**ourself: Avoid code duplication
- Use helper methods, base classes, and utilities
- Extract common functionality into reusable components

## Naming Conventions

### 1. C# Naming Conventions

#### Classes and Interfaces
```csharp
// Classes: PascalCase
public class EnquiryController { }
public class CustomerMaster { }
public class QuotationService { }

// Interfaces: PascalCase with 'I' prefix
public interface IEnquiryService { }
public interface IRepository<T> { }
public interface ILoggingService { }
```

#### Methods and Properties
```csharp
// Methods: PascalCase
public ActionResult GetEnquiryList() { }
public void SaveEnquiry(EnquiryVM model) { }
public bool ValidateInput(string input) { }

// Properties: PascalCase
public int EnquiryID { get; set; }
public string CustomerName { get; set; }
public DateTime CreatedDate { get; set; }
```

#### Variables and Parameters
```csharp
// Local variables: camelCase
var enquiryList = new List<EnquiryVM>();
var customerName = "ABC Corporation";
var totalAmount = 150000.00m;

// Parameters: camelCase
public void ProcessEnquiry(int enquiryId, string enquiryNo) { }
public ActionResult Create(CustomerVM customer) { }
```

#### Constants and Fields
```csharp
// Constants: UPPER_CASE
public const int MAX_FILE_SIZE = 10485760;
public const string DEFAULT_CURRENCY = "USD";
public const string CONNECTION_STRING_KEY = "myConnectionString";

// Private fields: camelCase with underscore prefix
private readonly HVACEntities _db;
private readonly ILoggingService _logger;
private static readonly string _connectionString;
```

### 2. Database Naming Conventions

#### Tables
```sql
-- Tables: PascalCase
AcCompany
BranchMaster
CustomerMaster
Enquiry
Quotation
PurchaseOrder
```

#### Columns
```sql
-- Columns: PascalCase
EnquiryID
CustomerName
EnquiryDate
ProjectName
CreatedDate
ModifiedDate
```

#### Stored Procedures
```sql
-- Stored Procedures: SP_ prefix + PascalCase
SP_GetEnquiryList
SP_SaveEnquiry
SP_DeleteEnquiry
SP_GetDashboardSummary
```

#### Indexes
```sql
-- Indexes: IX_ prefix + TableName + ColumnName
IX_Enquiry_EnquiryNo
IX_Enquiry_CustomerID
IX_Quotation_QuotationDate
IX_CustomerMaster_CustomerName
```

### 3. File Naming Conventions

#### C# Files
```
Controllers/
├── EnquiryController.cs
├── QuotationController.cs
├── CustomerMasterController.cs
└── SupplierController.cs

Models/
├── EnquiryVM.cs
├── QuotationVM.cs
├── CustomerMaster.cs
└── CommonFunctions.cs
```

#### View Files
```
Views/
├── Enquiry/
│   ├── Index.cshtml
│   ├── Create.cshtml
│   ├── Edit.cshtml
│   └── Details.cshtml
└── Quotation/
    ├── Index.cshtml
    ├── Create.cshtml
    └── Print.cshtml
```

## Code Organization

### 1. File Structure

```csharp
// 1. Using statements (alphabetical order)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HVAC.Models;
using HVAC.Common;

// 2. Namespace
namespace HVAC.Controllers
{
    // 3. Class declaration
    [SessionExpireFilter]
    public class EnquiryController : Controller
    {
        // 4. Private fields
        private readonly HVACEntities _db;
        private readonly ILoggingService _logger;
        
        // 5. Constructor
        public EnquiryController()
        {
            _db = new HVACEntities();
            _logger = new LoggingService();
        }
        
        // 6. Public properties
        public string PropertyName { get; set; }
        
        // 7. Public methods (alphabetical order)
        public ActionResult Create() { }
        public ActionResult Delete(int id) { }
        public ActionResult Edit(int id) { }
        public ActionResult Index() { }
        
        // 8. Private methods (alphabetical order)
        private void HelperMethod() { }
        private bool ValidateInput(string input) { }
    }
}
```

### 2. Method Organization

```csharp
public ActionResult Create(EnquiryVM model)
{
    // 1. Input validation
    if (!ModelState.IsValid)
    {
        return View(model);
    }
    
    // 2. Business logic validation
    if (string.IsNullOrEmpty(model.ProjectName))
    {
        ModelState.AddModelError("ProjectName", "Project name is required.");
        return View(model);
    }
    
    try
    {
        // 3. Data processing
        var enquiry = new Enquiry
        {
            ProjectName = model.ProjectName,
            ProjectDescription = model.ProjectDescription,
            EnquiryDate = DateTime.Now,
            CreatedBy = GetCurrentUserId(),
            CreatedDate = DateTime.Now
        };
        
        _db.Enquiries.Add(enquiry);
        _db.SaveChanges();
        
        // 4. Success response
        TempData["SuccessMsg"] = "Enquiry created successfully.";
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        // 5. Error handling
        _logger.LogError("Error creating enquiry", ex);
        ModelState.AddModelError("", "An error occurred while creating the enquiry.");
        return View(model);
    }
}
```

## C# Coding Standards

### 1. Variable Declarations

```csharp
// Good: Use var for obvious types
var enquiryList = new List<EnquiryVM>();
var customerName = "ABC Corporation";
var currentDate = DateTime.Now;

// Good: Explicit type for complex types
List<EnquiryVM> enquiryList = GetEnquiries();
Dictionary<string, int> statusCounts = GetStatusCounts();

// Bad: Unnecessary explicit types
string customerName = "ABC Corporation"; // Use var
List<EnquiryVM> enquiryList = new List<EnquiryVM>(); // Use var
```

### 2. Method Declarations

```csharp
// Good: Clear method signature
public ActionResult GetEnquiryList(DateTime fromDate, DateTime toDate, string enquiryNo = null)
{
    // Method implementation
}

// Good: Async methods
public async Task<ActionResult> GetEnquiryListAsync(DateTime fromDate, DateTime toDate)
{
    var enquiries = await _enquiryService.GetEnquiriesAsync(fromDate, toDate);
    return View(enquiries);
}

// Bad: Too many parameters
public ActionResult GetEnquiryList(DateTime fromDate, DateTime toDate, string enquiryNo, 
    int customerId, int statusId, int priorityId, int buildingTypeId, int enquiryTypeId)
{
    // Use a model class instead
}
```

### 3. Exception Handling

```csharp
// Good: Specific exception handling
try
{
    var enquiry = _db.Enquiries.Find(id);
    if (enquiry == null)
    {
        return HttpNotFound();
    }
    
    _db.Enquiries.Remove(enquiry);
    _db.SaveChanges();
    
    return RedirectToAction("Index");
}
catch (SqlException ex)
{
    _logger.LogError("Database error deleting enquiry", ex);
    ModelState.AddModelError("", "Database error occurred.");
    return View("Error");
}
catch (Exception ex)
{
    _logger.LogError("Unexpected error deleting enquiry", ex);
    ModelState.AddModelError("", "An unexpected error occurred.");
    return View("Error");
}

// Bad: Generic exception handling
try
{
    // Code
}
catch (Exception ex)
{
    // Always catches all exceptions
    throw ex; // Loses stack trace
}
```

### 4. LINQ Usage

```csharp
// Good: Readable LINQ queries
var activeEnquiries = _db.Enquiries
    .Where(e => e.StatusID == 1)
    .Where(e => e.EnquiryDate >= fromDate)
    .Where(e => e.EnquiryDate <= toDate)
    .OrderByDescending(e => e.EnquiryDate)
    .ToList();

// Good: Complex queries with joins
var enquiryDetails = (from e in _db.Enquiries
                     join c in _db.CustomerMasters on e.CustomerID equals c.CustomerID
                     join s in _db.EnquiryStatuses on e.StatusID equals s.StatusID
                     where e.EnquiryDate >= fromDate && e.EnquiryDate <= toDate
                     select new EnquiryVM
                     {
                         EnquiryID = e.EnquiryID,
                         EnquiryNo = e.EnquiryNo,
                         CustomerName = c.CustomerName,
                         StatusName = s.StatusName
                     }).ToList();

// Bad: Nested LINQ without proper formatting
var result = _db.Enquiries.Where(e => e.StatusID == 1).Where(e => e.EnquiryDate >= fromDate).Select(e => new { e.EnquiryID, e.EnquiryNo }).ToList();
```

## ASP.NET MVC Standards

### 1. Controller Standards

```csharp
[SessionExpireFilter]
public class EnquiryController : Controller
{
    private readonly HVACEntities _db;
    
    public EnquiryController()
    {
        _db = new HVACEntities();
    }
    
    // GET: Enquiry
    public ActionResult Index()
    {
        try
        {
            var enquiries = _db.Enquiries
                .Include(e => e.CustomerMaster)
                .Include(e => e.EnquiryStatus)
                .OrderByDescending(e => e.EnquiryDate)
                .ToList();
            
            return View(enquiries);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error loading enquiries", ex);
            return View("Error");
        }
    }
    
    // GET: Enquiry/Create
    public ActionResult Create()
    {
        var model = new EnquiryVM();
        PopulateDropdowns(model);
        return View(model);
    }
    
    // POST: Enquiry/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(EnquiryVM model)
    {
        if (!ModelState.IsValid)
        {
            PopulateDropdowns(model);
            return View(model);
        }
        
        try
        {
            var enquiry = MapToEntity(model);
            _db.Enquiries.Add(enquiry);
            _db.SaveChanges();
            
            TempData["SuccessMsg"] = "Enquiry created successfully.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating enquiry", ex);
            ModelState.AddModelError("", "An error occurred while creating the enquiry.");
            PopulateDropdowns(model);
            return View(model);
        }
    }
    
    private void PopulateDropdowns(EnquiryVM model)
    {
        model.Customers = _db.CustomerMasters.Where(c => c.IsActive).ToList();
        model.Statuses = _db.EnquiryStatuses.Where(s => s.IsActive).ToList();
        model.Priorities = _db.PriorityMasters.Where(p => p.IsActive).ToList();
    }
    
    private Enquiry MapToEntity(EnquiryVM model)
    {
        return new Enquiry
        {
            ProjectName = model.ProjectName,
            ProjectDescription = model.ProjectDescription,
            EnquiryDate = model.EnquiryDate,
            CustomerID = model.CustomerID,
            StatusID = model.StatusID,
            PriorityID = model.PriorityID,
            CreatedBy = GetCurrentUserId(),
            CreatedDate = DateTime.Now
        };
    }
}
```

### 2. View Standards

```html
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
                    <div class="card-tools">
                        @Html.ActionLink("Create New", "Create", null, new { @class = "btn btn-primary" })
                    </div>
                </div>
                <div class="card-body">
                    @if (TempData["SuccessMsg"] != null)
                    {
                        <div class="alert alert-success alert-dismissible fade show" role="alert">
                            @TempData["SuccessMsg"]
                            <button type="button" class="close" data-dismiss="alert">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                    }
                    
                    <div class="table-responsive">
                        <table class="table table-striped table-hover">
                            <thead class="thead-dark">
                                <tr>
                                    <th>@Html.DisplayNameFor(model => model.EnquiryNo)</th>
                                    <th>@Html.DisplayNameFor(model => model.EnquiryDate)</th>
                                    <th>@Html.DisplayNameFor(model => model.ProjectName)</th>
                                    <th>@Html.DisplayNameFor(model => model.CustomerName)</th>
                                    <th>@Html.DisplayNameFor(model => model.StatusName)</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                @foreach (var item in Model)
                                {
                                    <tr>
                                        <td>@Html.DisplayFor(modelItem => item.EnquiryNo)</td>
                                        <td>@Html.DisplayFor(modelItem => item.EnquiryDate)</td>
                                        <td>@Html.DisplayFor(modelItem => item.ProjectName)</td>
                                        <td>@Html.DisplayFor(modelItem => item.CustomerName)</td>
                                        <td>@Html.DisplayFor(modelItem => item.StatusName)</td>
                                        <td>
                                            @Html.ActionLink("Edit", "Edit", new { id = item.EnquiryID }, new { @class = "btn btn-sm btn-warning" })
                                            @Html.ActionLink("Details", "Details", new { id = item.EnquiryID }, new { @class = "btn btn-sm btn-info" })
                                            @Html.ActionLink("Delete", "Delete", new { id = item.EnquiryID }, new { @class = "btn btn-sm btn-danger" })
                                        </td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
```

## Database Standards

### 1. Stored Procedure Standards

```sql
-- Good: Clear procedure structure
CREATE PROCEDURE [dbo].[SP_GetEnquiryList]
    @FromDate DATETIME,
    @ToDate DATETIME,
    @EnquiryNo NVARCHAR(50) = NULL,
    @CustomerID INT = NULL,
    @StatusID INT = NULL,
    @BranchID INT = NULL,
    @YearID INT = NULL,
    @EmployeeID INT = NULL,
    @RoleID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Input validation
    IF @FromDate IS NULL OR @ToDate IS NULL
    BEGIN
        RAISERROR('FromDate and ToDate are required parameters', 16, 1);
        RETURN;
    END
    
    -- Main query
    SELECT 
        e.EnquiryID,
        e.EnquiryNo,
        e.EnquiryDate,
        e.ProjectName,
        e.ProjectDescription,
        c.CustomerName,
        s.StatusName,
        p.PriorityName
    FROM Enquiry e
    INNER JOIN CustomerMaster c ON e.CustomerID = c.CustomerID
    LEFT JOIN EnquiryStatus s ON e.StatusID = s.StatusID
    LEFT JOIN PriorityMaster p ON e.PriorityID = p.PriorityID
    WHERE e.EnquiryDate BETWEEN @FromDate AND @ToDate
    AND (@EnquiryNo IS NULL OR e.EnquiryNo LIKE '%' + @EnquiryNo + '%')
    AND (@CustomerID IS NULL OR e.CustomerID = @CustomerID)
    AND (@StatusID IS NULL OR e.StatusID = @StatusID)
    AND (@BranchID IS NULL OR e.BranchID = @BranchID)
    ORDER BY e.EnquiryDate DESC;
END
```

### 2. Query Standards

```sql
-- Good: Readable query with proper formatting
SELECT 
    e.EnquiryID,
    e.EnquiryNo,
    e.EnquiryDate,
    e.ProjectName,
    c.CustomerName,
    s.StatusName,
    p.PriorityName
FROM Enquiry e
INNER JOIN CustomerMaster c ON e.CustomerID = c.CustomerID
LEFT JOIN EnquiryStatus s ON e.StatusID = s.StatusID
LEFT JOIN PriorityMaster p ON e.PriorityID = p.PriorityID
WHERE e.EnquiryDate >= @FromDate
AND e.EnquiryDate <= @ToDate
AND e.BranchID = @BranchID
ORDER BY e.EnquiryDate DESC;

-- Bad: Poorly formatted query
SELECT e.EnquiryID,e.EnquiryNo,e.EnquiryDate,e.ProjectName,c.CustomerName,s.StatusName,p.PriorityName FROM Enquiry e INNER JOIN CustomerMaster c ON e.CustomerID=c.CustomerID LEFT JOIN EnquiryStatus s ON e.StatusID=s.StatusID LEFT JOIN PriorityMaster p ON e.PriorityID=p.PriorityID WHERE e.EnquiryDate>=@FromDate AND e.EnquiryDate<=@ToDate AND e.BranchID=@BranchID ORDER BY e.EnquiryDate DESC;
```

## JavaScript Standards

### 1. Variable Declarations

```javascript
// Good: Use const and let
const enquiryId = 123;
let enquiryList = [];
var globalVariable = 'avoid var';

// Good: Descriptive variable names
const enquiryTable = $('#enquiryTable');
const customerDropdown = $('#customerDropdown');
const statusFilter = $('#statusFilter');

// Bad: Unclear variable names
const id = 123;
const list = [];
const el = $('#table');
```

### 2. Function Declarations

```javascript
// Good: Function declaration
function loadEnquiryList() {
    // Function implementation
}

// Good: Arrow function for callbacks
const loadEnquiryList = () => {
    // Function implementation
};

// Good: Async function
async function loadEnquiryListAsync() {
    try {
        const response = await fetch('/Enquiry/GetEnquiryList');
        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error loading enquiry list:', error);
        throw error;
    }
}

// Bad: Inline functions without proper error handling
$('#btnLoad').click(function() { $.get('/Enquiry/GetEnquiryList', function(data) { $('#table').html(data); }); });
```

### 3. jQuery Standards

```javascript
// Good: Proper jQuery usage
$(document).ready(function() {
    initializeEnquiryTable();
    bindEvents();
});

function initializeEnquiryTable() {
    $('#enquiryTable').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Enquiry/GetEnquiryList',
            type: 'POST'
        },
        columns: [
            { data: 'enquiryNo' },
            { data: 'enquiryDate' },
            { data: 'projectName' },
            { data: 'customerName' },
            { data: 'statusName' }
        ]
    });
}

function bindEvents() {
    $('#btnCreate').on('click', function() {
        window.location.href = '/Enquiry/Create';
    });
    
    $('#btnExport').on('click', function() {
        exportToExcel();
    });
}
```

## CSS Standards

### 1. Class Naming

```css
/* Good: BEM methodology */
.enquiry-card { }
.enquiry-card__header { }
.enquiry-card__body { }
.enquiry-card__footer { }
.enquiry-card--highlighted { }

/* Good: Semantic class names */
.btn-primary { }
.btn-secondary { }
.table-striped { }
.alert-success { }

/* Bad: Unclear class names */
.red { }
.big { }
.left { }
```

### 2. CSS Organization

```css
/* 1. Reset and base styles */
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
}

/* 2. Layout styles */
.container { }
.row { }
.col-12 { }

/* 3. Component styles */
.card { }
.card-header { }
.card-body { }
.card-footer { }

/* 4. Utility classes */
.text-center { }
.text-right { }
.mb-3 { }
.mt-3 { }

/* 5. Responsive styles */
@media (max-width: 768px) {
    .container {
        padding: 0 15px;
    }
}
```

## Documentation Standards

### 1. XML Documentation

```csharp
/// <summary>
/// Retrieves a list of enquiries based on specified criteria
/// </summary>
/// <param name="fromDate">Start date for filtering enquiries</param>
/// <param name="toDate">End date for filtering enquiries</param>
/// <param name="enquiryNo">Optional enquiry number filter</param>
/// <param name="customerId">Optional customer ID filter</param>
/// <returns>List of enquiry view models</returns>
/// <exception cref="ArgumentException">Thrown when fromDate is greater than toDate</exception>
public List<EnquiryVM> GetEnquiryList(DateTime fromDate, DateTime toDate, 
    string enquiryNo = null, int? customerId = null)
{
    // Method implementation
}
```

### 2. Inline Comments

```csharp
public ActionResult Create(EnquiryVM model)
{
    // Validate model state before processing
    if (!ModelState.IsValid)
    {
        return View(model);
    }
    
    try
    {
        // Map view model to entity
        var enquiry = new Enquiry
        {
            ProjectName = model.ProjectName,
            ProjectDescription = model.ProjectDescription,
            EnquiryDate = DateTime.Now,
            CreatedBy = GetCurrentUserId(),
            CreatedDate = DateTime.Now
        };
        
        // Save to database
        _db.Enquiries.Add(enquiry);
        _db.SaveChanges();
        
        // Redirect to index with success message
        TempData["SuccessMsg"] = "Enquiry created successfully.";
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        // Log error and return to view with error message
        _logger.LogError("Error creating enquiry", ex);
        ModelState.AddModelError("", "An error occurred while creating the enquiry.");
        return View(model);
    }
}
```

## Error Handling

### 1. Exception Handling Strategy

```csharp
// Good: Specific exception handling
try
{
    var enquiry = _db.Enquiries.Find(id);
    if (enquiry == null)
    {
        return HttpNotFound();
    }
    
    _db.Enquiries.Remove(enquiry);
    _db.SaveChanges();
    
    return RedirectToAction("Index");
}
catch (SqlException ex)
{
    _logger.LogError("Database error deleting enquiry", ex);
    ModelState.AddModelError("", "Database error occurred. Please try again.");
    return View("Error");
}
catch (Exception ex)
{
    _logger.LogError("Unexpected error deleting enquiry", ex);
    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
    return View("Error");
}

// Bad: Generic exception handling
try
{
    // Code
}
catch (Exception ex)
{
    throw ex; // Loses stack trace
}
```

### 2. Validation Error Handling

```csharp
// Good: Comprehensive validation
public ActionResult Create(EnquiryVM model)
{
    // Model state validation
    if (!ModelState.IsValid)
    {
        PopulateDropdowns(model);
        return View(model);
    }
    
    // Business rule validation
    if (model.EnquiryDate > DateTime.Now)
    {
        ModelState.AddModelError("EnquiryDate", "Enquiry date cannot be in the future.");
        PopulateDropdowns(model);
        return View(model);
    }
    
    if (string.IsNullOrWhiteSpace(model.ProjectName))
    {
        ModelState.AddModelError("ProjectName", "Project name is required.");
        PopulateDropdowns(model);
        return View(model);
    }
    
    // Process valid model
    // ...
}
```

## Security Standards

### 1. Input Validation

```csharp
// Good: Input validation
public ActionResult Create(EnquiryVM model)
{
    // Validate required fields
    if (string.IsNullOrWhiteSpace(model.ProjectName))
    {
        ModelState.AddModelError("ProjectName", "Project name is required.");
    }
    
    // Validate string length
    if (model.ProjectName.Length > 200)
    {
        ModelState.AddModelError("ProjectName", "Project name cannot exceed 200 characters.");
    }
    
    // Validate date range
    if (model.EnquiryDate > DateTime.Now)
    {
        ModelState.AddModelError("EnquiryDate", "Enquiry date cannot be in the future.");
    }
    
    // Validate numeric ranges
    if (model.CustomerID <= 0)
    {
        ModelState.AddModelError("CustomerID", "Please select a valid customer.");
    }
}

// Bad: No input validation
public ActionResult Create(EnquiryVM model)
{
    var enquiry = new Enquiry
    {
        ProjectName = model.ProjectName, // No validation
        EnquiryDate = model.EnquiryDate, // No validation
        CustomerID = model.CustomerID    // No validation
    };
    
    _db.Enquiries.Add(enquiry);
    _db.SaveChanges();
    
    return RedirectToAction("Index");
}
```

### 2. SQL Injection Prevention

```csharp
// Good: Parameterized queries
public List<EnquiryVM> GetEnquiries(string enquiryNo)
{
    using (var connection = new SqlConnection(connectionString))
    using (var command = new SqlCommand())
    {
        command.Connection = connection;
        command.CommandText = "SELECT * FROM Enquiry WHERE EnquiryNo = @EnquiryNo";
        command.Parameters.AddWithValue("@EnquiryNo", enquiryNo);
        
        // Execute query
    }
}

// Bad: String concatenation (SQL injection risk)
public List<EnquiryVM> GetEnquiries(string enquiryNo)
{
    var query = "SELECT * FROM Enquiry WHERE EnquiryNo = '" + enquiryNo + "'";
    // SQL injection risk
}
```

## Performance Standards

### 1. Database Performance

```csharp
// Good: Efficient queries
public List<EnquiryVM> GetEnquiries(DateTime fromDate, DateTime toDate)
{
    return _db.Enquiries
        .Where(e => e.EnquiryDate >= fromDate && e.EnquiryDate <= toDate)
        .Select(e => new EnquiryVM
        {
            EnquiryID = e.EnquiryID,
            EnquiryNo = e.EnquiryNo,
            EnquiryDate = e.EnquiryDate,
            ProjectName = e.ProjectName,
            CustomerName = e.CustomerMaster.CustomerName,
            StatusName = e.EnquiryStatus.StatusName
        })
        .ToList();
}

// Bad: N+1 query problem
public List<EnquiryVM> GetEnquiries(DateTime fromDate, DateTime toDate)
{
    var enquiries = _db.Enquiries
        .Where(e => e.EnquiryDate >= fromDate && e.EnquiryDate <= toDate)
        .ToList();
    
    var result = new List<EnquiryVM>();
    foreach (var enquiry in enquiries)
    {
        result.Add(new EnquiryVM
        {
            EnquiryID = enquiry.EnquiryID,
            EnquiryNo = enquiry.EnquiryNo,
            CustomerName = enquiry.CustomerMaster.CustomerName, // N+1 query
            StatusName = enquiry.EnquiryStatus.StatusName        // N+1 query
        });
    }
    
    return result;
}
```

### 2. Caching

```csharp
// Good: Output caching
[OutputCache(Duration = 300, VaryByParam = "none")]
public ActionResult Index()
{
    var enquiries = GetEnquiries();
    return View(enquiries);
}

// Good: Data caching
public List<CustomerVM> GetCustomers()
{
    var cacheKey = "CustomerList";
    var customers = HttpContext.Cache[cacheKey] as List<CustomerVM>;
    
    if (customers == null)
    {
        customers = _db.CustomerMasters
            .Where(c => c.IsActive)
            .Select(c => new CustomerVM
            {
                CustomerID = c.CustomerID,
                CustomerName = c.CustomerName
            })
            .ToList();
        
        HttpContext.Cache.Insert(cacheKey, customers, null, 
            DateTime.Now.AddMinutes(30), TimeSpan.Zero);
    }
    
    return customers;
}
```

## Testing Standards

### 1. Unit Testing

```csharp
[TestClass]
public class EnquiryControllerTests
{
    private Mock<HVACEntities> _mockDb;
    private EnquiryController _controller;
    
    [TestInitialize]
    public void Setup()
    {
        _mockDb = new Mock<HVACEntities>();
        _controller = new EnquiryController();
    }
    
    [TestMethod]
    public void Index_ShouldReturnViewWithEnquiries()
    {
        // Arrange
        var enquiries = new List<Enquiry>
        {
            new Enquiry { EnquiryID = 1, EnquiryNo = "ENQ-001" },
            new Enquiry { EnquiryID = 2, EnquiryNo = "ENQ-002" }
        };
        
        _mockDb.Setup(db => db.Enquiries).Returns(enquiries.AsQueryable());
        
        // Act
        var result = _controller.Index();
        
        // Assert
        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult.Model);
    }
    
    [TestMethod]
    public void Create_WithValidModel_ShouldRedirectToIndex()
    {
        // Arrange
        var model = new EnquiryVM
        {
            ProjectName = "Test Project",
            EnquiryDate = DateTime.Now,
            CustomerID = 1
        };
        
        // Act
        var result = _controller.Create(model);
        
        // Assert
        Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        var redirectResult = result as RedirectToRouteResult;
        Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
    }
}
```

### 2. Integration Testing

```csharp
[TestClass]
public class EnquiryIntegrationTests
{
    private TestContext _testContext;
    
    [TestInitialize]
    public void Setup()
    {
        _testContext = new TestContext();
    }
    
    [TestMethod]
    public void CreateEnquiry_ShouldSaveToDatabase()
    {
        // Arrange
        var model = new EnquiryVM
        {
            ProjectName = "Integration Test Project",
            EnquiryDate = DateTime.Now,
            CustomerID = 1
        };
        
        // Act
        var result = _testContext.Controller.Create(model);
        
        // Assert
        var enquiry = _testContext.Db.Enquiries
            .FirstOrDefault(e => e.ProjectName == "Integration Test Project");
        Assert.IsNotNull(enquiry);
        Assert.AreEqual("Integration Test Project", enquiry.ProjectName);
    }
}
```

---

*These coding standards ensure consistent, maintainable, and high-quality code across the HVAC Management System development team.*
