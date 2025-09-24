# HVAC Management System - Installation Guide

## Table of Contents
1. [Installation Overview](#installation-overview)
2. [Pre-Installation Steps](#pre-installation-steps)
3. [Database Setup](#database-setup)
4. [Application Installation](#application-installation)
5. [Configuration](#configuration)
6. [Post-Installation Steps](#post-installation-steps)
7. [Verification](#verification)
8. [Troubleshooting](#troubleshooting)

## Installation Overview

This guide provides step-by-step instructions for installing the HVAC Management System on a Windows Server environment. The installation process involves database setup, application deployment, and configuration.

### Installation Types

1. **Fresh Installation**: New system installation
2. **Upgrade Installation**: Upgrading existing system
3. **Development Installation**: Development environment setup

## Pre-Installation Steps

### 1. Verify Prerequisites

Ensure all prerequisites from the [Prerequisites Guide](Prerequisites.md) are met:

```powershell
# Check .NET Framework version
Get-ItemProperty "HKLM:SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\" -Name Release

# Check IIS installation
Get-WindowsFeature -Name IIS-WebServerRole

# Check SQL Server installation
Get-Service -Name "MSSQLSERVER"
```

### 2. Prepare Installation Directory

```powershell
# Create application directory
New-Item -ItemType Directory -Path "C:\HVAC_Application" -Force

# Set permissions
icacls "C:\HVAC_Application" /grant "IIS_IUSRS:(OI)(CI)F" /T
icacls "C:\HVAC_Application" /grant "IIS AppPool\HVAC_AppPool:(OI)(CI)F" /T
```

### 3. Download Installation Files

1. Download the HVAC application package
2. Extract to temporary directory
3. Verify file integrity

## Database Setup

### 1. Create Database

```sql
-- Connect to SQL Server using SSMS
-- Create new database
CREATE DATABASE [HVAC_DB]
ON 
( NAME = 'HVAC_DB',
  FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\HVAC_DB.mdf',
  SIZE = 100MB,
  MAXSIZE = 10GB,
  FILEGROWTH = 10MB )
LOG ON 
( NAME = 'HVAC_DB_Log',
  FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\HVAC_DB_Log.ldf',
  SIZE = 10MB,
  MAXSIZE = 1GB,
  FILEGROWTH = 10% );
```

### 2. Configure Database Settings

```sql
-- Set compatibility level
ALTER DATABASE [HVAC_DB] SET COMPATIBILITY_LEVEL = 140;

-- Set recovery model
ALTER DATABASE [HVAC_DB] SET RECOVERY SIMPLE;

-- Enable full-text search
EXEC sp_fulltext_database 'enable';
```

### 3. Create Database Schema

```sql
-- Run the database schema script
-- This will create all required tables, views, and stored procedures
-- File: Database_Schema.sql

-- Example table creation
CREATE TABLE [dbo].[AcCompany] (
    [AcCompanyID] INT IDENTITY(1,1) NOT NULL,
    [AcCompany1] NVARCHAR(100) NOT NULL,
    [Address1] NVARCHAR(200) NULL,
    [Phone] NVARCHAR(20) NULL,
    [EMail] NVARCHAR(100) NULL,
    [AcceptSystem] BIT NULL DEFAULT(0),
    CONSTRAINT [PK_AcCompany] PRIMARY KEY CLUSTERED ([AcCompanyID] ASC)
);
```

### 4. Insert Initial Data

```sql
-- Insert default company data
INSERT INTO [dbo].[AcCompany] ([AcCompany1], [Address1], [Phone], [EMail], [AcceptSystem])
VALUES ('HVAC Company', '123 Business St', '555-0123', 'info@hvac.com', 1);

-- Insert default roles
INSERT INTO [dbo].[RoleMaster] ([RoleName], [IsActive])
VALUES 
    ('Administrator', 1),
    ('Manager', 1),
    ('Sales Executive', 1),
    ('Purchase Manager', 1);
```

## Application Installation

### 1. Deploy Application Files

```powershell
# Copy application files
Copy-Item -Path "C:\Temp\HVAC_Application\*" -Destination "C:\HVAC_Application\" -Recurse -Force

# Set file permissions
icacls "C:\HVAC_Application" /grant "IIS_IUSRS:(OI)(CI)RX" /T
icacls "C:\HVAC_Application" /grant "IIS AppPool\HVAC_AppPool:(OI)(CI)RX" /T
```

### 2. Create IIS Application Pool

```powershell
# Create application pool
Import-Module WebAdministration
New-WebAppPool -Name "HVAC_AppPool" -Force

# Configure application pool
Set-ItemProperty -Path "IIS:\AppPools\HVAC_AppPool" -Name processModel.identityType -Value ApplicationPoolIdentity
Set-ItemProperty -Path "IIS:\AppPools\HVAC_AppPool" -Name managedRuntimeVersion -Value "v4.0"
Set-ItemProperty -Path "IIS:\AppPools\HVAC_AppPool" -Name enable32BitAppOnWin64 -Value $false
```

### 3. Create IIS Website

```powershell
# Create website
New-Website -Name "HVAC_Application" -Port 80 -PhysicalPath "C:\HVAC_Application" -ApplicationPool "HVAC_AppPool"

# Configure HTTPS (if SSL certificate is available)
New-WebBinding -Name "HVAC_Application" -Protocol https -Port 443 -SslFlags 1
```

### 4. Configure Application Pool Identity

```powershell
# Set application pool identity
Set-ItemProperty -Path "IIS:\AppPools\HVAC_AppPool" -Name processModel.identityType -Value SpecificUser
Set-ItemProperty -Path "IIS:\AppPools\HVAC_AppPool" -Name processModel.userName -Value "DOMAIN\HVAC_Service"
Set-ItemProperty -Path "IIS:\AppPools\HVAC_AppPool" -Name processModel.password -Value "ServicePassword"
```

## Configuration

### 1. Update Web.config

```xml
<!-- Update connection strings -->
<connectionStrings>
  <add name="myConnectionString" 
       connectionString="data source=SERVER_NAME;initial catalog=HVAC_DB;user id=HVAC_User;password=SecurePassword;Connection Timeout=1000;Pooling=true;Max Pool Size=100;Min Pool Size=5" />
  <add name="HVACEntities" 
       connectionString="metadata=res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=SERVER_NAME;initial catalog=HVAC_DB;user id=HVAC_User;password=SecurePassword;MultipleActiveResultSets=True;App=EntityFramework;Pooling=true;Max Pool Size=100;Min Pool Size=5&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>

<!-- Update app settings -->
<appSettings>
  <add key="SMTPAdminEmail" value="admin@company.com" />
  <add key="SMTPPassword" value="EmailPassword" />
  <add key="AWSAccessKey" value="YOUR_AWS_ACCESS_KEY" />
  <add key="AWSSecretKey" value="YOUR_AWS_SECRET_KEY" />
  <add key="wasabiurl" value="https://s3.us-west-1.wasabisys.com/your-bucket" />
  <add key="wasabiurl1" value="https://s3.us-west-1.wasabisys.com/your-bucket/hvac/" />
  <add key="BucketName" value="your-bucket" />
</appSettings>
```

### 2. Configure Logging

```powershell
# Create log directory
New-Item -ItemType Directory -Path "C:\HVAC_Application\Logs" -Force

# Set permissions
icacls "C:\HVAC_Application\Logs" /grant "IIS_IUSRS:(OI)(CI)F" /T
icacls "C:\HVAC_Application\Logs" /grant "IIS AppPool\HVAC_AppPool:(OI)(CI)F" /T
```

### 3. Configure File Upload Directory

```powershell
# Create upload directory
New-Item -ItemType Directory -Path "C:\HVAC_Application\UploadDocuments" -Force

# Set permissions
icacls "C:\HVAC_Application\UploadDocuments" /grant "IIS_IUSRS:(OI)(CI)F" /T
icacls "C:\HVAC_Application\UploadDocuments" /grant "IIS AppPool\HVAC_AppPool:(OI)(CI)F" /T
```

## Post-Installation Steps

### 1. Test Database Connection

```sql
-- Test connection from application
SELECT @@VERSION;
SELECT DB_NAME();
```

### 2. Verify IIS Configuration

```powershell
# Check application pool status
Get-WebAppPoolState -Name "HVAC_AppPool"

# Check website status
Get-Website -Name "HVAC_Application"
```

### 3. Test Application Access

1. Open web browser
2. Navigate to `http://localhost/HVAC_Application`
3. Verify application loads without errors
4. Test login functionality

### 4. Configure SSL Certificate (Optional)

```powershell
# Import SSL certificate
Import-Certificate -FilePath "C:\Certificates\HVAC_Cert.pfx" -CertStoreLocation Cert:\LocalMachine\My

# Bind certificate to website
New-WebBinding -Name "HVAC_Application" -Protocol https -Port 443 -SslFlags 1
```

## Verification

### 1. Application Health Check

```powershell
# Check application pool health
Get-WebAppPoolState -Name "HVAC_AppPool"

# Check website response
Invoke-WebRequest -Uri "http://localhost/HVAC_Application" -UseBasicParsing
```

### 2. Database Connectivity Test

```sql
-- Test database connectivity
SELECT 
    'Database Connection' as Test,
    @@SERVERNAME as ServerName,
    DB_NAME() as DatabaseName,
    GETDATE() as TestTime;
```

### 3. File Upload Test

1. Login to application
2. Navigate to file upload section
3. Upload a test file
4. Verify file appears in upload directory

### 4. Email Test

1. Configure test email settings
2. Send test email from application
3. Verify email is received

## Troubleshooting

### Common Installation Issues

#### **1. Application Pool Startup Error**
```powershell
# Check application pool logs
Get-EventLog -LogName Application -Source "IIS*" -Newest 10

# Restart application pool
Restart-WebAppPool -Name "HVAC_AppPool"
```

#### **2. Database Connection Error**
```sql
-- Check SQL Server services
SELECT name, status FROM sys.dm_server_services;

-- Test connection
SELECT @@VERSION;
```

#### **3. File Permission Error**
```powershell
# Reset file permissions
icacls "C:\HVAC_Application" /reset /T
icacls "C:\HVAC_Application" /grant "IIS_IUSRS:(OI)(CI)RX" /T
```

#### **4. SSL Certificate Error**
```powershell
# Check certificate installation
Get-ChildItem -Path "Cert:\LocalMachine\My" | Where-Object {$_.Subject -like "*HVAC*"}

# Test SSL binding
Test-NetConnection -ComputerName localhost -Port 443
```

### Log Files Location

- **IIS Logs**: `C:\inetpub\logs\LogFiles\W3SVC1\`
- **Application Logs**: `C:\HVAC_Application\Logs\`
- **Windows Event Logs**: Event Viewer → Windows Logs → Application

### Performance Monitoring

```powershell
# Monitor application pool performance
Get-Counter "\Process(w3wp)\% Processor Time" -SampleInterval 1 -MaxSamples 10

# Monitor database performance
Get-Counter "\SQLServer:Databases(HVAC_DB)\Transactions/sec" -SampleInterval 1 -MaxSamples 10
```

---

*Follow this installation guide carefully to ensure a successful deployment of the HVAC Management System.*
