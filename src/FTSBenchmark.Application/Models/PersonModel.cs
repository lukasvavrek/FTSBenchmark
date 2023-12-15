using FTSBenchmark.Application.Common.Mappings;
using FTSBenchmark.Application.Handlers.Dto;

namespace FTSBenchmark.Application.Models;

public class PersonModel : IMapFrom<PersonDto>
{
    public string FirstName{ get; set; }
    public string LastName{ get; set; }
    public string Username { get; set; }
}