using CourseProject_SecondCourse_.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CourseProject_SecondCourse_.Controllers
{

    [Authorize]
    public class EventController : Controller
    {


        private UserManager<ApplicationUser> manager;
        private ApplicationDbContext db;
        private List<string> _categories;

        public EventController()
        {
            db = new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            _categories = new List<string>() {
                    "помощь",
                    "развлечения",
                    "образование",
                    "красота",
                    "спорт",
                    "культура",
                    "наука" };
        }

        public ActionResult Favorites()
        {
            AllEvents all = new AllEvents();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            all._events = db.Events.ToList();
            all._favorites = db.Favorites.Where(t => t.User.Id == currentUser.Id).ToList();
            var events=all.ReturnFavoriteEvents();         
            
            return View(events);
        }

        public ActionResult MyFavorites()
        {
            AllEvents all = new AllEvents();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            all._events = db.Events.ToList();
            all._favorites = db.Favorites.Where(t => t.User.Id == currentUser.Id).ToList();
            var events = all.ReturnFavoriteEvents();

            return PartialView(events);

        }

        public ActionResult MyEvents()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var @events = db.Events.Where(t => t.User.Id == currentUser.Id).ToList();
            return PartialView(@events);
        }

        public ActionResult Select(string category)
        {
            IEnumerable<Event> events = db.Events.ToList();
            if (!string.IsNullOrEmpty(category) && !category.Equals("Все"))
            {
                events = events.Where(p => p.Category == category);
            }
            EventListViewModel eventsList = new EventListViewModel
            {
                Events = events.ToList(),
                Categories = new SelectList(new List<string>()
                {
                    "помощь",
                    "развлечения",
                    "образование",
                    "красота",
                    "спорт",
                    "культура",
                    "наука"
                    
                })
            };
            return View(eventsList);
        }

        [HttpGet]
        public ActionResult AddFavorite(int id)
        {
            Favorite favorite = new Favorite();
            var currentUser = manager.FindById(User.Identity.GetUserId());
            Event @event = db.Events.Find(id);
            favorite.User = currentUser;
            favorite.Event = @event;
            db.Favorites.Add(favorite);
            db.SaveChanges();
            return PartialView(@event);

        }

        
        public ActionResult DeleteFavorite(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

       
        [HttpPost, ActionName("DeleteFavorite")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedFavorite(int id)
        {

            Event @event = db.Events.Find(id);
            var currentUser = manager.FindById(User.Identity.GetUserId());
            AllEvents all = new AllEvents();
            all._favorites=db.Favorites.Where(t => t.User.Id == currentUser.Id).ToList();
            db.Favorites.Remove(all.DeleteFavorite(@event));
            db.SaveChanges();
            return RedirectToAction("Favorites");
        }
        
        [HttpGet]
        public ActionResult EventPage(int? id)
        {
            if (id == null)
            {
                // возвращает экземпляр класса HttpNotFound 
                return HttpNotFound();
            }
            var currentUser = manager.FindById(User.Identity.GetUserId());

            AllEvents all = new AllEvents();
            all._events = db.Events.ToList();
            all._favorites = db.Favorites.Where(t => t.User.Id == currentUser.Id).ToList();
            var events = all.ReturnFavoriteEvents();
            bool check = false;
            int compare = 0;
            Event @event = db.Events.FirstOrDefault(t => t.Id == id);
            if (events.Count != 0)
            {
                if (events.Contains(@event))
                    check = true;
                else
                    check = false;

            }

            if (currentUser.Equals(@event.User))
                compare = 1;
            ViewBag.Compare = compare;
            ViewBag.CompareMessage = "Вы не можете добавить это событие в избранное так как вы являетесь его создателем";

            ViewBag.Check = check;

            ViewBag.MessageCheck = "Это событие уже находиться в ваших избранных";
            return View(@event);
        }

       
        public ActionResult Index()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var @event = db.Events.Where(t => t.User.Id == currentUser.Id).ToList();
            return View(@event);
        }


        // GET: ToDoes/Create
        public ActionResult Create()
        {
            ViewBag.ListOfCat = _categories;

            return View();
        }

        // POST: ToDoes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Event,Name,Description,Category,dateTime,Image")] Event @event, HttpPostedFileBase uploadImage)
        {

            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (ModelState.IsValid && uploadImage!=null)
            {
                byte[] imageData = null;
                using (var binaryReader=new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                @event.Image = imageData;

                @event.User = currentUser;
                db.Entry(@event).State = EntityState.Added;
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index", "Event");
            }
           
            if(uploadImage==null&&ModelState.IsValid)
            {
                string filename = Server.MapPath("/image/no-image.png");
                byte[] data = System.IO.File.ReadAllBytes(filename);
                @event.Image = data;
                @event.User = currentUser;
                db.Entry(@event).State = EntityState.Added;
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index", "Event");
            }

            ViewBag.ListOfCat = _categories;

            return View(@event);

            
        }

        // GET: ToDoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: ToDoes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, HttpPostedFileBase uploadImage)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventToUpdate = db.Events.Find(id);
            if (TryUpdateModel(eventToUpdate, "",
                new string[] { "Name", "Description", "Category","dateTime" }))
            {
                if (uploadImage != null && uploadImage.ContentLength > 0)
                {
                    byte[] imageData = null;

                    using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                    }
                    eventToUpdate.Image = imageData;
                }

                db.Entry(eventToUpdate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index"); 
            }
            return View(eventToUpdate);
        }

        // GET: ToDoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: ToDoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Event @event = db.Events.Find(id);
            var favorites = db.Favorites.Where(t => t.Event.Id == @event.Id).ToList();
            db.Favorites.RemoveRange(favorites);
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index","Account");
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
