using Microsoft.WindowsAzure.Storage.Table;
namespace models;

public class Report : TableEntity
{
    public Report() {}

    public string? ReporterEmail { get; set; }
    public string? Description { get; set; }
    public string? Street { get; set; }
    public string? Status { get; set; }
    public string? Problem { get; set; }
    public DateTime ReportDate { get; set; }
}