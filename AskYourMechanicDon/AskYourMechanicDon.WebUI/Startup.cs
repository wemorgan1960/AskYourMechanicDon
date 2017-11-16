using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AskYourMechanicDon.WebUI.Startup))]
namespace AskYourMechanicDon.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
