using System;
using System.IO;
using System.Numerics;
using System.Threading;

class FibonacciProgram
{
    static void Main(string[] args)
    {
        Console.WriteLine("Fibonacci Sequence Generator");
        Console.WriteLine("----------------------------");

        Console.Write("Enter base file name for saving output: ");
        string baseFileName = Console.ReadLine()?.Trim() ?? "Fibonacci_Output";
        string filePath = $"{baseFileName}_output.txt";

        Console.WriteLine("\nOptions:");
        Console.WriteLine("1. Just generate the Fibonacci sequence (no conversion)");
        Console.WriteLine("2. Generate Fibonacci sequence with time conversion (as daytime format)");
        Console.WriteLine("3. Generate Fibonacci sequence and check for sequence '003706'");
        Console.Write("Choose an option (1/2/3): ");
        int mode = int.TryParse(Console.ReadLine(), out var chosenMode) ? chosenMode : 1;

        Console.WriteLine("\nPress 'S' or 's' anytime to stop the program.");

        BigInteger a = 0, b = 1;
        int count = 0;
        bool keepRunning = true;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            if (chosenMode == 3)
            {
                // Call the specific sequence-checking option
                SequenceChecker.CheckForSubstring(writer, ref keepRunning);
                return;
            }

            // Main loop to generate Fibonacci numbers for options 1 and 2
            while (keepRunning)
            {
                try
                {
                    // Generate the next Fibonacci number
                    BigInteger c = a + b;

                    string output;

                    // Determine behavior based on the selected mode
                    if (chosenMode == 1)
                    {
                        // Just print the Fibonacci number
                        output = $"Fibonacci: {c}";
                    }
                    else
                    {
                        // Convert to time as daytime format
                        output = $"Fibonacci: {c}, Daytime: {ConvertToDaytime(c)}";
                    }

                    // Print and write output
                    Console.WriteLine(output);
                    writer.WriteLine(output);

                    count++;

                    // Save every 10 entries to the file
                    if (count % 10 == 0)
                    {
                        writer.Flush();
                        Console.WriteLine("Saved the last 10 Fibonacci results to the file.");
                    }

                    // Update Fibonacci sequence
                    a = b;
                    b = c;

                    // Detect if the user wants to stop
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true).Key;
                        if (key == ConsoleKey.S)
                        {
                            Console.WriteLine("Stopping the program...");
                            keepRunning = false;
                        }
                    }

                    // Optional delay for visibility
                    Thread.Sleep(200);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    break;
                }
            }
        }

        Console.WriteLine("Program has stopped. Thank you for using the Fibonacci generator.");
    }

    /// <summary>
    /// Converts a Fibonacci number to a daytime format (supports overflow to days).
    /// </summary>
    /// <param name="number">The Fibonacci number to convert.</param>
    /// <returns>The converted daytime value.</returns>
    private static string ConvertToDaytime(BigInteger number)
    {
        try
        {
            // Convert to total seconds
            double totalSeconds = (double)number;

            // Calculate days, hours, minutes, seconds
            int days = (int)(totalSeconds / 86400); // 1 day = 86400 seconds
            totalSeconds %= 86400;

            int hours = (int)(totalSeconds / 3600); // 1 hour = 3600 seconds
            totalSeconds %= 3600;

            int minutes = (int)(totalSeconds / 60); // 1 minute = 60 seconds
            int seconds = (int)(totalSeconds % 60);

            return days > 0
                ? $"Day {days}, Time {hours:D2}:{minutes:D2}:{seconds:D2}"
                : $"Time {hours:D2}:{minutes:D2}:{seconds:D2}";
        }
        catch
        {
            return "Overflow - Unable to convert";
        }
    }
}

class SequenceChecker
{
    /// <summary>
    /// Continuously generates Fibonacci numbers and checks if they contain a specific sequence.
    /// </summary>
    /// <param name="writer">The StreamWriter object to write results to a file.</param>
    /// <param name="keepRunning">Reference boolean to determine if the loop should stop.</param>
    public static void CheckForSubstring(StreamWriter writer, ref bool keepRunning)
    {
        BigInteger a = 0, b = 1;
        int iteration = 1; // Iteration counter to keep track of Fibonacci sequence index
        int occurrenceCount = 0; // Tracks the number of times the sequence has been found
        const string targetSequence = "003706";

        Console.WriteLine("Checking for Fibonacci numbers that contain the sequence '003706'...");
        Console.WriteLine("Writing the next 7 digits, the iteration, and occurrence number to the file.");

        while (keepRunning)
        {
            try
            {
                // Generate the next Fibonacci number
                BigInteger c = a + b;
                string fibString = c.ToString();

                // Check if the number contains the target sequence
                int index = fibString.IndexOf(targetSequence);
                if (index != -1 && index + targetSequence.Length + 7 <= fibString.Length)
                {
                    // Increment occurrence count as the sequence is found
                    occurrenceCount++;

                    // Extract the 7 digits following the target sequence
                    string followingDigits = fibString.Substring(index + targetSequence.Length, 7);

                    // Write and display the result
                    string output = $"Sequence found! Following numbers: {followingDigits}, Iteration: {iteration}, Found #{occurrenceCount}";
                    Console.WriteLine(output);
                    writer.WriteLine(output);
                }

                // Update Fibonacci sequence and iteration counter
                a = b;
                b = c;
                iteration++;

                // Detect if the user wants to stop
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true).Key;
                    if (key == ConsoleKey.S)
                    {
                        Console.WriteLine("Stopping the program...");
                        keepRunning = false;
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"File writing error: {ex.Message}");
                keepRunning = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                keepRunning = false;
            }
        }
    }
}