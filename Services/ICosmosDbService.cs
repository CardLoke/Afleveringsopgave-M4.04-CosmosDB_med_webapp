using Models;

namespace Services;

public interface ICosmosDbService
{
    Task<SupportMessage> CreateSupportMessageAsync(
        SupportMessage supportMessage,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupportMessage>> GetSupportMessagesAsync(
     CancellationToken cancellationToken = default);
}
