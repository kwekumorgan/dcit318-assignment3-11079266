using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public interface IInventoryEntity
{
    int Id { get; }
}

public record StockItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

public class StockLogger<T> where T : IInventoryEntity
{
    private List<T> log = new();
    private readonly string filePath;

    public StockLogger(string path)
    {
        filePath = path;
    }

    // Add a new record to the log
    public void Add(T item)
    {
        log.Add(item);
    }

    public List<T> GetAll()
    {
        return log;
    }

    // Save data to a JSON file
    public void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(log, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

    // Load data from a JSON file
    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No data file found.");
                return;
            }

            string json = File.ReadAllText(filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);
            log = items ?? new List<T>();
            Console.WriteLine("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }
}

public class StockApp
{
    private StockLogger<StockItem> logger;

    public StockApp(string path)
    {
        logger = new StockLogger<StockItem>(path);
    }

    // Load initial example stock
    public void SeedSampleData()
    {
        logger.Add(new StockItem(101, "Desk Lamp", 8, DateTime.Now));
        logger.Add(new StockItem(102, "Office Chair", 12, DateTime.Now));
        logger.Add(new StockItem(103, "Filing Cabinet", 5, DateTime.Now));
        logger.Add(new StockItem(104, "Whiteboard", 4, DateTime.Now));
        logger.Add(new StockItem(105, "Projector", 2, DateTime.Now));
    }

    public void SaveData()
    {
        logger.SaveToFile();
    }

    public void LoadData()
    {
        logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = logger.GetAll();
        foreach (var item in items)
        {
            Console.WriteLine($"{item.Id}: {item.Name} - Qty: {item.Quantity} (Added: {item.DateAdded:yyyy-MM-dd})");
        }
    }
}
