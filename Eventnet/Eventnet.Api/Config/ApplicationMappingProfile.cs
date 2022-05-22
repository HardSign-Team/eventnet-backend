using AutoMapper;
using Eventnet.Api.Models;
using Eventnet.Api.Models.Authentication;
using Eventnet.Api.Models.Events;
using Eventnet.Api.Models.Marks;
using Eventnet.Api.Models.Tags;
using Eventnet.Api.Services.UserAvatars;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events;
using Eventnet.Domain.Selectors;

namespace Eventnet.Api.Config;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateEventsMap();
        CreateLocationMap();
        CreateTagsMap();
        CreateMap<CreateEventModel, Event>();
        CreateProjection<UserEntity, UserNameModel>();
        CreateMap<UserEntity, UserViewModel>()
            .ForMember(x => x.AvatarUrl,
                opt => 
                    opt.MapFrom(x => UserAvatarHelpers.GetUserAvatar(x)));
        CreateMap<UpdateUserForm, UserEntity>();
        CreateMap<RegisterModel, UserEntity>()
            .ForSourceMember(x => x.Password,
                opt => opt.DoNotValidate());
    }

    private void CreateEventsMap()
    {
        CreateMap<EventEntity, Event>();
        CreateMap<EventEntity, EventViewModel>()
            .ForMember(x => x.TotalSubscriptions,
                expression => expression.MapFrom(x => x.Subscriptions.Count))
            .ForMember(x => x.Marks,
                e =>
                    e.MapFrom(x => new MarksCountViewModel(x.Marks.Count(mark => mark.IsLike),
                        x.Marks.Count(mark => !mark.IsLike))));
        CreateMap<EventEntity, EventName>();
        CreateMap<Event, EventLocationViewModel>();
        CreateMap<Event, EventEntity>();
        CreateMap<EventName, EventNameViewModel>();
        CreateMap<CreateEventModel, EventInfo>()
            .ForMember(x => x.Tags, opt => opt.MapFrom(
                src => src.Tags ?? Array.Empty<string>()))
            .ForMember(x => x.Location,
                opt => opt.MapFrom(
                    src => new Location(src.Latitude, src.Longitude)))
            .ForMember(x => x.OwnerId,
                opt => opt.MapFrom(
                    x => Guid.Empty));
    }

    private void CreateTagsMap()
    {
        CreateMap<TagEntity, Tag>();
        CreateProjection<TagEntity, TagName>();
        CreateMap<TagName, TagNameViewModel>();
        CreateMap<TagEntity, TagNameViewModel>();
    }

    private void CreateLocationMap()
    {
        CreateMap<LocationEntity, Location>();
        CreateMap<Location, LocationEntity>();
        CreateMap<LocationEntity, LocationViewModel>();
        CreateMap<Location, LocationViewModel>();
    }
}