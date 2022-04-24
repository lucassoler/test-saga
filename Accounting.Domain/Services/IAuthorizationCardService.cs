namespace Accounting.Domain.Services;

public interface IAuthorizationCardService
{
    Task<CardResult> Authorize(string commandCardNumber);
}

public record CardResult(bool IsAuthorized)
{
    public static CardResult Invalid()
    {
        return new CardResult(false);
    }
    
    public static CardResult Valid()
    {
        return new CardResult(true);
    }
}