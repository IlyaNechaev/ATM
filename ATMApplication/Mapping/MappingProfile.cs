using ATMApplication.Models;
using ATMApplication.Services;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ATMApplication.Mapping
{
    public partial class MappingProfile : Profile
    {
        ISecurityService _securityService { get; set; }
        public MappingProfile(ISecurityService securityService)
        {
            _securityService = securityService;

            var mappingMethods = typeof(MappingProfile)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.GetCustomAttribute(typeof(PerformMappingAttribute)) is not null);

            foreach (var method in mappingMethods)
            {
                try
                {
                    method.Invoke(this, null);
                }
                catch
                { }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PerformMappingAttribute : Attribute
    {
        public PerformMappingAttribute()
        {

        }
    }
}
