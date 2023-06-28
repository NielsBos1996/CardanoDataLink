using CardanoDataLink.Domain.Entities;
using CardanoDataLink.Domain.Gleif;

namespace CardanoDataLink.Domain.DataEnricher;

public class DataEnricher : IDataEnricher
{
    private readonly IGleifClientInterface _gleifClient;

    public DataEnricher(IGleifClientInterface gleifClient)
    {
        _gleifClient = gleifClient;
    }
    
    public Task<IEnumerable<Transaction>> EnrichData(IEnumerable<Transaction> transactions)
    {
        return Task.FromResult(transactions.Select(t =>
        {
            t.TransactionUti = "This changes information";
            return t;
        }));
    }
}