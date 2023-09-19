using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;

namespace SimpleDB
{
    public sealed class CSVDatabase<T> : IDatabaseRepository<T>
    {
        private string file;
        private string filename = "data\\chirp_cli_db.csv";

        public CSVDatabase()
        {
            var projectFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            file = Path.Combine(projectFolder, filename);
            //file = "C:\\Users\\Bruger\\OneDrive\\Documents\\.ITU\\3.Semester\\Software_Architecture\\Chirp\\data\\chirp_cli_db.csv";
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
            using (var writer = new StreamWriter(file, true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecord(record);
                writer.Write("\n");
            }
        }
    }
}
