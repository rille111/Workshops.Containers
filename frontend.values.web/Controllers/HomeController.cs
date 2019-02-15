using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using frontend.values.web.Models;
using Flurl;
using Flurl.Http;

namespace frontend.values.web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //var apiUrl = "http://localhost:8080/"; //Environment.GetEnvironmentVariable("APIURL");
            var apiUrl = Environment.GetEnvironmentVariable("APIURL");
            var sev = string.IsNullOrEmpty(apiUrl)
                ? "alert-danger"
                : "alert-success";

            ViewData["apiurl"] = apiUrl ?? "Not Configured!";
            ViewData["sev"] = sev;

            var apiResponse = GetValuesFromApi(apiUrl);
            ViewData["response_sev"] = apiResponse.sev;
            ViewData["response_msg"] = apiResponse.message;

            return View();
        }

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
