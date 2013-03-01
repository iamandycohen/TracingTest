using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Tracing;

namespace TracingTestApi.Logging
{
    public class SimpleTracer : ITraceWriter
    {

        private const string TRACE_FMT = "{0} {1}: Category={2}, Level={3}, Kind={4}, Operator={5}, Operation={6}, Message={7}, Exception={8}";

        public virtual void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if (level != TraceLevel.Off)
            {
                var record = new TraceRecord(request, category, level);
                traceAction(record);
                WriteTrace(record);
            }
        }

        public virtual string GetTraceString(TraceRecord traceRecord)
        {
            string trace = string.Format(TRACE_FMT,
                 traceRecord.Request != null ? traceRecord.Request.Method.ToString() : string.Empty,
                 traceRecord.Request != null && traceRecord.Request.RequestUri != null ? traceRecord.Request.RequestUri.ToString() : string.Empty,
                 traceRecord.Category,
                 traceRecord.Level,
                 traceRecord.Kind,
                 traceRecord.Operator,
                 traceRecord.Operation,
                 traceRecord.Message,
                 traceRecord.Exception != null ? traceRecord.Exception.GetBaseException().Message : string.Empty
             );

            return trace;
        }

        public virtual void WriteTrace(TraceRecord traceRecord)
        {
            string trace = GetTraceString(traceRecord);
            System.Diagnostics.Trace.WriteLine(trace);
        }
    }
}