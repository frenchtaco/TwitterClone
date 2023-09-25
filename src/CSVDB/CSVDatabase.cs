using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;

namespace SimpleDB
{
    //sealed ensures that this class can't be inherited in any other class.
    public sealed class CSVDatabase<T> : IDatabaseRepository<T>
    {


        public static CSVDatabase<T> Instance { get { return lazy.Value; } }

        //I think is instance is only made when called which is the meaning of "lazy"
        private static readonly Lazy<CSVDatabase<T>> lazy = new Lazy<CSVDatabase<T>>(() => 
        new CSVDatabase<T>());


        //relative path
        private static string file = "/Users/victorlacour1/Desktop/Chirp/data/chirp_cli_db.csv";
        //private static string projectFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName!;
        //private static string file = Path.Combine(projectFolder, filename);
        
        //** THIS  CONSTRUCTOR IS SUPPOSED TO BE PRIVATE
        private CSVDatabase()
        {
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
            Console.WriteLine("Your cheep has been stored.");
            using (var writer = new StreamWriter(file, true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecord(record);
                writer.Write("\n");
            }
        }
    };
}