using Microsoft.AspNetCore.Mvc;
using OpenAi_Assistant.Models;
using OpenAi_Assistant.Services;


namespace OpenAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task Get(string question)
        {
            // Best approach would be to load the api key from env vars
            var aiService = new OpenAiAssistantService("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");

            var assistant = await aiService.GetAssistantById("asst_9J6JPoWQMJtkdjJ42TjgMVEB");

            var thread = new ThreadModel
            {
                thread_id = "thread_rcnSx008fFNSlMlZPtp7iZCm"
            };

            var message = await aiService.SendMsgToThread(question, "user", thread);

            // Start the run operation
            var status = await aiService.RunAssistant();

            // Finally get the response from the assistant
            var response = await aiService.GetResponseFromAssistant();

            Console.WriteLine(response);
        }
    }
}
