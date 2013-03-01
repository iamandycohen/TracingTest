using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Tracing;
using TracingTestApi.Hubs;

namespace TracingTestApi.Logging
{
    public class NLogger : SimpleTracer
    {
        private static readonly Logger classLogger = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<Dictionary<TraceLevel, Action<string>>> loggingMap =
            new Lazy<Dictionary<TraceLevel, Action<string>>>(() => new Dictionary<TraceLevel, Action<string>>
                {
                    {TraceLevel.Info, classLogger.Info},
                    {TraceLevel.Debug, classLogger.Debug},
                    {TraceLevel.Error, classLogger.Error},
                    {TraceLevel.Fatal, classLogger.Fatal},
                    {TraceLevel.Warn, classLogger.Warn}
                });

        private Dictionary<TraceLevel, Action<string>> Logger
        {
            get { return loggingMap.Value; }
        }

        private static readonly SignalRTrace SignalRHub = new SignalRTrace();

        public override void WriteTrace(TraceRecord traceRecord)
        {
            string message = base.GetTraceString(traceRecord);

            Logger[traceRecord.Level](message);

            SignalRHub.Hub.Clients.All.logMessage(base.TraceDynamic);
        }
    }

    public class SignalRTrace : SignalRBase<TraceHub>
    {

    }
}