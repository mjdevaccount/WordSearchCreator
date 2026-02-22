using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WordSearch.Web.Controllers
{
    public class WordSearchController : Controller
    {
        // GET: WordSearch
        public ActionResult Index()
        {
            return View();
        }
    }
}