using System;

namespace LogAnalysis.Core.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public required string EventType { get; set; }
        public required string UserId { get; set; }
        public required string StreamId { get; set; }
        public required string Message { get; set; }
        public string? OptionalData { get; set; }

        public bool IsUserActivity => EventType == "USER_JOIN" || EventType == "USER_LEAVE";
        public bool IsError => EventType == "ERROR" || EventType == "CRITICAL" || EventType == "WARNING";
    }
}
