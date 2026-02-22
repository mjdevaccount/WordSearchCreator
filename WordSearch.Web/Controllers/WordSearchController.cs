using System.Web.Mvc;
using WordSearch.Engine.Logic;
using WordSearch.Engine.Models;

namespace WordSearch.Web.Controllers
{
    public class WordSearchController : Controller
    {
        private const string SessionKey = "WordSearchGenerator";

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateGrid(int size)
        {
            var grid = new WordSearchGrid(size);
            var generator = new WordSearchGenerator(grid);
            Session[SessionKey] = generator;

            return PartialView("_Grid", generator.Grid);
        }

        [HttpPost]
        public ActionResult AddWord(string word)
        {
            if (!(Session[SessionKey] is WordSearchGenerator generator))
                return Json(new { success = false, message = "No grid created" });

            var result = generator.AddWord(word);

            if (result.Success)
                return PartialView("_Grid", generator.Grid);

            return Json(new { success = false, message = result.ErrorMessage });
        }

        [HttpPost]
        public ActionResult Rebuild()
        {
            if (!(Session[SessionKey] is WordSearchGenerator generator))
                return Json(new { success = false, message = "No grid created" });

            generator.Rebuild();
            return PartialView("_Grid", generator.Grid);
        }

        public ActionResult Solve()
        {
            var generator = Session[SessionKey] as WordSearchGenerator;
            if (generator == null)
                return RedirectToAction("Index");

            generator.FillEmptySpaces();
            return View(generator.Grid);
        }
    }
}