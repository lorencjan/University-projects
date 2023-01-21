using System;
using System.Threading.Tasks;
using BooksWeb.ViewModels;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime.Filters;
using Microsoft.Extensions.Logging;

namespace BooksWeb.Model
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger) => _logger = logger;

        protected override Task OnPageExceptionAsync(IDotvvmRequestContext context, Exception e)
        {
            _logger.LogError(e, "Error during loading of page.");
            if (!context.Configuration.Debug)
            {
                context.IsPageExceptionHandled = true;
                context.RedirectToLocalUrl("/500");
            }
            return base.OnPageExceptionAsync(context, e);
        }

        protected override async Task OnCommandExceptionAsync(IDotvvmRequestContext context, ActionInfo actionInfo, Exception e)
        {
            _logger.LogError(e, "Error during command execution.");
            if (context.ViewModel is MasterPageViewModel vm)
            {
                context.IsCommandExceptionHandled = true;
                vm.SetError(e, "Internal error occured");
            }
            await base.OnCommandExceptionAsync(context, actionInfo, e);
        }
    }
}