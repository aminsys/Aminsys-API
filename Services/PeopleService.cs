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

    public async Task<List<People>> GetAsync()
    {
        var projection = Builders<People>.Projection.Expression(p => new People
        {
            Person = new Person
            {
                Id = p.Person.Id,
                Name = p.Person.Name,
                Email = p.Person.Email
            }
        });

        var result = await _peopleCollection.Find(_ => true).Project(projection).ToListAsync();
        return result;
    }

    public async Task<People> GetAsync(int personId) =>
        await _peopleCollection.Find(p => p.Person.Id == personId).FirstOrDefaultAsync();

    public async Task<List<People>> GetAsync(DateTime fromDate, DateTime toDate)
    {
        var pipeline = new BsonDocument[]
        {
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