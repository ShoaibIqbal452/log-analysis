# Log Analysis Service

This service processes WebRTC studio log files and exposes a Web API endpoint to retrieve structured user activity data.

## Features

- Log file processing and parsing
- User activity tracking (JOIN/LEAVE events)
- Unique user counting
- Error aggregation by severity (ERROR, CRITICAL, WARNING)
- RESTful API endpoint
- Comprehensive test coverage
- Docker support

## Prerequisites

Choose one:
- Docker and Docker Compose (recommended)
- .NET 7.0 SDK

## Running with Docker (Recommended)

1. Clone the repository
2. Navigate to the project directory
3. Create a `logs` directory and place your log file:
   ```bash
   mkdir -p logs
   # Copy your webrtc_studio.log file into the logs directory
   ```
4. Build and run with Docker:
   ```bash
   docker-compose up --build
   ```
5. Access the API:
   - Swagger UI: http://localhost:5001/swagger
   - API Endpoint: http://localhost:5001/api/loganalysis
   - Health Check: http://localhost:5001/health

## Running Locally (Alternative)

1. Clone the repository
2. Navigate to the project directory
3. Build the solution:
   ```bash
   dotnet build
   ```
4. Run the tests:
   ```bash
   dotnet test
   ```
5. Run the application:
   ```bash
   cd src/LogAnalysis.Api
   dotnet run
   ```

The API will be available at:
 - http://localhost:5001

## API Endpoint

### GET /api/loganalysis

Returns the analyzed log data including:
- Total number of unique users
- List of user activity events (JOIN/LEAVE)
- Error counts by severity level

Example Response:
```json
{
    "uniqueUsers": 3,
    "userActivity": [
        {
            "userId": "456",
            "event": "JOIN",
            "timestamp": "2023-10-27 10:01:00"
        }
    ],
    "errors": {
        "ERROR": 4,
        "CRITICAL": 2,
        "WARNING": 3
    }
}
```

## Project Structure

- `LogAnalysis.Api`: Web API project
- `LogAnalysis.Core`: Core library containing log processing logic
- `LogAnalysis.Tests`: Unit tests

## Design Decisions

1. Used regular expressions for efficient log parsing
2. Implemented async/await pattern for better scalability
3. Separated concerns between API and core logic
4. Added comprehensive error handling
5. Included unit tests for core functionality
