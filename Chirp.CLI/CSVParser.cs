using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public class CSVParser {
    string filename = "data\\chirp_cli_db.csv";

    public List<Cheep> read() {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);
        using (var reader = new StreamReader(filename))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<Cheep>().ToList();;
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
}