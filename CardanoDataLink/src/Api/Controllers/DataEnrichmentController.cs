using System.Globalization;
using System.Text;
using CardanoDataLink.Domain.Entities;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using CardanoDataLink.Domain.DataEnricher;

namespace CardanoDataLink.Api.Controllers;

[ApiController]
[Route("/api/data-enrichment")]
public class DataEnrichmentController : ControllerBase
{
    private readonly ILogger<DataEnrichmentController> _logger;
    private readonly IDataEnricher _dataEnricher;

    public DataEnrichmentController(ILogger<DataEnrichmentController> logger, IDataEnricher dataEnricher)
    {
        _logger = logger;
        _dataEnricher = dataEnricher;
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        IEnumerable<Transaction> transactions;
        try
        {
            transactions = await GetTransactions();
        }
        catch (Exception)
        {
            _logger.LogError("Unexpected error while reading the input data");
            return Problem("Unable to interpret the CSV data. Are you sure the data has all the required fields?", statusCode: 500);
        }

        var processedTransactions = await _dataEnricher.EnrichData(transactions);

        var result = new StringBuilder();
        await using (var writer = new StringWriter(result))
        await using (var csvExportWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csvExportWriter.WriteRecordsAsync(processedTransactions);
        }
        
        return Content(result.ToString(), "text/csv");
    }
    
    private async Task<IEnumerable<Transaction>> GetTransactions()
    {
        var transactions = new List<Transaction>();
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                await foreach (var record in csv.GetRecordsAsync<Transaction>())
                {
                    transactions.Add(record);
                }
            }
        }

        return transactions;
    }
}
