using System;
using System.IO;
using System.Web;

namespace HVAC.Common
{
    /// <summary>
    /// Simple logging helper for the HVAC application
    /// </summary>
    public static class LoggingHelper
    {
        private static readonly string LogPath = HttpContext.Current?.Server?.MapPath("~/Logs/") ?? "~/Logs/";
        
        /// <summary>
        /// Logs an information message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="controller">The controller name (optional)</param>
        /// <param name="action">The action name (optional)</param>
        public static void LogInfo(string message, string controller = "", string action = "")
        {
            LogMessage("INFO", message, controller, action);
        }
        
        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="controller">The controller name (optional)</param>
        /// <param name="action">The action name (optional)</param>
        public static void LogWarning(string message, string controller = "", string action = "")
        {
            LogMessage("WARNING", message, controller, action);
        }
        
        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="controller">The controller name (optional)</param>
        /// <param name="action">The action name (optional)</param>
        /// <param name="exception">The exception to log (optional)</param>
        public static void LogError(string message, string controller = "", string action = "", Exception exception = null)
        {
            var fullMessage = exception != null ? $"{message} - Exception: {exception.Message} - StackTrace: {exception.StackTrace}" : message;
            LogMessage("ERROR", fullMessage, controller, action);
        }
        
        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="controller">The controller name (optional)</param>
        /// <param name="action">The action name (optional)</param>
        public static void LogDebug(string message, string controller = "", string action = "")
        {
            LogMessage("DEBUG", message, controller, action);
        }
        
        /// <summary>
        /// Internal method to write log messages to file
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        /// <param name="controller">The controller name</param>
        /// <param name="action">The action name</param>
        private static void LogMessage(string level, string message, string controller, string action)
        {
            try
            {
                // Ensure log directory exists
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                
                var fileName = $"HVAC_{DateTime.Now:yyyyMMdd}.log";
                var filePath = Path.Combine(LogPath, fileName);
                
                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}]";
                if (!string.IsNullOrEmpty(controller))
                {
                    logEntry += $" [{controller}";
                    if (!string.IsNullOrEmpty(action))
                    {
                        logEntry += $".{action}";
                    }
                    logEntry += "]";
                }
                logEntry += $" {message}{Environment.NewLine}";
                
                File.AppendAllText(filePath, logEntry);
            }
            catch
            {
                // Silently fail if logging fails to prevent application crashes
            }
        }
    }
}
