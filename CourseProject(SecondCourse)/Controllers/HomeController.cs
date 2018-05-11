using CourseProject_SecondCourse_.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
namespace CourseProject_SecondCourse_.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<ApplicationUser> manager;

        private ApplicationDbContext db;
        public HomeController()
        {
            db = new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        public ActionResult All(int? page)
        {
            var @events = db.Events.Include(p => p.User);
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(@events.ToList().ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}