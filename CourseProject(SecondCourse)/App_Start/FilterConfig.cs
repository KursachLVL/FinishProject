using System.Web;
using System.Web.Mvc;

namespace CourseProject_SecondCourse_
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
