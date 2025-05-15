# OracleBackgroundSink

**OracleBackgroundSink** is a custom [Serilog](https://serilog.net/) sink that enables asynchronous background logging directly into an Oracle database using `Oracle.ManagedDataAccess.Core`.

## Features

- Asynchronous logging to Oracle
- Non-blocking background processing
- Optimized for high-throughput scenarios
- Compatible with .NET 8
- Lightweight and easy to integrate

## Installation

Install via NuGet:

```bash
dotnet add package OracleBackgroundSink

USAGE

using Serilog;
using OracleBackgroundSink;

Log.Logger = new LoggerConfiguration()
    .WriteTo.OracleBackgroundSink("your-oracle-connection-string")
    .CreateLogger();

Log.Information("This is a test log entry.");



Oracle Table Structure

REATE TABLE Logs (
    Id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Timestamp TIMESTAMP,
    Level VARCHAR2(128),
    Message CLOB,
    Exception CLOB,
    Properties CLOB
);





EXAMPLE 1
------------------------------------------------------------------------------------------------

Program.cs

using Oracle.ManagedDataAccess.Client;
using oracle_sink;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



var connectionString = "User Id=system;Password=XXXXXXX;Data Source=localhost:1521/XEPDB1";

// Create an OracleSink object manually
var oracleSink = new OracleBackgroundSink(connectionString, tableName: "LOGS");

// Configure Serilog to use the OracleSink
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Information()
    .WriteTo.Sink(oracleSink) 
    .CreateLogger();

Log.Information("Test log message in Oracle database.3");


// Hook for graceful shutdown
AppDomain.CurrentDomain.ProcessExit += (s, e) =>
{
    Log.Information("Application shutting down. Flushing logs.");
    Log.CloseAndFlush();
    oracleSink.Dispose();
};



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();




EXAMPLE 2

-------------------------------------------------------------------------------------------

using Oracle.ManagedDataAccess.Client;
using oracle_sink; //replace
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



var connectionString = "User Id=system;Password=S1erver;Data Source=localhost:1521/XEPDB1";



Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.OracleLogSink(connectionString, tableName: "LOGS") 
    .CreateLogger();

Log.Information("Test log message in Oracle database.");



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

