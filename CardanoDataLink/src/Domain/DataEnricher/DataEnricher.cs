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
    
    public async Task<IEnumerable<Transaction>> EnrichData(IEnumerable<Transaction> transactions)
    {
        var Leifs = transactions.Select(t => t.Lei).Distinct();
        var data = await _gleifClient.GetByIdentifier(Leifs);
        
        return transactions.Select(t =>
        {
            t.TransactionUti = "This changes information";
            return t;
        });
    }
}