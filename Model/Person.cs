using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace aminsys_api.Model;

[BsonNoId]
public class Person
{
    [BsonElement("id")]
    [JsonPropertyName("ID")]
    public int Id { get; set; }

    [BsonElement("name")]
    [JsonPropertyName("Name")]
    public required string Name { get; set; }

    [BsonElement("email")]
    [JsonPropertyName("Email Adress")]
    public string? Email { get; set; }
}