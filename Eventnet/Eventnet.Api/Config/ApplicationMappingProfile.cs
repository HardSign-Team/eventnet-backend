using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.Domain.Events.Filters.Data;
using Eventnet.Models;

namespace Eventnet.Config;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<EventEntity, Event>();
        CreateMap<LocationEntity, Location>();
    }
}