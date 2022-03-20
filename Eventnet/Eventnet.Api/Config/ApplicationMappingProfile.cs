using AutoMapper;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events;
using Eventnet.Domain.Selectors;
using Eventnet.Models;

namespace Eventnet.Config;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<EventEntity, Event>();
        CreateMap<EventEntity, EventName>();
        CreateMap<LocationEntity, Location>();
        CreateProjection<TagEntity, TagName>();
        CreateMap<TagName, TagNameModel>();
        CreateProjection<UserEntity, UserNameModel>();
    }
}