using System;
using System.Collections.Generic;
using System.Linq;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.Routing;

namespace RockFests.Model
{
    public class RouteStrategy : DefaultRouteStrategy
    {
        public static readonly List<string> ViewsWithId = new List<string> { "detail" };

        public RouteStrategy(DotvvmConfiguration configuration) : base(configuration) {}

        protected override string GetRouteUrl(RouteStrategyMarkupFileInfo file)
        {
            var url = base.GetRouteUrl(file);
            return ViewsWithId.Any(x => url.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)) ? url + "/{Id}" : url;
        }
    }
}
