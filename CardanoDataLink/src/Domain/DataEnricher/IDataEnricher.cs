using CardanoDataLink.Domain.Entities;

namespace CardanoDataLink.Domain.DataEnricher;

public interface IDataEnricher
{
    Task<IEnumerable<Transaction>> EnrichData(IEnumerable<Transaction> transactions);
}