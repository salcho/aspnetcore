using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Csp
{
    public class CspReportingMiddleware
    {
        private readonly CspReportLogger _loggingConfig;
        private readonly JsonSerializerOptions _serializerOptions;

        public CspReportingMiddleware(RequestDelegate next, CspReportLogger reportLogger)
        {
            _loggingConfig = reportLogger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
        }

        private bool IsReportRequest(HttpRequest request)
        {
            return request.Path.StartsWithSegments(_loggingConfig.ReportUri)
                && request.ContentType?.StartsWith(CspConstants.CspReportContentType) == true
                && request.ContentLength != 0;
        }

        private async void HandleIncomingReport(Stream body)
        {
            try
            {
                CspReport cspReport = await JsonSerializer.DeserializeAsync<CspReport>(body, _serializerOptions);
                if (cspReport.ReportData != null)
                {
                    _loggingConfig.Log(_loggingConfig.LogLevel, cspReport);
                }
            } catch (JsonException)
            {
                return;
            }
        }

        public Task Invoke(HttpContext context)
        {
            if (IsReportRequest(context.Request))
            {
                HandleIncomingReport(context.Request.Body);
            }

            context.Response.StatusCode = (int) HttpStatusCode.NoContent;
            return Task.FromResult(0);
        }
    }
}
