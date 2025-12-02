using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MCT.Functions.Repositories; // To find the Repository
using MCT.Functions.Models;       // To find the CoffeeTransaction model

namespace MCT.Functions
{
    public class GetTransactions
    {
        private readonly ILogger<GetTransactions> _logger;

        public GetTransactions(ILogger<GetTransactions> logger)
        {
            _logger = logger;
        }

        [Function("GetTransactions")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "transactions")] HttpRequest req)
        {
            _logger.LogInformation("Getting all transactions...");

            try
            {
                var repository = new TransactionRepository();
                var transactions = await repository.GetAllTransactionsAsync();
        
                // Returns 200 OK with data
                return new OkObjectResult(transactions);
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