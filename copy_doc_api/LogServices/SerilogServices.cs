using Serilog;
using Serilog.Events;

namespace copy_doc_api.LogServices
{
    public class SerilogServices
    {
        public static Serilog.ILogger CreateLogger()
        {
            return new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information || e.Level == LogEventLevel.Error)
                .WriteTo.File("Logs/InformationAndError_Logs.txt", rollingInterval: RollingInterval.Day))
            .CreateLogger();
        }
    }
}
