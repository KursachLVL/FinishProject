using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CourseProject_SecondCourse_.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Это поле обязательно для заполнения")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Это поле обязательно для заполнения")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Это поле обязательно для заполнения")]
        public string Category { get; set; }
        public ApplicationUser User { get; set; }
        [Required(ErrorMessage = "Это поле обязательно для заполнения")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:dd/MM/yyyy}",ApplyFormatInEditMode =true)]
        public DateTime dateTime { get; set; }
       public byte[] Image { get; set; }

    }

    public class Favorite
    {
        public int Id { get; set; }
        public Event Event { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class AllEvents
    {       
        public List<Favorite> _favorites { get; set; }
        public List<Event> _events { get; set; }
        public List<Event> ReturnFavoriteEvents()
        {
            List<Event> list = new List<Event>();

            foreach (Favorite fav in _favorites)
            {
                foreach (Event ev in _events)                    
                    if (ev.Id == fav.Event.Id)
                        list.Add(ev);
            }
            return list;
                           
        }
        public bool Check(Event @event)
        {
            bool check=false;
            foreach (Favorite fav in _favorites)          
                if (@event.Id.Equals(fav.Event.Id))
                {
                    check=true;
                    break;
                }
            return check;
            
        }
        public Favorite  DeleteFavorite(Event ev)
        {           
            foreach (Favorite _fav in _favorites)
                if (_fav.Event.Id == ev.Id)
                    return _fav;
            return new Favorite();
        }
    }
}