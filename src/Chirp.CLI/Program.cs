using System.CommandLine;
using System.Collections.Generic;
using SimpleDB;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create the root command for our command line application.
        var rootCommand = new RootCommand();

        // Create an option to read a single "cheep" from the database.
        // Note: Changed from 'string' to 'bool' as per Æmill's instruction.
        var readone = new Option<bool>(
            name: "--readone",
            description: "Gets and reads a specified cheep in the CHIRPIN' Database.©"
        );

        // Create an option to read all "cheeps" from the database.
        var readall = new Option<bool>(
            name: "--readall",
            description: "Gets and reads all of the cheeps stored in our Chirpin' database."
        );

        // Create an option to write a new "cheep" to the database.
        var cheep = new Option<string>(
            name: "--cheep",
            description: "Cheeps a cheep to the CHIRPIN' Database."
        );

        // Add the created options to the root command.
        rootCommand.Add(readone);
        rootCommand.Add(readall);
        rootCommand.Add(cheep);

        // Define a handler to process the command line arguments.
        rootCommand.SetHandler((readOneValue, readAllValue, cheepValue) =>
        {
            Fondle(readOneValue, readAllValue, cheepValue);
        }, readone, readall, cheep);

        // Invoke the root command to process the command line arguments.
        var result = await rootCommand.InvokeAsync(args);
    }

    // Function to handle the options provided in the command line.
    public static void Fondle(bool shouldReadOne, bool shouldReadAll, string msg)
    {
        // Create an instance of the database for "cheeps".
        CSVDatabase<Cheep> database_cheeps = new CSVDatabase<Cheep>();

        // If "--readall" option is set, read all "cheeps" from the database.
        if (shouldReadAll)
        {
            IEnumerable<Cheep> cheeps = database_cheeps.Read();
            foreach (Cheep cheep in cheeps)
            {
                Console.WriteLine(cheep);
            }
        }
        // If "--readone" option is set, read only the first "cheep" from the database.
        else if (shouldReadOne)
        {
            IEnumerable<Cheep> cheeps = database_cheeps.Read();
            Console.WriteLine(cheeps.ElementAt(0));
        }
        // If "--cheep" option is set with a message, store that message to the database.
        else if (msg != null)
        {
            try
            {
                string author = Environment.UserName;
                var timestamp = DateTime.Now;
                long unixTime = ((DateTimeOffset)timestamp).ToUnixTimeSeconds();
                var cheep = new Cheep(author, msg.ToString(), unixTime);
                database_cheeps.Store(cheep);
            }
            catch (IOException e)
            {
                Console.WriteLine("The file does not exist");
                Console.WriteLine(e.Message);
            }
        }
    }
}
