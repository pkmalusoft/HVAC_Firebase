# HVAC Management System - System Overview

## Table of Contents
1. [Introduction](#introduction)
2. [System Purpose](#system-purpose)
3. [Key Features](#key-features)
4. [User Roles](#user-roles)
5. [Business Processes](#business-processes)
6. [System Modules](#system-modules)
7. [Integration Points](#integration-points)

## Introduction

The HVAC Management System is a comprehensive web-based application designed to manage Heating, Ventilation, and Air Conditioning (HVAC) business operations. Built on ASP.NET MVC framework, it provides end-to-end management of enquiries, quotations, purchase orders, inventory, and financial transactions.

## System Purpose

The system serves as a centralized platform for HVAC businesses to:
- Manage customer enquiries and quotations
- Track project progress and job handovers
- Handle purchase orders and supplier management
- Maintain inventory and material tracking
- Process financial transactions and reporting
- Generate comprehensive business reports

## Key Features

### 1. **Enquiry Management**
- Customer enquiry creation and tracking
- Project details and specifications
- Priority and status management
- Document attachment support

### 2. **Quotation System**
- Automated quotation generation
- Pricing calculations with margins
- Terms and conditions management
- Revision and approval workflows

### 3. **Purchase Order Management**
- Supplier management and selection
- Purchase order creation and tracking
- Material requirement planning
- Delivery and receipt management

### 4. **Inventory Management**
- Material tracking and stock levels
- Issue and receipt transactions
- Location-based inventory
- Stock valuation and reporting

### 5. **Financial Management**
- Invoice generation and tracking
- Payment processing
- Credit note management
- Financial reporting and analytics

### 6. **Project Management**
- Job handover tracking
- Project status monitoring
- Resource allocation
- Timeline management

### 7. **Reporting & Analytics**
- Dashboard with key metrics
- Custom report generation
- Export capabilities (PDF, Excel)
- Business intelligence insights

## User Roles

### 1. **Administrator**
- Full system access
- User management
- System configuration
- Master data management

### 2. **Manager**
- Business process oversight
- Report generation
- Approval workflows
- Team management

### 3. **Sales Executive**
- Enquiry management
- Quotation creation
- Customer communication
- Sales reporting

### 4. **Purchase Manager**
- Supplier management
- Purchase order processing
- Material planning
- Vendor evaluation

### 5. **Project Manager**
- Project oversight
- Resource allocation
- Progress tracking
- Client communication

### 6. **Accountant**
- Financial transactions
- Invoice processing
- Payment tracking
- Financial reporting

## Business Processes

### 1. **Enquiry to Quotation Process**
```
Customer Enquiry → Project Assessment → Quotation Creation → 
Customer Approval → Project Initiation
```

### 2. **Purchase Order Process**
```
Material Requirement → Supplier Selection → Purchase Order → 
Delivery Tracking → Receipt Processing → Invoice Matching
```

### 3. **Project Execution Process**
```
Project Handover → Resource Allocation → Progress Tracking → 
Quality Control → Project Completion → Client Handover
```

### 4. **Financial Process**
```
Invoice Generation → Payment Tracking → Credit Management → 
Financial Reporting → Audit Trail
```

## System Modules

### 1. **Master Data Module**
- Company management
- Customer master
- Supplier master
- Product catalog
- Location management

### 2. **Enquiry Module**
- Enquiry creation
- Project specifications
- Status tracking
- Document management

### 3. **Quotation Module**
- Quotation generation
- Pricing management
- Terms and conditions
- Approval workflow

### 4. **Purchase Module**
- Purchase order management
- Supplier relations
- Material planning
- Delivery tracking

### 5. **Inventory Module**
- Stock management
- Material tracking
- Issue and receipt
- Valuation

### 6. **Project Module**
- Job handover
- Progress tracking
- Resource management
- Timeline control

### 7. **Financial Module**
- Invoice management
- Payment processing
- Credit management
- Financial reporting

### 8. **Reporting Module**
- Dashboard analytics
- Custom reports
- Export functionality
- Business intelligence

## Integration Points

### 1. **AWS S3 Integration**
- **Purpose**: File storage and management
- **Services**: Document uploads, image storage
- **Configuration**: Wasabi S3 compatible storage

### 2. **Email Integration**
- **Purpose**: Automated notifications
- **Services**: Quotation sending, status updates
- **Configuration**: SMTP server integration

### 3. **Crystal Reports Integration**
- **Purpose**: Report generation
- **Services**: PDF reports, formatted documents
- **Configuration**: Report templates and data sources

### 4. **Database Integration**
- **Purpose**: Data persistence
- **Services**: SQL Server database
- **Configuration**: Entity Framework ORM

## System Architecture

The system follows a three-tier architecture:

1. **Presentation Layer**: ASP.NET MVC Views and Controllers
2. **Business Logic Layer**: Service classes and business rules
3. **Data Access Layer**: Entity Framework and custom DAO classes

## Technology Stack

- **Framework**: .NET Framework 4.8
- **Web Framework**: ASP.NET MVC 5
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework 6
- **Reporting**: Crystal Reports
- **Cloud Storage**: AWS S3 (Wasabi)
- **Frontend**: HTML5, CSS3, JavaScript, jQuery
- **UI Framework**: Bootstrap

## Security Features

- **Authentication**: Session-based authentication
- **Authorization**: Role-based access control
- **Data Protection**: Input validation and sanitization
- **CSRF Protection**: Anti-forgery tokens
- **XSS Prevention**: Output encoding
- **SQL Injection Prevention**: Parameterized queries

## Performance Features

- **Caching**: Output caching and data caching
- **Database Optimization**: Connection pooling and query optimization
- **Async Operations**: Asynchronous processing for better performance
- **Resource Management**: Proper disposal of database connections

## Maintenance and Support

- **Logging**: Comprehensive logging framework
- **Error Handling**: Global exception handling
- **Monitoring**: Performance monitoring capabilities
- **Backup**: Database backup and recovery procedures

---

*This document provides a high-level overview of the HVAC Management System. For detailed technical information, refer to the specific documentation sections.*
