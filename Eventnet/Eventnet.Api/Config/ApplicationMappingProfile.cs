using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.Models;

namespace Eventnet.Config;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<EventEntity, Event>();
        CreateMap<EventEntity, EventNameModel>();
        CreateMap<LocationEntity, Location>();
        CreateMap<Event, CreateEventModel>()
            .ForMember(x => x.UserId,
                opt => opt.MapFrom(model => Guid.NewGuid()));
        CreateProjection<UserEntity, UserNameModel>();
    }
}