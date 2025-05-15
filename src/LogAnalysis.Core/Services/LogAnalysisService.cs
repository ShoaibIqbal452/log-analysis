using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LogAnalysis.Core.Interfaces;
using LogAnalysis.Core.Models;

namespace LogAnalysis.Core.Services
{
    public class LogAnalysisService : ILogAnalysisService
    {
        private static readonly Regex LogPattern = new(
            @"\[(.*?)\]\s+(\w+)\s+(\S+)\s+(\S+)\s+(.*?)(?:\s+\[(.*?)\])?$",
            RegexOptions.Compiled
        );

        public async Task<LogAnalysisResponse> AnalyzeLogFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Log file not found", filePath);

            var response = new LogAnalysisResponse();
            var uniqueUsers = new HashSet<string>();

            string[] lines = await File.ReadAllLinesAsync(filePath);

            foreach (var line in lines)
            {
                var match = LogPattern.Match(line);
                if (!match.Success) continue;

                var entry = new LogEntry
                {
                    Timestamp = DateTime.Parse(match.Groups[1].Value),
                    EventType = match.Groups[2].Value,
                    UserId = match.Groups[3].Value,
                    StreamId = match.Groups[4].Value,
                    Message = match.Groups[5].Value.Trim(),
                    OptionalData = match.Groups[6].Success ? match.Groups[6].Value : null
                };

                ProcessLogEntry(entry, response, uniqueUsers);
            }

            response.UniqueUsers = uniqueUsers.Count;
            return response;
        }

        private void ProcessLogEntry(LogEntry entry, LogAnalysisResponse response, HashSet<string> uniqueUsers)
        {
            if (entry.IsUserActivity)
            {
                response.UserActivity.Add(new UserActivity
                {
                    UserId = entry.UserId,
                    Event = entry.EventType.Replace("USER_", ""),
                    Timestamp = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                });

                if (entry.EventType == "USER_JOIN")
                {
                    uniqueUsers.Add(entry.UserId);
                }
            }

            if (entry.IsError)
            {
                var errorType = entry.EventType;
                if (!response.Errors.ContainsKey(errorType))
                {
                    response.Errors[errorType] = 0;
                }
                response.Errors[errorType]++;
            }
        }
    }
}
