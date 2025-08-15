using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public interface IInventoryEntity
{
    int Id { get; }
}

public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new List<T>();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            using var writer = new StreamWriter(_filePath);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_log, options);
            writer.Write(json);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Error: No permission to write to the file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error while saving: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("No existing data file found.");
                return;
            }

            using var reader = new StreamReader(_filePath);
            string json = reader.ReadToEnd();
            var items = JsonSerializer.Deserialize<List<T>>(json);
            
            _log.Clear();
            if (items != null)
            {
                _log.AddRange(items);
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON data: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error while loading: {ex.Message}");
        }
    }
}

public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp()
    {
        _logger = new InventoryLogger<InventoryItem>("inventory.json");
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Keyboard", 25, DateTime.Now.AddDays(-1)));
        _logger.Add(new InventoryItem(2, "Laptop", 10, DateTime.Now.AddDays(-5)));
        _logger.Add(new InventoryItem(3, "Monitor", 15, DateTime.Now.AddDays(-3)));
        _logger.Add(new InventoryItem(4, "Speakers", 30, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Headphones", 12, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Added: {item.DateAdded:yyyy-MM-dd}");
        }
    }
}

class Program
{
    static void Main()
    {
        try
        {
            var app = new InventoryApp();
            app.SeedSampleData();
            app.SaveData();

            var newApp = new InventoryApp();
            newApp.LoadData();
            newApp.PrintAllItems();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}