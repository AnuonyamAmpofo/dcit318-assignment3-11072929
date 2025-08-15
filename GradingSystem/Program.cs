using System;
using System.Collections.Generic;
using System.IO;

public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70 && Score <= 79) return "B";
        if (Score >= 60 && Score <= 69) return "C";
        if (Score >= 50 && Score <= 59) return "D";
        return "F";
    }
}

public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();
        
        using (var reader = new StreamReader(inputFilePath))
        {
            int lineNumber = 0;
            while (!reader.EndOfStream)
            {
                lineNumber++;
                string line = reader.ReadLine();
                string[] fields = line.Split(',');

                if (fields.Length != 3)
                {
                    throw new MissingFieldException($"Line {lineNumber}: Expected 3 fields but found {fields.Length}");
                }

                if (!int.TryParse(fields[0].Trim(), out int id))
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid ID format '{fields[0]}'");
                }

                string fullName = fields[1].Trim();
                if (string.IsNullOrEmpty(fullName))
                {
                    throw new MissingFieldException($"Line {lineNumber}: Missing student name");
                }

                if (!int.TryParse(fields[2].Trim(), out int score))
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format '{fields[2]}'");
                }

                students.Add(new Student(id, fullName, score));
            }
        }
        
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            string inputFile = "students.txt";
            string outputFile = "report.txt";

            var processor = new StudentResultProcessor();
            var students = processor.ReadStudentsFromFile(inputFile);
            processor.WriteReportToFile(students, outputFile);

            Console.WriteLine($"Successfully processed {students.Count} students. Report saved to {outputFile}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: Input file not found - {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Error: Invalid score format - {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Error: Missing required field - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}