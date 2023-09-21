namespace Chirp.CLI.Tests;

using CommandHandle;

public class UnitTest1
{

    // Test passes when using absolute path as oppose to the relative path:
    [Fact]
    public void Test1()
    {
        var commandHandler = new CommandHandler();
        var csvDatabase = commandHandler.GetCSVDatabaseWithCheeps();
        IEnumerable<Cheep> cheeps = csvDatabase.Read();
        var firstValue = cheeps.ElementAt(0).Message;

        Assert.NotNull(csvDatabase);
        Assert.NotEmpty(cheeps);
        Assert.Equal("bonjour", firstValue);
    }
}