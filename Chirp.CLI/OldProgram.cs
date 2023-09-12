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