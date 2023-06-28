using CardanoDataLink.Infra.Gleif;

namespace CardanoDataLink.Domain.Gleif;

public interface IGleifClientInterface
{
    Task<LeiRecords> GetByIdentifier(IEnumerable<string> leis);
}