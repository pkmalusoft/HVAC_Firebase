# HVAC Management System - Configuration Guide

## Table of Contents
1. [Configuration Overview](#configuration-overview)
2. [Web.config Configuration](#webconfig-configuration)
3. [Database Configuration](#database-configuration)
4. [Application Settings](#application-settings)
5. [Security Configuration](#security-configuration)
6. [Performance Configuration](#performance-configuration)
7. [External Service Configuration](#external-service-configuration)
8. [Environment-Specific Configuration](#environment-specific-configuration)
9. [Configuration Validation](#configuration-validation)
10. [Troubleshooting](#troubleshooting)

## Configuration Overview

The HVAC Management System requires comprehensive configuration across multiple layers including web server, database, application settings, and external services. This guide provides detailed configuration instructions for all components.

### Configuration Files
- **Web.config**: Main application configuration
- **Connection Strings**: Database connectivity settings
- **App Settings**: Application-specific parameters
- **IIS Configuration**: Web server settings
- **External Services**: Third-party integrations

## Web.config Configuration

### 1. System Configuration

```xml
<system.web>
  <!-- Compilation Settings -->
  <compilation debug="false" targetFramework="4.8" />
  
  <!-- HTTP Runtime Settings -->
  <httpRuntime 
    maxRequestLength="10485760" 
    executionTimeout="300" 
    requestPathInvalidCharacters="" 
    requestValidationMode="2.0" 
    requestLengthDiskThreshold="10485760" 
    targetFramework="4.8" />
  
  <!-- Custom Errors -->
  <customErrors mode="RemoteOnly" defaultRedirect="~/Error/Index">
    <error statusCode="404" redirect="~/Error/NotFound" />
    <error statusCode="500" redirect="~/Error/InternalServerError" />
  </customErrors>
  
  <!-- Pages Configuration -->
  <pages validateRequest="true" enableViewState="true" />
  
  <!-- Session State -->
  <sessionState 
    mode="InProc" 
    timeout="30" 
    cookieless="false" 
    regenerateExpiredSessionId="true" />
  
  <!-- HTTP Handlers -->
  <httpHandlers>
    <add verb="*" path="routes.axd" type="AttributeRouting.Web.Logging.LogRoutesHandler, AttributeRouting.Web" />
  </httpHandlers>
</system.web>
```

### 2. System.WebServer Configuration

```xml
<system.webServer>
  <!-- Default Document -->
  <defaultDocument>
    <files>
      <clear />
      <add value="index.html" />
      <add value="default.aspx" />
    </files>
  </defaultDocument>
  
  <!-- Static Content -->
  <staticContent>
    <mimeMap fileExtension=".json" mimeType="application/json" />
    <mimeMap fileExtension=".woff" mimeType="application/font-woff" />
    <mimeMap fileExtension=".woff2" mimeType="application/font-woff2" />
  </staticContent>
  
  <!-- URL Rewrite -->
  <rewrite>
    <rules>
      <rule name="Redirect to HTTPS" stopProcessing="true">
        <match url="(.*)" />
        <conditions>
          <add input="{HTTPS}" pattern="off" ignoreCase="true" />
        </conditions>
        <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" redirectType="Permanent" />
      </rule>
    </rules>
  </rewrite>
  
  <!-- Security Headers -->
  <httpProtocol>
    <customHeaders>
      <add name="X-Content-Type-Options" value="nosniff" />
      <add name="X-Frame-Options" value="DENY" />
      <add name="X-XSS-Protection" value="1; mode=block" />
      <add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains" />
    </customHeaders>
  </httpProtocol>
</system.webServer>
```

## Database Configuration

### 1. Connection Strings

```xml
<connectionStrings>
  <!-- Primary Database Connection -->
  <add name="myConnectionString" 
       connectionString="data source=SERVER_NAME;initial catalog=HVAC_DB;user id=HVAC_User;password=SecurePassword;Connection Timeout=1000;Pooling=true;Max Pool Size=100;Min Pool Size=5;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True" />
  
  <!-- Entity Framework Connection -->
  <add name="HVACEntities" 
       connectionString="metadata=res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=SERVER_NAME;initial catalog=HVAC_DB;user id=HVAC_User;password=SecurePassword;MultipleActiveResultSets=True;App=EntityFramework;Pooling=true;Max Pool Size=100;Min Pool Size=5;Encrypt=True;TrustServerCertificate=True&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>
```

### 2. Database Security Configuration

```sql
-- Create application user
CREATE LOGIN [HVAC_User] WITH PASSWORD = 'SecurePassword123!';

-- Create database user
USE [HVAC_DB];
CREATE USER [HVAC_User] FOR LOGIN [HVAC_User];

-- Grant permissions
ALTER ROLE [db_datareader] ADD MEMBER [HVAC_User];
ALTER ROLE [db_datawriter] ADD MEMBER [HVAC_User];
ALTER ROLE [db_ddladmin] ADD MEMBER [HVAC_User];

-- Grant specific permissions
GRANT EXECUTE ON SCHEMA::dbo TO [HVAC_User];
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [HVAC_User];
```

### 3. Database Optimization Settings

```sql
-- Configure database settings
ALTER DATABASE [HVAC_DB] SET COMPATIBILITY_LEVEL = 140;
ALTER DATABASE [HVAC_DB] SET RECOVERY SIMPLE;
ALTER DATABASE [HVAC_DB] SET AUTO_SHRINK OFF;
ALTER DATABASE [HVAC_DB] SET AUTO_CREATE_STATISTICS ON;
ALTER DATABASE [HVAC_DB] SET AUTO_UPDATE_STATISTICS ON;

-- Configure connection pooling
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'max worker threads', 0;
RECONFIGURE;
```

## Application Settings

### 1. Core Application Settings

```xml
<appSettings>
  <!-- Application Information -->
  <add key="ApplicationName" value="HVAC Management System" />
  <add key="ApplicationVersion" value="1.0.0" />
  <add key="Environment" value="Production" />
  
  <!-- Web Pages Configuration -->
  <add key="webpages:Version" value="3.0.0.0" />
  <add key="webpages:Enabled" value="false" />
  <add key="ClientValidationEnabled" value="true" />
  <add key="UnobtrusiveJavaScriptEnabled" value="true" />
  
  <!-- Session Configuration -->
  <add key="SessionTimeout" value="30" />
  <add key="SessionCookieName" value="HVAC_Session" />
  
  <!-- Cache Configuration -->
  <add key="CacheTimeout" value="300" />
  <add key="EnableOutputCache" value="true" />
  <add key="EnableDataCache" value="true" />
</appSettings>
```

### 2. Email Configuration

```xml
<appSettings>
  <!-- SMTP Configuration -->
  <add key="EmailPort" value="587" />
  <add key="MailServer" value="smtp.company.com" />
  <add key="SMTPAdminEmail" value="admin@company.com" />
  <add key="SMTPPassword" value="EmailPassword123!" />
  <add key="SMTPEnableSSL" value="true" />
  <add key="SMTPTimeout" value="30000" />
  
  <!-- Email Templates -->
  <add key="EmailFromAddress" value="noreply@company.com" />
  <add key="EmailFromName" value="HVAC Management System" />
  <add key="SiteName" value="https://hvac.company.com/" />
</appSettings>
```

### 3. File Upload Configuration

```xml
<appSettings>
  <!-- File Upload Settings -->
  <add key="MaxFileSize" value="10485760" />
  <add key="AllowedFileExtensions" value=".pdf,.doc,.docx,.xls,.xlsx,.jpg,.jpeg,.png,.gif" />
  <add key="UploadPath" value="~/UploadDocuments/" />
  <add key="TempUploadPath" value="~/Temp/" />
  
  <!-- AWS S3 Configuration -->
  <add key="AWSProfileName" value="hvac-profile" />
  <add key="AWSAccessKey" value="YOUR_AWS_ACCESS_KEY" />
  <add key="AWSSecretKey" value="YOUR_AWS_SECRET_KEY" />
  <add key="wasabiurl" value="https://s3.us-west-1.wasabisys.com/hvac-bucket" />
  <add key="wasabiurl1" value="https://s3.us-west-1.wasabisys.com/hvac-bucket/documents/" />
  <add key="BucketName" value="hvac-bucket" />
  <add key="AWSRegion" value="us-west-1" />
</appSettings>
```

## Security Configuration

### 1. Authentication Configuration

```xml
<system.web>
  <!-- Authentication -->
  <authentication mode="Forms">
    <forms loginUrl="~/Login/Index" 
           timeout="30" 
           name="HVAC_Auth" 
           requireSSL="true" 
           slidingExpiration="true" />
  </authentication>
  
  <!-- Authorization -->
  <authorization>
    <deny users="?" />
  </authorization>
</system.web>
```

### 2. Security Headers

```xml
<system.webServer>
  <httpProtocol>
    <customHeaders>
      <!-- Security Headers -->
      <add name="X-Content-Type-Options" value="nosniff" />
      <add name="X-Frame-Options" value="DENY" />
      <add name="X-XSS-Protection" value="1; mode=block" />
      <add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains" />
      <add name="Content-Security-Policy" value="default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:;" />
      <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
    </customHeaders>
  </httpProtocol>
</system.webServer>
```

### 3. Request Validation

```xml
<system.web>
  <!-- Request Validation -->
  <pages validateRequest="true" enableViewStateMac="true" />
  
  <!-- HTTP Runtime Security -->
  <httpRuntime 
    requestValidationMode="2.0" 
    requestPathInvalidCharacters="&lt;&gt;*%&amp;:&quot;?[]{}|" />
</system.web>
```

## Performance Configuration

### 1. Caching Configuration

```xml
<system.web>
  <!-- Output Caching -->
  <caching>
    <outputCacheSettings>
      <outputCacheProfiles>
        <add name="DefaultCache" duration="300" varyByParam="none" />
        <add name="UserCache" duration="180" varyByParam="id" />
        <add name="DataCache" duration="600" varyByParam="none" />
      </outputCacheProfiles>
    </outputCacheSettings>
  </caching>
</system.web>
```

### 2. Compression Configuration

```xml
<system.webServer>
  <!-- Compression -->
  <urlCompression doStaticCompression="true" doDynamicCompression="true" />
  
  <!-- Static Content Compression -->
  <httpCompression>
    <scheme name="gzip" dll="%Windir%\system32\inetsrv\gzip.dll" />
    <dynamicTypes>
      <add mimeType="text/*" enabled="true" />
      <add mimeType="message/*" enabled="true" />
      <add mimeType="application/javascript" enabled="true" />
      <add mimeType="application/json" enabled="true" />
      <add mimeType="*/*" enabled="false" />
    </dynamicTypes>
    <staticTypes>
      <add mimeType="text/*" enabled="true" />
      <add mimeType="message/*" enabled="true" />
      <add mimeType="application/javascript" enabled="true" />
      <add mimeType="application/json" enabled="true" />
      <add mimeType="*/*" enabled="false" />
    </staticTypes>
  </httpCompression>
</system.webServer>
```

## External Service Configuration

### 1. AWS S3 Configuration

```csharp
// AWS S3 Configuration in code
public static class AWSConfig
{
    public static readonly string BucketName = ConfigurationManager.AppSettings["BucketName"];
    public static readonly RegionEndpoint BucketRegion = RegionEndpoint.GetBySystemName(ConfigurationManager.AppSettings["AWSRegion"]);
    public static readonly string AccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
    public static readonly string SecretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
}
```

### 2. Email Service Configuration

```csharp
// Email Configuration in code
public static class EmailConfig
{
    public static readonly string SmtpServer = ConfigurationManager.AppSettings["MailServer"];
    public static readonly int SmtpPort = int.Parse(ConfigurationManager.AppSettings["EmailPort"]);
    public static readonly string SmtpUsername = ConfigurationManager.AppSettings["SMTPAdminEmail"];
    public static readonly string SmtpPassword = ConfigurationManager.AppSettings["SMTPPassword"];
    public static readonly bool EnableSSL = bool.Parse(ConfigurationManager.AppSettings["SMTPEnableSSL"]);
}
```

## Environment-Specific Configuration

### 1. Development Environment

```xml
<!-- Development Web.config -->
<appSettings>
  <add key="Environment" value="Development" />
  <add key="DebugMode" value="true" />
  <add key="LogLevel" value="Debug" />
  <add key="EnableDetailedErrors" value="true" />
</appSettings>

<system.web>
  <compilation debug="true" targetFramework="4.8" />
  <customErrors mode="Off" />
</system.web>
```

### 2. Staging Environment

```xml
<!-- Staging Web.config -->
<appSettings>
  <add key="Environment" value="Staging" />
  <add key="DebugMode" value="false" />
  <add key="LogLevel" value="Info" />
  <add key="EnableDetailedErrors" value="false" />
</appSettings>

<system.web>
  <compilation debug="false" targetFramework="4.8" />
  <customErrors mode="RemoteOnly" />
</system.web>
```

### 3. Production Environment

```xml
<!-- Production Web.config -->
<appSettings>
  <add key="Environment" value="Production" />
  <add key="DebugMode" value="false" />
  <add key="LogLevel" value="Error" />
  <add key="EnableDetailedErrors" value="false" />
</appSettings>

<system.web>
  <compilation debug="false" targetFramework="4.8" />
  <customErrors mode="On" defaultRedirect="~/Error/Index" />
</system.web>
```

## Configuration Validation

### 1. Configuration Test Script

```powershell
# Configuration Validation Script
function Test-HVACConfiguration {
    Write-Host "Testing HVAC Configuration..." -ForegroundColor Green
    
    # Test Web.config
    $webConfig = "C:\HVAC_Application\Web.config"
    if (Test-Path $webConfig) {
        Write-Host "✓ Web.config found" -ForegroundColor Green
    } else {
        Write-Host "✗ Web.config not found" -ForegroundColor Red
    }
    
    # Test Connection String
    $config = [xml](Get-Content $webConfig)
    $connectionString = $config.configuration.connectionStrings.add | Where-Object { $_.name -eq "myConnectionString" }
    if ($connectionString) {
        Write-Host "✓ Connection string configured" -ForegroundColor Green
    } else {
        Write-Host "✗ Connection string not configured" -ForegroundColor Red
    }
    
    # Test Database Connection
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString.connectionString)
        $connection.Open()
        Write-Host "✓ Database connection successful" -ForegroundColor Green
        $connection.Close()
    } catch {
        Write-Host "✗ Database connection failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}
```

### 2. Configuration Health Check

```csharp
// Configuration Health Check in Application
public class ConfigurationHealthCheck
{
    public static bool ValidateConfiguration()
    {
        try
        {
            // Check connection string
            var connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"];
            if (string.IsNullOrEmpty(connectionString?.ConnectionString))
                return false;
            
            // Check required app settings
            var requiredSettings = new[] { "ApplicationName", "Environment", "LogLevel" };
            foreach (var setting in requiredSettings)
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[setting]))
                    return false;
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

## Troubleshooting

### Common Configuration Issues

#### 1. Connection String Issues
```xml
<!-- Incorrect -->
<add name="myConnectionString" connectionString="Server=localhost;Database=HVAC_DB;" />

<!-- Correct -->
<add name="myConnectionString" connectionString="data source=localhost;initial catalog=HVAC_DB;user id=HVAC_User;password=SecurePassword;Connection Timeout=1000;Pooling=true;" />
```

#### 2. Session State Issues
```xml
<!-- Incorrect -->
<sessionState mode="InProc" timeout="550" />

<!-- Correct -->
<sessionState mode="InProc" timeout="30" cookieless="false" regenerateExpiredSessionId="true" />
```

#### 3. Security Configuration Issues
```xml
<!-- Missing Security Headers -->
<system.webServer>
  <httpProtocol>
    <customHeaders>
      <add name="X-Content-Type-Options" value="nosniff" />
    </customHeaders>
  </httpProtocol>
</system.webServer>
```

### Configuration Validation Commands

```powershell
# Validate Web.config syntax
[System.Xml.XmlDocument]$xml = New-Object System.Xml.XmlDocument
$xml.Load("C:\HVAC_Application\Web.config")
Write-Host "Web.config syntax is valid" -ForegroundColor Green

# Test IIS configuration
Import-Module WebAdministration
Get-WebConfiguration -Filter "system.web/compilation" -PSPath "IIS:\Sites\HVAC_Application"

# Test database connectivity
$connectionString = "data source=localhost;initial catalog=HVAC_DB;user id=HVAC_User;password=SecurePassword;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$connection.Open()
$connection.Close()
Write-Host "Database connection successful" -ForegroundColor Green
```

---

*This configuration guide provides comprehensive setup instructions for all aspects of the HVAC Management System.*
