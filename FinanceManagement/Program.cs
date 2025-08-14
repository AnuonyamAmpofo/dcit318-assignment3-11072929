using System;
using System.Collections.Generic;


public record Transaction(int Id, DateTime Date, decimal Amount, string Category);


public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}


public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}


public class Account
{
    public string AccountNumber { get; private set; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction applied. New Balance: {Balance:C}");
    }
}


public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. Updated Balance: {Balance:C}");
        }
    }
}


public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        
        var account = new SavingsAccount("ACC583", 1000m);

        
        var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 300m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 120m, "Entertainment");

        
        ITransactionProcessor mobileProcessor = new MobileMoneyProcessor();
        ITransactionProcessor bankProcessor = new BankTransferProcessor();
        ITransactionProcessor cryptoProcessor = new CryptoWalletProcessor();

        mobileProcessor.Process(t1);
        account.ApplyTransaction(t1);

        bankProcessor.Process(t2);
        account.ApplyTransaction(t2);

        cryptoProcessor.Process(t3);
        account.ApplyTransaction(t3);

    
        _transactions.AddRange(new[] { t1, t2, t3 });

    
        Console.WriteLine("\n--- Transaction Summary ---");
        foreach (var t in _transactions)
        {
            Console.WriteLine($"ID: {t.Id}, Date: {t.Date}, Amount: {t.Amount:C}, Category: {t.Category}");
        }
        Console.WriteLine($"Final Balance: {account.Balance:C}");
    }
}


public class Program
{
    public static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
