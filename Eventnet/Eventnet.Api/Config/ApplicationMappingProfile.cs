using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.Models;
using Eventnet.Models.Authentication;

namespace Eventnet.Config;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<EventEntity, Event>();
        CreateMap<EventEntity, EventNameModel>();
        CreateMap<LocationEntity, Location>();
        CreateProjection<UserEntity, UserNameModel>();
        CreateMap<UserEntity, UserViewModel>();
        CreateMap<RegisterModel, UserEntity>()
            .ForSourceMember(x => x.Password, 
                opt => opt.DoNotValidate());
    }
}