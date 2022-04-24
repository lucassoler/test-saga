using Accounting.Domain.Services;

namespace Accounting.Infrastructure.Services;

public class FakeAuthorizationCardService : IAuthorizationCardService
{
    public Task<CardResult> Authorize(string cardNumber)
    {
        return Task.FromResult(cardNumber == "INVALID_CARD_NUMBER" ? 
            CardResult.Invalid() : 
            CardResult.Valid());
    }
}