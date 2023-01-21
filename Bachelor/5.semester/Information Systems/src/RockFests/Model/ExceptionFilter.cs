using System;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime.Filters;
using Microsoft.Extensions.Logging;
using RockFests.BL.Resources;
using RockFests.ViewModels;

namespace RockFests.Model
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
                vm.SetError(e, Errors.Modal500Body);
            }
            await base.OnCommandExceptionAsync(context, actionInfo, e);
        }
    }
}