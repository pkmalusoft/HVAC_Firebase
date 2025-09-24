# HVAC Management System - API Documentation

## Table of Contents
1. [API Overview](#api-overview)
2. [Authentication](#authentication)
3. [Controller APIs](#controller-apis)
4. [Data Transfer Objects](#data-transfer-objects)
5. [Response Formats](#response-formats)
6. [Error Handling](#error-handling)
7. [Rate Limiting](#rate-limiting)
8. [API Examples](#api-examples)
9. [Testing](#testing)
10. [Versioning](#versioning)

## API Overview

The HVAC Management System provides a comprehensive RESTful API built on ASP.NET MVC, offering endpoints for all major business operations including enquiry management, quotation generation, purchase orders, and reporting.

### Base URL
```
Production: https://hvac.company.com/api/
Development: http://localhost:port/api/
```

### API Characteristics
- **Protocol**: HTTP/HTTPS
- **Format**: JSON
- **Authentication**: Session-based
- **Content-Type**: application/json
- **Character Encoding**: UTF-8

## Authentication

### Session-Based Authentication

All API endpoints require valid session authentication. The system uses ASP.NET session state for user authentication.

#### Login Endpoint
```http
POST /Login/Login
Content-Type: application/x-www-form-urlencoded

UserName=admin&Password=password123
```

#### Response
```json
{
  "success": true,
  "message": "Login successful",
  "redirectUrl": "/Dashboard/Index",
  "userInfo": {
    "userId": 1,
    "userName": "admin",
    "roleId": 1,
    "roleName": "Administrator"
  }
}
```

#### Session Management
- **Session Timeout**: 30 minutes
- **Session Cookie**: HVAC_Session
- **Auto-Renewal**: Enabled
- **Logout**: Automatic on timeout

## Controller APIs

### 1. Home Controller

#### Dashboard Data
```http
GET /Home/Home
```

**Response:**
```json
{
  "companyName": "HVAC Management Company",
  "contactPerson": "John Smith",
  "contactTelephone": "555-0123",
  "email": "info@hvac.com",
  "branchCount": 5,
  "acceptSystem": true
}
```

#### File Upload
```http
POST /Home/UploadFiles
Content-Type: multipart/form-data

file: [binary data]
```

**Response:**
```json
{
  "success": true,
  "message": "File uploaded successfully",
  "fileName": "document.pdf",
  "fileUrl": "https://s3.us-west-1.wasabisys.com/hvac-bucket/documents/document.pdf",
  "fileSize": 1024000
}
```

### 2. Enquiry Controller

#### Get Enquiry List
```http
GET /Enquiry/Index
```

**Query Parameters:**
- `fromDate` (optional): Start date filter (yyyy-MM-dd)
- `toDate` (optional): End date filter (yyyy-MM-dd)
- `enquiryNo` (optional): Enquiry number filter
- `customerId` (optional): Customer ID filter
- `statusId` (optional): Status ID filter

**Response:**
```json
{
  "enquiries": [
    {
      "enquiryId": 1,
      "enquiryNo": "ENQ-2024-001",
      "enquiryDate": "2024-09-24T10:30:00",
      "projectName": "Office Building HVAC",
      "projectDescription": "Complete HVAC system for 10-story office building",
      "customerName": "ABC Corporation",
      "statusName": "In Progress",
      "priorityName": "High",
      "quotationValue": 150000.00
    }
  ],
  "totalCount": 1,
  "pageSize": 10,
  "currentPage": 1
}
```

#### Create Enquiry
```http
POST /Enquiry/Create
Content-Type: application/json

{
  "branchId": 1,
  "customerId": 1,
  "projectName": "New Project",
  "projectDescription": "Project description",
  "enquiryDate": "2024-09-24T10:30:00",
  "dueDate": "2024-10-24T10:30:00",
  "priorityId": 2,
  "buildingTypeId": 1,
  "enquiryTypeId": 1
}
```

**Response:**
```json
{
  "success": true,
  "message": "Enquiry created successfully",
  "enquiryId": 123,
  "enquiryNo": "ENQ-2024-123"
}
```

#### Update Enquiry
```http
POST /Enquiry/Edit
Content-Type: application/json

{
  "enquiryId": 123,
  "projectName": "Updated Project Name",
  "projectDescription": "Updated description",
  "statusId": 2
}
```

#### Delete Enquiry
```http
POST /Enquiry/Delete
Content-Type: application/json

{
  "enquiryId": 123
}
```

### 3. Quotation Controller

#### Get Quotation List
```http
GET /Quotation/Index
```

**Query Parameters:**
- `fromDate` (optional): Start date filter
- `toDate` (optional): End date filter
- `quotationNo` (optional): Quotation number filter
- `enquiryNo` (optional): Enquiry number filter
- `statusId` (optional): Status ID filter

**Response:**
```json
{
  "quotations": [
    {
      "quotationId": 1,
      "quotationNo": "QUO-2024-001",
      "quotationDate": "2024-09-24T10:30:00",
      "enquiryNo": "ENQ-2024-001",
      "customerName": "ABC Corporation",
      "quotationValue": 150000.00,
      "netAmount": 135000.00,
      "statusName": "Sent",
      "validity": 30
    }
  ],
  "totalCount": 1
}
```

#### Create Quotation
```http
POST /Quotation/Create
Content-Type: application/json

{
  "enquiryId": 1,
  "quotationValue": 150000.00,
  "discountAmount": 15000.00,
  "discountPercent": 10.00,
  "vatPercent": 5.00,
  "vatAmount": 7500.00,
  "validity": 30,
  "deliveryTerms": "30 days",
  "paymentTerms": "Net 30",
  "termsAndConditions": "Standard terms apply"
}
```

#### Generate Quotation PDF
```http
GET /Quotation/Print?id=123
```

**Response:** PDF file download

### 4. Purchase Order Controller

#### Get Purchase Order List
```http
GET /PurchaseOrder/Index
```

**Response:**
```json
{
  "purchaseOrders": [
    {
      "purchaseOrderId": 1,
      "poNumber": "PO-2024-001",
      "poDate": "2024-09-24T10:30:00",
      "supplierName": "XYZ Suppliers",
      "poValue": 50000.00,
      "statusName": "Pending",
      "expectedDelivery": "2024-10-24T10:30:00"
    }
  ]
}
```

#### Create Purchase Order
```http
POST /PurchaseOrder/Create
Content-Type: application/json

{
  "supplierId": 1,
  "poDate": "2024-09-24T10:30:00",
  "expectedDelivery": "2024-10-24T10:30:00",
  "remarks": "Urgent delivery required",
  "details": [
    {
      "productId": 1,
      "quantity": 10,
      "unitPrice": 1000.00,
      "totalPrice": 10000.00
    }
  ]
}
```

### 5. Customer Controller

#### Get Customer List
```http
GET /CustomerMaster/Index
```

**Response:**
```json
{
  "customers": [
    {
      "customerId": 1,
      "customerName": "ABC Corporation",
      "contactPerson": "John Doe",
      "phone": "555-0123",
      "email": "john@abc.com",
      "city": "New York",
      "country": "USA",
      "gstNumber": "GST123456789"
    }
  ]
}
```

#### Create Customer
```http
POST /CustomerMaster/Create
Content-Type: application/json

{
  "customerName": "New Customer",
  "contactPerson": "Jane Smith",
  "address": "123 Main Street",
  "city": "Los Angeles",
  "state": "California",
  "country": "USA",
  "phone": "555-0456",
  "email": "jane@newcustomer.com",
  "gstNumber": "GST987654321"
}
```

### 6. Supplier Controller

#### Get Supplier List
```http
GET /Supplier/Index
```

**Response:**
```json
{
  "suppliers": [
    {
      "supplierId": 1,
      "supplierName": "XYZ Suppliers",
      "contactPerson": "Bob Johnson",
      "phone": "555-0789",
      "email": "bob@xyz.com",
      "address": "456 Supplier Street",
      "city": "Chicago",
      "supplierType": "Equipment Supplier"
    }
  ]
}
```

### 7. Reports Controller

#### Generate Dashboard Report
```http
GET /Reports/Dashboard
```

**Response:**
```json
{
  "summary": {
    "totalEnquiries": 150,
    "totalQuotations": 120,
    "totalOrders": 80,
    "totalRevenue": 2500000.00
  },
  "recentEnquiries": [
    {
      "enquiryId": 1,
      "enquiryNo": "ENQ-2024-001",
      "projectName": "Office Building HVAC",
      "customerName": "ABC Corporation",
      "enquiryDate": "2024-09-24T10:30:00"
    }
  ],
  "monthlyData": [
    {
      "month": "January",
      "enquiries": 25,
      "quotations": 20,
      "orders": 15
    }
  ]
}
```

#### Export Report
```http
GET /Reports/Export?reportType=enquiry&format=excel&fromDate=2024-01-01&toDate=2024-12-31
```

**Response:** Excel file download

## Data Transfer Objects

### Common DTOs

#### EnquiryVM
```json
{
  "enquiryId": 1,
  "enquiryNo": "ENQ-2024-001",
  "enquiryDate": "2024-09-24T10:30:00",
  "projectName": "Office Building HVAC",
  "projectDescription": "Complete HVAC system",
  "projectNumber": "PRJ-001",
  "customerId": 1,
  "customerName": "ABC Corporation",
  "cityId": 1,
  "cityName": "New York",
  "countryId": 1,
  "countryName": "USA",
  "dueDate": "2024-10-24T10:30:00",
  "priorityId": 2,
  "priorityName": "High",
  "buildingTypeId": 1,
  "buildingTypeName": "Commercial",
  "enquiryTypeId": 1,
  "enquiryTypeName": "New Installation",
  "statusId": 1,
  "statusName": "In Progress",
  "createdBy": 1,
  "createdDate": "2024-09-24T10:30:00",
  "modifiedBy": 1,
  "modifiedDate": "2024-09-24T10:30:00"
}
```

#### QuotationVM
```json
{
  "quotationId": 1,
  "quotationNo": "QUO-2024-001",
  "quotationDate": "2024-09-24T10:30:00",
  "enquiryId": 1,
  "enquiryNo": "ENQ-2024-001",
  "customerId": 1,
  "customerName": "ABC Corporation",
  "quotationValue": 150000.00,
  "discountAmount": 15000.00,
  "discountPercent": 10.00,
  "vatPercent": 5.00,
  "vatAmount": 7500.00,
  "netAmount": 135000.00,
  "quotationStatusId": 2,
  "quotationStatus": "Sent",
  "validity": 30,
  "deliveryTerms": "30 days",
  "paymentTerms": "Net 30",
  "termsAndConditions": "Standard terms apply",
  "createdBy": 1,
  "createdDate": "2024-09-24T10:30:00"
}
```

## Response Formats

### Success Response
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {
    // Response data
  },
  "timestamp": "2024-09-24T10:30:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error description",
  "errorCode": "VALIDATION_ERROR",
  "details": [
    {
      "field": "customerName",
      "message": "Customer name is required"
    }
  ],
  "timestamp": "2024-09-24T10:30:00Z"
}
```

### Pagination Response
```json
{
  "data": [
    // Array of items
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

## Error Handling

### HTTP Status Codes

| Code | Description | Usage |
|------|-------------|-------|
| **200** | OK | Successful GET, PUT, PATCH |
| **201** | Created | Successful POST |
| **204** | No Content | Successful DELETE |
| **400** | Bad Request | Invalid request data |
| **401** | Unauthorized | Authentication required |
| **403** | Forbidden | Insufficient permissions |
| **404** | Not Found | Resource not found |
| **409** | Conflict | Duplicate resource |
| **422** | Unprocessable Entity | Validation errors |
| **500** | Internal Server Error | Server error |

### Error Codes

| Code | Description |
|------|-------------|
| **VALIDATION_ERROR** | Input validation failed |
| **AUTHENTICATION_REQUIRED** | User not authenticated |
| **AUTHORIZATION_FAILED** | Insufficient permissions |
| **RESOURCE_NOT_FOUND** | Requested resource not found |
| **DUPLICATE_RESOURCE** | Resource already exists |
| **DATABASE_ERROR** | Database operation failed |
| **FILE_UPLOAD_ERROR** | File upload failed |
| **EXTERNAL_SERVICE_ERROR** | External service unavailable |

## Rate Limiting

### Current Implementation
- **Session-based**: Rate limiting per user session
- **Request Limit**: 1000 requests per hour per user
- **Burst Limit**: 100 requests per minute
- **File Upload**: 10 uploads per hour per user

### Rate Limit Headers
```http
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1640995200
```

## API Examples

### Complete Enquiry Workflow

#### 1. Create Enquiry
```bash
curl -X POST "https://hvac.company.com/Enquiry/Create" \
  -H "Content-Type: application/json" \
  -d '{
    "branchId": 1,
    "customerId": 1,
    "projectName": "Office Building HVAC",
    "projectDescription": "Complete HVAC system for 10-story office building",
    "enquiryDate": "2024-09-24T10:30:00",
    "dueDate": "2024-10-24T10:30:00",
    "priorityId": 2,
    "buildingTypeId": 1,
    "enquiryTypeId": 1
  }'
```

#### 2. Create Quotation
```bash
curl -X POST "https://hvac.company.com/Quotation/Create" \
  -H "Content-Type: application/json" \
  -d '{
    "enquiryId": 123,
    "quotationValue": 150000.00,
    "discountAmount": 15000.00,
    "discountPercent": 10.00,
    "vatPercent": 5.00,
    "vatAmount": 7500.00,
    "validity": 30,
    "deliveryTerms": "30 days",
    "paymentTerms": "Net 30"
  }'
```

#### 3. Generate PDF
```bash
curl -X GET "https://hvac.company.com/Quotation/Print?id=123" \
  -H "Accept: application/pdf" \
  --output quotation.pdf
```

### File Upload Example

```bash
curl -X POST "https://hvac.company.com/Home/UploadFiles" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@document.pdf"
```

## Testing

### API Testing Tools

#### Postman Collection
```json
{
  "info": {
    "name": "HVAC Management System API",
    "description": "Complete API collection for HVAC system",
    "version": "1.0.0"
  },
  "item": [
    {
      "name": "Authentication",
      "item": [
        {
          "name": "Login",
          "request": {
            "method": "POST",
            "url": "{{baseUrl}}/Login/Login",
            "body": {
              "mode": "urlencoded",
              "urlencoded": [
                {"key": "UserName", "value": "admin"},
                {"key": "Password", "value": "password123"}
              ]
            }
          }
        }
      ]
    }
  ]
}
```

#### Unit Testing Example
```csharp
[Test]
public void GetEnquiryList_ShouldReturnEnquiries()
{
    // Arrange
    var controller = new EnquiryController();
    var mockService = new Mock<IEnquiryService>();
    mockService.Setup(s => s.GetEnquiries(It.IsAny<EnquirySearch>()))
               .Returns(new List<EnquiryVM> { new EnquiryVM { EnquiryID = 1 } });
    
    // Act
    var result = controller.Index();
    
    // Assert
    Assert.IsInstanceOf<ViewResult>(result);
    var viewResult = result as ViewResult;
    Assert.IsNotNull(viewResult.Model);
}
```

## Versioning

### Current Version
- **API Version**: 1.0.0
- **Supported Versions**: 1.0.x
- **Deprecation Policy**: 6 months notice for breaking changes

### Version Headers
```http
API-Version: 1.0
Accept-Version: 1.0
```

### Versioning Strategy
- **URL Versioning**: `/api/v1/enquiry`
- **Header Versioning**: `API-Version: 1.0`
- **Backward Compatibility**: Maintained for 2 major versions

---

*This API documentation provides comprehensive information for integrating with the HVAC Management System's RESTful API.*
