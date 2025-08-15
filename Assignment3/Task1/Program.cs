using System;
using System.Collections.Generic;


public record Transaction(int Id, DateTime Date, decimal Amount, string Category);


public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Processor for bank transfer transactions
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[BankTransfer] Processing {transaction.Amount:C} for {transaction.Category}");
    }
}

// Processor for mobile money transactions
public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[MobileMoney] Processing {transaction.Amount:C} for {transaction.Category}");
    }
}

// Processor for cryptocurrency wallet transactions
public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[CryptoWallet] Processing {transaction.Amount:C} for {transaction.Category}");
    }
}

// General account class
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    
    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"[Account] {transaction.Amount:C} deducted. New Balance: {Balance:C}");
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
            Console.WriteLine("Transaction declined: Insufficient funds.");
        }
        else
        {
            base.ApplyTransaction(transaction);
        }
    }
}

// Main finance management application
public class FinanceApp
{
    private List<Transaction> transactions = new List<Transaction>();

    public void Run()
    {
        // Create a savings account with an initial balance
        var account = new SavingsAccount("SB20250815001", 2000);

        // Create example transactions
        Transaction t1 = new Transaction(101, DateTime.Now, 450, "Electronics Purchase");
        Transaction t2 = new Transaction(102, DateTime.Now, 275, "Restaurant Bill");
        Transaction t3 = new Transaction(103, DateTime.Now, 900, "Flight Booking");

        // Select processors for each transaction type
        ITransactionProcessor p1 = new MobileMoneyProcessor();
        ITransactionProcessor p2 = new BankTransferProcessor();
        ITransactionProcessor p3 = new CryptoWalletProcessor();

        // Process transactions through their respective processors
        p1.Process(t1);
        p2.Process(t2);
        p3.Process(t3);

        // Apply each transaction to the account balance
        account.ApplyTransaction(t1);
        account.ApplyTransaction(t2);
        account.ApplyTransaction(t3);

        
         transactions.Add(t1);
         transactions.Add(t2);
         transactions.Add(t3);
    }
}

class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
