using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CourseProject_SecondCourse_.Startup))]
namespace CourseProject_SecondCourse_
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
