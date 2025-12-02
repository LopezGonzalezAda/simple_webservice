using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MCT.Functions.Repositories;

namespace MCT.Functions
{
    public class GetTotalTransactions
    {
        private readonly ILogger<GetTotalTransactions> _logger;

        public GetTotalTransactions(ILogger<GetTotalTransactions> logger)
        {
            _logger = logger;
        }

        [Function("GetTotalTransactions")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "transactions/count")] HttpRequest req)
        {
            try
            {
                var repository = new TransactionRepository();
                int count = await repository.GetTotalTransactionsAsync();
                
                // Return an anonymous object for clean JSON: { "count": 50 }
                // Returns 200 OK with the count object
                return new OkObjectResult(new { count = count });
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