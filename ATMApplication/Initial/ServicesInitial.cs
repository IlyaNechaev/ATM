using ATMApplication.Data;
using ATMApplication.Services;
using ATMApplication.Services.Substitute;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ATMApplication.Initial
{
    // Класс, реализующий предварительную инициализацию сервисов приложения
    public static class ServicesInitial
    {
        private static List<ServiceImplementation> SubstituteServices { get; set; } // Список сервисов-имитаторов (заглушек)
        static ServicesInitial()
        {
            SubstituteServices = new();

            SubstituteServices.AddServiceImplementation<ICardService, CardServiceSubstitute>(ServiceLifetime.Transient);
        }

        /// <summary>
        /// Внедряет сервисы-имитаторы, добавленные в <see cref="ServicesInitial"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="forceInject">Если <strong>true</strong>, то внедренные ранее сервисы будут переписаны</param>
        public static void AddSubstituteServices(this IServiceCollection services, bool forceInject = true)
        {
            var extensionType = typeof(ServiceCollectionServiceExtensions);

            foreach (var service in SubstituteServices)
            {
                var descriptor = services.FirstOrDefault(s => s.ServiceType == service.Service);
                if (descriptor is not null)
                {
                    if (forceInject)
                        services.Remove(descriptor);
                    else
                        continue;
                }

                MethodInfo genericMethod = null;
                try
                {
                    genericMethod = extensionType
                        ?.GetMethod($"Add{nameof(service.Lifetime)}", 2, new Type[] { typeof(IServiceCollection) })
                        ?.MakeGenericMethod(service.Service, service.Implementation);
                }
                catch { }

                genericMethod?.Invoke(null, new object[] { services });
            }
        }

        /// <summary>
        /// Внедряет сервис-имитатор, добавленный в <see cref="ServicesInitial"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="forceInject">Если <strong>true</strong>, то внедренный ранее сервис будет переписан</param>
        public static void AddSubstituteService<TService>(this IServiceCollection services, bool forceInject = true)
        {
            var extensionType = typeof(ServiceCollectionServiceExtensions);
            var service = SubstituteServices.FirstOrDefault(s => s.Service == typeof(TService));
            var descriptor = services.FirstOrDefault(s => s.ServiceType == service.Service);

            if (descriptor is not null)
            {
                if (forceInject)
                    services.Remove(descriptor);
                else
                    return;
            }

            MethodInfo genericMethod = null;
            try
            {
                genericMethod = extensionType
                    ?.GetMethod($"Add{nameof(service.Lifetime)}", 2, new Type[] { typeof(IServiceCollection) })
                    ?.MakeGenericMethod(service.Service, service.Implementation);
            }
            catch { }

            genericMethod?.Invoke(null, new object[] { services });
        }

        private static void AddServiceImplementation<TService, TImplementation>(this List<ServiceImplementation> Services, ServiceLifetime Lifetime)
        {
            Services.Add(new()
            {
                Service = typeof(TService),
                Implementation = typeof(TImplementation),
                Lifetime = Lifetime
            });
        }

        private class ServiceImplementation
        {
            public Type Service { get; init; }
            public Type Implementation { get; init; }
            public ServiceLifetime Lifetime { get; init; }
        }
    }
}
