using Owin;
using Microsoft.Owin;

[assembly: OwinStartupAttribute(typeof(Exercise.Startup))]
namespace Exercise
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}