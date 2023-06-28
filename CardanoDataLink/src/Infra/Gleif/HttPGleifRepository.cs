using CardanoDataLink.Domain.Gleif;

namespace CardanoDataLink.Infra.Gleif;

public class HttPGleifRepository : IGleifClientInterface
{
    private readonly ILogger<HttPGleifRepository> _logger;
    
    public HttPGleifRepository(ILogger<HttPGleifRepository> logger)
    {
        _logger = logger;
        // todo: inject HTTP client so this becomes testable
    }
    
    public string[] GetByIdentifier(string[] leis)
    {
        throw new NotImplementedException();
    }
}