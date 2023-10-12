using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using vault_demo.Models;

namespace vault_demo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    public HomeController(ILogger<HomeController> logger, IConfiguration iConfig)
    {
        _logger = logger;
        _configuration = iConfig;
    }

    public IActionResult Index()
    {
        
        //var dbConn = _configuration.GetSection("MySettings").GetSection("DbConnection").Value;
        _logger.LogError("Deneme Index Ercan ESE", DateTime.UtcNow);

        var dbConn2 = _configuration.GetValue<string>("ELKEnable");
        ViewBag.Conn = dbConn2.ToString();
        return View();
    }

    public IActionResult Privacy()
    {
        _logger.LogError("Mustafa kasikci response time : 10s", DateTime.UtcNow);
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
