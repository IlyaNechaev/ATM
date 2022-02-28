using ATMApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Mapping
{
    public partial class MappingProfile
    {
        [PerformMapping]
        private void UserViewProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(model => model.FirstName, options => options.MapFrom(user => user.FirstName))
                .ForMember(model => model.MiddleName, options => options.MapFrom(user => user.MiddleName))
                .ForMember(model => model.LastName, options => options.MapFrom(user => user.LastName))
                .ForMember(model => model.Age, options => options.MapFrom(user => user.Age));
        }
    }
}
