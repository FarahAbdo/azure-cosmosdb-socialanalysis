using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

class Program
{
    private static readonly string EndpointUri = "https://localhost:8081/";  // Cosmos DB Emulator endpoint
    private static readonly string PrimaryKey = "primary_key";   // Emulator primary key
    private static readonly string DatabaseId = "ToDoList";
    private static readonly string ContainerId = "Items";

    static async Task Main(string[] args)
    {
        // Create Cosmos Client
        CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

        // Create a database and container
        Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
        Container container = await database.CreateContainerIfNotExistsAsync(ContainerId, "/id");

        Console.WriteLine("Connected to Cosmos DB Emulator!");

        // Perform CRUD operations
        await CreateItemAsync(container);
        await ReadItemAsync(container);
        await UpdateItemAsync(container);
        await DeleteItemAsync(container);
    }

    // Create a new item
    private static async Task CreateItemAsync(Container container)
    {
        var toDoItem = new
        {
            id = "1",
            task = "Learn Azure Cosmos DB",
            isCompleted = false
        };

        await container.CreateItemAsync(toDoItem, new PartitionKey(toDoItem.id));
        Console.WriteLine("Item Created!");
    }

    // Read an item
    private static async Task ReadItemAsync(Container container)
    {
        try
        {
            var response = await container.ReadItemAsync<dynamic>("1", new PartitionKey("1"));
            Console.WriteLine($"Item Read: {response.Resource.task}");
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine("Item not found!");
        }
    }

    // Update an item
    private static async Task UpdateItemAsync(Container container)
    {
        var toDoItem = new
        {
            id = "1",
            task = "Learn Azure Cosmos DB and CRUD operations",
            isCompleted = true
        };

        await container.ReplaceItemAsync(toDoItem, toDoItem.id, new PartitionKey(toDoItem.id));
        Console.WriteLine("Item Updated!");
    }

    // Delete an item
    private static async Task DeleteItemAsync(Container container)
    {
        await container.DeleteItemAsync<dynamic>("1", new PartitionKey("1"));
        Console.WriteLine("Item Deleted!");
    }
}
