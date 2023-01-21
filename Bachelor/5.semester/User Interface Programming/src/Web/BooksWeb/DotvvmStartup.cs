using System.IO;
using BooksWeb.Model;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BooksWeb
{
    public class DotvvmStartup : IDotvvmStartup, IDotvvmServiceConfigurator
    {
        // For more information about this class, visit https://dotvvm.com/docs/tutorials/basics-project-structure
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            config.Debug = false;  //Uncomment before releasing
            ConfigureRoutes(config, applicationPath);
            ConfigureControls(config, applicationPath);
            ConfigureResources(config, applicationPath);
        }

        private void ConfigureRoutes(DotvvmConfiguration config, string applicationPath)
        {
            config.RouteTable.Add("Default", "", "Views/Home.dothtml");
            config.RouteTable.AutoDiscoverRoutes(new RouteStrategy(config));    
        }

        private void ConfigureControls(DotvvmConfiguration config, string applicationPath)
        {
            RegisterFiles(config, "Controls", "dotcontrol");
        }

        private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
        {
            RegisterFiles(config, "wwwroot", "css");
            
            config.Resources.Register("font-awesome", new ScriptResource(new FileResourceLocation("wwwroot/lib/font-awesome/js/font-awesome-all.min.js")));
            config.Resources.Register("jquery", new ScriptResource(new FileResourceLocation("wwwroot/lib/jQuery/jquery-2.1.3.min.js")));
            config.Resources.Register("popper", new ScriptResource(new FileResourceLocation("wwwroot/lib/popper/popper.min.js")));
            config.Resources.Register("bootstrap", new ScriptResource
            {
                Location = new FileResourceLocation("wwwroot/lib/bootstrap/js/bootstrap.bundle.min.js"),
                Dependencies = new[] { "jquery", "popper", }
            });
        }

        private static void RegisterFiles(DotvvmConfiguration config, string folder, string fileType)
        {
            var dir = Path.Combine(config.ApplicationPhysicalPath, folder);
            var files = new DirectoryInfo(dir).EnumerateFiles($"*.{fileType}", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                switch (fileType)
                {
                    case "css":
                        config.Resources.Register(file.Name, new StylesheetResource(new FileResourceLocation(file.FullName)));
                        break;
                    case "js":
                        config.Resources.Register(file.Name, new StylesheetResource(new FileResourceLocation(file.FullName)));
                        break;
                    case "dotcontrol":
                        config.Markup.AddMarkupControl("cc", file.Name.Substring(0, file.Name.Length - (fileType.Length+1)), file.FullName);
                        break;
                    default:
                        break;
                }
                
                config.Resources.Register(file.Name, new StylesheetResource(new FileResourceLocation(file.FullName)));
            }
        }

        public void ConfigureServices(IDotvvmServiceCollection options)
        {
            options.AddDefaultTempStorages("temp");
		}
    }
}
