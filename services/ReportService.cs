using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using models;
namespace services;

public class ReportService
{
    public ReportService(IConfiguration configuration)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configuration.GetValue<string>(AZURE_STORAGE));
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        _reportsTable = tableClient.GetTableReference(TABLE_NAME);
        _reportsTable.CreateIfNotExistsAsync().Wait();
    }

    public async Task<List<Report>> GetReportsFromLastDaysAsync(int numberOfDays)
    {
        var reports = new List<Report>();
        DateTime currentDate = DateTime.Now;
        DateTime startDate = currentDate.AddDays(-numberOfDays);

        TableQuery<Report> query = new TableQuery<Report>().Where(
            TableQuery.CombineFilters(
                TableQuery.GenerateFilterConditionForDate(REPORT_DATE, QueryComparisons.GreaterThanOrEqual, startDate),
                TableOperators.And,
                TableQuery.GenerateFilterConditionForDate(REPORT_DATE, QueryComparisons.LessThanOrEqual, currentDate)));
        TableQuerySegment<Report>? segment = null;
        do
        {
            segment = await _reportsTable.ExecuteQuerySegmentedAsync(query, segment?.ContinuationToken);
            reports.AddRange(segment.Results);
        } while (segment.ContinuationToken != null);
        return reports;
    }

    private readonly CloudTable _reportsTable;
    private const string TABLE_NAME = "Reports";
    private const string AZURE_STORAGE = "AzureWebJobsStorage";
    private const string REPORT_DATE = "ReportDate";
}