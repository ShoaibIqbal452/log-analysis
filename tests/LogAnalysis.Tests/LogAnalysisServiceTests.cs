using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using LogAnalysis.Core.Services;
using LogAnalysis.Core.Models;
using Xunit;

namespace LogAnalysis.Tests
{
    public class LogAnalysisServiceTests
    {
        private readonly LogAnalysisService _service;
        private readonly string _testLogPath;

        public LogAnalysisServiceTests()
        {
            _service = new LogAnalysisService();
            _testLogPath = Path.Combine(Path.GetTempPath(), "test_webrtc.log");
        }

        [Fact]
        public async Task AnalyzeLogFileAsync_WithValidLogFile_ReturnsCorrectAnalysis()
        {
            var logContent = @"[2023-10-27 10:01:00] USER_JOIN 456 - User 456 joined the session
[2023-10-27 10:02:00] ERROR 789 stream_789 Stream error [Error Code:500]
[2023-10-27 10:03:00] USER_JOIN 123 - User 123 joined the session
[2023-10-27 10:04:00] USER_LEAVE 456 - User 456 left the session
[2023-10-27 10:05:00] CRITICAL 123 - Critical error occurred";

            await File.WriteAllTextAsync(_testLogPath, logContent);

            try
            {
                var result = await _service.AnalyzeLogFileAsync(_testLogPath);

                Assert.Equal(2, result.UniqueUsers);
                Assert.Equal(3, result.UserActivity.Count);
                Assert.Equal(2, result.Errors.Count);
                Assert.Equal(1, result.Errors["ERROR"]);
                Assert.Equal(1, result.Errors["CRITICAL"]);
            }
            finally
            {
                if (File.Exists(_testLogPath))
                    File.Delete(_testLogPath);
            }
        }

        [Fact]
        public async Task AnalyzeLogFileAsync_WithInvalidPath_ThrowsFileNotFoundException()
        {
            var invalidPath = "nonexistent.log";

            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _service.AnalyzeLogFileAsync(invalidPath));
        }

        [Fact]
        public async Task AnalyzeLogFileAsync_WithEmptyFile_ReturnsEmptyAnalysis()
        {
            await File.WriteAllTextAsync(_testLogPath, string.Empty);

            try
            {
                var result = await _service.AnalyzeLogFileAsync(_testLogPath);

                Assert.Equal(0, result.UniqueUsers);
                Assert.Empty(result.UserActivity);
                Assert.Empty(result.Errors);
            }
            finally
            {
                if (File.Exists(_testLogPath))
                    File.Delete(_testLogPath);
            }
        }

        [Fact]
        public async Task AnalyzeLogFileAsync_WithMalformedLines_SkipsInvalidLines()
        {
            var logContent = @"[2023-10-27 10:01:00] USER_JOIN 456 - User 456 joined the session
This is an invalid line
[2023-10-27 10:04:00] USER_LEAVE 456 - User 456 left the session";

            await File.WriteAllTextAsync(_testLogPath, logContent);

            try
            {
                var result = await _service.AnalyzeLogFileAsync(_testLogPath);

                Assert.Equal(1, result.UniqueUsers);
                Assert.Equal(2, result.UserActivity.Count);
                Assert.Empty(result.Errors);
            }
            finally
            {
                if (File.Exists(_testLogPath))
                    File.Delete(_testLogPath);
            }
        }

        [Fact]
        public async Task AnalyzeLogFileAsync_WithMultipleJoinsFromSameUser_CountsUserOnce()
        {
            var logContent = @"[2023-10-27 10:01:00] USER_JOIN 456 - User 456 joined the session
[2023-10-27 10:02:00] USER_LEAVE 456 - User 456 left the session
[2023-10-27 10:03:00] USER_JOIN 456 - User 456 joined the session";

            await File.WriteAllTextAsync(_testLogPath, logContent);

            try
            {
                var result = await _service.AnalyzeLogFileAsync(_testLogPath);

                Assert.Equal(1, result.UniqueUsers);
                Assert.Equal(3, result.UserActivity.Count);
                var activities = result.UserActivity.ToList();
                Assert.Equal("456", activities[0].UserId);
                Assert.Equal("JOIN", activities[0].Event);
                Assert.Equal("2023-10-27 10:01:00", activities[0].Timestamp);
            }
            finally
            {
                if (File.Exists(_testLogPath))
                    File.Delete(_testLogPath);
            }
        }

        [Fact]
        public async Task AnalyzeLogFileAsync_WithAllErrorTypes_CountsErrorsCorrectly()
        {
            var logContent = @"[2023-10-27 10:01:00] ERROR 456 - Error message
[2023-10-27 10:02:00] WARNING 456 - Warning message
[2023-10-27 10:03:00] CRITICAL 456 - Critical message
[2023-10-27 10:04:00] ERROR 456 - Another error";

            await File.WriteAllTextAsync(_testLogPath, logContent);

            try
            {
                var result = await _service.AnalyzeLogFileAsync(_testLogPath);

                Assert.Equal(3, result.Errors.Count);
                Assert.Equal(2, result.Errors["ERROR"]);
                Assert.Equal(1, result.Errors["WARNING"]);
                Assert.Equal(1, result.Errors["CRITICAL"]);
            }
            finally
            {
                if (File.Exists(_testLogPath))
                    File.Delete(_testLogPath);
            }
        }
    }
}
