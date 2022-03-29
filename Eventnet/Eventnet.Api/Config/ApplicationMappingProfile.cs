using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.Models;

namespace Eventnet.Config;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<EventEntity, Event>();
        CreateMap<Event, EventEntity>();
        CreateMap<Location, LocationEntity>();
        CreateMap<EventEntity, EventNameModel>();
        CreateMap<LocationEntity, Location>();
        CreateMap<CreateEventModel, Event>();
        CreateProjection<UserEntity, UserNameModel>();
    }
}