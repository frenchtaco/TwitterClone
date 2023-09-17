using System.CommandLine; 
using System.Collections.Generic;
using SimpleDB;
 
 

partial class Program
{

    public static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand(); 
        
        var readone = new Option<bool> //changed from 'string' to 'bool' under hte command of Æmill.
        (
            name : "--readone",
            description : "Gets and reads a specified cheep in the CHIRPIN' Database.©"
        );

        var readall = new Option<bool> //changed from 'string' to 'bool' under hte command of Æmill.
        (
            name : "--readall",
            description : "Gets and reads all of the cheeps stored in our Chirpin' database."
        );

        var cheep = new Option<string>
        (
            name : "--cheep",
            description : "Cheeps a cheep to the CHIRPIN' Database."
        );

        rootCommand.Add(readone);
        rootCommand.Add(readall);
        rootCommand.Add(cheep);

        rootCommand.SetHandler((readOneValue, readAllValue, cheepValue) =>
        {
            Fondle(readOneValue, readAllValue, cheepValue);
       
        }, readone, readall, cheep);

        var result = await rootCommand.InvokeAsync(args);
    }

    public static void Fondle(bool shouldReadOne, bool shouldReadAll, string msg)
    {
        //Console.WriteLine("yummy yummy I love brogramming! " + msg + shouldReadOne + shouldReadAll);
        CSVDatabase<Cheep> database_cheeps = new CSVDatabase<Cheep>();  
        
        if (shouldReadAll)
        {  
            IEnumerable<Cheep> cheeps = database_cheeps.Read();
            foreach (Cheep cheep in cheeps) {
                Console.WriteLine(cheep);
            }
        }
        else if (shouldReadOne)
        {
            IEnumerable<Cheep> cheeps = database_cheeps.Read();
            Console.WriteLine(cheeps.ElementAt(0));
        }
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




/*using System.IO; 
using System.Collections.Generic;
using System; 
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string filename = "./data/chirp_cli_db.csv";  //excel ref.
        Console.WriteLine("\nHello, would you like to read all messages or write a message?\nPress: \n  - 1 to read, \n  - 2 to write and \n  - 3 to exit program: ");

        while(true) 
        {
            string readOrWrite = Console.ReadLine();  

            if(readOrWrite == "1")
            {
                Console.WriteLine("Reading...");
                Read(filename);
                Console.WriteLine("Read.");
                Console.WriteLine("\nHello, would you like to read all messages or write a message?\nPress: \n  - 1 to read, \n  - 2 to write and \n  - 3 to exit program: ");
            } 
            else if(readOrWrite == "2")
            {
                string msg = Console.ReadLine();
                Write(filename, msg);
                Console.WriteLine("\nHello, would you like to read all messages or write a message?\nPress: \n  - 1 to read, \n  - 2 to write and \n  - 3 to exit program: ");
            } else if(readOrWrite == "3") 
            {
                Console.WriteLine("Exiting...");
                break;
            }
        }
    }

    static void Read(string filename)
    {
        try
        {
            using (var sr = new StreamReader(filename))
            {
                sr.ReadLine();
                while(!sr.EndOfStream)
                {
                    string? curr_line = sr.ReadLine();
                    string[] arguments = curr_line.Split(',');
                    arguments[0].Replace(",",""); 
                    arguments[2].Replace(",","");

                    long stamp = long.Parse(arguments[2]);

                    var conversion = UnixTimeToDateTime(stamp);

                    Console.WriteLine(arguments[0] + " @ " + conversion + " : " + arguments[1]); 
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static void Write(string filename, string msg)
    {
        var user = Environment.UserName;
        // you need to figure this out my niggah
        StringBuilder sb = new StringBuilder();
        sb.Append(user);
        sb.Append("," + msg);
        sb.Append("," + WriteUnixTime().ToString());
        string valueForCSV = sb.ToString();
        
        using (StreamWriter sw = File.AppendText(filename))
        {
            sw.WriteLine(valueForCSV);
        }
    }

    public static long WriteUnixTime()
    {
        DateTime currentDateTime = DateTime.Now; 

        TimeSpan ts = currentDateTime - new DateTime(1970, 1, 1, 0, 0, 0);
        return (long) ts.TotalSeconds;
    }


    public static string UnixTimeToDateTime(long unixtime)
    {
        System.DateTime dtDateTime = new System.DateTime(2023, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
        
        return dtDateTime.ToString();
    }
}*/


