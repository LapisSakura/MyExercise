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
        public ActionResult vCreateDiscuss()
        {
            return View();
        }
        public ActionResult vCreateReDiscuss()
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
            var list = db.tArticle.Where(m => m.Category.Contains(Category)).Select(m => new
            {
                ArticleID = m.ArticleID,
                Category = m.Category,
                Title = m.Title,
                UpTime = m.UpTime,
                LoveCount = m.LoveCount,
                ViewCount = m.ViewCount,
                ReCount = db.tReArticle.Where(t => t.ArticleID == m.ArticleID).Count(),
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------顯示單筆主要文章---------------------------->
        public JsonResult DiscussSingle(int ArticleID, int MemberID)
        {
            db.tArticle.FirstOrDefault(m => m.ArticleID == ArticleID).ViewCount++;
            db.SaveChanges();
            var list = db.tArticle.Where(m => m.ArticleID == ArticleID).Select(m => new
            {
                ArticleID = m.ArticleID,
                MemberName = db.tMembers.FirstOrDefault(t => t.MemberID == m.MemberID).MemberName,
                Category = m.Category,
                MemberID = m.MemberID,
                Title = m.Title,
                Main = m.Main,
                UpTime = m.UpTime,
                LoveCount = m.LoveCount,
                ViewCount = m.ViewCount,
                LoveStatus = db.tArticleLove.FirstOrDefault(t => t.MemberID == MemberID && t.ArticleID == ArticleID) != null,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------顯示留言---------------------------->
        public JsonResult Comment(int ArticleID, int MemberID)
        {
            var list = db.tComment.Where(m => m.ArticleID == ArticleID).Select(m => new
            {
                MemberName = db.tMembers.FirstOrDefault(t => t.MemberID == m.MemberID).MemberName,
                Main = m.Main,
                UpTime = m.UpTime,
                MemberID = m.MemberID,
                No = m.No,
                ImgURL = db.tMembers.FirstOrDefault(t => t.MemberID == m.MemberID).ImgURL,
            });
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CommentInput(int MemberID)
        {
            var NowImgURL = "";
            if (MemberID == 0)
                NowImgURL = "null";
            else
                NowImgURL = db.tMembers.FirstOrDefault(m => m.MemberID == MemberID).ImgURL == null ? "null" : db.tMembers.FirstOrDefault(m => m.MemberID == MemberID).ImgURL;
            return Json(NowImgURL, JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------新增留言---------------------------->
        public JsonResult CreateComment(int MemberID, string Main, int ArticleID)
        {
            tComment list = new tComment()
            {
                MemberID = MemberID,
                Main = Main,
                ArticleID = ArticleID,
                UpTime = DateTime.Now,
            };
            tc.create(list);

            return Json(tc.getAll().LastOrDefault(t => t.No > 0).No, JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------新增回文留言---------------------------->
        public JsonResult CreateReComment(int MemberID, string Main, int ReArticleID)
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
                ImgURL = tm.getAll().FirstOrDefault(t => t.MemberID == m.MemberID).ImgURL,
                No = m.No,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        //<---------------------------------顯示單筆回文文章---------------------------->
        public JsonResult ReDiscussSingle(int ArticleID, int MemberID)
        {
            var list = db.tReArticle.Where(m => m.ArticleID == ArticleID).Select(m => new
            {
                MemberName = db.tMembers.FirstOrDefault(t => t.MemberID == m.MemberID).MemberName,
                MemberID = db.tMembers.FirstOrDefault(t => t.MemberID == m.MemberID).MemberID,
                Main = m.Main,
                UpTime = m.UpTime,
                LoveCount = m.LoveCount,
                ReArticleID = m.ReArticleID,
                NowImgURL = db.tMembers.FirstOrDefault(t => t.MemberID == MemberID).ImgURL,
                LoveStatus = db.tReArticleLove.FirstOrDefault(t => t.MemberID == MemberID && t.ReArticleID == m.ReArticleID) != null,
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
            var ID = ta.getAll().LastOrDefault(m => m.ArticleID > 0).ArticleID;
            return Json(ID, JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------回傳文章---------------------------->
        public JsonResult ReturnDiscuss(int ArticleID)
        {
            var list = ta.getAll().Where(m => m.ArticleID == ArticleID).Select(m => new
            {
                Category = m.Category,
                Main = m.Main,
                Title = m.Title,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //<---------------------------------編輯文章---------------------------->
        public JsonResult EditDiscuss(tArticle formdata)
        {
            var edit = db.tArticle.FirstOrDefault(m => m.ArticleID == formdata.ArticleID);
            edit.Main = formdata.Main;
            edit.Title = formdata.Title;
            edit.Category = formdata.Category;
            edit.UpTime = DateTime.Now;
            db.SaveChanges();
            return Json(formdata.ArticleID, JsonRequestBehavior.AllowGet);
        }
        //<---------------------------------刪除文章---------------------------->
        public JsonResult DelDiscuss(int ArticleID)
        {
            var count = tra.getAll().Where(m => m.ArticleID == ArticleID).Count();                     //抓到回文數
            for (int i = 0; i < count; i++)
            {
                var raid = tra.getAll().FirstOrDefault(m => m.ArticleID == ArticleID).ReArticleID;
                var count1 = trc.getAll().Where(m => m.ReArticleID == raid).Count();
                for (int k = 0; k < count1; k++)
                {
                    trc.delete(trc.getAll().FirstOrDefault(m => m.ReArticleID == raid).No);           //殺回文留言
                }

                var count2 = tral.getAll().Where(m => m.ReArticleID == raid).Count();
                for (int k = 0; k < count2; k++)
                {
                    tral.delete(tral.getAll().FirstOrDefault(m => m.ReArticleID == raid).No);        //殺回文愛心
                }
                tra.delete(tra.getAll().FirstOrDefault(m => m.ArticleID == ArticleID).ReArticleID);  //殺回文
            }

            var ccount = tc.getAll().Where(m => m.ArticleID == ArticleID).Count();
            for (int i = 0; i < ccount; i++)
            {
                tc.delete(tc.getAll().FirstOrDefault(m => m.ArticleID == ArticleID).No);            //殺本文留言
            }

            var ccount1 = tal.getAll().Where(m => m.ArticleID == ArticleID).Count();
            for (int i = 0; i < ccount1; i++)
            {
                tal.delete(tal.getAll().FirstOrDefault(m => m.ArticleID == ArticleID).No);          //殺本文愛心
            }
            ta.delete(ArticleID);                                                                   //殺本文
            return Json("", JsonRequestBehavior.AllowGet);
        }


        //<---------------------------------新增回文---------------------------->
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

        //<---------------------------------回傳回文---------------------------->
        public JsonResult ReturnReDiscuss(int ReArticleID)
        {
            var TheRA = tra.getAll().FirstOrDefault(m => m.ReArticleID == ReArticleID);
            var list = ta.getAll().Where(m => m.ArticleID == TheRA.ArticleID).Select(m => new
            {
                Title = m.Title,
                Category = m.Category,
                Main = TheRA.Main,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------編輯回文---------------------------->
        public JsonResult EditReDiscuss(string Main, int ReArticleID)
        {
            db.tReArticle.FirstOrDefault(m => m.ReArticleID == ReArticleID).Main = Main;
            db.SaveChanges();
            return Json(db.tReArticle.FirstOrDefault(m => m.ReArticleID == ReArticleID).ArticleID, JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------刪除回文---------------------------->
        public JsonResult DelReArticle(int ReArticleID)
        {
            var count = trc.getAll().Where(m => m.ReArticleID == ReArticleID).Count();
            for (int i = 0; i < count; i++)
            {
                trc.delete(trc.getAll().FirstOrDefault(m => m.ReArticleID == ReArticleID).No);
            }
            var count2 = tral.getAll().Where(m => m.ReArticleID == ReArticleID).Count();
            for (int i = 0; i < count2; i++)
            {
                tral.delete(tral.getAll().FirstOrDefault(m => m.ReArticleID == ReArticleID).No);
            }
            
            tra.delete(ReArticleID);
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
        public JsonResult MainLoveCount(int ArticleID, bool LoveStatus, int MemberID)
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
        //<---------------------------------刪除回文留言---------------------------->
        public JsonResult DelReComment(int no)
        {
            trc.delete(no);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------編輯留言---------------------------->
        public JsonResult EditComment(int no, string Main)
        {
            db.tComment.FirstOrDefault(m => m.No == no).Main = Main;
            db.tComment.FirstOrDefault(m => m.No == no).UpTime = DateTime.Now;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //<---------------------------------編輯回文留言---------------------------->
        public JsonResult EditReComment(int no, string Main)
        {
            db.tReComment.FirstOrDefault(m => m.No == no).Main = Main;
            db.tReComment.FirstOrDefault(m => m.No == no).UpTime = DateTime.Now;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        //<---------------------------------回傳標題---------------------------->
        public JsonResult ReturnTitle(int ArticleID)
        {
            var list = ta.getAll().Where(m => m.ArticleID == ArticleID).Select(m => new
            {
                Category = m.Category,
                Title = m.Title,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }   
}