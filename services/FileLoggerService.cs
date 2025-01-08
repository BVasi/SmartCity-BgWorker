using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
namespace services;

public class FileLoggerService
{
    public FileLoggerService(IConfiguration configuration)
    {
        var blobServiceClient = new BlobServiceClient(configuration.GetValue<string>(AZURE_STORAGE));
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(DAILY_REPORTS);
        _blobContainerClient.CreateIfNotExistsAsync().Wait();
    }

    public async Task LogToFileAsync(string fileName, string message)
    {
        var appendBlobClient = _blobContainerClient.GetAppendBlobClient(fileName);
        if (!await appendBlobClient.ExistsAsync())
        {
            await appendBlobClient.CreateAsync();
        }
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(message));
        await appendBlobClient.AppendBlockAsync(stream);
    }

    private readonly BlobContainerClient _blobContainerClient;
    private const string AZURE_STORAGE = "AzureWebJobsStorage";
    private const string DAILY_REPORTS = "dailyreports";
}
