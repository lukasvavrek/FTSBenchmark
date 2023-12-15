using AutoMapper;
using FTSBenchmark.Application.Handlers.Dto;
using FTSBenchmark.Domain;

namespace FTSBenchmark.Application.Common.Mappings;

public class ModelProfile : Profile
{
    public ModelProfile()
    {
        CreateMap<PersonDto, PersonModel>();
    }
}