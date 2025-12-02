using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MCT.Functions.Models;
using System.IO;
using System.Text.Json; // Needed for JSON operations

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
        public async Task<IActionResult> Run( // Note: "async Task" because reading body is async
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "calculator")] HttpRequest req) // Changed to POST, removed params from Route
        {
            _logger.LogInformation("C# HTTP trigger function processed a POST request.");

            // 1. Read the JSON body into our object
            // This matches the line in your screenshot: var request = await req.ReadFromJsonAsync...
            var request = await req.ReadFromJsonAsync<CalculationRequest>();
            
            if (request == null)
            {
                return new BadRequestObjectResult("Please pass a valid JSON body");
            }

            int result = 0;
            string op = request.Operator; // Use properties from the object (A, B, Operator)

            switch (op.ToLower())
            {
                case "+":
                case "add":
                    result = request.A + request.B;
                    break;
                case "-":
                case "sub":
                    result = request.A - request.B;
                    break;
                case "*":
                case "mul":
                    result = request.A * request.B;
                    break;
                case "div":
                    if (request.B == 0) return new BadRequestObjectResult("Cannot divide by zero.");
                    result = request.A / request.B;
                    break;
                default:
                    return new BadRequestObjectResult("Invalid operator.");
            }

            // Return the result
            return new OkObjectResult(new CalculationResult 
            { 
                Result = result, 
                Operation = op 
            });
        }
    }
}