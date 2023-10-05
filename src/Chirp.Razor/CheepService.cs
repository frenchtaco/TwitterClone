using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SimpleDB;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    IDatabaseRepository<CheepViewModel> database_cheeps =  (CSVDatabase<CheepViewModel>)Activator.CreateInstance(typeof(CSVDatabase<CheepViewModel>), nonPublic: true);//because something

    //Collects cheeps directly from the database. Meant to reach page 1 instead of listing all cheeps.
    public List<CheepViewModel> GetCheeps()
    {
        List<CheepViewModel> _cheeps = new();
        IEnumerable<CheepViewModel> cheeps = database_cheeps.Read(1);
            foreach (CheepViewModel cheep in cheeps)
            {
                _cheeps.Add(new CheepViewModel(cheep.Author, cheep.Message, UnixTimeStampToDateTimeString(Convert.ToDouble(cheep.Timestamp))));
            }
        return _cheeps;
    }

    //Currently same as above. Meant to reach a specific page instead of listing all cheeps.
    public List<CheepViewModel> GetCheepsPage(int page)
    {
        List<CheepViewModel> _cheeps = new();
        IEnumerable<CheepViewModel> cheeps = database_cheeps.Read(page);
            foreach (CheepViewModel cheep in cheeps)
            {
                _cheeps.Add(new CheepViewModel(cheep.Author, cheep.Message, UnixTimeStampToDateTimeString(Convert.ToDouble(cheep.Timestamp))));
            }
        return _cheeps;
    }

    //Meant to reach page 1 of a specific user's cheeps. 
    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return GetCheeps().Where(x => x.Author == author).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
