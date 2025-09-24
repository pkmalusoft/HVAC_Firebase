# HVAC Management System - Architecture Diagram

## Table of Contents
1. [System Architecture Overview](#system-architecture-overview)
2. [Database Architecture](#database-architecture)
3. [Security Architecture](#security-architecture)
4. [Integration Architecture](#integration-architecture)
5. [Deployment Architecture](#deployment-architecture)

## System Architecture Overview

```mermaid
graph TB
    subgraph "Client Layer"
        A[Web Browser]
        B[Mobile Browser]
    end
    
    subgraph "Presentation Layer"
        C[ASP.NET MVC Controllers]
        D[Views (Razor)]
        E[Static Content]
    end
    
    subgraph "Business Logic Layer"
        F[Service Classes]
        G[Business Rules]
        H[Validation Logic]
    end
    
    subgraph "Data Access Layer"
        I[Entity Framework]
        J[Custom DAO Classes]
        K[Repository Pattern]
    end
    
    subgraph "Data Layer"
        L[SQL Server Database]
        M[File System]
    end
    
    subgraph "External Services"
        N[AWS S3 Storage]
        O[Email Service]
        P[Crystal Reports]
    end
    
    A --> C
    B --> C
    C --> D
    C --> E
    C --> F
    F --> G
    F --> H
    F --> I
    I --> J
    J --> K
    K --> L
    C --> M
    C --> N
    C --> O
    C --> P
```

## Database Architecture

```mermaid
erDiagram
    AcCompany ||--o{ BranchMaster : has
    AcCompany ||--o{ UserRegistration : employs
    BranchMaster ||--o{ Enquiry : receives
    BranchMaster ||--o{ Quotation : generates
    BranchMaster ||--o{ PurchaseOrder : creates
    
    CustomerMaster ||--o{ Enquiry : submits
    CustomerMaster ||--o{ Quotation : receives
    CustomerMaster ||--o{ Invoice : billed
    
    SupplierMaster ||--o{ PurchaseOrder : supplies
    SupplierMaster ||--o{ SupplierInvoice : issues
    
    Enquiry ||--o{ Quotation : generates
    Enquiry ||--o{ EnquiryEquipment : contains
    Enquiry ||--o{ EnquiryClient : involves
    
    Quotation ||--o{ QuotationDetail : contains
    Quotation ||--o{ QuotationScope : includes
    Quotation ||--o{ QuotationWarranty : covers
    
    PurchaseOrder ||--o{ PurchaseOrderDetail : contains
    PurchaseOrder ||--o{ GRN : receives
    
    ProductMaster ||--o{ QuotationDetail : quoted
    ProductMaster ||--o{ PurchaseOrderDetail : ordered
    ProductMaster ||--o{ MaterialIssue : issued
    
    AcCompany {
        int AcCompanyID PK
        string AcCompany1
        string Address1
        string Phone
        string EMail
        bool AcceptSystem
    }
    
    BranchMaster {
        int BranchID PK
        int AcCompanyID FK
        string BranchName
        string Address
        string Phone
    }
    
    UserRegistration {
        int UserID PK
        int AcCompanyID FK
        string UserName
        string Password
        int RoleID
        bool IsActive
    }
    
    Enquiry {
        int EnquiryID PK
        int BranchID FK
        int CustomerID FK
        string EnquiryNo
        datetime EnquiryDate
        string ProjectName
        string ProjectDescription
    }
    
    Quotation {
        int QuotationID PK
        int EnquiryID FK
        int BranchID FK
        string QuotationNo
        datetime QuotationDate
        decimal QuotationValue
        int QuotationStatusID FK
    }
    
    PurchaseOrder {
        int PurchaseOrderID PK
        int BranchID FK
        int SupplierID FK
        string PONumber
        datetime PODate
        decimal POValue
    }
```

## Security Architecture

```mermaid
graph TB
    subgraph "Security Layers"
        A[Authentication Layer]
        B[Authorization Layer]
        C[Data Protection Layer]
        D[Network Security Layer]
    end
    
    subgraph "Authentication Components"
        E[Session Management]
        F[User Validation]
        G[Password Hashing]
    end
    
    subgraph "Authorization Components"
        H[Role-Based Access Control]
        I[Permission Matrix]
        J[Resource Protection]
    end
    
    subgraph "Data Protection Components"
        K[Input Validation]
        L[Output Encoding]
        M[SQL Injection Prevention]
        N[XSS Prevention]
        O[CSRF Protection]
    end
    
    subgraph "Network Security Components"
        P[HTTPS Encryption]
        Q[Request Validation]
        R[File Upload Security]
    end
    
    A --> E
    A --> F
    A --> G
    
    B --> H
    B --> I
    B --> J
    
    C --> K
    C --> L
    C --> M
    C --> N
    C --> O
    
    D --> P
    D --> Q
    D --> R
```

## Integration Architecture

```mermaid
graph LR
    subgraph "HVAC Application"
        A[Controllers]
        B[Services]
        C[Data Access]
    end
    
    subgraph "External Integrations"
        D[AWS S3 Storage]
        E[Email Service]
        F[Crystal Reports]
        G[SQL Server]
    end
    
    subgraph "Integration Points"
        H[File Upload Service]
        I[Email Notification Service]
        J[Report Generation Service]
        K[Database Service]
    end
    
    A --> H
    A --> I
    A --> J
    A --> K
    
    H --> D
    I --> E
    J --> F
    K --> G
    
    B --> H
    B --> I
    B --> J
    B --> K
    
    C --> K
```

## Deployment Architecture

```mermaid
graph TB
    subgraph "Production Environment"
        A[Load Balancer]
        B[Web Server 1]
        C[Web Server 2]
        D[Database Server]
        E[File Storage]
    end
    
    subgraph "External Services"
        F[AWS S3]
        G[Email Service]
        H[Monitoring Service]
    end
    
    subgraph "Security Components"
        I[Firewall]
        J[SSL Certificate]
        K[VPN Access]
    end
    
    A --> B
    A --> C
    B --> D
    C --> D
    B --> E
    C --> E
    
    B --> F
    C --> F
    B --> G
    C --> G
    B --> H
    C --> H
    
    I --> A
    J --> A
    K --> I
```

## Component Interaction Flow

```mermaid
sequenceDiagram
    participant U as User
    participant C as Controller
    participant S as Service
    participant D as Data Access
    participant DB as Database
    participant E as External Service
    
    U->>C: HTTP Request
    C->>C: Validate Input
    C->>C: Check Authorization
    C->>S: Process Business Logic
    S->>D: Data Operations
    D->>DB: Database Query
    DB-->>D: Query Result
    D-->>S: Data Object
    S->>E: External Service Call
    E-->>S: Service Response
    S-->>C: Business Result
    C->>C: Format Response
    C-->>U: HTTP Response
```

## Performance Architecture

```mermaid
graph TB
    subgraph "Performance Layers"
        A[Client-Side Caching]
        B[Server-Side Caching]
        C[Database Optimization]
        D[Resource Management]
    end
    
    subgraph "Caching Strategy"
        E[Output Caching]
        F[Data Caching]
        G[Session Caching]
        H[Static Content Caching]
    end
    
    subgraph "Database Optimization"
        I[Connection Pooling]
        J[Query Optimization]
        K[Indexing Strategy]
        L[Async Operations]
    end
    
    subgraph "Resource Management"
        M[Memory Management]
        N[Connection Disposal]
        O[File Handling]
        P[Garbage Collection]
    end
    
    A --> E
    A --> H
    B --> F
    B --> G
    C --> I
    C --> J
    C --> K
    C --> L
    D --> M
    D --> N
    D --> O
    D --> P
```

---

*These architecture diagrams provide a comprehensive view of the HVAC Management System's technical structure and component relationships.*
