using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KaamShaam.Startup))]
namespace KaamShaam
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
