
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
namespace SimpleDB;


namespace SimpleDB
{
    public sealed class CSVDatabase<T> : IDatabaseRepository<T>
    {
        private string file;

        //relative path
        private string filename = "data//chirp_cli_db.csv";
        

        public CSVDatabase()
        {
          
            //takes absolute path from the user, and combines it with the path to the csv file
            var projectFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName!;
            file = Path.Combine(projectFolder, filename);
          
        }
        

        public IEnumerable<T> Read(int? limit = null)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<T>().ToList();
                return records;
            }
        }

    public void Store(T record)
    {
        Console.WriteLine("howdy.");
        using (var writer = new StreamWriter(filename, true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecord(record);
            writer.Write("\n");
        }
    }
};
}


/* 

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public class CSVParser {
    string filename = "data/chirp_cli_db.csv";

    public List<Cheep> read() {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);
        using (var reader = new StreamReader(filename))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<Cheep>().ToList();
            return records;
        }
    }

    public void write(Cheep record) {
        var records = read();
        using (var writer = new StreamWriter(filename))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            records.Add(record);
            csv.WriteRecords(records);
        }
    }
}*/