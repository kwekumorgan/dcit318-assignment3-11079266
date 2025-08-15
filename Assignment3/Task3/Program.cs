using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

public class TechProduct : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public TechProduct(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"Electronic: {Name} (ID: {Id}, Brand: {Brand}, Warranty: {WarrantyMonths} months, Qty: {Quantity})";
    }
}

public class FoodProduct : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public FoodProduct(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"Grocery: {Name} (ID: {Id}, Expires: {ExpiryDate:yyyy-MM-dd}, Qty: {Quantity})";
    }
}

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

public class StockRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> items = new Dictionary<int, T>();

    // Add new item to inventory
    public void AddItem(T item)
    {
        if (items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (items.TryGetValue(id, out var item))
            return item;
        throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public void RemoveItem(int id)
    {
        if (!items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        items.Remove(id);
    }

    public List<T> GetAllItems()
    {
        return new List<T>(items.Values);
    }

    // Update item quantity
    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");

        if (!items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");

        items[id].Quantity = newQuantity;
    }
}

public class StoreManager
{
    private StockRepository<TechProduct> electronics = new StockRepository<TechProduct>();
    private StockRepository<FoodProduct> groceries = new StockRepository<FoodProduct>();

    // Load initial data
    public void SeedData()
    {
        electronics.AddItem(new TechProduct(10, "Tablet", 12, "Apple", 18));
        electronics.AddItem(new TechProduct(11, "Smartwatch", 8, "Garmin", 24));

        groceries.AddItem(new FoodProduct(201, "Yoghurt", 25, DateTime.Now.AddDays(10)));
        groceries.AddItem(new FoodProduct(202, "Eggs", 50, DateTime.Now.AddDays(14)));
    }

    public void PrintAllItems<T>(StockRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine(item);
        }
    }

    public void IncreaseStock<T>(StockRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock updated: {item.Name} now has {item.Quantity + quantity} units.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
        }
    }

    public void RemoveItemById<T>(StockRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item with ID {id} removed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
        }
    }

    public StockRepository<TechProduct> GetElectronicsRepo() => electronics;
    public StockRepository<FoodProduct> GetGroceriesRepo() => groceries;
}

public class Program
{
    public static void Main()
    {
        var manager = new StoreManager();
        manager.SeedData();

        Console.WriteLine(" Grocery Items ");
        manager.PrintAllItems(manager.GetGroceriesRepo());

        Console.WriteLine("\n Electronic Items ");
        manager.PrintAllItems(manager.GetElectronicsRepo());

        Console.WriteLine("\n TEST CASES ");

        // Duplicate item test
        try
        {
            manager.GetElectronicsRepo().AddItem(new TechProduct(10, "Monitor", 5, "LG", 18));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"[Duplicate Error] {ex.Message}");
        }

        // Non-existent item removal test
        manager.RemoveItemById(manager.GetGroceriesRepo(), 999);

        // Invalid quantity update test
        try
        {
            manager.GetGroceriesRepo().UpdateQuantity(201, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"[Invalid Quantity] {ex.Message}");
        }

        // Final grocery state
        Console.WriteLine("\n== Final Grocery Inventory ==");
        manager.PrintAllItems(manager.GetGroceriesRepo());
    }
}
