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
        private void CardEditProfile()
        {
            CreateMap<CardEditModel, Card>()
                .ForMember(card => card.CardNumber, options => options.MapFrom(model => model.CardNumber))
                .ForMember(card => card.HashCVV, options => options.MapFrom(model => _securityService.GetPasswordHash(model.CVV.ToString())))
                .ForMember(card => card.HashPin, options => options.MapFrom(model => _securityService.GetPasswordHash(model.Pin.ToString())))
                .ForMember(card => card.OwnerName, options => options.MapFrom(model => model.OnwerName))
                .ForMember(card => card.MonthYear, options => options.MapFrom(model => model.MonthYear));
        }
    }
}
