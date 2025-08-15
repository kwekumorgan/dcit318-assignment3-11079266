using System;
using System.Collections.Generic;
using System.Linq;

public class DataRepository<T>
{
    private List<T> records = new List<T>();

    public void Add(T item) => records.Add(item);

    public List<T> GetAll() => new List<T>(records);

    public T? GetById(Func<T, bool> predicate) => records.FirstOrDefault(predicate);

    public bool Remove(Func<T, bool> predicate)
    {
        var item = records.FirstOrDefault(predicate);
        if (item != null)
        {
            records.Remove(item);
            return true;
        }
        return false;
    }
}

// Stores client information
public class Client
{
    public int Id { get; }
    public string FullName { get; }
    public int Age { get; }
    public string Gender { get; }

    public Client(int id, string fullName, int age, string gender)
    {
        Id = id;
        FullName = fullName;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"Client [Id={Id}, Name={FullName}, Age={Age}, Gender={Gender}]";
    }
}

// Stores prescription details
public class MedicationOrder
{
    public int Id { get; }
    public int ClientId { get; }
    public string DrugName { get; }
    public DateTime DateIssued { get; }

    public MedicationOrder(int id, int clientId, string drugName, DateTime dateIssued)
    {
        Id = id;
        ClientId = clientId;
        DrugName = drugName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"MedicationOrder [Id={Id}, ClientId={ClientId}, Drug={DrugName}, DateIssued={DateIssued:d}]";
    }
}

public class MedicalSystemApp
{
    private DataRepository<Client> clientRepository = new DataRepository<Client>();
    private DataRepository<MedicationOrder> orderRepository = new DataRepository<MedicationOrder>();
    private Dictionary<int, List<MedicationOrder>> orderMap = new Dictionary<int, List<MedicationOrder>>();

    // Loads initial clients and prescriptions into the system
    public void LoadSampleData()
    {
        clientRepository.Add(new Client(101, "David Owusu", 34, "Male"));
        clientRepository.Add(new Client(102, "Mary Abena", 29, "Female"));
        clientRepository.Add(new Client(103, "Kwame Mensah", 52, "Male"));

        orderRepository.Add(new MedicationOrder(201, 101, "Azithromycin", DateTime.Now.AddDays(-4)));
        orderRepository.Add(new MedicationOrder(202, 101, "Ciprofloxacin", DateTime.Now.AddDays(-1)));
        orderRepository.Add(new MedicationOrder(203, 102, "Vitamin C", DateTime.Now.AddDays(-6)));
        orderRepository.Add(new MedicationOrder(204, 103, "Ibuprofen", DateTime.Now.AddDays(-2)));
        orderRepository.Add(new MedicationOrder(205, 101, "Loratadine", DateTime.Now.AddDays(-3)));
    }

    // Groups prescriptions by client ID
    public void GenerateOrderMap()
    {
        orderMap.Clear();

        foreach (var order in orderRepository.GetAll())
        {
            if (!orderMap.ContainsKey(order.ClientId))
            {
                orderMap[order.ClientId] = new List<MedicationOrder>();
            }
            orderMap[order.ClientId].Add(order);
        }
    }

    public void ShowAllClients()
    {
        Console.WriteLine("=== Client List ===");
        foreach (var client in clientRepository.GetAll())
        {
            Console.WriteLine(client);
        }
        Console.WriteLine();
    }

    public void ShowOrdersForClient(int clientId)
    {
        if (orderMap.ContainsKey(clientId))
        {
            Console.WriteLine($"=== Medication Orders for Client ID {clientId} ===");
            foreach (var order in orderMap[clientId])
            {
                Console.WriteLine(order);
            }
        }
        else
        {
            Console.WriteLine($"No medication orders found for Client ID {clientId}.");
        }
    }
}

public class Program
{
    public static void Main()
    {
        var app = new MedicalSystemApp();

        app.LoadSampleData();
        app.GenerateOrderMap();
        app.ShowAllClients();

        Console.Write("Enter Client ID to view prescriptions: ");
        if (int.TryParse(Console.ReadLine(), out int clientId))
        {
            app.ShowOrdersForClient(clientId);
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a number.");
        }
    }
}
