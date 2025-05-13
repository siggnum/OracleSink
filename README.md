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