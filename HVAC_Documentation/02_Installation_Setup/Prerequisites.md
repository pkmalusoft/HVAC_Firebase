# HVAC Management System - Prerequisites

## Table of Contents
1. [System Requirements](#system-requirements)
2. [Software Dependencies](#software-dependencies)
3. [Database Requirements](#database-requirements)
4. [Network Requirements](#network-requirements)
5. [Security Requirements](#security-requirements)
6. [Pre-Installation Checklist](#pre-installation-checklist)

## System Requirements

### Minimum Hardware Requirements

#### **Server Requirements**
| Component | Minimum | Recommended |
|-----------|---------|-------------|
| **CPU** | 2 cores, 2.0 GHz | 4 cores, 3.0 GHz |
| **RAM** | 4 GB | 8 GB |
| **Storage** | 50 GB free space | 100 GB free space |
| **Network** | 100 Mbps | 1 Gbps |

#### **Client Requirements**
| Component | Minimum | Recommended |
|-----------|---------|-------------|
| **CPU** | 1 core, 1.5 GHz | 2 cores, 2.0 GHz |
| **RAM** | 2 GB | 4 GB |
| **Storage** | 1 GB free space | 2 GB free space |
| **Display** | 1024x768 | 1920x1080 |

### Operating System Requirements

#### **Server Operating Systems**
- **Windows Server 2016** (Minimum)
- **Windows Server 2019** (Recommended)
- **Windows Server 2022** (Latest)

#### **Client Operating Systems**
- **Windows 10** (Minimum)
- **Windows 11** (Recommended)
- **Windows Server 2016+** (Server clients)

## Software Dependencies

### Required Software

#### **1. Microsoft .NET Framework 4.8**
- **Download**: [Microsoft .NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)
- **Purpose**: Core application runtime
- **Installation**: Run as Administrator
- **Verification**: Check in Programs and Features

#### **2. Internet Information Services (IIS) 10.0**
- **Purpose**: Web server
- **Features Required**:
  - ASP.NET 4.8
  - .NET Extensibility 4.8
  - ISAPI Extensions
  - ISAPI Filters
  - Static Content
  - Default Document
  - Directory Browsing
  - HTTP Errors
  - HTTP Redirection
  - Request Filtering
  - Static Content Compression
  - Dynamic Content Compression

#### **3. Microsoft SQL Server 2019**
- **Edition**: Standard or Enterprise
- **Features Required**:
  - Database Engine Services
  - SQL Server Replication
  - Full-Text and Semantic Extractions
  - Analysis Services
  - Reporting Services
  - Integration Services

#### **4. SQL Server Management Studio (SSMS)**
- **Version**: 18.x or later
- **Purpose**: Database management
- **Download**: [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

### Optional Software

#### **1. Visual Studio 2022**
- **Purpose**: Development and debugging
- **Edition**: Community, Professional, or Enterprise
- **Features**: ASP.NET and web development workload

#### **2. Crystal Reports Runtime**
- **Version**: 13.0.4000 or later
- **Purpose**: Report generation
- **Installation**: Required on web server

## Database Requirements

### SQL Server Configuration

#### **Database Engine Settings**
```sql
-- Minimum configuration
ALTER DATABASE [HVAC_DB] SET COMPATIBILITY_LEVEL = 140;
ALTER DATABASE [HVAC_DB] SET RECOVERY SIMPLE;
ALTER DATABASE [HVAC_DB] SET AUTO_SHRINK OFF;
```

#### **Required Database Features**
- **Full-Text Search**: For document search functionality
- **Service Broker**: For asynchronous processing
- **CLR Integration**: For advanced functionality

#### **Database Size Requirements**
- **Initial Size**: 1 GB
- **Growth Rate**: 10% per month
- **Log File Size**: 500 MB initial, 100 MB growth
- **Backup Space**: 2x database size

### Connection Requirements

#### **SQL Server Authentication**
- **Authentication Mode**: Mixed (Windows + SQL Server)
- **Default Port**: 1433
- **Named Pipes**: Enabled
- **TCP/IP**: Enabled

#### **Service Accounts**
- **SQL Server Service**: Local System or Domain Account
- **SQL Server Agent**: Local System or Domain Account
- **Application Pool Identity**: Domain Account (Recommended)

## Network Requirements

### Port Requirements

#### **Web Server Ports**
| Port | Protocol | Purpose | Direction |
|------|----------|---------|-----------|
| 80 | HTTP | Web traffic | Inbound |
| 443 | HTTPS | Secure web traffic | Inbound |
| 21 | FTP | File transfer (optional) | Inbound |

#### **Database Server Ports**
| Port | Protocol | Purpose | Direction |
|------|----------|---------|-----------|
| 1433 | TCP | SQL Server | Inbound |
| 1434 | UDP | SQL Browser | Inbound |

### Firewall Configuration

#### **Windows Firewall Rules**
```powershell
# Allow HTTP traffic
New-NetFirewallRule -DisplayName "HVAC HTTP" -Direction Inbound -Protocol TCP -LocalPort 80 -Action Allow

# Allow HTTPS traffic
New-NetFirewallRule -DisplayName "HVAC HTTPS" -Direction Inbound -Protocol TCP -LocalPort 443 -Action Allow

# Allow SQL Server traffic
New-NetFirewallRule -DisplayName "HVAC SQL Server" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow
```

#### **Network Security**
- **SSL Certificate**: Required for HTTPS
- **Domain Controller**: For authentication (optional)
- **DNS Resolution**: Required for external services

## Security Requirements

### SSL/TLS Configuration

#### **SSL Certificate Requirements**
- **Type**: X.509 certificate
- **Key Length**: 2048 bits minimum
- **Validity**: 1-3 years
- **Subject Alternative Names**: Include all domain names

#### **TLS Configuration**
- **Minimum Version**: TLS 1.2
- **Recommended**: TLS 1.3
- **Cipher Suites**: Strong encryption only

### User Account Requirements

#### **Service Accounts**
- **Application Pool Identity**: Domain account with minimal privileges
- **Database Access**: Read/Write permissions to HVAC database
- **File System Access**: Read/Write to application directory

#### **Administrative Accounts**
- **Local Administrator**: Required for installation
- **Domain Administrator**: Required for domain integration
- **SQL Server Administrator**: Required for database setup

## Pre-Installation Checklist

### System Preparation

- [ ] **Operating System**: Windows Server 2016+ installed and updated
- [ ] **Windows Updates**: All critical updates installed
- [ ] **Antivirus Software**: Configured and updated
- [ ] **Backup Software**: Installed and configured
- [ ] **Monitoring Software**: Installed (optional)

### Software Installation

- [ ] **IIS**: Installed with required features
- [ ] **.NET Framework 4.8**: Installed and verified
- [ ] **SQL Server 2019**: Installed and configured
- [ ] **SSMS**: Installed for database management
- [ ] **Crystal Reports Runtime**: Installed

### Database Preparation

- [ ] **SQL Server Service**: Running and accessible
- [ ] **Database Created**: HVAC database created
- [ ] **Service Accounts**: Configured with proper permissions
- [ ] **Backup Strategy**: Implemented and tested

### Network Configuration

- [ ] **Firewall Rules**: Configured for required ports
- [ ] **SSL Certificate**: Obtained and installed
- [ ] **DNS Configuration**: Resolved and tested
- [ ] **Load Balancer**: Configured (if applicable)

### Security Configuration

- [ ] **User Accounts**: Created with appropriate permissions
- [ ] **Group Policies**: Applied for security hardening
- [ ] **Audit Logging**: Enabled for security monitoring
- [ ] **Access Controls**: Implemented and tested

### External Services

- [ ] **AWS S3**: Account created and configured
- [ ] **Email Service**: SMTP server configured
- [ ] **Domain Services**: Active Directory configured (if applicable)

## Troubleshooting Common Issues

### Common Prerequisites Issues

#### **1. .NET Framework Installation Issues**
- **Problem**: Installation fails or incomplete
- **Solution**: Use .NET Framework Repair Tool
- **Prevention**: Run as Administrator, disable antivirus temporarily

#### **2. IIS Configuration Issues**
- **Problem**: ASP.NET not working
- **Solution**: Register ASP.NET with IIS using aspnet_regiis.exe
- **Prevention**: Install IIS features in correct order

#### **3. SQL Server Connection Issues**
- **Problem**: Cannot connect to database
- **Solution**: Check SQL Server services, firewall, and authentication
- **Prevention**: Test connectivity before application installation

#### **4. SSL Certificate Issues**
- **Problem**: HTTPS not working
- **Solution**: Verify certificate installation and binding
- **Prevention**: Test certificate before application deployment

---

*Complete this checklist before proceeding with the HVAC Management System installation to ensure a smooth deployment process.*
