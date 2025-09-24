# HVAC Management System - Security Best Practices

## Table of Contents
1. [Security Development Lifecycle](#security-development-lifecycle)
2. [Secure Coding Practices](#secure-coding-practices)
3. [Input Validation and Sanitization](#input-validation-and-sanitization)
4. [Output Encoding and XSS Prevention](#output-encoding-and-xss-prevention)
5. [SQL Injection Prevention](#sql-injection-prevention)
6. [Authentication Security](#authentication-security)
7. [Session Management Security](#session-management-security)
8. [File Upload Security](#file-upload-security)
9. [Error Handling and Information Disclosure](#error-handling-and-information-disclosure)
10. [Cryptography and Data Protection](#cryptography-and-data-protection)
11. [Network Security](#network-security)
12. [Database Security](#database-security)
13. [Logging and Monitoring](#logging-and-monitoring)
14. [Security Testing](#security-testing)
15. [Incident Response](#incident-response)

## Security Development Lifecycle

### 1. Security Requirements

#### Security Requirements Gathering
```csharp
// Security requirements should be defined at the beginning of development
public class SecurityRequirements
{
    // Authentication Requirements
    public const bool RequireStrongPasswords = true;
    public const bool RequireMultiFactorAuth = true;
    public const int SessionTimeoutMinutes = 30;
    public const int MaxFailedLoginAttempts = 5;
    
    // Authorization Requirements
    public const bool RequireRoleBasedAccess = true;
    public const bool RequireResourceBasedAccess = true;
    public const bool RequireLeastPrivilege = true;
    
    // Data Protection Requirements
    public const bool RequireDataEncryption = true;
    public const bool RequireSecureTransmission = true;
    public const bool RequireAuditLogging = true;
    
    // Input Validation Requirements
    public const bool RequireInputValidation = true;
    public const bool RequireOutputEncoding = true;
    public const bool RequireCSRFProtection = true;
}
```

#### Threat Modeling
```csharp
// Threat modeling should be performed for each component
public class ThreatModel
{
    public List<Threat> IdentifyThreats(string component)
    {
        var threats = new List<Threat>();
        
        switch (component.ToLower())
        {
            case "authentication":
                threats.AddRange(new[]
                {
                    new Threat("Brute Force Attack", "High", "Implement account lockout"),
                    new Threat("Password Guessing", "High", "Enforce strong password policy"),
                    new Threat("Session Hijacking", "Medium", "Use secure session management")
                });
                break;
                
            case "fileupload":
                threats.AddRange(new[]
                {
                    new Threat("Malicious File Upload", "High", "Validate file types and scan content"),
                    new Threat("Path Traversal", "Medium", "Validate file paths"),
                    new Threat("Storage Exhaustion", "Medium", "Implement file size limits")
                });
                break;
        }
        
        return threats;
    }
}
```

### 2. Security Design Principles

#### Secure Design Patterns
```csharp
// Use secure design patterns throughout the application
public class SecureDesignPatterns
{
    // 1. Fail Secure - Default to secure state
    public bool ValidateUserAccess(int userId, string resource)
    {
        // Default to deny access
        if (userId <= 0 || string.IsNullOrEmpty(resource))
            return false;
        
        // Check permissions
        return HasPermission(userId, resource);
    }
    
    // 2. Defense in Depth - Multiple layers of security
    public ActionResult ProcessSensitiveData(SensitiveDataModel model)
    {
        // Layer 1: Input validation
        if (!ModelState.IsValid)
            return View(model);
        
        // Layer 2: Authentication check
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login");
        
        // Layer 3: Authorization check
        if (!HasPermission(GetCurrentUserId(), "SensitiveData", "Process"))
            return RedirectToAction("Unauthorized");
        
        // Layer 4: Business logic validation
        if (!ValidateBusinessRules(model))
            return View(model);
        
        // Process data
        return ProcessData(model);
    }
    
    // 3. Least Privilege - Minimum required access
    public void CreateUser(UserModel model)
    {
        // Only assign minimum required permissions
        var defaultPermissions = GetDefaultPermissionsForRole(model.RoleId);
        model.Permissions = defaultPermissions;
        
        // Don't grant administrative access unless explicitly required
        if (model.RoleId != 1) // Administrator role
        {
            model.Permissions = model.Permissions.Where(p => p.Resource != "System").ToList();
        }
    }
}
```

## Secure Coding Practices

### 1. Input Validation

#### Comprehensive Input Validation
```csharp
public class InputValidator
{
    public ValidationResult ValidateEnquiryInput(EnquiryVM model)
    {
        var result = new ValidationResult();
        
        // Required field validation
        if (string.IsNullOrWhiteSpace(model.ProjectName))
        {
            result.AddError("ProjectName", "Project name is required.");
        }
        
        // Length validation
        if (model.ProjectName?.Length > 200)
        {
            result.AddError("ProjectName", "Project name cannot exceed 200 characters.");
        }
        
        // Format validation
        if (!string.IsNullOrEmpty(model.Email) && !IsValidEmail(model.Email))
        {
            result.AddError("Email", "Invalid email format.");
        }
        
        // Range validation
        if (model.EnquiryDate > DateTime.Now)
        {
            result.AddError("EnquiryDate", "Enquiry date cannot be in the future.");
        }
        
        // Business rule validation
        if (model.CustomerID <= 0)
        {
            result.AddError("CustomerID", "Please select a valid customer.");
        }
        
        // SQL injection prevention
        if (ContainsSQLInjection(model.ProjectName))
        {
            result.AddError("ProjectName", "Invalid characters detected.");
        }
        
        // XSS prevention
        if (ContainsXSS(model.ProjectDescription))
        {
            result.AddError("ProjectDescription", "Invalid characters detected.");
        }
        
        return result;
    }
    
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    private bool ContainsSQLInjection(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;
        
        var sqlKeywords = new[] { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "UNION", "EXEC", "EXECUTE" };
        var upperInput = input.ToUpper();
        
        return sqlKeywords.Any(keyword => upperInput.Contains(keyword));
    }
    
    private bool ContainsXSS(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;
        
        var xssPatterns = new[] { "<script", "javascript:", "onload=", "onerror=", "onclick=" };
        var lowerInput = input.ToLower();
        
        return xssPatterns.Any(pattern => lowerInput.Contains(pattern));
    }
}
```

#### Server-Side Validation Attributes
```csharp
public class EnquiryVM
{
    [Required(ErrorMessage = "Project name is required")]
    [StringLength(200, ErrorMessage = "Project name cannot exceed 200 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-_.,()]+$", ErrorMessage = "Project name contains invalid characters")]
    public string ProjectName { get; set; }
    
    [StringLength(1000, ErrorMessage = "Project description cannot exceed 1000 characters")]
    [AllowHtml] // Only if HTML is required, otherwise remove
    public string ProjectDescription { get; set; }
    
    [Required(ErrorMessage = "Enquiry date is required")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime EnquiryDate { get; set; }
    
    [Required(ErrorMessage = "Customer is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid customer")]
    public int CustomerID { get; set; }
    
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; }
    
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string Phone { get; set; }
}
```

### 2. Output Encoding

#### XSS Prevention
```csharp
public class XSSPrevention
{
    // HTML Encoding
    public string EncodeHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        
        return HttpUtility.HtmlEncode(input);
    }
    
    // JavaScript Encoding
    public string EncodeJavaScript(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        
        return HttpUtility.JavaScriptStringEncode(input);
    }
    
    // URL Encoding
    public string EncodeUrl(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        
        return HttpUtility.UrlEncode(input);
    }
    
    // JSON Encoding
    public string EncodeJson(object input)
    {
        if (input == null)
            return "null";
        
        return JsonConvert.SerializeObject(input);
    }
}
```

#### Razor View Encoding
```html
@* Always encode output in Razor views *@
<div class="project-name">
    @Html.Encode(Model.ProjectName)
</div>

@* For JSON data, use Json.Encode *@
<script>
    var projectData = @Html.Raw(Json.Encode(Model.ProjectData));
</script>

@* For URLs, use Url.Encode *@
<a href="@Url.Action("Details", "Enquiry", new { id = Url.Encode(Model.EnquiryID.ToString()) })">
    View Details
</a>

@* For HTML content that needs to be displayed as HTML *@
<div class="description">
    @Html.Raw(HttpUtility.HtmlDecode(Model.ProjectDescription))
</div>
```

### 3. SQL Injection Prevention

#### Parameterized Queries
```csharp
public class SecureDataAccess
{
    public List<EnquiryVM> GetEnquiries(string enquiryNo, DateTime fromDate, DateTime toDate)
    {
        using (var connection = new SqlConnection(connectionString))
        using (var command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = @"
                SELECT e.EnquiryID, e.EnquiryNo, e.EnquiryDate, e.ProjectName, c.CustomerName
                FROM Enquiry e
                INNER JOIN CustomerMaster c ON e.CustomerID = c.CustomerID
                WHERE e.EnquiryDate BETWEEN @FromDate AND @ToDate
                AND (@EnquiryNo IS NULL OR e.EnquiryNo LIKE @EnquiryNoPattern)";
            
            // Use parameters to prevent SQL injection
            command.Parameters.AddWithValue("@FromDate", fromDate);
            command.Parameters.AddWithValue("@ToDate", toDate);
            command.Parameters.AddWithValue("@EnquiryNoPattern", $"%{enquiryNo}%");
            
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                var enquiries = new List<EnquiryVM>();
                while (reader.Read())
                {
                    enquiries.Add(new EnquiryVM
                    {
                        EnquiryID = reader.GetInt32("EnquiryID"),
                        EnquiryNo = reader.GetString("EnquiryNo"),
                        EnquiryDate = reader.GetDateTime("EnquiryDate"),
                        ProjectName = reader.GetString("ProjectName"),
                        CustomerName = reader.GetString("CustomerName")
                    });
                }
                return enquiries;
            }
        }
    }
    
    // Using Entity Framework (also prevents SQL injection)
    public List<EnquiryVM> GetEnquiriesEF(string enquiryNo, DateTime fromDate, DateTime toDate)
    {
        using (var db = new HVACEntities())
        {
            return db.Enquiries
                .Where(e => e.EnquiryDate >= fromDate && e.EnquiryDate <= toDate)
                .Where(e => string.IsNullOrEmpty(enquiryNo) || e.EnquiryNo.Contains(enquiryNo))
                .Select(e => new EnquiryVM
                {
                    EnquiryID = e.EnquiryID,
                    EnquiryNo = e.EnquiryNo,
                    EnquiryDate = e.EnquiryDate,
                    ProjectName = e.ProjectName,
                    CustomerName = e.CustomerMaster.CustomerName
                })
                .ToList();
        }
    }
}
```

#### Stored Procedures
```sql
-- Use stored procedures for complex queries
CREATE PROCEDURE [dbo].[SP_GetEnquiryList]
    @FromDate DATETIME,
    @ToDate DATETIME,
    @EnquiryNo NVARCHAR(50) = NULL,
    @CustomerID INT = NULL,
    @StatusID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Input validation
    IF @FromDate IS NULL OR @ToDate IS NULL
    BEGIN
        RAISERROR('FromDate and ToDate are required parameters', 16, 1);
        RETURN;
    END
    
    -- Main query with parameters
    SELECT 
        e.EnquiryID,
        e.EnquiryNo,
        e.EnquiryDate,
        e.ProjectName,
        c.CustomerName,
        s.StatusName
    FROM Enquiry e
    INNER JOIN CustomerMaster c ON e.CustomerID = c.CustomerID
    LEFT JOIN EnquiryStatus s ON e.StatusID = s.StatusID
    WHERE e.EnquiryDate BETWEEN @FromDate AND @ToDate
    AND (@EnquiryNo IS NULL OR e.EnquiryNo LIKE '%' + @EnquiryNo + '%')
    AND (@CustomerID IS NULL OR e.CustomerID = @CustomerID)
    AND (@StatusID IS NULL OR e.StatusID = @StatusID)
    ORDER BY e.EnquiryDate DESC;
END
```

## Authentication Security

### 1. Password Security

#### Strong Password Requirements
```csharp
public class PasswordSecurity
{
    public ValidationResult ValidatePasswordStrength(string password, string username = null)
    {
        var result = new ValidationResult();
        
        // Length requirements
        if (password.Length < 8)
        {
            result.AddError("Password", "Password must be at least 8 characters long.");
        }
        
        if (password.Length > 128)
        {
            result.AddError("Password", "Password cannot exceed 128 characters.");
        }
        
        // Character requirements
        if (!password.Any(char.IsUpper))
        {
            result.AddError("Password", "Password must contain at least one uppercase letter.");
        }
        
        if (!password.Any(char.IsLower))
        {
            result.AddError("Password", "Password must contain at least one lowercase letter.");
        }
        
        if (!password.Any(char.IsDigit))
        {
            result.AddError("Password", "Password must contain at least one number.");
        }
        
        if (!password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c)))
        {
            result.AddError("Password", "Password must contain at least one special character.");
        }
        
        // Username similarity check
        if (!string.IsNullOrEmpty(username) && password.ToLower().Contains(username.ToLower()))
        {
            result.AddError("Password", "Password cannot contain the username.");
        }
        
        // Common password check
        if (IsCommonPassword(password))
        {
            result.AddError("Password", "Password is too common. Please choose a stronger password.");
        }
        
        return result;
    }
    
    private bool IsCommonPassword(string password)
    {
        var commonPasswords = new[]
        {
            "password", "123456", "123456789", "qwerty", "abc123",
            "password123", "admin", "letmein", "welcome", "monkey",
            "12345678", "1234567890", "1234567", "password1", "qwerty123"
        };
        
        return commonPasswords.Contains(password.ToLower());
    }
}
```

#### Secure Password Hashing
```csharp
public class SecurePasswordHashing
{
    private const int WorkFactor = 12;
    
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty.");
        
        // Use BCrypt for password hashing
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }
    
    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;
        
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError("Error verifying password", "SecurePasswordHashing", "VerifyPassword", ex);
            return false;
        }
    }
    
    public bool NeedsRehash(string hash)
    {
        try
        {
            var currentWorkFactor = BCrypt.Net.BCrypt.GetWorkFactor(hash);
            return currentWorkFactor < WorkFactor;
        }
        catch
        {
            return true; // Rehash if we can't determine work factor
        }
    }
}
```

### 2. Account Security

#### Account Lockout
```csharp
public class AccountLockout
{
    private const int MaxFailedAttempts = 5;
    private const int LockoutDurationMinutes = 30;
    
    public bool IsAccountLocked(int userId)
    {
        var user = GetUser(userId);
        if (user == null)
            return false;
        
        return user.FailedLoginAttempts >= MaxFailedAttempts &&
               user.LastFailedLogin > DateTime.Now.AddMinutes(-LockoutDurationMinutes);
    }
    
    public void RecordFailedLogin(int userId)
    {
        var user = GetUser(userId);
        if (user == null)
            return;
        
        user.FailedLoginAttempts++;
        user.LastFailedLogin = DateTime.Now;
        
        if (user.FailedLoginAttempts >= MaxFailedAttempts)
        {
            user.IsLocked = true;
            user.LockedUntil = DateTime.Now.AddMinutes(LockoutDurationMinutes);
            
            // Log security event
            LogSecurityEvent("ACCOUNT_LOCKED", userId, "Account locked due to failed login attempts");
        }
        
        SaveUser(user);
    }
    
    public void ResetFailedLogins(int userId)
    {
        var user = GetUser(userId);
        if (user == null)
            return;
        
        user.FailedLoginAttempts = 0;
        user.IsLocked = false;
        user.LockedUntil = null;
        
        SaveUser(user);
    }
}
```

## Session Management Security

### 1. Secure Session Configuration

#### Session Security Settings
```xml
<system.web>
  <sessionState 
    mode="InProc" 
    timeout="30" 
    cookieless="false" 
    regenerateExpiredSessionId="true"
    cookieName="HVAC_Session"
    cookieTimeout="30"
    cookieRequireSSL="true"
    cookieHttpOnly="true"
    cookieSameSite="Strict" />
</system.web>
```

#### Session Security Implementation
```csharp
public class SecureSessionManager
{
    private const int SessionTimeoutMinutes = 30;
    private const string SessionCookieName = "HVAC_Session";
    
    public void CreateSecureSession(int userId)
    {
        // Generate cryptographically secure session ID
        var sessionId = GenerateSecureSessionId();
        
        // Create session data
        var sessionData = new SessionData
        {
            SessionId = sessionId,
            UserId = userId,
            CreatedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddMinutes(SessionTimeoutMinutes),
            IPAddress = GetClientIPAddress(),
            UserAgent = GetUserAgent()
        };
        
        // Store session in database
        StoreSession(sessionData);
        
        // Set secure cookie
        SetSecureCookie(sessionId);
    }
    
    private string GenerateSecureSessionId()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
    
    private void SetSecureCookie(string sessionId)
    {
        var cookie = new HttpCookie(SessionCookieName, sessionId)
        {
            HttpOnly = true,           // Prevent XSS
            Secure = true,             // HTTPS only
            SameSite = SameSiteMode.Strict, // CSRF protection
            Expires = DateTime.Now.AddMinutes(SessionTimeoutMinutes)
        };
        
        HttpContext.Current.Response.Cookies.Add(cookie);
    }
}
```

### 2. Session Validation

#### Session Validation Filter
```csharp
public class SessionValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var session = HttpContext.Current.Session;
        var request = HttpContext.Current.Request;
        
        // Skip validation for public actions
        if (IsPublicAction(filterContext))
        {
            base.OnActionExecuting(filterContext);
            return;
        }
        
        // Check if user is authenticated
        if (session["UserID"] == null)
        {
            RedirectToLogin(filterContext);
            return;
        }
        
        // Validate session
        var sessionId = GetSessionIdFromCookie();
        if (string.IsNullOrEmpty(sessionId))
        {
            RedirectToLogin(filterContext);
            return;
        }
        
        var sessionData = ValidateSession(sessionId);
        if (sessionData == null)
        {
            RedirectToLogin(filterContext);
            return;
        }
        
        // Check session timeout
        if (IsSessionExpired(sessionData))
        {
            LogoutUser(sessionData.UserId);
            RedirectToLogin(filterContext);
            return;
        }
        
        // Check IP address (optional)
        if (HasIPChanged(sessionData))
        {
            LogoutUser(sessionData.UserId);
            RedirectToLogin(filterContext);
            return;
        }
        
        // Extend session if needed
        ExtendSession(sessionData);
        
        base.OnActionExecuting(filterContext);
    }
}
```

## File Upload Security

### 1. File Validation

#### Comprehensive File Validation
```csharp
public class FileUploadSecurity
{
    private readonly string[] _allowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif" };
    private readonly string[] _allowedMimeTypes = { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "image/jpeg", "image/png", "image/gif" };
    private readonly int _maxFileSize = 10 * 1024 * 1024; // 10MB
    
    public ValidationResult ValidateFile(HttpPostedFileBase file)
    {
        var result = new ValidationResult();
        
        // Check if file exists
        if (file == null || file.ContentLength == 0)
        {
            result.AddError("File", "Please select a file to upload.");
            return result;
        }
        
        // Check file size
        if (file.ContentLength > _maxFileSize)
        {
            result.AddError("File", $"File size cannot exceed {_maxFileSize / (1024 * 1024)} MB.");
            return result;
        }
        
        // Check file extension
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!_allowedExtensions.Contains(extension))
        {
            result.AddError("File", $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");
            return result;
        }
        
        // Check MIME type
        if (!_allowedMimeTypes.Contains(file.ContentType))
        {
            result.AddError("File", $"File MIME type '{file.ContentType}' is not allowed.");
            return result;
        }
        
        // Check file header (magic bytes)
        if (!ValidateFileHeader(file))
        {
            result.AddError("File", "File content does not match the file extension.");
            return result;
        }
        
        // Check for malicious content
        if (ContainsMaliciousContent(file))
        {
            result.AddError("File", "File contains potentially malicious content.");
            return result;
        }
        
        return result;
    }
    
    private bool ValidateFileHeader(HttpPostedFileBase file)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();
        var header = new byte[8];
        
        file.InputStream.Position = 0;
        file.InputStream.Read(header, 0, 8);
        file.InputStream.Position = 0;
        
        switch (extension)
        {
            case ".pdf":
                return header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46; // %PDF
            case ".jpg":
            case ".jpeg":
                return header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF; // JPEG
            case ".png":
                return header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47; // PNG
            case ".gif":
                return header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46; // GIF
            default:
                return true; // Allow other types for now
        }
    }
    
    private bool ContainsMaliciousContent(HttpPostedFileBase file)
    {
        // Check for common malicious patterns
        var maliciousPatterns = new[]
        {
            "<script", "javascript:", "vbscript:", "onload=", "onerror=", "onclick=",
            "eval(", "document.cookie", "document.write", "window.location"
        };
        
        file.InputStream.Position = 0;
        var content = new byte[file.ContentLength];
        file.InputStream.Read(content, 0, file.ContentLength);
        file.InputStream.Position = 0;
        
        var textContent = Encoding.UTF8.GetString(content).ToLower();
        
        return maliciousPatterns.Any(pattern => textContent.Contains(pattern));
    }
}
```

### 2. Secure File Storage

#### Secure File Storage Implementation
```csharp
public class SecureFileStorage
{
    public string SaveFile(HttpPostedFileBase file, string uploadPath)
    {
        // Generate secure filename
        var fileName = GenerateSecureFileName(file.FileName);
        
        // Create secure directory path
        var securePath = Path.Combine(uploadPath, GetSecureSubDirectory());
        if (!Directory.Exists(securePath))
        {
            Directory.CreateDirectory(securePath);
        }
        
        // Create full file path
        var filePath = Path.Combine(securePath, fileName);
        
        // Save file
        file.SaveAs(filePath);
        
        // Set secure file permissions
        SetSecureFilePermissions(filePath);
        
        return filePath;
    }
    
    private string GenerateSecureFileName(string originalName)
    {
        var extension = Path.GetExtension(originalName);
        var fileName = Guid.NewGuid().ToString() + extension;
        return fileName;
    }
    
    private string GetSecureSubDirectory()
    {
        // Create subdirectory based on date to prevent directory listing
        return DateTime.Now.ToString("yyyy/MM/dd");
    }
    
    private void SetSecureFilePermissions(string filePath)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);
            fileInfo.Attributes = FileAttributes.Normal;
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError("Error setting file permissions", "SecureFileStorage", "SetSecureFilePermissions", ex);
        }
    }
}
```

## Error Handling and Information Disclosure

### 1. Secure Error Handling

#### Error Handling Implementation
```csharp
public class SecureErrorHandling
{
    public ActionResult HandleError(Exception ex, string actionName, string controllerName)
    {
        // Log the error
        LoggingHelper.LogError($"Error in {controllerName}.{actionName}", controllerName, actionName, ex);
        
        // Don't expose sensitive information
        var errorMessage = GetSafeErrorMessage(ex);
        
        // Return appropriate error response
        if (Request.IsAjaxRequest())
        {
            return Json(new { success = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }
        
        return View("Error", new ErrorViewModel { Message = errorMessage });
    }
    
    private string GetSafeErrorMessage(Exception ex)
    {
        // Return generic error message to users
        return "An error occurred while processing your request. Please try again.";
    }
    
    private string GetDetailedErrorMessage(Exception ex)
    {
        // Return detailed error message for logging
        var sb = new StringBuilder();
        sb.AppendLine($"Exception Type: {ex.GetType().FullName}");
        sb.AppendLine($"Exception Message: {ex.Message}");
        sb.AppendLine($"Stack Trace: {ex.StackTrace}");
        
        if (ex.InnerException != null)
        {
            sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
        }
        
        return sb.ToString();
    }
}
```

#### Global Error Handling
```csharp
public class GlobalErrorHandler : IExceptionFilter
{
    public void OnException(ExceptionContext filterContext)
    {
        if (filterContext.ExceptionHandled)
            return;
        
        var ex = filterContext.Exception;
        var controllerName = filterContext.RouteData.Values["controller"].ToString();
        var actionName = filterContext.RouteData.Values["action"].ToString();
        
        // Log the error
        LoggingHelper.LogError($"Unhandled exception in {controllerName}.{actionName}", controllerName, actionName, ex);
        
        // Set error response
        filterContext.Result = new ViewResult
        {
            ViewName = "Error",
            ViewData = new ViewDataDictionary(new ErrorViewModel
            {
                Message = "An error occurred while processing your request."
            })
        };
        
        filterContext.ExceptionHandled = true;
    }
}
```

### 2. Information Disclosure Prevention

#### Safe Error Messages
```csharp
public class SafeErrorMessages
{
    public static string GetSafeErrorMessage(Exception ex)
    {
        // Don't expose internal details
        switch (ex)
        {
            case SqlException sqlEx:
                return "A database error occurred. Please try again.";
            case UnauthorizedAccessException:
                return "You do not have permission to perform this action.";
            case ArgumentException:
                return "Invalid input provided. Please check your data.";
            case TimeoutException:
                return "The request timed out. Please try again.";
            default:
                return "An error occurred while processing your request.";
        }
    }
}
```

## Cryptography and Data Protection

### 1. Data Encryption

#### Encryption Service
```csharp
public class EncryptionService
{
    private readonly string _encryptionKey;
    
    public EncryptionService()
    {
        _encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
    }
    
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;
        
        using (var aes = Aes.Create())
        {
            aes.Key = GetEncryptionKey();
            aes.GenerateIV();
            
            using (var encryptor = aes.CreateEncryptor())
            using (var msEncrypt = new MemoryStream())
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
                swEncrypt.Close();
                
                var encrypted = msEncrypt.ToArray();
                var result = new byte[aes.IV.Length + encrypted.Length];
                Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);
                
                return Convert.ToBase64String(result);
            }
        }
    }
    
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;
        
        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            
            using (var aes = Aes.Create())
            {
                aes.Key = GetEncryptionKey();
                
                var iv = new byte[aes.IV.Length];
                var cipher = new byte[fullCipher.Length - iv.Length];
                
                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
                
                aes.IV = iv;
                
                using (var decryptor = aes.CreateDecryptor())
                using (var msDecrypt = new MemoryStream(cipher))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError("Error decrypting data", "EncryptionService", "Decrypt", ex);
            return string.Empty;
        }
    }
    
    private byte[] GetEncryptionKey()
    {
        // Derive key from configuration
        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(_encryptionKey));
        }
    }
}
```

### 2. Sensitive Data Protection

#### Sensitive Data Handling
```csharp
public class SensitiveDataProtection
{
    private readonly EncryptionService _encryptionService;
    
    public SensitiveDataProtection()
    {
        _encryptionService = new EncryptionService();
    }
    
    public string ProtectSensitiveData(string data)
    {
        if (string.IsNullOrEmpty(data))
            return string.Empty;
        
        return _encryptionService.Encrypt(data);
    }
    
    public string UnprotectSensitiveData(string encryptedData)
    {
        if (string.IsNullOrEmpty(encryptedData))
            return string.Empty;
        
        return _encryptionService.Decrypt(encryptedData);
    }
    
    public string MaskSensitiveData(string data, int visibleChars = 4)
    {
        if (string.IsNullOrEmpty(data) || data.Length <= visibleChars)
            return new string('*', data?.Length ?? 0);
        
        var masked = new string('*', data.Length - visibleChars);
        var visible = data.Substring(data.Length - visibleChars);
        
        return masked + visible;
    }
}
```

## Network Security

### 1. HTTPS Configuration

#### SSL/TLS Configuration
```xml
<system.webServer>
  <httpProtocol>
    <customHeaders>
      <add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains; preload" />
      <add name="X-Content-Type-Options" value="nosniff" />
      <add name="X-Frame-Options" value="DENY" />
      <add name="X-XSS-Protection" value="1; mode=block" />
      <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
    </customHeaders>
  </httpProtocol>
  
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
</system.webServer>
```

### 2. Request Validation

#### Request Validation Configuration
```xml
<system.web>
  <pages validateRequest="true" enableViewStateMac="true" />
  <httpRuntime requestValidationMode="2.0" requestPathInvalidCharacters="&lt;&gt;*%&amp;:&quot;?[]{}|" />
</system.web>
```

## Database Security

### 1. Connection Security

#### Secure Connection Strings
```xml
<connectionStrings>
  <add name="myConnectionString" 
       connectionString="data source=server;initial catalog=HVAC_DB;user id=user;password=pass;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Pooling=true;Max Pool Size=100;Min Pool Size=5;" />
</connectionStrings>
```

### 2. Database Access Control

#### Database User Permissions
```sql
-- Create application user with minimal permissions
CREATE LOGIN [HVAC_App] WITH PASSWORD = 'SecurePassword123!';
CREATE USER [HVAC_App] FOR LOGIN [HVAC_App];

-- Grant only necessary permissions
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [HVAC_App];
GRANT EXECUTE ON SCHEMA::dbo TO [HVAC_App];

-- Deny dangerous permissions
DENY ALTER ON SCHEMA::dbo TO [HVAC_App];
DENY CREATE ON SCHEMA::dbo TO [HVAC_App];
DENY DROP ON SCHEMA::dbo TO [HVAC_App];
```

## Logging and Monitoring

### 1. Security Event Logging

#### Security Logger
```csharp
public class SecurityLogger
{
    public void LogSecurityEvent(string eventType, int userId, string details)
    {
        var logEntry = new SecurityAuditLog
        {
            EventType = eventType,
            UserID = userId,
            IPAddress = GetClientIPAddress(),
            UserAgent = GetUserAgent(),
            Details = details,
            Timestamp = DateTime.Now,
            Severity = GetEventSeverity(eventType)
        };
        
        SaveAuditLog(logEntry);
    }
    
    public void LogAuthenticationEvent(string eventType, int userId, bool success)
    {
        var details = $"Authentication {eventType}: {(success ? "Success" : "Failed")}";
        LogSecurityEvent(eventType, userId, details);
    }
    
    public void LogAuthorizationEvent(string eventType, int userId, string resource, string action, bool success)
    {
        var details = $"Authorization {eventType}: Resource={resource}, Action={action}, Success={success}";
        LogSecurityEvent(eventType, userId, details);
    }
}
```

### 2. Security Monitoring

#### Security Monitor
```csharp
public class SecurityMonitor
{
    public void MonitorSecurityEvents()
    {
        // Check for suspicious activity
        CheckFailedLoginAttempts();
        CheckUnusualAccessPatterns();
        CheckPrivilegeEscalations();
        CheckDataAccessAnomalies();
    }
    
    private void CheckFailedLoginAttempts()
    {
        var recentFailures = GetFailedLoginAttempts(DateTime.Now.AddMinutes(-15));
        if (recentFailures.Count > 10)
        {
            SendSecurityAlert("High number of failed login attempts detected");
        }
    }
    
    private void CheckUnusualAccessPatterns()
    {
        var unusualAccess = GetUnusualAccessPatterns(DateTime.Now.AddHours(-1));
        if (unusualAccess.Any())
        {
            SendSecurityAlert("Unusual access patterns detected");
        }
    }
}
```

## Security Testing

### 1. Automated Security Testing

#### Security Test Framework
```csharp
[TestClass]
public class SecurityTests
{
    [TestMethod]
    public void TestSQLInjectionPrevention()
    {
        // Test SQL injection prevention
        var maliciousInput = "'; DROP TABLE Users; --";
        var result = new EnquiryController().Create(new EnquiryVM { ProjectName = maliciousInput });
        
        // Should not cause SQL injection
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public void TestXSSPrevention()
    {
        // Test XSS prevention
        var maliciousInput = "<script>alert('XSS')</script>";
        var encoded = HttpUtility.HtmlEncode(maliciousInput);
        
        Assert.IsFalse(encoded.Contains("<script>"));
    }
    
    [TestMethod]
    public void TestAuthenticationRequired()
    {
        // Test that authentication is required
        var controller = new EnquiryController();
        var result = controller.Index();
        
        // Should redirect to login if not authenticated
        Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
    }
}
```

### 2. Penetration Testing

#### Security Checklist
```csharp
public class SecurityChecklist
{
    public List<SecurityCheck> GetSecurityChecks()
    {
        return new List<SecurityCheck>
        {
            new SecurityCheck("Authentication", "Strong password policy enforced", CheckPasswordPolicy),
            new SecurityCheck("Authentication", "Account lockout implemented", CheckAccountLockout),
            new SecurityCheck("Authorization", "Role-based access control", CheckRBAC),
            new SecurityCheck("Input Validation", "SQL injection prevention", CheckSQLInjectionPrevention),
            new SecurityCheck("Input Validation", "XSS prevention", CheckXSSPrevention),
            new SecurityCheck("Session Management", "Secure session handling", CheckSessionSecurity),
            new SecurityCheck("File Upload", "File validation implemented", CheckFileUploadSecurity),
            new SecurityCheck("Error Handling", "Information disclosure prevention", CheckErrorHandling),
            new SecurityCheck("Cryptography", "Data encryption implemented", CheckDataEncryption),
            new SecurityCheck("Network Security", "HTTPS enforced", CheckHTTPSEnforcement)
        };
    }
}
```

## Incident Response

### 1. Security Incident Response

#### Incident Response Plan
```csharp
public class SecurityIncidentResponse
{
    public void HandleSecurityIncident(SecurityIncident incident)
    {
        // 1. Immediate containment
        ContainIncident(incident);
        
        // 2. Assess impact
        var impact = AssessImpact(incident);
        
        // 3. Notify stakeholders
        NotifyStakeholders(incident, impact);
        
        // 4. Collect evidence
        CollectEvidence(incident);
        
        // 5. Remediate
        RemediateIncident(incident);
        
        // 6. Post-incident review
        ConductPostIncidentReview(incident);
    }
    
    private void ContainIncident(SecurityIncident incident)
    {
        switch (incident.Type)
        {
            case "DATA_BREACH":
                // Isolate affected systems
                IsolateAffectedSystems(incident);
                break;
            case "UNAUTHORIZED_ACCESS":
                // Revoke access
                RevokeUserAccess(incident.UserId);
                break;
            case "MALWARE_DETECTION":
                // Quarantine affected files
                QuarantineAffectedFiles(incident);
                break;
        }
    }
}
```

### 2. Security Incident Classification

#### Incident Severity Levels
```csharp
public enum SecurityIncidentSeverity
{
    Low = 1,        // Minor security events
    Medium = 2,     // Moderate security events
    High = 3,       // Significant security events
    Critical = 4    // Major security breaches
}

public class SecurityIncident
{
    public int IncidentID { get; set; }
    public string Type { get; set; }
    public SecurityIncidentSeverity Severity { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
    public string IPAddress { get; set; }
    public DateTime DetectedAt { get; set; }
    public DateTime ResolvedAt { get; set; }
    public string Resolution { get; set; }
}
```

---

*This security best practices documentation provides comprehensive guidance for implementing secure coding practices, security controls, and incident response procedures in the HVAC Management System.*
