namespace OcpCore.LichessClient.Infrastructure;

public class ClientException : Exception
{
    public ClientException(string message) : base (message)
    {
    }
}