using AutoMapper;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.User;

namespace Predictly_Api.Mappings
{
    public class UserProfiles: Profile
    {
        public UserProfiles()
        {
            CreateMap<ApplicationUserModel, UserViewModel>();
        }
    }
}
