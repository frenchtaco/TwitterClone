public class UserInterface {
    static CSVParser csv = new CSVParser();
    public static void read() {
        List<Cheep> cheeps = csv.read();
        foreach (Cheep cheep in cheeps) {
        Console.WriteLine(cheep);
        }
    }

    public static void cheep(string message) {
        string author = Environment.UserName;
        var timestamp = DateTime.Now;
        long unixTime = ((DateTimeOffset)timestamp).ToUnixTimeSeconds();
        var cheep = new Cheep(author, message, unixTime);
        csv.write(cheep);
        Console.WriteLine("Cheep sent");
    }
}