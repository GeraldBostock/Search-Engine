using SearchEngine.Logic;
using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SearchEngine.Controllers
{
    public class HomeController : Controller
    {
        static Handler handler = new Handler();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Keyword()
        {
            ViewBag.Message = "Keyword counter";
            KeywordModel model = new KeywordModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Keyword(KeywordModel model)
        {
            if(ModelState.IsValid)
                return Json(handler.getKeywordCountResult(model.keywords, model.url), JsonRequestBehavior.AllowGet);

            return Json("Invalid Inputs", JsonRequestBehavior.AllowGet);
        }

        public ActionResult RankUrl()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RankUrl(PageRankModel model)
        {
            PageRankResultModel[] resultModel = null;

            if (ModelState.IsValid)
            {
                handler.setPageRankModel(model);
                handler.clearUrlList();
                handler.rankPages();
                resultModel = handler.getRankingResults();

                return Json("success");
            }

            return Json("failure");
        }

        public ActionResult Site()
        {
            ViewBag.Message = "Site ranking";

            return View();
        }

        [HttpPost]
        public ActionResult Site(PageRankModel model)
        {
            if (ModelState.IsValid)
            {
                handler.setPageRankModel(model);
                handler.clearUrlList();
                handler.rankSites();

                return Json("success");
            }

            return Json("failure");
        }

        public ActionResult Semantic()
        {
            ViewBag.Message = "Semantic analysis";
            

            return View();
        }

        [HttpPost]
        public ActionResult getSynonyms(PageRankModel model)
        {
            if (ModelState.IsValid)
            {
                string[] words = model.keywords.Split(' ');

                SynonymModel synonymModel = new SynonymModel();
                synonymModel.keywords = words;
                synonymModel.synonyms = handler.semanticAnalysis(model.keywords);
                synonymModel.arraySize = words.Length;

                return Json(synonymModel, JsonRequestBehavior.AllowGet);
            }

            return Json("Error", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Semantic(PageRankModel model)
        {
            if(ModelState.IsValid)
            {
                for(int i = 0; i < model.synonyms.Length; i++)
                {
                    model.keywords += " " + model.synonyms[i];
                }
                handler.setPageRankModel(model);
                handler.clearUrlList();
                handler.rankSites();

                return Json("Success", JsonRequestBehavior.AllowGet);
            }

            return Json("Error", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Result()
        {
            PageRankResultModel[] resultModel = handler.getRankingResults();

            return View(resultModel);
        }
    }
}