using Microsoft.Azure.Cosmos;
using Models;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Services;

public class CosmosDbService : ICosmosDbService
{
    private readonly Container _container;

    public CosmosDbService(
        CosmosClient cosmosClient,
        string databaseName,
        string containerName)
    {
        _container = cosmosClient.GetContainer(
            databaseName,
            containerName);
    }

    public async Task<SupportMessage> CreateSupportMessageAsync(
        SupportMessage supportMessage, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(supportMessage.Id))
        {
            supportMessage.Id = Guid.NewGuid().ToString(); //burde være random nok til at ungå colisions (jeg gider ikke til at lave skrive locks )
        }

        supportMessage.DateTime = DateTime.UtcNow;

        await _container.CreateItemAsync(
            item: supportMessage,
            partitionKey: new PartitionKey(supportMessage.Category));
        return supportMessage;
    }
    public async Task<IReadOnlyList<SupportMessage>> GetSupportMessagesAsync(
        CancellationToken cancellationToken = default)
    {
        var supportMessages = new List<SupportMessage>();

        var query = new QueryDefinition(
            "SELECT * FROM c ORDER BY c.dateTime DESC"
        );

        using FeedIterator<SupportMessage> iterator =
            _container.GetItemQueryIterator<SupportMessage>(query);

        while (iterator.HasMoreResults)
        {
            FeedResponse<SupportMessage> response =
                await iterator.ReadNextAsync(cancellationToken);

            supportMessages.AddRange(response);
        }
        return supportMessages;
    }
}