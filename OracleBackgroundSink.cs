using Serilog.Core;
using Serilog.Events;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Concurrent;
using Newtonsoft.Json;
namespace OracleSink
{
    public class OracleBackgroundSink : ILogEventSink, IDisposable
    {
        private readonly IFormatProvider _formatProvider;
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly ConcurrentQueue<LogEvent> _queue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Task _backgroundTask;

        public OracleBackgroundSink(string connectionString, string tableName, IFormatProvider formatProvider = null)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            _formatProvider = formatProvider;
            _backgroundTask = Task.Run(ProcessQueueAsync);
        }

        public void Emit(LogEvent logEvent)
        {
            _queue.Enqueue(logEvent);
        }

        private async Task ProcessQueueAsync()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var logEvent))
                {
                    try
                    {
                        await WriteToOracleAsync(logEvent);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"[OracleSink ERROR] {ex.Message}");
                    }
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        private async Task WriteToOracleAsync(LogEvent logEvent)
        {
            try
            {
                var logMessage = logEvent.RenderMessage(_formatProvider);

                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandTimeout = 30;

                command.BindByName = true;
                command.CommandText = $@"
                INSERT INTO {_tableName}
                (""MESSAGE"", ""LOG_LEVEL"", ""TIMESTAMP"", ""EXCEPTION"", ""PROPERTIES"", ""TEMPLATE"")
                VALUES (:LogMessage, :LogLevel, :LogTimestamp, :LogException, :LogProperties, :LogTemplates)";

                command.Parameters.Add(new OracleParameter("LogMessage", logMessage));
                command.Parameters.Add(new OracleParameter("LogLevel", logEvent.Level.ToString()));
                command.Parameters.Add(new OracleParameter("LogTimestamp", logEvent.Timestamp.UtcDateTime));
                command.Parameters.Add(new OracleParameter("LogException",
                    logEvent.Exception?.ToString() ?? (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("LogProperties",
                    logEvent.Properties != null
                        ? JsonConvert.SerializeObject(logEvent.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString()))
                        : (object)DBNull.Value));
                command.Parameters.Add(new OracleParameter("LogTemplates",
                    logEvent.MessageTemplate?.Text ?? (object)DBNull.Value));

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{ex.Message}");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _backgroundTask.Wait();
            _cancellationTokenSource.Dispose();
        }
    }
}
