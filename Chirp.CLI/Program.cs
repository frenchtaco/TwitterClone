CSVParser csv = new CSVParser();

if (args[0] == "read")
{
    List<Cheep> cheeps = csv.read();
    foreach (Cheep cheep in cheeps) {
        Console.WriteLine(cheep);
    }

}
else if (args[0] == "cheep")
{
    try
    {
        string author = Environment.UserName;
        var timestamp = DateTime.Now;
        long unixTime = ((DateTimeOffset)timestamp).ToUnixTimeSeconds();
        var cheep = new Cheep(author, args[1], unixTime);
        csv.write(cheep);
    }
    catch (IOException e)
    {
        Console.WriteLine("The file does not exist");
        Console.WriteLine(e.Message);
    }
}