using CardanoDataLink.Domain.Entities;
using CardanoDataLink.Domain.Gleif;

namespace CardanoDataLink.Domain.DataHandler;

public class DataEnricher : IDataEnricher
{
    private readonly IGleifClientInterface _gleifClient;

    public DataEnricher(IGleifClientInterface gleifClient)
    {
        _gleifClient = gleifClient;
    }
    
    public async Task<IEnumerable<Transaction>> EnrichData(IEnumerable<Transaction> transactions)
    {
        var gleifs = transactions.Select(t => t.Lei).Distinct();
        var data = await _gleifClient.GetByIdentifier(gleifs);
        
        return transactions.Select(t =>
        {
            var transactionData = data.data.First(d => d.attributes.lei == t.Lei);
            var leiEntity = transactionData?.attributes?.entity;

            var transactionCost = CalculateTransactionCost(leiEntity?.legalAddress.country, t.Notional, t.Rate);
            t.TransactionCost = transactionCost.Result;
            t.TransactionCostReason = transactionCost.Reason;
            t.LegalName = leiEntity?.legalName.name;
            t.Bic = transactionData?.attributes?.bic == null ? "" : string.Join(";", transactionData.attributes.bic);
            
            return t;
        });
    }

    private TransactionCostResult CalculateTransactionCost(string? country, double notional, double rate)
    {
        // the fancy way would be to create and implement something like a transactionCostInterface, which would be
        // returned by a transactionCostFactory.
        // but fow now I'll stick to the KISS priniple
        return country switch
        {
            "NL" => CalculateTransactionCostNL(notional, rate),
            "GB" => CalculateTransactionCostGB(notional, rate),
            _ => new TransactionCostResult { Reason = $"no calculation available for county {country}" }
        };
    }

    private TransactionCostResult CalculateTransactionCostNL(double notional, double rate)
    {
        return new TransactionCostResult
        {
            Result = Math.Abs(notional * (1 / rate) - notional),
            Reason = $"Abs({notional} * (1 / {rate}) - {notional})"
        };
    }
    
    private TransactionCostResult CalculateTransactionCostGB(double notional, double rate)
    {
        return new TransactionCostResult
        {
            Result = notional * rate - notional,
            Reason = $"{notional} * {rate} - {notional}"
        };
    }
}
