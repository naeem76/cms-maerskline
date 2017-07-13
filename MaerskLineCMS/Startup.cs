using Microsoft.Owin;
using Owin;
using System.Collections;

[assembly: OwinStartupAttribute(typeof(MaerskLineCMS.Startup))]
namespace MaerskLineCMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            
        }
    }
}
