using System;
using System.Collections.Generic;
using System.Linq;

public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => new List<T>(items);
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate) => items.Remove(items.FirstOrDefault(predicate));
}

public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString() => $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString() => $"ID: {Id}, Patient ID: {PatientId}, Medication: {MedicationName}, Date Issued: {DateIssued:yyyy-MM-dd}";
}

public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Kofi Aminu", 41, "Male"));
        _patientRepo.Add(new Patient(2, "Yaa Boahema", 32, "Female"));
        _patientRepo.Add(new Patient(3, "Isaac Mensah", 62, "Male"));

        _prescriptionRepo.Add(new Prescription(101, 1, "Lisinopril", new DateTime(2023, 1, 10)));
        _prescriptionRepo.Add(new Prescription(102, 1, "Atorvastatin", new DateTime(2023, 2, 15)));
        _prescriptionRepo.Add(new Prescription(103, 2, "Ibuprofen", new DateTime(2023, 3, 5)));
        _prescriptionRepo.Add(new Prescription(104, 2, "Metformin", new DateTime(2023, 3, 20)));
        _prescriptionRepo.Add(new Prescription(105, 3, "Amoxicillin", new DateTime(2023, 4, 1)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo.GetAll()
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId) =>
        _prescriptionMap.TryGetValue(patientId, out var prescriptions) ? prescriptions : new List<Prescription>();

    public void PrintAllPatients()
    {
        Console.WriteLine("All Patients:");

        _patientRepo.GetAll().ForEach(Console.WriteLine);
        Console.WriteLine();
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        var patient = _patientRepo.GetById(p => p.Id == patientId);
        if (patient == null)
        {
            Console.WriteLine($"Patient with ID {patientId} not found.");
            return;
        }

        Console.WriteLine($"Prescriptions for Patient {patient.Name} (ID: {patientId}):\n");
        
        
        var prescriptions = GetPrescriptionsByPatientId(patientId);
        prescriptions.ForEach(Console.WriteLine);
        Console.WriteLine();
    }
}

class Program
{
    static void Main()
    {
        var healthSystem = new HealthSystemApp();
        healthSystem.SeedData();
        healthSystem.BuildPrescriptionMap();
        healthSystem.PrintAllPatients();
        healthSystem.PrintPrescriptionsForPatient(1);

        Console.WriteLine("Enter a Patient ID to view prescriptions (or 0 to exit):");
        while (int.TryParse(Console.ReadLine(), out int patientId) && patientId != 0)
        {
            healthSystem.PrintPrescriptionsForPatient(patientId);
            Console.WriteLine("Enter another Patient ID (or 0 to exit):");
        }
    }
}