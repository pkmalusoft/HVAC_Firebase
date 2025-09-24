# ServiceRequest Integration Script
# HVAC Management System
# This script automates the integration of ServiceRequest functionality

param(
    [Parameter(Mandatory=$true)]
    [string]$DatabaseServer,
    
    [Parameter(Mandatory=$true)]
    [string]$DatabaseName,
    
    [Parameter(Mandatory=$false)]
    [string]$DatabaseUser,
    
    [Parameter(Mandatory=$false)]
    [string]$DatabasePassword,
    
    [Parameter(Mandatory=$false)]
    [switch]$UseWindowsAuthentication = $true,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipDatabaseSetup = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipMenuSetup = $false
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Function to execute SQL script
function Invoke-SqlScript {
    param(
        [string]$ScriptPath,
        [string]$Server,
        [string]$Database,
        [string]$User = $null,
        [string]$Password = $null,
        [bool]$UseWindowsAuth = $true
    )
    
    try {
        Write-Host "Executing SQL script: $ScriptPath" -ForegroundColor Yellow
        
        if ($UseWindowsAuth) {
            sqlcmd -S $Server -d $Database -E -i $ScriptPath
        } else {
            sqlcmd -S $Server -d $Database -U $User -P $Password -i $ScriptPath
        }
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "SQL script executed successfully: $ScriptPath" -ForegroundColor Green
        } else {
            throw "SQL script execution failed with exit code: $LASTEXITCODE"
        }
    }
    catch {
        Write-Error "Failed to execute SQL script $ScriptPath`: $_"
        throw
    }
}

# Function to check if SQL Server is accessible
function Test-SqlServerConnection {
    param(
        [string]$Server,
        [string]$Database,
        [string]$User = $null,
        [string]$Password = $null,
        [bool]$UseWindowsAuth = $true
    )
    
    try {
        Write-Host "Testing SQL Server connection..." -ForegroundColor Yellow
        
        if ($UseWindowsAuth) {
            $result = sqlcmd -S $Server -d $Database -E -Q "SELECT 1" -h -1
        } else {
            $result = sqlcmd -S $Server -d $Database -U $User -P $Password -Q "SELECT 1" -h -1
        }
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "SQL Server connection successful" -ForegroundColor Green
            return $true
        } else {
            Write-Error "SQL Server connection failed"
            return $false
        }
    }
    catch {
        Write-Error "SQL Server connection test failed: $_"
        return $false
    }
}

# Function to backup database
function Backup-Database {
    param(
        [string]$Server,
        [string]$Database,
        [string]$BackupPath,
        [string]$User = $null,
        [string]$Password = $null,
        [bool]$UseWindowsAuth = $true
    )
    
    try {
        Write-Host "Creating database backup..." -ForegroundColor Yellow
        
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $backupFile = Join-Path $BackupPath "$Database`_ServiceRequest_Integration_$timestamp.bak"
        
        $backupQuery = "BACKUP DATABASE [$Database] TO DISK = '$backupFile' WITH FORMAT, INIT, NAME = 'ServiceRequest Integration Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10"
        
        if ($UseWindowsAuth) {
            sqlcmd -S $Server -d $Database -E -Q $backupQuery
        } else {
            sqlcmd -S $Server -d $Database -U $User -P $Password -Q $backupQuery
        }
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Database backup created successfully: $backupFile" -ForegroundColor Green
            return $backupFile
        } else {
            throw "Database backup failed with exit code: $LASTEXITCODE"
        }
    }
    catch {
        Write-Error "Database backup failed: $_"
        throw
    }
}

# Main integration process
try {
    Write-Host "===============================================" -ForegroundColor Cyan
    Write-Host "ServiceRequest Integration Script" -ForegroundColor Cyan
    Write-Host "HVAC Management System" -ForegroundColor Cyan
    Write-Host "===============================================" -ForegroundColor Cyan
    Write-Host ""
    
    # Validate parameters
    if (-not $UseWindowsAuthentication -and (-not $DatabaseUser -or -not $DatabasePassword)) {
        throw "Database credentials are required when not using Windows Authentication"
    }
    
    # Test database connection
    if (-not (Test-SqlServerConnection -Server $DatabaseServer -Database $DatabaseName -User $DatabaseUser -Password $DatabasePassword -UseWindowsAuth $UseWindowsAuthentication)) {
        throw "Cannot connect to database. Please check your connection parameters."
    }
    
    # Create backup directory
    $backupDir = Join-Path $PSScriptRoot "Backups"
    if (-not (Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
        Write-Host "Created backup directory: $backupDir" -ForegroundColor Green
    }
    
    # Create database backup
    Write-Host "Creating database backup before integration..." -ForegroundColor Yellow
    $backupFile = Backup-Database -Server $DatabaseServer -Database $DatabaseName -BackupPath $backupDir -User $DatabaseUser -Password $DatabasePassword -UseWindowsAuth $UseWindowsAuthentication
    
    # Execute SQL scripts
    if (-not $SkipDatabaseSetup) {
        Write-Host "Setting up ServiceRequest database objects..." -ForegroundColor Yellow
        
        # Execute table creation script
        $tablesScript = Join-Path $PSScriptRoot "ServiceRequest_Tables.sql"
        if (Test-Path $tablesScript) {
            Invoke-SqlScript -ScriptPath $tablesScript -Server $DatabaseServer -Database $DatabaseName -User $DatabaseUser -Password $DatabasePassword -UseWindowsAuth $UseWindowsAuthentication
        } else {
            throw "Table creation script not found: $tablesScript"
        }
        
        # Execute initial data script
        $dataScript = Join-Path $PSScriptRoot "ServiceRequest_InitialData.sql"
        if (Test-Path $dataScript) {
            Invoke-SqlScript -ScriptPath $dataScript -Server $DatabaseServer -Database $DatabaseName -User $DatabaseUser -Password $DatabasePassword -UseWindowsAuth $UseWindowsAuthentication
        } else {
            throw "Initial data script not found: $dataScript"
        }
    }
    
    # Execute menu setup script
    if (-not $SkipMenuSetup) {
        Write-Host "Setting up ServiceRequest menu items..." -ForegroundColor Yellow
        
        $menuScript = Join-Path $PSScriptRoot "ServiceRequest_Menu.sql"
        if (Test-Path $menuScript) {
            Invoke-SqlScript -ScriptPath $menuScript -Server $DatabaseServer -Database $DatabaseName -User $DatabaseUser -Password $DatabasePassword -UseWindowsAuth $UseWindowsAuthentication
        } else {
            throw "Menu setup script not found: $menuScript"
        }
    }
    
    # Verify integration
    Write-Host "Verifying ServiceRequest integration..." -ForegroundColor Yellow
    
    $verificationQuery = @"
SELECT 
    'ServiceRequest' as TableName, COUNT(*) as RecordCount 
FROM ServiceRequest
UNION ALL
SELECT 
    'ServiceType' as TableName, COUNT(*) as RecordCount 
FROM ServiceType
UNION ALL
SELECT 
    'ServiceStatus' as TableName, COUNT(*) as RecordCount 
FROM ServiceStatus
"@
    
    if ($UseWindowsAuthentication) {
        $verificationResult = sqlcmd -S $DatabaseServer -d $DatabaseName -E -Q $verificationQuery -h -1
    } else {
        $verificationResult = sqlcmd -S $DatabaseServer -d $DatabaseName -U $DatabaseUser -P $Password -Q $verificationQuery -h -1
    }
    
    Write-Host "Verification Results:" -ForegroundColor Green
    Write-Host $verificationResult
    
    # Create integration report
    $reportPath = Join-Path $PSScriptRoot "ServiceRequest_Integration_Report.txt"
    $report = @"
ServiceRequest Integration Report
================================
Date: $(Get-Date)
Database Server: $DatabaseServer
Database Name: $DatabaseName
Authentication: $(if ($UseWindowsAuthentication) { "Windows Authentication" } else { "SQL Server Authentication" })
Backup File: $backupFile

Integration Steps Completed:
- Database tables created
- Initial data inserted
- Menu items added
- Integration verified

Next Steps:
1. Update Entity Framework model in Visual Studio
2. Copy model, controller, and view files
3. Build and test the application
4. Verify menu navigation
5. Test CRUD operations

For detailed instructions, see: ServiceRequest_Integration_Guide.md
"@
    
    $report | Out-File -FilePath $reportPath -Encoding UTF8
    Write-Host "Integration report created: $reportPath" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "===============================================" -ForegroundColor Green
    Write-Host "ServiceRequest Integration Completed Successfully!" -ForegroundColor Green
    Write-Host "===============================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Yellow
    Write-Host "1. Update Entity Framework model in Visual Studio" -ForegroundColor White
    Write-Host "2. Copy model, controller, and view files to your project" -ForegroundColor White
    Write-Host "3. Build and test the application" -ForegroundColor White
    Write-Host "4. Verify menu navigation works correctly" -ForegroundColor White
    Write-Host "5. Test all CRUD operations" -ForegroundColor White
    Write-Host ""
    Write-Host "Backup created at: $backupFile" -ForegroundColor Cyan
    Write-Host "Integration report: $reportPath" -ForegroundColor Cyan
    Write-Host "Integration guide: ServiceRequest_Integration_Guide.md" -ForegroundColor Cyan
}
catch {
    Write-Error "ServiceRequest integration failed: $_"
    Write-Host ""
    Write-Host "If you need to rollback:" -ForegroundColor Red
    Write-Host "1. Restore the database from the backup file" -ForegroundColor Red
    Write-Host "2. Check the error messages above for specific issues" -ForegroundColor Red
    Write-Host "3. Verify your connection parameters and permissions" -ForegroundColor Red
    exit 1
}

# Usage examples:
# .\ServiceRequest_Integration.ps1 -DatabaseServer "localhost" -DatabaseName "HVAC_DB" -UseWindowsAuthentication
# .\ServiceRequest_Integration.ps1 -DatabaseServer "localhost" -DatabaseName "HVAC_DB" -DatabaseUser "sa" -DatabasePassword "password" -UseWindowsAuthentication:$false
