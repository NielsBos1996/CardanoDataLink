namespace CardanoDataLink.Domain.Exceptions;

public class GleifUnavailalbeException : Exception
{
    public GleifUnavailalbeException() { }
    
    public GleifUnavailalbeException(string message) : base(message) { }
    
    public GleifUnavailalbeException(string message, Exception inner) : base(message, inner) { }
}
