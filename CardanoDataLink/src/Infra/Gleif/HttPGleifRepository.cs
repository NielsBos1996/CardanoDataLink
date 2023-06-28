using System.Text.Json;
using CardanoDataLink.Domain.Exceptions;
using CardanoDataLink.Domain.Gleif;

namespace CardanoDataLink.Infra.Gleif;

public class HttPGleifRepository : IGleifClientInterface
{
    private const string BaseUrl = "api.gleif.org";

    private readonly ILogger<HttPGleifRepository> _logger;
    private readonly HttpClient _httpClient;

    public HttPGleifRepository(ILogger<HttPGleifRepository> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }
    
    public async Task<LeiRecords> GetByIdentifier(IEnumerable<string> leis)
    {
        var url = $"https://{BaseUrl}/api/v1/lei-records?filter[lei]=" + string.Join(",", leis);
        var result = await _httpClient.GetAsync(url);

        if ((int)result.StatusCode != 200)
        {
            throw new GleifUnavailalbeException("Expected status code 200, got " + (int)result.StatusCode);
        }
        
        var jsonResponse = await result.Content.ReadAsStringAsync();

        LeiRecords? parsedResponse;
        try
        {
            parsedResponse = JsonSerializer.Deserialize<LeiRecords>(jsonResponse);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to parse data with message: {errorMessage}", e.Message);
            throw new GleifUnavailalbeException();
        }
        
        if (parsedResponse?.data == null)
        {
            throw new GleifUnavailalbeException("Data not available in response");
        }

        return parsedResponse;
    }
}