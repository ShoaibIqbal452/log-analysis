using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LogAnalysis.Core.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LogAnalysis.Tests
{
    public class LogAnalysisApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public LogAnalysisApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Health_ReturnsOk()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", content);
        }

        [Fact]
        public async Task LogAnalysis_WhenLogFileExists_ReturnsOkWithData()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/loganalysis");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("uniqueUsers", content);
            Assert.Contains("userActivity", content);
            Assert.Contains("errors", content);
        }

        [Fact]
        public async Task LogAnalysis_WhenLogFileNotFound_ReturnsInternalServerError()
        {
            var client = _factory.CreateDefaultClient();

            var response = await client.GetAsync("/api/loganalysis");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Error analyzing log file", content);
        }

        [Fact]
        public async Task Swagger_ReturnsOk()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/swagger/v1/swagger.json");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Log Analysis API", content);
            Assert.Contains("/api/loganalysis", content);
        }
    }
}
