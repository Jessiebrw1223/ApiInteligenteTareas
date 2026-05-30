namespace ApiInteligenteTareas.Services;

public class ExternalApiException : Exception
{
    public ExternalApiException(string message, Exception? innerException = null) : base(message, innerException) { }
}
