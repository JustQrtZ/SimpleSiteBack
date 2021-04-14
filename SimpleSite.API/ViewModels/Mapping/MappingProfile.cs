using AutoMapper;
using SimpleSite.Model;
using SimpleSite.Model.Entities;

namespace SimpleSite.API.ViewModels.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserViewModel, UserTableViewModel>();
            CreateMap<User, UserViewModel>();
        }
    }
}