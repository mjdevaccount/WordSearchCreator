using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using WordSearch.Engine.Logic;
using WordSearch.Engine.Models;

namespace WordSearch.Web.Controllers
{
    public class WordSearchController : Controller
    {
        private const string SessionKey = "WordSearchGenerator";

        private static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateGrid(int size)
        {
            size = Math.Max(5, Math.Min(25, size));
            var grid = new WordSearchGrid(size);
            var generator = new WordSearchGenerator(grid);
            Session[SessionKey] = generator;

            return PartialView("_Grid", generator.Grid);
        }

        [HttpPost]
        public ActionResult AddWord(string word)
        {
            if (!(Session[SessionKey] is WordSearchGenerator generator))
                return Json(new { success = false, message = "No grid created. Please create a grid first." });

            var result = generator.AddWord(word);

            if (result.Success)
                return PartialView("_Grid", generator.Grid);

            return Json(new { success = false, message = result.ErrorMessage });
        }

        [HttpPost]
        public ActionResult ValidateWord(string word)
        {
            if (!(Session[SessionKey] is WordSearchGenerator generator))
                return Json(new { valid = true, exists = false });

            if (string.IsNullOrWhiteSpace(word))
                return Json(new { valid = false, exists = false, message = "" });

            word = word.Trim().ToUpper();

            if (!word.All(char.IsLetter))
                return Json(new { valid = false, exists = false, message = "Letters only" });

            if (word.Length > generator.Grid.Size)
                return Json(new { valid = false, exists = false, message = $"Too long (max {generator.Grid.Size})" });

            bool exists = generator.Grid.IncludedWords.Any(w =>
                string.Equals(w.Word, word, StringComparison.OrdinalIgnoreCase));

            return Json(new { valid = true, exists, message = exists ? "Already added" : "" });
        }

        [HttpPost]
        public ActionResult AddBulkWords(string wordsText)
        {
            if (!(Session[SessionKey] is WordSearchGenerator generator))
                return Json(new { success = false, message = "No grid created. Please create a grid first." });

            if (string.IsNullOrWhiteSpace(wordsText))
                return Json(new { success = false, message = "No words provided." });

            var words = wordsText
                .Split(new[] { '\r', '\n', ',', ';', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim().ToUpper())
                .Where(w => w.Length > 0)
                .Distinct()
                .ToList();

            int added = 0;
            var failed = new List<string>();

            foreach (var word in words)
            {
                var result = generator.AddWord(word);
                if (result.Success)
                    added++;
                else
                    failed.Add(word);
            }

            string html = RenderPartialViewToString("_Grid", generator.Grid);

            return Json(new
            {
                success = added > 0,
                added,
                failed,
                html,
                message = $"Added {added} word(s)." + (failed.Count > 0 ? $" {failed.Count} could not be placed." : "")
            });
        }

        [HttpPost]
        public async Task<ActionResult> FetchWords(string topic, int count = 20)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return Json(new { success = false, message = "Please enter a topic." });

            count = Math.Max(5, Math.Min(50, count));
            string encoded = Uri.EscapeDataString(topic.Trim());

            try
            {
                // Fetch from two Datamuse relationships and merge:
                // rel_trg = "trigger" words (words strongly associated with / triggered by the topic)
                // ml      = "means like" (semantically similar words)
                var trgTask = HttpClient.GetStringAsync($"https://api.datamuse.com/words?rel_trg={encoded}&max={count}");
                var mlTask  = HttpClient.GetStringAsync($"https://api.datamuse.com/words?ml={encoded}&max={count}");

                await Task.WhenAll(trgTask, mlTask);

                var trgItems = JsonConvert.DeserializeObject<List<DatamuseWord>>(trgTask.Result) ?? new List<DatamuseWord>();
                var mlItems  = JsonConvert.DeserializeObject<List<DatamuseWord>>(mlTask.Result)  ?? new List<DatamuseWord>();

                // Merge: trigger words first (more thematically relevant), then ml words for variety
                var words = trgItems
                    .Concat(mlItems)
                    .Select(w => w.word?.Trim().ToUpper())
                    .Where(w => !string.IsNullOrEmpty(w) && w.All(char.IsLetter) && w.Length >= 3 && w.Length <= 15)
                    .Distinct()
                    .Take(count)
                    .ToList();

                if (words.Count == 0)
                    return Json(new { success = false, message = $"No words found for \"{topic}\". Try a different topic." });

                return Json(new { success = true, words });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Could not fetch words: " + ex.Message });
            }
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

        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        private class DatamuseWord
        {
            public string word { get; set; }
            public int score { get; set; }
        }
    }
}
