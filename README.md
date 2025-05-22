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

--------------------------------------------------------
--  DDL for Table LOGS
--------------------------------------------------------

  CREATE TABLE "SYSTEM"."LOGS" 
   (	"ID" NUMBER GENERATED ALWAYS AS IDENTITY MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 1 CACHE 20 NOORDER  NOCYCLE  NOKEEP  NOSCALE , 
	"MESSAGE" VARCHAR2(4000 BYTE), 
	"TEMPLATE" VARCHAR2(4000 BYTE), 
	"LOG_LEVEL" VARCHAR2(128 BYTE), 
	"TIMESTAMP" TIMESTAMP (6), 
	"EXCEPTION" CLOB, 
	"PROPERTIES" CLOB
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" 
 LOB ("EXCEPTION") STORE AS BASICFILE (
  TABLESPACE "SYSTEM" ENABLE STORAGE IN ROW 4000 CHUNK 8192 RETENTION 
  NOCACHE LOGGING 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) 
 LOB ("PROPERTIES") STORE AS BASICFILE (
  TABLESPACE "SYSTEM" ENABLE STORAGE IN ROW 4000 CHUNK 8192 RETENTION 
  NOCACHE LOGGING 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) ;
--------------------------------------------------------
--  DDL for Index SYS_C008221
--------------------------------------------------------

  CREATE UNIQUE INDEX "SYSTEM"."SYS_C008221" ON "SYSTEM"."LOGS" ("ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM" ;
--------------------------------------------------------
--  Constraints for Table LOGS
--------------------------------------------------------

  ALTER TABLE "SYSTEM"."LOGS" MODIFY ("ID" NOT NULL ENABLE);
  ALTER TABLE "SYSTEM"."LOGS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSTEM"  ENABLE;




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



var connectionString = "User Id=system;Password=XXXXXXXXXX;Data Source=localhost:1521/XEPDB1";



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

