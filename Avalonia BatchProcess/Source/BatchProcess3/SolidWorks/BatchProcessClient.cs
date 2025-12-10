using BatchProcess3.Core.SolidWorks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BatchProcess3.SolidWorks;

public class BatchProcessClient
{
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
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(_hostAddress + BatchProcessHostUrls.SolidWorksActiveFileList);

            var responseString = await response.Content.ReadAsStringAsync();
        
            var result = JsonSerializer.Deserialize<List<SolidWorksFileDetails>>(responseString, _jsonOptions) ?? [];

            return result;
        }
#pragma warning disable CS0168 // Variable is declared but never used
        catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
        {
            // TODO: Handle somewhere
            return [];
        }

    }
}