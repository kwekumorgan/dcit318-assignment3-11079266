using System;
using System.Collections.Generic;
using System.IO;

public class Learner
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Score { get; set; }

    public Learner(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    // Determines the grade based on score
    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
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

public class ResultProcessor
{
    // Reads learners from a file and validates data
    public List<Learner> ReadFromFile(string inputFilePath)
    {
        List<Learner> learners = new List<Learner>();

        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');

                if (parts.Length != 3)
                    throw new MissingFieldException($"Line is missing fields: \"{line}\"");

                if (!int.TryParse(parts[0].Trim(), out int id))
                    throw new InvalidScoreFormatException($"Invalid ID format: \"{parts[0]}\"");

                string fullName = parts[1].Trim();

                if (!int.TryParse(parts[2].Trim(), out int score))
                    throw new InvalidScoreFormatException($"Invalid score format: \"{parts[2]}\"");

                learners.Add(new Learner(id, fullName, score));
            }
        }

        return learners;
    }

    // Writes learner results to a file
    public void WriteToFile(List<Learner> learners, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (var learner in learners)
            {
                string report = $"{learner.FullName} (ID: {learner.Id}): Score = {learner.Score}, Grade = {learner.GetGrade()}";
                writer.WriteLine(report);
            }
        }
    }
}

public class Program
{
    public static void Main()
    {
        // Update paths for your own machine
        string inputFilePath = @"C:\Work\DCIT\learners.txt";
        string outputFilePath = @"C:\Work\DCIT\grade_report.txt";

        ResultProcessor processor = new ResultProcessor();

        try
        {
            var learners = processor.ReadFromFile(inputFilePath);
            processor.WriteToFile(learners, outputFilePath);
            Console.WriteLine("Report generated successfully!");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Input file not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine("Invalid score error: " + ex.Message);
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine("Missing field error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error: " + ex.Message);
        }
    }
}
