using API.DTO;
using API.Entities;
using AutoMapper;

namespace API.Helper
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>();
            CreateMap<Photo, PhotoDTO>();
        }
    }
}
