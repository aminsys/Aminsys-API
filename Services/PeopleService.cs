using aminsys_api.Model;
using aminsys_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace aminsys_api.Services;

public class PeopleService
{
    private readonly IMongoCollection<People> _peopleCollection;

    public PeopleService(IOptions<PeopleDatabaseSettings> peopleDatabaseSettings)
    {
        // PeopleDatabaseSettings is retrieved from DI via constructor injection
        var mongoClient = new MongoClient(peopleDatabaseSettings.Value.ConnectionString);
        var mongoDabase = mongoClient.GetDatabase(peopleDatabaseSettings.Value.DatabaseName);
        _peopleCollection = mongoDabase.GetCollection<People>(peopleDatabaseSettings.Value.CollectionName);
    }

    public async Task<List<People>> GetAsync() =>
        await _peopleCollection.Find(_ => true).ToListAsync();

    public async Task<People> GetAsync(int personId) =>
        await _peopleCollection.Find(p => p.Person.Id == personId).FirstOrDefaultAsync();

    public async Task<List<People>> GetAsync(DateTime fromDate, DateTime toDate)
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", new BsonDocument
            {
                { "registrations.date", new BsonDocument
                    {
                        { "$gte", fromDate },
                        { "$lte", toDate }
                    }
                }
            }),
            new BsonDocument("$project", new BsonDocument
            {
                { "person.name", 1 },
                { "person.id", 1 },
                { "person.email", 1 },
                { "registrations", new BsonDocument
                    {
                        { "$filter", new BsonDocument
                            {
                                { "input", "$registrations" },
                                { "as", "registration" },
                                { "cond", new BsonDocument
                                    {
                                        { "$and", new BsonArray
                                            {
                                                new BsonDocument("$gte", new BsonArray { "$$registration.date", fromDate }),
                                                new BsonDocument("$lte", new BsonArray { "$$registration.date", toDate })
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }),
            new BsonDocument("$unwind", "$registrations"),
            new BsonDocument("$sort", new BsonDocument
            {
                { "registrations.date", 1 } // 1 ascending sort, -1 descending sort
            }),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$_id" },
                { "person", new BsonDocument("$first", "$person") },
                { "registrations", new BsonDocument("$push", "$registrations") }
            })
        };

        var result = await _peopleCollection.Aggregate<People>(pipeline).ToListAsync();

        return result;
    }
}