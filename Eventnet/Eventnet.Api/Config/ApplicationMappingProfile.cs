using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Selectors;
using Eventnet.Models;

namespace Eventnet.Config;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<EventEntity, Event>();
        CreateMap<EventEntity, EventNameModel>();
        CreateMap<LocationEntity, Location>();
        CreateProjection<TagEntity, TagName>();
        CreateMap<TagName, TagNameModel>();
        CreateProjection<UserEntity, UserNameModel>();
    }
}