using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
namespace OracleSink
{
    public static class OracleSinkExtensions
    {
        public static LoggerConfiguration OracleLogSink(
       this LoggerSinkConfiguration loggerConfiguration,
       string connectionString,
       string tableName = "LOGS")
        {
            var sink = new OracleBackgroundSink(connectionString, tableName);
            return loggerConfiguration.Sink(sink);
        }
    }
}
