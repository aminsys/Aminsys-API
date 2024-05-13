using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace aminsys_api.Model;
public class Registration
{
    [BsonElement("date")]
    [JsonPropertyName("Date Object")]
    public DateTime DateObject { get; set; }

    [BsonElement("status")]
    [JsonPropertyName("Status")]
    public int Status { get; set; }
}