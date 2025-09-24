# HVAC Management System - Error Codes and Debugging

## Table of Contents
1. [Error Code System](#error-code-system)
2. [Application Error Codes](#application-error-codes)
3. [Database Error Codes](#database-error-codes)
4. [Authentication Error Codes](#authentication-error-codes)
5. [File Upload Error Codes](#file-upload-error-codes)
6. [Email Error Codes](#email-error-codes)
7. [Performance Error Codes](#performance-error-codes)
8. [Debugging Tools](#debugging-tools)
9. [Log Analysis](#log-analysis)
10. [Error Recovery Procedures](#error-recovery-procedures)

## Error Code System

### Error Code Format

The HVAC Management System uses a structured error code format:

```
HVAC-{CATEGORY}-{SEVERITY}-{NUMBER}
```

**Format Breakdown:**
- **HVAC**: System identifier
- **CATEGORY**: Error category (AUTH, DB, FILE, EMAIL, PERF, SYS)
- **SEVERITY**: Error severity (C, H, M, L)
- **NUMBER**: Unique error number (0001-9999)

### Severity Levels

| Code | Level | Description | Response Time |
|------|-------|-------------|---------------|
| **C** | Critical | System down, data loss | Immediate |
| **H** | High | Major functionality affected | 1 hour |
| **M** | Medium | Minor functionality affected | 4 hours |
| **L** | Low | Cosmetic issues | 24 hours |

## Application Error Codes

### 1. General Application Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-SYS-C-0001** | Application startup failed | Critical | Configuration error, missing dependencies | Check web.config, verify dependencies |
| **HVAC-SYS-C-0002** | Memory allocation failed | Critical | Insufficient memory, memory leak | Restart application pool, check for memory leaks |
| **HVAC-SYS-C-0003** | Unhandled exception | Critical | Code bug, unexpected condition | Check logs, fix code issue |
| **HVAC-SYS-H-0001** | Session state unavailable | High | Session configuration error | Check session configuration |
| **HVAC-SYS-H-0002** | Cache initialization failed | High | Cache configuration error | Verify cache settings |
| **HVAC-SYS-M-0001** | View rendering failed | Medium | View syntax error, missing model | Check view syntax, verify model |
| **HVAC-SYS-M-0002** | Partial view not found | Medium | Missing partial view | Create missing partial view |
| **HVAC-SYS-L-0001** | CSS file not found | Low | Missing CSS file | Add missing CSS file |

### 2. Controller Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-SYS-H-0101** | Controller not found | High | Missing controller, routing error | Check routing configuration |
| **HVAC-SYS-H-0102** | Action not found | High | Missing action method | Add missing action method |
| **HVAC-SYS-M-0101** | Model binding failed | Medium | Invalid model data | Check model validation |
| **HVAC-SYS-M-0102** | View not found | Medium | Missing view file | Create missing view |
| **HVAC-SYS-L-0101** | TempData lost | Low | Session timeout | Implement TempData persistence |

### 3. Business Logic Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-SYS-H-0201** | Business rule violation | High | Invalid business logic | Review business rules |
| **HVAC-SYS-M-0201** | Data validation failed | Medium | Invalid input data | Implement proper validation |
| **HVAC-SYS-M-0202** | Workflow state invalid | Medium | Invalid workflow transition | Check workflow logic |
| **HVAC-SYS-L-0201** | Optional feature unavailable | Low | Feature disabled | Enable feature or handle gracefully |

## Database Error Codes

### 1. Connection Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-DB-C-0001** | Database connection failed | Critical | Database server down, network issue | Check database server, network connectivity |
| **HVAC-DB-C-0002** | Connection pool exhausted | Critical | Too many connections, connection leak | Check connection management, increase pool size |
| **HVAC-DB-C-0003** | Database timeout | Critical | Long-running query, deadlock | Optimize queries, check for deadlocks |
| **HVAC-DB-H-0001** | Connection string invalid | High | Incorrect connection string | Verify connection string configuration |
| **HVAC-DB-H-0002** | Database permissions insufficient | High | User lacks permissions | Grant required permissions |
| **HVAC-DB-M-0001** | Connection slow | Medium | Network latency, server load | Optimize network, check server performance |

### 2. Query Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-DB-H-0101** | SQL syntax error | High | Invalid SQL query | Fix SQL syntax |
| **HVAC-DB-H-0102** | Table not found | High | Missing table, incorrect schema | Create table or fix schema reference |
| **HVAC-DB-H-0103** | Column not found | High | Missing column, incorrect column name | Add column or fix column reference |
| **HVAC-DB-M-0101** | Query timeout | Medium | Slow query, missing index | Optimize query, add indexes |
| **HVAC-DB-M-0102** | Deadlock detected | Medium | Concurrent access conflict | Implement retry logic, optimize queries |
| **HVAC-DB-L-0101** | Query returned no data | Low | No matching records | Handle empty result set |

### 3. Data Integrity Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-DB-H-0201** | Primary key violation | High | Duplicate key, constraint violation | Check for duplicates, fix data |
| **HVAC-DB-H-0202** | Foreign key violation | High | Referenced record not found | Create referenced record or fix reference |
| **HVAC-DB-H-0203** | Check constraint violation | High | Data violates business rules | Fix data to comply with constraints |
| **HVAC-DB-M-0201** | Unique constraint violation | Medium | Duplicate unique value | Handle duplicate, implement upsert |
| **HVAC-DB-L-0201** | Data truncation warning | Low | Data too long for column | Truncate data or increase column size |

## Authentication Error Codes

### 1. Login Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-AUTH-H-0001** | Invalid username or password | High | Wrong credentials | Verify credentials, reset password |
| **HVAC-AUTH-H-0002** | Account locked | High | Too many failed attempts | Unlock account, reset failed attempts |
| **HVAC-AUTH-H-0003** | Account disabled | High | Account deactivated | Enable account |
| **HVAC-AUTH-H-0004** | Password expired | High | Password past expiration date | Reset password |
| **HVAC-AUTH-M-0001** | Session expired | Medium | Session timeout | Re-login, extend session |
| **HVAC-AUTH-M-0002** | Invalid session | Medium | Corrupted session data | Clear session, re-login |
| **HVAC-AUTH-L-0001** | Remember me failed | Low | Cookie issue | Clear cookies, re-login |

### 2. Authorization Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-AUTH-H-0101** | Access denied | High | Insufficient permissions | Grant required permissions |
| **HVAC-AUTH-H-0102** | Role not found | High | User has invalid role | Assign valid role |
| **HVAC-AUTH-M-0101** | Resource access denied | Medium | User cannot access resource | Check resource permissions |
| **HVAC-AUTH-M-0102** | Action not allowed | Medium | User cannot perform action | Check action permissions |
| **HVAC-AUTH-L-0101** | Feature not available | Low | Feature disabled for user | Enable feature or inform user |

### 3. Security Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-AUTH-C-0201** | Security breach detected | Critical | Unauthorized access attempt | Block IP, investigate, notify security |
| **HVAC-AUTH-H-0201** | CSRF token invalid | High | CSRF attack attempt | Implement CSRF protection |
| **HVAC-AUTH-H-0202** | XSS attack detected | High | Malicious script injection | Implement XSS protection |
| **HVAC-AUTH-M-0201** | Suspicious activity | Medium | Unusual user behavior | Monitor, investigate |
| **HVAC-AUTH-L-0201** | Password too weak | Low | Password doesn't meet policy | Enforce password policy |

## File Upload Error Codes

### 1. File Validation Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-FILE-H-0001** | File too large | High | File exceeds size limit | Increase size limit or compress file |
| **HVAC-FILE-H-0002** | Invalid file type | High | File type not allowed | Add file type to allowed list |
| **HVAC-FILE-H-0003** | Malicious file detected | High | File contains malware | Scan file, block upload |
| **HVAC-FILE-M-0001** | File name invalid | Medium | Invalid characters in filename | Sanitize filename |
| **HVAC-FILE-M-0002** | File corrupted | Medium | File data corrupted | Request new file |
| **HVAC-FILE-L-0001** | File extension missing | Low | No file extension | Add file extension |

### 2. Storage Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-FILE-C-0101** | Storage full | Critical | No disk space available | Free up space, add storage |
| **HVAC-FILE-H-0101** | Storage access denied | High | Insufficient permissions | Fix storage permissions |
| **HVAC-FILE-H-0102** | File not found | High | File deleted or moved | Check file location |
| **HVAC-FILE-M-0101** | Storage slow | Medium | Storage performance issue | Optimize storage, check I/O |
| **HVAC-FILE-L-0101** | File metadata missing | Low | Missing file information | Recreate metadata |

### 3. Upload Process Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-FILE-H-0201** | Upload interrupted | High | Network issue, timeout | Retry upload |
| **HVAC-FILE-H-0202** | Upload failed | High | Server error, configuration issue | Check server configuration |
| **HVAC-FILE-M-0201** | Chunk upload failed | Medium | Partial upload failure | Implement chunk retry |
| **HVAC-FILE-L-0201** | Upload progress lost | Low | Progress tracking issue | Implement progress persistence |

## Email Error Codes

### 1. SMTP Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-EMAIL-H-0001** | SMTP server unavailable | High | SMTP server down | Check SMTP server status |
| **HVAC-EMAIL-H-0002** | SMTP authentication failed | High | Invalid credentials | Verify SMTP credentials |
| **HVAC-EMAIL-H-0003** | SMTP connection timeout | High | Network issue, firewall | Check network, firewall settings |
| **HVAC-EMAIL-M-0001** | SMTP rate limit exceeded | Medium | Too many emails sent | Implement rate limiting |
| **HVAC-EMAIL-M-0002** | SMTP quota exceeded | Medium | Email quota reached | Increase quota or wait |
| **HVAC-EMAIL-L-0001** | SMTP response slow | Low | SMTP server slow | Optimize SMTP configuration |

### 2. Email Content Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-EMAIL-H-0101** | Invalid email address | High | Malformed email address | Validate email format |
| **HVAC-EMAIL-H-0102** | Email too large | High | Email exceeds size limit | Compress content, split email |
| **HVAC-EMAIL-M-0101** | Email template not found | Medium | Missing email template | Create email template |
| **HVAC-EMAIL-M-0102** | Email encoding error | Medium | Character encoding issue | Fix encoding settings |
| **HVAC-EMAIL-L-0101** | Email formatting issue | Low | HTML/CSS formatting error | Fix email formatting |

### 3. Delivery Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-EMAIL-H-0201** | Email delivery failed | High | Recipient server rejected | Check recipient address, retry |
| **HVAC-EMAIL-M-0201** | Email bounced | Medium | Invalid recipient, full mailbox | Update recipient list |
| **HVAC-EMAIL-M-0202** | Email delayed | Medium | Recipient server slow | Implement retry logic |
| **HVAC-EMAIL-L-0201** | Email read receipt failed | Low | Recipient doesn't support receipts | Handle gracefully |

## Performance Error Codes

### 1. Response Time Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-PERF-H-0001** | Page load timeout | High | Slow query, resource issue | Optimize queries, add resources |
| **HVAC-PERF-H-0002** | API timeout | High | Slow API response | Optimize API, implement caching |
| **HVAC-PERF-M-0001** | Slow page load | Medium | Unoptimized code, missing cache | Optimize code, implement caching |
| **HVAC-PERF-M-0002** | Slow database query | Medium | Missing index, inefficient query | Add indexes, optimize queries |
| **HVAC-PERF-L-0001** | Slow asset loading | Low | Large files, slow CDN | Optimize assets, use CDN |

### 2. Resource Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-PERF-C-0101** | Memory exhausted | Critical | Memory leak, insufficient RAM | Fix memory leaks, add RAM |
| **HVAC-PERF-C-0102** | CPU overloaded | Critical | Infinite loop, heavy processing | Fix code, optimize algorithms |
| **HVAC-PERF-H-0101** | Disk space full | High | Log files, temp files | Clean up files, add storage |
| **HVAC-PERF-M-0101** | High memory usage | Medium | Memory leak, large objects | Optimize memory usage |
| **HVAC-PERF-L-0101** | High CPU usage | Low | Inefficient code | Optimize code performance |

### 3. Concurrency Errors

| Error Code | Description | Severity | Possible Causes | Solutions |
|------------|-------------|----------|-----------------|-----------|
| **HVAC-PERF-H-0201** | Deadlock detected | High | Concurrent access conflict | Implement retry logic |
| **HVAC-PERF-H-0202** | Lock timeout | High | Long-running transaction | Optimize transactions |
| **HVAC-PERF-M-0201** | High contention | Medium | Too many concurrent users | Implement queuing, scaling |
| **HVAC-PERF-L-0201** | Slow lock acquisition | Low | Lock management overhead | Optimize locking strategy |

## Debugging Tools

### 1. Application Debugging

#### Debug Configuration
```xml
<!-- Enable debugging in web.config -->
<system.web>
  <compilation debug="true" targetFramework="4.8" />
  <customErrors mode="Off" />
</system.web>

<!-- Enable detailed errors -->
<system.webServer>
  <httpErrors errorMode="Detailed" />
</system.webServer>
```

#### Debug Logging
```csharp
public class DebugLogger
{
    public static void LogDebug(string message, string category = "Debug")
    {
        if (HttpContext.Current.IsDebuggingEnabled)
        {
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}] {category}: {message}");
        }
    }
    
    public static void LogException(Exception ex, string context = "")
    {
        var message = $"Exception in {context}: {ex.Message}\nStack Trace: {ex.StackTrace}";
        LogDebug(message, "Exception");
    }
}
```

### 2. Database Debugging

#### Query Profiling
```csharp
public class DatabaseProfiler
{
    public static void LogQuery(string sql, TimeSpan duration)
    {
        if (HttpContext.Current.IsDebuggingEnabled)
        {
            System.Diagnostics.Debug.WriteLine($"SQL Query: {sql}");
            System.Diagnostics.Debug.WriteLine($"Duration: {duration.TotalMilliseconds}ms");
        }
    }
    
    public static void LogSlowQuery(string sql, TimeSpan duration)
    {
        if (duration.TotalMilliseconds > 1000) // 1 second
        {
            LoggingHelper.LogWarning($"Slow query detected: {sql} - {duration.TotalMilliseconds}ms");
        }
    }
}
```

### 3. Performance Debugging

#### Performance Profiler
```csharp
public class PerformanceProfiler
{
    private static readonly Dictionary<string, Stopwatch> Timers = new Dictionary<string, Stopwatch>();
    
    public static void StartTimer(string operationName)
    {
        if (HttpContext.Current.IsDebuggingEnabled)
        {
            Timers[operationName] = Stopwatch.StartNew();
        }
    }
    
    public static void StopTimer(string operationName)
    {
        if (HttpContext.Current.IsDebuggingEnabled && Timers.ContainsKey(operationName))
        {
            Timers[operationName].Stop();
            var duration = Timers[operationName].ElapsedMilliseconds;
            System.Diagnostics.Debug.WriteLine($"Operation '{operationName}' took {duration}ms");
            Timers.Remove(operationName);
        }
    }
}
```

## Log Analysis

### 1. Log Structure

#### Standard Log Format
```
[Timestamp] [Level] [Category] [Message] [Exception] [Properties]
```

**Example:**
```
[2024-09-24 10:30:15] [ERROR] [Authentication] [Login failed for user: admin] [System.ArgumentException: Invalid password] [UserID: 1, IP: 192.168.1.100]
```

### 2. Log Analysis Tools

#### Log Parser
```csharp
public class LogAnalyzer
{
    public List<LogEntry> ParseLogs(string logFilePath)
    {
        var logs = new List<LogEntry>();
        var lines = File.ReadAllLines(logFilePath);
        
        foreach (var line in lines)
        {
            var logEntry = ParseLogLine(line);
            if (logEntry != null)
            {
                logs.Add(logEntry);
            }
        }
        
        return logs;
    }
    
    public List<LogEntry> FilterByLevel(List<LogEntry> logs, LogLevel level)
    {
        return logs.Where(l => l.Level == level).ToList();
    }
    
    public List<LogEntry> FilterByCategory(List<LogEntry> logs, string category)
    {
        return logs.Where(l => l.Category == category).ToList();
    }
    
    public List<LogEntry> FilterByTimeRange(List<LogEntry> logs, DateTime from, DateTime to)
    {
        return logs.Where(l => l.Timestamp >= from && l.Timestamp <= to).ToList();
    }
}
```

### 3. Error Pattern Analysis

#### Error Pattern Detector
```csharp
public class ErrorPatternAnalyzer
{
    public List<ErrorPattern> AnalyzeErrorPatterns(List<LogEntry> logs)
    {
        var errorLogs = logs.Where(l => l.Level == LogLevel.Error).ToList();
        var patterns = new List<ErrorPattern>();
        
        // Group by error message
        var groupedErrors = errorLogs.GroupBy(l => l.Message);
        
        foreach (var group in groupedErrors)
        {
            var pattern = new ErrorPattern
            {
                Message = group.Key,
                Count = group.Count(),
                FirstOccurrence = group.Min(l => l.Timestamp),
                LastOccurrence = group.Max(l => l.Timestamp),
                Frequency = group.Count() / (group.Max(l => l.Timestamp) - group.Min(l => l.Timestamp)).TotalHours
            };
            
            patterns.Add(pattern);
        }
        
        return patterns.OrderByDescending(p => p.Count).ToList();
    }
}
```

## Error Recovery Procedures

### 1. Automatic Recovery

#### Retry Logic
```csharp
public class RetryHandler
{
    public static async Task<T> ExecuteWithRetry<T>(Func<Task<T>> operation, int maxRetries = 3, int delayMs = 1000)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                LoggingHelper.LogWarning($"Operation failed, retrying... (Attempt {i + 1}/{maxRetries})", ex);
                await Task.Delay(delayMs * (i + 1)); // Exponential backoff
            }
        }
        
        throw new Exception("Max retries exceeded");
    }
}
```

#### Circuit Breaker
```csharp
public class CircuitBreaker
{
    private int _failureCount = 0;
    private DateTime _lastFailureTime = DateTime.MinValue;
    private readonly int _threshold;
    private readonly TimeSpan _timeout;
    
    public CircuitBreaker(int threshold = 5, TimeSpan timeout = TimeSpan.FromMinutes(1))
    {
        _threshold = threshold;
        _timeout = timeout;
    }
    
    public async Task<T> Execute<T>(Func<Task<T>> operation)
    {
        if (IsOpen())
        {
            throw new CircuitBreakerOpenException("Circuit breaker is open");
        }
        
        try
        {
            var result = await operation();
            OnSuccess();
            return result;
        }
        catch (Exception ex)
        {
            OnFailure();
            throw;
        }
    }
    
    private bool IsOpen()
    {
        return _failureCount >= _threshold && 
               DateTime.Now - _lastFailureTime < _timeout;
    }
    
    private void OnSuccess()
    {
        _failureCount = 0;
    }
    
    private void OnFailure()
    {
        _failureCount++;
        _lastFailureTime = DateTime.Now;
    }
}
```

### 2. Manual Recovery

#### Recovery Procedures
```csharp
public class RecoveryProcedures
{
    public static void RecoverFromDatabaseError()
    {
        // 1. Check database connectivity
        if (!CheckDatabaseConnection())
        {
            // 2. Restart database service
            RestartDatabaseService();
        }
        
        // 3. Clear connection pool
        ClearConnectionPool();
        
        // 4. Verify recovery
        if (CheckDatabaseConnection())
        {
            LoggingHelper.LogInfo("Database recovery successful");
        }
    }
    
    public static void RecoverFromMemoryError()
    {
        // 1. Clear application cache
        HttpContext.Current.Cache.Clear();
        
        // 2. Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        // 3. Restart application pool if necessary
        if (GetMemoryUsage() > 0.9) // 90% memory usage
        {
            RestartApplicationPool();
        }
    }
    
    public static void RecoverFromSessionError()
    {
        // 1. Clear session data
        HttpContext.Current.Session.Clear();
        
        // 2. Regenerate session ID
        HttpContext.Current.Session.Abandon();
        
        // 3. Redirect to login
        HttpContext.Current.Response.Redirect("~/Login/Index");
    }
}
```

### 3. Data Recovery

#### Backup and Restore
```csharp
public class DataRecovery
{
    public static void RestoreFromBackup(DateTime backupDate)
    {
        try
        {
            // 1. Stop application
            StopApplication();
            
            // 2. Restore database
            RestoreDatabase(backupDate);
            
            // 3. Restore files
            RestoreFiles(backupDate);
            
            // 4. Start application
            StartApplication();
            
            LoggingHelper.LogInfo($"Data recovery completed from backup: {backupDate}");
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError("Data recovery failed", ex);
            throw;
        }
    }
    
    public static void RecoverCorruptedData()
    {
        // 1. Identify corrupted data
        var corruptedRecords = IdentifyCorruptedRecords();
        
        // 2. Restore from backup
        foreach (var record in corruptedRecords)
        {
            RestoreRecord(record);
        }
        
        // 3. Verify data integrity
        VerifyDataIntegrity();
    }
}
```

---

*This error codes and debugging guide provides comprehensive information for identifying, diagnosing, and resolving issues in the HVAC Management System.*
