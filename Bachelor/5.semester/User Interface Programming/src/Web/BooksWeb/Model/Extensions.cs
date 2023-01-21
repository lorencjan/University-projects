using System.Linq;
using System.Reflection;
using BooksWeb.DAL.Attributes;
using BooksWeb.DAL.Services;
using BooksWeb.ViewModels;
using DotVVM.Framework.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BooksWeb.Model
{
    public static class Extensions
    {
        public static void AddServicesWithAttribute(this IServiceCollection serviceCollection)
        {
            var services = typeof(AuthorsService).Assembly.GetTypes()
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