using System;
using System.Collections.Generic;

namespace LogAnalysis.Core.Models
{
    public class UserActivity
    {
        public required string UserId { get; set; }
        public required string Event { get; set; }
        public required string Timestamp { get; set; }
    }

    public class LogAnalysisResponse
    {
        public int UniqueUsers { get; set; }
        public List<UserActivity> UserActivity { get; set; }
        public Dictionary<string, int> Errors { get; set; }

        public LogAnalysisResponse()
        {
            UserActivity = new List<UserActivity>();
            Errors = new Dictionary<string, int>();
        }
    }
}
