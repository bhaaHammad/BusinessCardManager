public interface IBusinessCardExportService
{
    Task<string> ExportToCsvAsync<T>(IEnumerable<T> records);
    Task<string> ExportToXmlAsync<T>(IEnumerable<T> records);
}
