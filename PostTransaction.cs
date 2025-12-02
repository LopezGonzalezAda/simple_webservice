using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MCT.Functions.Repositories;
using MCT.Functions.Models;
using System.IO;
using System.Text.Json;

namespace MCT.Functions
{
    public class PostTransaction
    {
        private readonly ILogger<PostTransaction> _logger;

        public PostTransaction(ILogger<PostTransaction> logger)
        {
            _logger = logger;
        }

        [Function("PostTransaction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "transactions")] HttpRequest req)
        {
            try
            {
                // Read the JSON body
                var transaction = await req.ReadFromJsonAsync<CoffeeTransaction>();
                
                if (transaction == null)
                    return new BadRequestObjectResult("Invalid transaction data.");

                // Save to DB
                var repository = new TransactionRepository();
                await repository.CreateTransactionAsync(transaction);

                // Return 201 Created (standard for successful inserts)
                return new StatusCodeResult(StatusCodes.Status201Created);
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