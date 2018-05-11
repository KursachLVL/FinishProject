using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CourseProject_SecondCourse_.Models
{
    public class EventListViewModel
    {
        public IEnumerable<Event> Events { get; set; }
        public SelectList Categories { get; set; }
    }
}