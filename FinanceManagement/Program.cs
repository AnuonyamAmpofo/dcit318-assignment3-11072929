using System;

namespace FinanceManagement

{
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"\n\n[MobileMoney] Processing mobile money transaction of {transaction.Amount:C} for {transaction.Category}\n");
        }

    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Processing bank transfer transaction of {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Processing cyrpto wallet transaction of {transaction.Amount} for {transaction.Category}");
        }
    }

    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }


        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            Balance -= transaction.Amount;

            Console.WriteLine($"[Account] Transaction {transaction.Id} applied. \nNew Balance: {Balance:C}\n\n");
            

        }
    }

    public sealed class SavingsAccount:Account
    {
        
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base (accountNumber, initialBalance)
        {
        }
        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            if (transaction.Amount > Balance)
            {
                Console.WriteLine($"Insufficient funds to complete transaction: {transaction.Id} for {transaction.Amount:C}. Available Balance: {Balance:C}");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"[SavingsAccount] Transaction {transaction.Id} successful. New balance: {Balance:C}");

        }
    }


    public static class Program
    {
        public static void Main()
        {
            Account account = new SavingsAccount("SA-1400-39998", 500m);
            Console.WriteLine($"Account created: {account.AccountNumber}. \nNew Balance:{account.Balance}\n");
            Transaction t1 = new Transaction(1, DateTime.Now, 150.00m, "Groceries\n");
            Transaction t2 = new Transaction(2, DateTime.Now, 600.50m, "Transportation\n");

            Console.WriteLine($"Transaction: {t1.Id}, {t1.Date}, {t1.Amount:C}, {t1.Category}");

            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();


            p1.Process(t1);

            account.ApplyTransaction(t1);

            p2.Process(t2);
            account.ApplyTransaction(t2);


        }
    }
}