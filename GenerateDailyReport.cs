using Domain.models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using services;
namespace worker;

public class GenerateDailyReport
{
    public GenerateDailyReport(ReportService reportService, FileLoggerService fileLoggerService, ILoggerFactory loggerFactory)
    {
        _reportService = reportService;
        _logger = loggerFactory.CreateLogger<GenerateDailyReport>();
        _fileLoggerService = fileLoggerService;
    }

    [Function(GENERATE_DAILY_REPORT_FUNCTION_NAME)]
    public async Task Run([TimerTrigger(CRON_EXPRESSION)] TimerInfo timerInfo)
    {
        _logger.LogInformation($"Generation of daily report started at {DateTime.Now}");
        var yesterdaysReports = await _reportService.GetReportsFromLastDaysAsync(ONE_DAY);
        var problemTypeCount = yesterdaysReports
            .Where(report => Enum.TryParse<ProblemType>(report.Problem, out _))
            .GroupBy(report => Enum.Parse<ProblemType>(report.Problem!))
            .ToDictionary(group => group.Key, group => group.Count());
        foreach (var entry in problemTypeCount)
        {
            await _fileLoggerService.LogToFileAsync($"report_{DateTime.Now.AddDays(-ONE_DAY):yyyy-MM-dd}.txt", $"{entry.Key}: {entry.Value}\n");
        }
    }

    private readonly ILogger _logger;
    private readonly ReportService _reportService;
    private readonly FileLoggerService _fileLoggerService;
    private const string GENERATE_DAILY_REPORT_FUNCTION_NAME = "GenerateDailyReport";
    private const string CRON_EXPRESSION = "0 0 * * *";
    private const int ONE_DAY = 1;
}
