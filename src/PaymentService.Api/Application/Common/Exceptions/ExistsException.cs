namespace PaymentService.Api.Application.Common.Exceptions;

public class ExistsException : Exception
{
    public ExistsException(string name, string key)
        : base($"Entity {name} with key {key} exists!")
    {
    }
}