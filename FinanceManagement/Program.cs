using System;

namespace FinanceManagement

{
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    public static class Program
{
    public static void Main()
    {
        Transaction t1 = new Transaction(1, DateTime.Now, 150.75m, "Groceries");
        Console.WriteLine($"Transaction: {t1.Id}, {t1.Date}, {t1.Amount}, {t1.Category}");
        }
    }
}