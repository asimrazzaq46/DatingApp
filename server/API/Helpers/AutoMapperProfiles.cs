using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        //what if some props are not populating with AUTOMAPPER than we can configure it by ourselves
        //in this case photoUrl is null so we are configuring it by ourselves with automapper built in function "ForMember"
        // ForMember taking two parameters what we want to change(destination) and what is the source

        CreateMap<AppUser, MemberDto>()
        .ForMember(dest => dest.Age, o => o.MapFrom(source => source.DateOfBirth.CalculateAge()))
        .ForMember(dest => dest.PhotoUrl, o => o.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain)!.Url));

        CreateMap<Photo, PhotoDto>();
    }

}
