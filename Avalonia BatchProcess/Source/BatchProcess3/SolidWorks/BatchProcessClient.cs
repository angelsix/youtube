using BatchProcess3.Core.Jobs;
using BatchProcess3.Core.SolidWorks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BatchProcess3.SolidWorks;

public class BatchProcessClient
{
    public bool DummyData { get; set; } = true;

    private readonly string _dummyDataPath = Path.Combine(
        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!,
        "..", "..", "..", "..", "BatchProcess3", "SolidWorks", "SampleFiles");

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    private string _hostAddress = "";

    public void Connect(string hostAddress)
    {
        _hostAddress = hostAddress;
    }

    public async Task<List<SolidWorksFileDetails>> GetActiveFileReferencesAsync()
    {
        try
        {
            if (DummyData)
            {
                var files = Directory.GetFiles(_dummyDataPath)
                    .Select(Path.GetFullPath)
                    .Where(f => !Path.GetFileName(f).StartsWith("~$"))
                    .Where(f => f.EndsWith(".sldprt", StringComparison.InvariantCultureIgnoreCase)
                                || f.EndsWith(".sldasm", StringComparison.InvariantCultureIgnoreCase)
                                || f.EndsWith(".slddrw", StringComparison.InvariantCultureIgnoreCase))
                    .Select(f => new SolidWorksFileDetails(f)).ToList();

                // By default, make the first *.sldasm the active file
                files.FirstOrDefault(f => f.FileName.EndsWith(".sldasm", StringComparison.InvariantCultureIgnoreCase))
                    ?.IsActiveInSolidWorks = true;

                return files;
            }

            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(_hostAddress + BatchProcessHostUrls.SolidWorksActiveFileList);

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<List<SolidWorksFileDetails>>(responseString, _jsonOptions) ?? [];

            return result;
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<BatchJobResponse> SubmitJobAsync(BatchJobRequest request)
    {
        try
        {
            var httpClient = new HttpClient();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_hostAddress + BatchProcessHostUrls.JobsSubmit, content);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<BatchJobResponse>(responseString, _jsonOptions)
                   ?? new BatchJobResponse { JobId = request.JobId, Accepted = false, Message = "Failed to parse response" };
        }
        catch (Exception ex)
        {
            return new BatchJobResponse { JobId = request.JobId, Accepted = false, Message = ex.Message };
        }
    }

    public async Task<BatchJobProgressResponse?> GetJobProgressAsync(string jobId)
    {
        try
        {
            var httpClient = new HttpClient();
            var url = _hostAddress + string.Format(BatchProcessHostUrls.JobsProgress, jobId);

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BatchJobProgressResponse>(responseString, _jsonOptions);
        }
        catch
        {
            return null;
        }
    }
}
