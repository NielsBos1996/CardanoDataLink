using System.Globalization;
using System.Text;
using CardanoDataLink.Domain.Entities;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using CardanoDataLink.Domain.DataEnricher;
using CsvHelper.Configuration;

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> Post()
    {
        IEnumerable<Transaction> transactions;
        try
        {
            transactions = await GetTransactionsFromBody();
        }
        catch (Exception)
        {
            _logger.LogError("Unexpected error while reading the input data");
            return Problem("Unable to interpret the CSV data. Are you sure the data has all the required fields?", statusCode: StatusCodes.Status500InternalServerError);
        }

        var processedTransactions = await _dataEnricher.EnrichData(transactions);

        _logger.LogDebug("Successfully processes request");
        var result = await TransactionsToString(processedTransactions);
        return Content(result, "text/csv");
    }

    private async Task<string> TransactionsToString(IEnumerable<Transaction> processedTransactions)
    {
        var result = new StringBuilder();
        await using (var writer = new StringWriter(result))
        await using (var csvExportWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csvExportWriter.WriteRecordsAsync(processedTransactions);
        }

        return result.ToString();
    }
    
    private async Task<IEnumerable<Transaction>> GetTransactionsFromBody()
    {
        var transactions = new List<Transaction>();
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            var csvReaderConf = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ","
            };
            using (var csv = new CsvReader(reader, csvReaderConf))
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
