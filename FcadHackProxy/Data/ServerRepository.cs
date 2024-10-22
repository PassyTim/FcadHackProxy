using FcadHackProxy.Dto;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FcadHackProxy.Data;

public class ServerRepository
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<BsonDocument> _serversCollection;

    public ServerRepository(MongoDbService mongoDbService)
    {
        _database = mongoDbService.Database;
        _serversCollection = _database.GetCollection<BsonDocument>("Servers");
    }

    public async Task<List<Server>> GetAllServersAsync()
    {
        var servers = await _serversCollection.Find(new BsonDocument()).ToListAsync();
        return servers.Select(doc => new Server
        {
            Id = doc["_id"].AsObjectId.ToString(),
            Name = doc["name"].AsString,
            Url = doc["url"].AsString
        }).ToList();
    }

    public async Task AddServerAsync(Server server)
    {
        var document = new BsonDocument
        {
            { "name", server.Name },
            { "url", server.Url }
        };
        await _serversCollection.InsertOneAsync(document);
    }

    public async Task DeleteServerAsync(string id)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
        await _serversCollection.DeleteOneAsync(filter);
    }
}