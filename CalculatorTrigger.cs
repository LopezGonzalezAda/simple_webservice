using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MCT.Functions
{
    public class CalculatorTrigger
    {
        private readonly ILogger<CalculatorTrigger> _logger;

        public CalculatorTrigger(ILogger<CalculatorTrigger> logger)
        {
            _logger = logger;
        }

        [Function("CalculatorTrigger")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "calculator/{num1:int}/{num2:int}")] HttpRequest req,
            int num1,
            int num2)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            int result = num1 + num2;

            return new OkObjectResult($"The result is: {result}");
        }
    }
}