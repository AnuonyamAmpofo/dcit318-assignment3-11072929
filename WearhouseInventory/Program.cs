using System;
using System.Collections.Generic;

public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
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

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}

public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists in the inventory.");
        }
        _items.Add(item.Id, item);
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out T item))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found in the inventory.");
        }
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found in the inventory.");
        }
        _items.Remove(id);
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException($"Quantity {newQuantity} is invalid. Quantity cannot be negative.");
        }

        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private readonly InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(3, "Headphones", 50, "Sony", 6));
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 23, "Samsung", 12));
        
        _groceries.AddItem(new GroceryItem(103, "Eggs", 120, DateTime.Now.AddDays(14)));
        _groceries.AddItem(new GroceryItem(101, "Milk", 100, DateTime.Now.AddDays(7)));
        _groceries.AddItem(new GroceryItem(102, "Bread", 75, DateTime.Now.AddDays(3)));
        
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        var items = repo.GetAllItems();
        Console.WriteLine($"\nInventory of {typeof(T).Name}s:");
        
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
            
            if (item is ElectronicItem electronicItem)
            {
                Console.WriteLine($"    Brand: {electronicItem.Brand}, \nWarranty: {electronicItem.WarrantyMonths} months");
            }
            else if (item is GroceryItem groceryItem)
            {
                Console.WriteLine($"    Expiry Date: {groceryItem.ExpiryDate.ToShortDateString()}");
            }
        }
        Console.WriteLine();
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Successfully increased stock for item ID {id} by {quantity} units.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Successfully removed item with ID {id}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }

    public InventoryRepository<ElectronicItem> Electronics => _electronics;
    public InventoryRepository<GroceryItem> Groceries => _groceries;
}

class Program
{
    static void Main(string[] args)
    {
        var manager = new WareHouseManager();
        
        manager.SeedData();
        
        manager.PrintAllItems(manager.Groceries);
        manager.PrintAllItems(manager.Electronics);
        
        Console.WriteLine("Testing error scenarios:");
        
        try
        {
            manager.Electronics.AddItem(new ElectronicItem(1, "Duplicate Laptop", 5, "HP", 12));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Error that is expected: {ex.Message}");
        }
        
        try
        {
            manager.Groceries.RemoveItem(999);
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"The error that is expected: {ex.Message}");
        }
        
        try
        {
            manager.Electronics.UpdateQuantity(2, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"The Error that is expected : {ex.Message}");
        }
        
        Console.WriteLine("\nTesting successful operations:");
        
        manager.IncreaseStock(manager.Groceries, 101, 50);
        manager.PrintAllItems(manager.Groceries);
        
        manager.RemoveItemById(manager.Electronics, 3);
        manager.PrintAllItems(manager.Electronics);
    }
}