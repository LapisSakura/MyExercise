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
        MRepository<tComment> tc = new MRepository<tComment>();
        MRepository<tReComment> trc = new MRepository<tReComment>();
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

        //<---------------------------------文章分類---------------------------->
        public JsonResult CategoryDiscussList(string Category)
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

        //<---------------------------------顯示單筆主要文章---------------------------->
        public JsonResult DiscussSingle(int ArticleID, int MemberID)
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

        //<---------------------------------顯示留言---------------------------->
        public JsonResult Comment(int? ArticleID)
        {
            var list = tc.getAll().Where(m => m.ArticleID == ArticleID).Select(m => new
            {
                MemberName= tm.getAll().FirstOrDefault(t => t.MemberID == m.MemberID).MemberName,
                Main=m.Main,
                UpTime=m.UpTime,
                MemberID=m.MemberID,
                No =m.No,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------新增留言---------------------------->
        public JsonResult CreateComment(int MemberID, string Main, int ArticleID)
        {
            tComment list = new tComment()
            {
                MemberID = MemberID,
                Main = Main,
                ArticleID =ArticleID,
                UpTime =DateTime.Now,
            };
            tc.create(list);
            
            return Json(tc.getAll().LastOrDefault(t => t.No > 0).No, JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------顯示回文留言---------------------------->
        public JsonResult ReComment()
        {
            var list = trc.getAll().Select(m => new
            {
                MemberName = tm.getAll().FirstOrDefault(t => t.MemberID == m.MemberID).MemberName,
                Main = m.Main,
                UpTime = m.UpTime,
                MemberID = m.MemberID,
                ReArticleID = m.ReArticleID,
                No = m.No,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------新增回文留言---------------------------->
        public JsonResult CreateReCommend(int MemberID, string Main, int ReArticleID)
        {
            tReComment list = new tReComment()
            {
                MemberID = MemberID,
                Main = Main,
                ReArticleID = ReArticleID,
                UpTime = DateTime.Now,
            };
            trc.create(list);

            return Json(trc.getAll().LastOrDefault(t => t.No > 0).No, JsonRequestBehavior.AllowGet);
        }


        //<---------------------------------顯示單筆回文文章---------------------------->
        public JsonResult ReDiscussSingle(int ArticleID, int MemberID)                   
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

        //<---------------------------------新增文章---------------------------->
        public JsonResult CreateDiscuss(tArticle form)
        {
            tArticle list = new tArticle()
            {
                Category = form.Category,
                Title = form.Title,
                Main = form.Main,
                UpTime = DateTime.Now,
                LoveCount = 0,
                ViewCount = 0,
                MemberID = form.MemberID
            };
            ta.create(list);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------新增回文---------------------------->要改
        public JsonResult CreateReDiscuss(int MemberID, string Main, int ArticleID)
        {
            tReArticle list = new tReArticle()
            {
                ArticleID = ArticleID,
                Main = Main,
                MemberID = MemberID,
                UpTime = DateTime.Now,
                LoveCount = 0,
            };
            tra.create(list);
            return Json("", JsonRequestBehavior.AllowGet);
        }


        //<---------------------------------文章搜尋---------------------------->
        public JsonResult TitleDiscussList(string Title)
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

        //<---------------------------------主文愛心＋－---------------------------->
        public JsonResult MainLoveCount(int ArticleID, bool LoveStatus ,int MemberID)
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

        //<---------------------------------回文愛心＋－---------------------------->
        public JsonResult ReLoveCount(int ReArticleID, bool LoveStatus, int MemberID)
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


        //<---------------------------------刪除留言---------------------------->
        public JsonResult DelComment(int no)
        {
            tc.delete(no);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------編輯留言---------------------------->
        public JsonResult EditComment(int no,string Main)
        {
            db.tComment.FirstOrDefault(m => m.No == no).Main = Main;
            db.tComment.FirstOrDefault(m => m.No == no).UpTime = DateTime.Now;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }   
}