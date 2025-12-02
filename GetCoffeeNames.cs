using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MCT.Functions.Repositories;

namespace MCT.Functions
{
    public class GetCoffeeNames
    {
        private readonly ILogger<GetCoffeeNames> _logger;

        public GetCoffeeNames(ILogger<GetCoffeeNames> logger)
        {
            _logger = logger;
        }

        [Function("GetCoffeeNames")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "coffees")] HttpRequest req)
        {
            try
            {
                var repository = new TransactionRepository();
                var names = await repository.GetCoffeeNamesAsync();
                // Returns 200 OK with list of names
                return new OkObjectResult(names);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                // Returns 500 Internal Server Error
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}