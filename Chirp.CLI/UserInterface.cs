/*using SimpleDB;

public class UserInterface {
    static CSVDatabase<T> csv = new();
    public static void read() {
        List<Cheep> cheeps = csv.Read();
        foreach (Cheep cheep in cheeps) {
        Console.WriteLine(cheep);
        }
    }

    public static void cheep(string message) {
        string author = Environment.UserName;
        var timestamp = DateTime.Now;
        long unixTime = ((DateTimeOffset)timestamp).ToUnixTimeSeconds();
        var cheep = new Cheep(author, message, unixTime);
        csv.Store(cheep);
        Console.WriteLine("Cheep sent");
    }
}*/