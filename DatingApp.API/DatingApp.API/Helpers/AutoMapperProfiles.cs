using AutoMapper;
using DatingApp.API.DTOS;
using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>().ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain).Url);
            }).ForMember(dest => dest.Age, opt =>
              {
                  opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
              });
            CreateMap<User, UserForDetailedDto>().ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain).Url);
            }).ForMember(dest => dest.Age, opt =>
            {
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });
            CreateMap<Photo, PhotosForDetailedDto>();

            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
        }
    }
}
