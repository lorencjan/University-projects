using Microsoft.Extensions.DependencyInjection;
using System;

namespace BooksWeb.DAL.Attributes
{
    public class RegisterServiceAttribute : Attribute
    {
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
    }
}