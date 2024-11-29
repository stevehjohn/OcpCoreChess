namespace Engine.LiChessClient.Infrastructure;

public class ClientException : Exception
{
    public ClientException(string message) : base (message)
    {
    }
}