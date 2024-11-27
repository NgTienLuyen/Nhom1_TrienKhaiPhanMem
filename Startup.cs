using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebQuanLyNhanSu.Startup))]
namespace WebQuanLyNhanSu
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
