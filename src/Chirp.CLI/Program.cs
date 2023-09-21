using System.CommandLine;
using System.Collections.Generic;
using CommandHandle;

public class Program
{
    /*****************************
    
        C.O.R.E P.R.O.G.R.A.M
    
    *****************************/
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

        // Create a new instance of our CommandHandler
        var commandHandler = new CommandHandler();

        // Define a handler to process the command line arguments.
        rootCommand.SetHandler((readOneValue, readAllValue, cheepValue) =>
        {
            commandHandler.Fondle(readOneValue, readAllValue, cheepValue);
        }, readone, readall, cheep);

        // Invoke the root command to process the command line arguments.
        var result = await rootCommand.InvokeAsync(args);
    }
}
