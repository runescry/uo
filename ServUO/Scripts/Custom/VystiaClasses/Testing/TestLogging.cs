/*
 * Vystia Test Suite Logging
 *
 * Server-side logging for test execution, failures, and exceptions
 */

using System;
using System.IO;
using System.Text;

namespace Server.Custom.VystiaClasses.Testing
{
    /// <summary>
    /// Logging utility for test suite
    /// </summary>
    public static class TestLogging
    {
        private static StreamWriter m_LogWriter;
        private static readonly object m_Lock = new object();
        private static bool m_Initialized = false;

        /// <summary>
        /// Initialize test logging
        /// </summary>
        public static void Initialize()
        {
            if (m_Initialized)
                return;

            try
            {
                string logDirectory = Path.Combine(Core.BaseDirectory, "Logs", "VystiaTests");
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                string logFile = Path.Combine(logDirectory, 
                    $"TestSuite_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.log");

                m_LogWriter = new StreamWriter(logFile, true, Encoding.UTF8)
                {
                    AutoFlush = true
                };

                WriteLine("=== Vystia Test Suite Logging Initialized ===");
                WriteLine("Log file: {0}", logFile);
                WriteLine("");

                m_Initialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Vystia Tests] Failed to initialize logging: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Write a log entry
        /// </summary>
        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        /// <summary>
        /// Write a log entry
        /// </summary>
        public static void WriteLine(string message)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] {message}";

            // Write to console
            Console.WriteLine("[Vystia Tests] {0}", message);

            // Write to file
            lock (m_Lock)
            {
                if (m_LogWriter != null && !m_LogWriter.BaseStream.CanWrite)
                {
                    // Stream closed, try to reinitialize
                    try
                    {
                        m_LogWriter.Close();
                        Initialize();
                    }
                    catch { }
                }

                if (m_LogWriter != null)
                {
                    try
                    {
                        m_LogWriter.WriteLine(logEntry);
                        m_LogWriter.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Vystia Tests] Log write error: {0}", ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Log test execution start
        /// </summary>
        public static void LogTestStart(string testName, string category)
        {
            WriteLine("START: {0} ({1})", testName, category);
        }

        /// <summary>
        /// Log test execution result
        /// </summary>
        public static void LogTestResult(string testName, string category, bool passed, string errorMessage = null)
        {
            if (passed)
            {
                WriteLine("PASS: {0} ({1})", testName, category);
            }
            else
            {
                WriteLine("FAIL: {0} ({1}) - {2}", testName, category, errorMessage ?? "Unknown error");
            }
        }

        /// <summary>
        /// Log test suite execution start
        /// </summary>
        public static void LogTestSuiteStart(string scope, int totalTests)
        {
            WriteLine("");
            WriteLine("========================================");
            WriteLine("TEST SUITE EXECUTION STARTED");
            WriteLine("Scope: {0}", scope);
            WriteLine("Total Tests: {0}", totalTests);
            WriteLine("Time: {0}", DateTime.UtcNow);
            WriteLine("========================================");
        }

        /// <summary>
        /// Log test suite execution end
        /// </summary>
        public static void LogTestSuiteEnd(string scope, int passed, int failed, int total, TimeSpan duration)
        {
            WriteLine("");
            WriteLine("========================================");
            WriteLine("TEST SUITE EXECUTION COMPLETED");
            WriteLine("Scope: {0}", scope);
            WriteLine("Passed: {0} | Failed: {1} | Total: {2}", passed, failed, total);
            WriteLine("Duration: {0:F2} seconds", duration.TotalSeconds);
            WriteLine("Time: {0}", DateTime.UtcNow);
            WriteLine("========================================");
            WriteLine("");
        }

        /// <summary>
        /// Log exception with full stack trace
        /// </summary>
        public static void LogException(string context, Exception ex)
        {
            WriteLine("EXCEPTION in {0}: {1}", context, ex.Message);
            WriteLine("Stack Trace: {0}", ex.StackTrace);
            
            if (ex.InnerException != null)
            {
                WriteLine("Inner Exception: {0}", ex.InnerException.Message);
                WriteLine("Inner Stack Trace: {0}", ex.InnerException.StackTrace);
            }
        }

        /// <summary>
        /// Log test category summary
        /// </summary>
        public static void LogCategorySummary(string category, int passed, int total)
        {
            WriteLine("Category: {0} - {1}/{2} passed", category, passed, total);
        }

        /// <summary>
        /// Close logging
        /// </summary>
        public static void Close()
        {
            lock (m_Lock)
            {
                if (m_LogWriter != null)
                {
                    try
                    {
                        WriteLine("=== Test Suite Logging Closed ===");
                        m_LogWriter.Close();
                        m_LogWriter = null;
                    }
                    catch { }
                }
            }
        }
    }
}
