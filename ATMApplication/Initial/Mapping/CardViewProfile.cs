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
        private void CardViewProfile()
        {
            CreateMap<Card, CardViewModel>()
                .ForMember(model => model.Id, options => options.MapFrom(card => card.Id.ToString()))
                .ForMember(model => model.CardNumber, options => options.MapFrom(card => card.CardNumber))
                .ForMember(model => model.MonthYear, options => options.MapFrom(card => card.MonthYear))
                .ForMember(model => model.OwnerName, options => options.MapFrom(card => card.OwnerName));
        }
    }
}
