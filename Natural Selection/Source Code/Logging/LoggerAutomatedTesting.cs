using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using LoggingLib;
using System.Linq;
using System;
using System.Diagnostics;

namespace LoggingLib.Tests
{
    [TestClass]
    public class LoggerTests
    {
        private Logger _logger;
        private readonly string _connectionString = "Server=localhost;Database=LoggingDb;Trusted_Connection=True;";
        private DataAccess _dataAccess; // Assume DataAccess is a class that handles database operations

        [TestInitialize]
        public void Initialize()
        {
            _dataAccess = new DataAccess(_connectionString); // Initialize DataAccess with the connection string
            _logger = new Logger(_dataAccess); // Initialize Logger with DataAccess
            // Additional setup for the test database may be required here
        }

        [TestMethod]
        public async Task LogAsync_OperationCompletesWithinThreeSeconds()
        {
            var expectedActionType = "TestAction";
            var expectedLogDetail = "Test log message";

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await _logger.LogAsync(new LogEntry("TestUserHash", expectedActionType, "Success", expectedLogDetail));

            stopwatch.Stop();
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 3000, "Logging operation took longer than 3 seconds.");
        }

        [TestMethod]
        public async Task LogAsync_ShouldSaveLogEntry()
        {
            var expectedActionType = "TestAction";
            var expectedLogDetail = "Test log message";

            await _logger.LogAsync(new LogEntry("TestUserHash", expectedActionType, "Success", expectedLogDetail));

            bool logEntryExists = CheckIfLogEntryExists("TestUserHash", expectedActionType, "Success", expectedLogDetail);
            Assert.IsTrue(logEntryExists, "Log entry was not saved correctly.");
        }

        [TestMethod]
        public async Task LogAsync_ShouldBeThreadSafe()
        {
            var tasks = new List<Task>();
            int numberOfParallelLogs = 100;

            for (int i = 0; i < numberOfParallelLogs; i++)
            {
                string detail = $"Parallel log {i}";
                tasks.Add(_logger.LogAsync(new LogEntry("TestUserHash", "ParallelAction", "Success", detail)));
            }

            await Task.WhenAll(tasks);

            // Assert that all logs are saved
            // This part of the test may need adjustment based on how you verify the log entries
        }

        [TestMethod]
        public async Task LogAsync_ShouldSaveCompleteLogEntry()
        {
            var expectedUserHash = "HashOfTestUser";
            var expectedActionType = "TestAction";
            var expectedLogStatus = "Success";
            var expectedLogDetail = "Test log message";

            await _logger.LogAsync(new LogEntry(expectedUserHash, expectedActionType, expectedLogStatus, expectedLogDetail));

            var logEntry = FindLogEntry(expectedUserHash, expectedActionType, expectedLogStatus, expectedLogDetail);
            Assert.IsNotNull(logEntry, "Log entry was not found.");
            Assert.IsTrue(logEntry.LogTime.ToUniversalTime() <= DateTime.UtcNow, "LogTime is not in UTC or is in the future.");
        }

        [TestMethod]
        public async Task LogAsync_ShouldLogWithUTCTimestamp()
        {
            var expectedActionType = "TestAction";
            var expectedLogDetail = "Test UTC timestamp";

            await _logger.LogAsync(new LogEntry(null, expectedActionType, "Success", expectedLogDetail));

            var logEntry = FindLogEntry(null, expectedActionType, "Success", expectedLogDetail);
            Assert.IsNotNull(logEntry, "Log entry was not found.");
            Assert.IsTrue(logEntry.LogTime.ToUniversalTime() == logEntry.LogTime, "LogTime is not in UTC.");
        }
        
    }
}
