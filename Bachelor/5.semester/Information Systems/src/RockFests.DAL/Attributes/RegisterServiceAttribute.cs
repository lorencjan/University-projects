using System;
using Microsoft.Extensions.DependencyInjection;

namespace RockFests.DAL.Attributes
{
    public class RegisterServiceAttribute : Attribute
    {
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
    }
}
