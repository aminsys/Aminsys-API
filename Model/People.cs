using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace aminsys_api.Model;
public class People
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("person")]
    [JsonPropertyName("Person")]
    public Person Person { get; set; }

    [BsonElement("registrations")]
    [JsonPropertyName("Registrations")]
    public List<Registration>? Registrations { get; set; }
}