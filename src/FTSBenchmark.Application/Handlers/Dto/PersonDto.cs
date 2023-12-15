using System.Text.Json.Serialization;

namespace FTSBenchmark.Application.Handlers.Dto;

public class PersonDto
{
    [JsonPropertyName("first_name")]
    public string FirstName{ get; set; }
    
    [JsonPropertyName("last_name")]
    public string LastName{ get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }
}


