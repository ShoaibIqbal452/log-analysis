using System.Threading.Tasks;
using LogAnalysis.Core.Models;

namespace LogAnalysis.Core.Interfaces
{
    public interface ILogAnalysisService
    {
        Task<LogAnalysisResponse> AnalyzeLogFileAsync(string filePath);
    }
}
