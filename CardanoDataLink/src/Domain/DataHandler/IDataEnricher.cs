using CardanoDataLink.Domain.Entities;

namespace CardanoDataLink.Domain.DataHandler;

public interface IDataEnricher
{
    Task<IEnumerable<Transaction>> EnrichData(IEnumerable<Transaction> transactions);
}