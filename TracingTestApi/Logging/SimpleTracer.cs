using System;
using System.Collections.Generic;
using System.Dynamic;
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
            var traceDynamic = GetTraceDynamic(traceRecord);

            string traceString = string.Format(TRACE_FMT,
                 traceDynamic.Method,
                 traceDynamic.Uri,
                 traceDynamic.Category,
                 traceDynamic.Level,
                 traceDynamic.Kind,
                 traceDynamic.Operator,
                 traceDynamic.Operation,
                 traceDynamic.Message,
                 traceDynamic.Exception
             );

            return traceString;
        }

        public virtual dynamic GetTraceDynamic(TraceRecord traceRecord)
        {
            dynamic traceDynamic = new ExpandoObject();

            traceDynamic.Timestamp = traceRecord.Timestamp;
            traceDynamic.Method = traceRecord.Request != null ? traceRecord.Request.Method.ToString() : string.Empty;
            traceDynamic.Uri = traceRecord.Request != null && traceRecord.Request.RequestUri != null ? traceRecord.Request.RequestUri.ToString() : null;
            traceDynamic.Category = traceRecord.Category;
            traceDynamic.Level = traceRecord.Level;
            traceDynamic.Kind = traceRecord.Kind;
            traceDynamic.Operator = traceRecord.Operator;
            traceDynamic.Operation = traceRecord.Operation;
            traceDynamic.Message = traceRecord.Message;
            traceDynamic.Exception = traceRecord.Exception != null ? traceRecord.Exception.GetBaseException().Message : null;

            return traceDynamic;
        }

        public virtual void WriteTrace(TraceRecord traceRecord)
        {
            string traceString = GetTraceString(traceRecord);

            System.Diagnostics.Trace.WriteLine(traceString);
        }
    }
}