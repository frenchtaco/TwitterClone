using System.CommandLine;
using CommandHandle;
using CommandType;

public class Program
{
    /*****************************

        C.O.R.E P.R.O.G.R.A.M
    
    
    *****************************/
    public static void Main(string[] args)
    {
        Program.CoreProgram(args).Wait(); // Don't delete the .Wait();!!! 
    }

    public static async Task CoreProgram(string[] arguments)
    {
        // Create an option to read a single "cheep" from the database.
        // Note: Changed from 'string' to 'bool' as per Æmill's instruction.
        var readUserSpecific_Option = new Option<bool>(
            aliases: new[] {"--readspecific", "-rs"},
            description: "Gets and reads a specified cheep in the CHIRPIN' Database.©"
        );

        // Create an option to read all "cheeps" from the database.
        var readAll_Option = new Option<bool>(
            aliases: new[] {"--readall", "-ra"},
            description: "Gets and reads all of the cheeps stored in our Chirpin' database."
        );

        // Create an option to write a new "cheep" to the database.
        var cheep_Option = new Option<string>(
            aliases: new[] {"--cheep", "-c"},
            description: "Cheeps a cheep to the CHIRPIN' Database."
        );

        RootCommand rootCommand = new RootCommand() 
        {
            readUserSpecific_Option,
            readAll_Option,
            cheep_Option,
        };

        var commandHandler = new CommandHandler();

        rootCommand.SetHandler(async (readSpecificValue, readAllValue, cheepValue) =>
        {
            await commandHandler.Fondle(readSpecificValue, readAllValue, cheepValue);
        }, readUserSpecific_Option, readAll_Option, cheep_Option);

        var result = await rootCommand.InvokeAsync(arguments);
    }
}
