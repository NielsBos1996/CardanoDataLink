namespace CardanoDataLink.Domain.Gleif;

public interface IGleifClientInterface
{
    string[] GetByIdentifier(string[] leis);
}