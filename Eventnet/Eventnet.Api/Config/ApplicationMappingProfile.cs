using AutoMapper;
using Eventnet.Api.Models;
using Eventnet.Api.Models.Authentication;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events;
using Eventnet.Domain.Selectors;
using Eventnet.Models;

namespace Eventnet.Api.Config;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<EventEntity, Event>();
        CreateMap<EventEntity, EventName>();
        CreateMap<Event, EventLocationModel>();
        CreateMap<LocationEntity, Location>();
        CreateProjection<TagEntity, TagName>();
        CreateMap<TagName, TagNameModel>();
        CreateProjection<UserEntity, UserNameModel>();
        CreateMap<UserEntity, UserViewModel>();
        CreateMap<RegisterModel, UserEntity>()
            .ForMember(x => x.PhoneNumber,
                opt => opt.MapFrom(x => x.Phone))
            .ForSourceMember(x => x.Password,
                opt => opt.DoNotValidate());
    }
}