using AutoMapper;
using Eventnet.Api.Models;
using Eventnet.Api.Models.Authentication;
using Eventnet.Api.Models.Events;
using Eventnet.Api.Models.Tags;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events;
using Eventnet.Domain.Selectors;

namespace Eventnet.Api.Config;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<EventEntity, Event>();
        CreateMap<EventEntity, EventName>();
        CreateMap<Event, EventLocationModel>();
        CreateMap<LocationEntity, Location>();
        CreateMap<Event, EventEntity>();
        CreateMap<Location, LocationEntity>();
        CreateMap<CreateEventModel, Event>();
        CreateMap<LocationEntity, LocationViewModel>();
        CreateMap<TagEntity, Tag>();
        CreateProjection<TagEntity, TagName>();
        CreateMap<TagName, TagNameModel>();
        CreateMap<TagEntity, TagNameModel>();
        CreateProjection<UserEntity, UserNameModel>();
        CreateMap<UserEntity, UserViewModel>();
        CreateMap<RegisterModel, UserEntity>()
            .ForSourceMember(x => x.Password,
                opt => opt.DoNotValidate());
    }
}