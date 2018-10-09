using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab03.Models;

namespace Lab03.Controllers
{
    public class CategoryController : Controller
    {
        dbDataContext db = new dbDataContext();
        // GET: Category
        public ActionResult Index()
        {
            var cate = from s in db.Categories select s;
            return View(cate);
        }
    }
}