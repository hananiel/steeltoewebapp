using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IOptionsMonitor<LoggerFilterOptions> _filterOptions;
        private readonly HttpClient _client;

        public WeatherForecastController(IConfiguration configuration, ILogger<WeatherForecastController> logger, IOptionsMonitor<LoggerFilterOptions> filterOptions, HttpClient client)
        {
            _configuration = configuration;
            _logger = logger;
            _filterOptions = filterOptions;
            _client = client;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Getting the weather forecast");

            var root = (IConfigurationRoot)_configuration;
            var debugView = root.GetDebugView();
            Console.WriteLine("Config: "+ debugView);

            foreach (var rule in _filterOptions.CurrentValue.Rules)
            {
                Console.WriteLine("Cat: " + rule.CategoryName + ", Provider: " + rule.ProviderName + ", LogLevel: " + rule.LogLevel);
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetLoad")]
        public string GetLoad()
        {

            // Get Traffic
            _logger.LogInformation("Making upstream requests");
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(_client.GetAsync("https://www.google.com"));
            }
            Task.WaitAll(tasks.ToArray());

            // Get CPU
            double a = 10.1;
            for (int i = 0; i < 10000; i++)
            {
                Task.Run(() =>
                {
                    for (int j = 0; j < 100000; j++)
                    {
                        a = j / a;
                    }
                });
            }
            // Gc
            GC.Collect();

            return "done";
        }

        [HttpGet("Throw")]
        public string ThrowEx()
        {
            throw new Exception("Testing Exceptions");
        }
    }
}