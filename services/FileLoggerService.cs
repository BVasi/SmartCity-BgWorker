using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
namespace services;

public class FileLoggerService
{
    public FileLoggerService(IConfiguration configuration)
    {
        var blobServiceClient = new BlobServiceClient(configuration.GetConnectionString(AZURE_STORAGE));
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(DAILY_REPORTS);
        _blobContainerClient.CreateIfNotExistsAsync().Wait();
    }

    public async Task LogToFileAsync(string fileName, string message)
    {
        var blobClient = _blobContainerClient.GetBlobClient(fileName);
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(message));
        await blobClient.UploadAsync(stream);
    }

    private readonly BlobContainerClient _blobContainerClient;
    private const string AZURE_STORAGE = "AzureStorage";
    private const string DAILY_REPORTS = "dailyreports";
}
