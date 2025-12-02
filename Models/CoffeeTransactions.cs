public class CoffeeTransaction
{
    public Guid ID { get; set; }

    public DateTime DateTime { get; set; }

    public string? CashType { get; set; }

    public string? Card { get; set; }

    public decimal Money { get; set; }

    public string? CoffeeName { get; set; }
}