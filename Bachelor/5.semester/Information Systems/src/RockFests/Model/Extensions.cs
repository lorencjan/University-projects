using System.Linq;
using System.Reflection;
using DotVVM.Framework.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RockFests.BL.Services;
using RockFests.DAL.Attributes;
using RockFests.ViewModels;

namespace RockFests.Model
{
    public static class Extensions
    {
        public static void AddServicesWithAttribute(this IServiceCollection serviceCollection)
        {
            var services = typeof(UserService).Assembly.GetTypes()
                .Where(x => x.GetCustomAttributes(typeof(RegisterServiceAttribute), true).Length > 0);
            foreach (var service in services)
            {
                var attribute = service.GetCustomAttribute<RegisterServiceAttribute>();
                serviceCollection.TryAdd(ServiceDescriptor.Describe(service, service, attribute.ServiceLifetime));
            }
        }

        public static void AddViewModels(this IServiceCollection services)
        {
            var viewModels = (typeof(MasterPageViewModel).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(DotvvmViewModelBase)) && !x.IsAbstract)).ToList();
            viewModels.ForEach(services.TryAddTransient);
        }
    }
}