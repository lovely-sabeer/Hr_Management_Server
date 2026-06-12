using CsvHelper;
using System.Globalization;

namespace RestAPI.Service
{
    public static class CsvExporter
    {
        public static byte[] ToCsv<T>(IEnumerable<T> records)
        {
            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
                writer.Flush();
            }
            return memoryStream.ToArray();
        }
    }
}
