using System.Xml.Serialization;

namespace BusinessCardManager.Application.Services
{
    public class BusinessCardExportService : IBusinessCardExportService
    {
        public async Task<string> ExportToCsvAsync<T>(IEnumerable<T> records)
        {
            if (records == null || !records.Any())
                return string.Empty;

            var type = typeof(T);
            var props = type.GetProperties();

            using var stringWriter = new StringWriter();
            stringWriter.WriteLine(string.Join(",", props.Select(p => p.Name)));

            foreach (var record in records)
            {
                var values = props.Select(p =>
                {
                    var val = p.GetValue(record)?.ToString() ?? string.Empty;
                    val = val.Replace("\"", "\"\"");
                    if (val.Contains(",") || val.Contains("\"") || val.Contains("\n"))
                    {
                        val = $"\"{val}\"";
                    }
                    return val;
                });

                stringWriter.WriteLine(string.Join(",", values));
            }

            return await Task.FromResult(stringWriter.ToString());
        }

        public async Task<string> ExportToXmlAsync<T>(IEnumerable<T> records)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            using var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, records.ToList());
            return await Task.FromResult(stringWriter.ToString());
        }
    }
}
