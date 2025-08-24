using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TaskManagment.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LogsController : BaseController
    {
        private readonly string _logsDir;

        public LogsController(IWebHostEnvironment env)
        {
            _logsDir = Path.Combine(env.ContentRootPath, "Logs");
            Directory.CreateDirectory(_logsDir);
        }

        [HttpGet]
        public IActionResult GetLogs([FromQuery] int last = 200, [FromQuery] string? level = null, [FromQuery] string? search = null)
        {
            ;
            if (!Directory.Exists(_logsDir))
                return Ok(new { items = Array.Empty<object>() });

            var file = Directory.EnumerateFiles(_logsDir, "app-*.clef")
                                .OrderByDescending(x => x)
                                .FirstOrDefault();
            if (file is null)
                return Ok(new { items = Array.Empty<object>() });

            using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);

            var lines = new List<string>();
            while (!sr.EndOfStream) lines.Add(sr.ReadLine()!);
            var slice = lines.TakeLast(Math.Max(1, last));

            var items = new List<object>();

            foreach (var l in slice)
            {
                try
                {
                    using var doc = JsonDocument.Parse(l);
                    var r = doc.RootElement;

                    var lvlRaw = r.TryGetProperty("@l", out var L) ? L.GetString() : "Information";
                    var lvl = CanonLevel(lvlRaw);
                    if (!string.IsNullOrWhiteSpace(level) && !string.Equals(level, lvl, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var message =
                        r.TryGetProperty("@mt", out var mt) ? mt.GetString() :
                        r.TryGetProperty("@m", out var m) ? m.GetString() : "";

                    var obj = new
                    {
                        time = r.GetProperty("@t").GetDateTime(),
                        level = lvl,
                        message,
                        exception = r.TryGetProperty("@x", out var ex) ? ex.GetString() : null,
                        path = r.TryGetProperty("RequestPath", out var p) ? p.GetString() : null,
                        source = r.TryGetProperty("SourceContext", out var sc) ? sc.GetString() : null,
                        status = r.TryGetProperty("StatusCode", out var st) ? st.GetInt32() : (int?)null,
                        elapsed = r.TryGetProperty("Elapsed", out var el) ? el.GetDouble() : (double?)null
                    };

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        var hay = $"{obj.message} {obj.exception} {obj.path} {obj.source}";
                        if (!hay.Contains(search, StringComparison.OrdinalIgnoreCase)) continue;
                    }

                    items.Add(obj);
                }
                catch
                {
                }
            }

            return Ok(new { items });
        }

        private static string CanonLevel(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            switch (s.Trim().ToUpperInvariant())
            {
                case "INF":
                case "INFO":
                case "INFORMATION": return "INF";
                case "WRN":
                case "WARN":
                case "WARNING": return "WRN";
                case "ERR":
                case "ERROR": return "ERR";
                case "DBG":
                case "DEBUG": return "DBG";
                case "VRB":
                case "VERBOSE": return "VRB";
                case "FTL":
                case "FATAL": return "FTL";
                default: return s.Trim().ToUpperInvariant();
            }
        }
    }
}
