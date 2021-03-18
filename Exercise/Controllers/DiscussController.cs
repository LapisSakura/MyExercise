using Exercise.Models;
using Exercise.Models.genericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exercise.Controllers
{
    public class DiscussController : Controller
    {
        // GET: Discuss
        MRepository<tArticle> ta = new MRepository<tArticle>();
        MRepository<tMembers> tm = new MRepository<tMembers>();
        MRepository<tReArticle> tra = new MRepository<tReArticle>();
        MRepository<tArticleLove> tal = new MRepository<tArticleLove>();
        MRepository<tReArticleLove> tral = new MRepository<tReArticleLove>();
        dbTeam2_FinalEntities db = new dbTeam2_FinalEntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult vDiscussList()
        {
            return View();
        }
        public ActionResult vDiscussSingle()
        {
            return View();
        }
        public ActionResult vEditDiscuss()
        {
            return View();
        }
        public ActionResult vEditReDiscuss()
        {
            return View();
        }





        public JsonResult CategoryDiscussList(string Category)       //分類清單
        {
            var list = ta.getAll().Where(m => m.Category.Contains(Category)).Select(m => new
            {
                ArticleID = m.ArticleID,
                Category = m.Category,
                Title = m.Title,
                UpTime = m.UpTime,
                LoveCount = m.LoveCount,
                ViewCount = m.ViewCount,
                ReCount = tra.getAll().Where(t => t.ArticleID == m.ArticleID).Count(),
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DiscussSingle(int ArticleID, int MemberID)      //單筆主要文章
        {
            db.tArticle.FirstOrDefault(m => m.ArticleID == ArticleID).ViewCount++;
            db.SaveChanges();
            var list = ta.getAll().Where(m => m.ArticleID == ArticleID).Select(m => new
            {
                ArticleID = m.ArticleID,
                MemberName = tm.getAll().FirstOrDefault(t => t.MemberID == m.MemberID).MemberName,
                Category = m.Category,
                Title = m.Title,
                Main = m.Main,
                UpTime = m.UpTime,
                LoveCount = m.LoveCount,
                ViewCount = m.ViewCount,
                LoveStatus = tal.getAll().FirstOrDefault(t => t.MemberID == MemberID && t.ArticleID == ArticleID) != null,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReDiscussSingle(int ArticleID, int MemberID)                   //單筆回文文章
        {
            var list = tra.getAll().Where(m => m.ArticleID == ArticleID).Select(m => new
            {
                MemberName = tm.getAll().FirstOrDefault(t => t.MemberID == m.MemberID).MemberName,
                Main = m.Main,
                UpTime = m.UpTime,
                LoveCount = m.LoveCount,
                ReArticleID = m.ReArticleID,
                LoveStatus = tral.getAll().FirstOrDefault(t => t.MemberID == MemberID && t.ReArticleID == m.ReArticleID) != null,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateDiscuss(tArticle form)              //新增文章
        {
            tArticle aa = new tArticle()
            {
                Category = form.Category,
                Title = form.Title,
                Main = form.Main,
                UpTime = DateTime.Now,
                LoveCount = 0,
                ViewCount = 0,
                MemberID = form.MemberID
            };
            ta.create(aa);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult TitleDiscussList(string Title)                   //文章搜尋
        {
            var list = ta.getAll().Where(m => m.Title.Contains(Title)).Select(m => new
            {
                ArticleID = m.ArticleID,
                Category = m.Category,
                Title = m.Title,
                UpTime = m.UpTime,
                LoveCount = m.LoveCount,
                ViewCount = m.ViewCount,
                ReCount = tra.getAll().Where(t => t.ArticleID == m.ArticleID).Count(),
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MainLoveCount(int ArticleID, bool LoveStatus ,int MemberID)    //主文愛心+-
        {
            if (LoveStatus)
            {
                db.tArticle.FirstOrDefault(m => m.ArticleID == ArticleID).LoveCount++;
                tArticleLove love = new tArticleLove()
                {
                    MemberID = MemberID,
                    ArticleID = ArticleID,
                };
                db.tArticleLove.Add(love);
            }
            else
            {
                db.tArticle.FirstOrDefault(m => m.ArticleID == ArticleID).LoveCount--;
                var love = db.tArticleLove.FirstOrDefault(m => m.ArticleID == ArticleID && m.MemberID == MemberID);
                db.tArticleLove.Remove(love);
            }
            db.SaveChanges();

       
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReLoveCount(int ReArticleID, bool LoveStatus, int MemberID)       //回文愛心+-
        {
            if (LoveStatus)
            {
                db.tReArticle.FirstOrDefault(m => m.ReArticleID == ReArticleID).LoveCount++;
                tReArticleLove love = new tReArticleLove()
                {
                    MemberID = MemberID,
                    ReArticleID = ReArticleID,
                };
                db.tReArticleLove.Add(love);
            }
            else
            {
                db.tReArticle.FirstOrDefault(m => m.ReArticleID == ReArticleID).LoveCount--;
                var love = db.tReArticleLove.FirstOrDefault(m => m.ReArticleID == ReArticleID && m.MemberID == MemberID);
                db.tReArticleLove.Remove(love);
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);

        }


    }   
}