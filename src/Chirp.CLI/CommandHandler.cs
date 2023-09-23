using SimpleDB;

namespace CommandHandle;

public class CommandHandler 
{
    private int commandCounter = 0;

         // This long line is called a reflection, and it forces the CSVDatabase's constructor to create an instance even though the constructor is PRIVATE.
        // this is smart because by using this, we keep singleton property and can still use the method.
        CSVDatabase<Cheep> database_cheeps =  (CSVDatabase<Cheep>)Activator.CreateInstance(typeof(CSVDatabase<Cheep>), nonPublic: true);//because something


    public void Fondle(bool shouldReadOne, bool shouldReadAll, string msg)
    {
        // If "--readall" option is set, read all "cheeps" from the database.
        if (shouldReadAll)
        {
            IEnumerable<Cheep> cheeps = database_cheeps.Read();
            foreach (Cheep cheep in cheeps)
            {
                commandCounter++;
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

    public int getCommandCounter()
    {
        return commandCounter;
    }

    public CSVDatabase<Cheep> GetCSVDatabaseWithCheeps()
    {
        return database_cheeps;
    }
}