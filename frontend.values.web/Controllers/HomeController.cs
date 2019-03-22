using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using frontend.values.web.Models;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace frontend.values.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var apiUrl = _config.GetValue<string>("API_URL", "Not configured");
            var sev = string.IsNullOrEmpty(apiUrl)
                ? "alert-danger"
                : "alert-success";


            ViewData["apiurl"] = apiUrl ?? "Not Configured!";
            ViewData["sev"] = sev;

            ViewData["Env:LOG_ELASTICFORMAT"] = _config.GetValue<bool>("LOG_ELASTICFORMAT", false);
            ViewData["Env:TRY_SOMECONFIGKEY"] = _config.GetValue<string>("TRY_SOMECONFIGKEY", "Not found");
            ViewData["Env:TRY_OTHERCONFIGKEY"] = _config.GetValue<string>("TRY_OTHERCONFIGKEY", "Not found");
            ViewData["Env:TRY_THIRDKEY"] = _config.GetValue<string>("TRY_THIRDKEY", "Not found");

            var apiResponse = GetValuesFromApi(apiUrl);
            ViewData["response_sev"] = apiResponse.sev;
            ViewData["response_msg"] = apiResponse.message;

            return View();
            
        }

        [HttpPost]
        public IActionResult WriteTrace(TraceModel trace)
        {
            if (trace.Level == LogLevel.Critical)
                _logger.Log(trace.Level, new ApplicationException("Boom!"), "Text: {Message}", trace.Message);
            else
                _logger.Log(trace.Level, "Text: {Text}", trace.Message);

            ViewData["trace_written_message"] = "Wrote to logger";
            
            return View("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Helpers
        public (string message, string sev) GetValuesFromApi(string apiUrl)
        {
            if (string.IsNullOrEmpty(apiUrl))
                return ("Not executed", "alert-warning");

            string[] vals;
            try
            {
                vals = apiUrl.GetJsonAsync<string[]>().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return ("Exception!: " + ex.Message, "alert-danger");
            }

            var msg =
                "[" +
                string.Join(',', vals) +
                "]";

            return (msg, "alert-success");
        }
        #endregion
    }
}

