using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace FcadHackProxy.Data;

public class MessageRepository(
    MongoDbService mongoDbService,
    IConfiguration configuration) : IMessageRepository
{
    private readonly IMongoDatabase? _mongoDatabase = mongoDbService.Database;
    private readonly string _collectionName = "SensitiveMessages";
    public async Task SaveAsync(JObject jsonMessage)
    {
        var collection = _mongoDatabase.GetCollection<BsonDocument>(_collectionName);
        var document = BsonDocument.Parse(jsonMessage.ToString());
        await collection.InsertOneAsync(document);
    }

    public async Task<JObject> GetAsync(string id)
    {
        var collection = _mongoDatabase.GetCollection<BsonDocument>(_collectionName);
        var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
        var document = await collection.Find(filter).FirstOrDefaultAsync();

        if (document is not null)
        {
            var json = document.ToJson();
            return JObject.Parse(json);
        }

        return null;
    }

    public async Task<IEnumerable<JObject>> GetAllAsync(int pageSize, int pageNumber)
    {
        var collection = _mongoDatabase.GetCollection<BsonDocument>(_collectionName);
        
        var documents = await collection.Find(Builders<BsonDocument>.Filter.Empty)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
        
        var jsonObjects = documents.Select(doc => JObject.Parse(doc.ToJson())).ToList();

        return jsonObjects;
    }

    public async Task DeleteAsync(string id)
    {
        var collection = _mongoDatabase.GetCollection<BsonDocument>(_collectionName);
        var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
        var result = await collection.DeleteOneAsync(filter);
    }
}