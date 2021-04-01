using Exercise.Models;
using Exercise.Models.genericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exercise.Controllers
{
    public class MemberController : Controller
    {
        // GET: Member

        MRepository<tArticle> ta = new MRepository<tArticle>();
        MRepository<tMembers> tm = new MRepository<tMembers>();
        MRepository<tReArticle> tra = new MRepository<tReArticle>();
        dbTeam2_FinalEntities db = new dbTeam2_FinalEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult vRegistered()
        {
            return View();
        }
        public ActionResult vMemberFile()
        {
            return View();
        }


        //<-----------------登入--------------------->
        public string Login(string Account ,string Password,bool RememberMe)
        {
            string result ;
            if (tm.getAll().FirstOrDefault(m => m.Account == Account && m.Password == Password) != null)
            {
                if (tm.getAll().FirstOrDefault(m => m.Account == Account).Status == 1)
                {
                    Response.Cookies["AutoLogin"]["MemberID"] = tm.getAll().FirstOrDefault(m => m.Account == Account).MemberID.ToString();
                    Response.Cookies["AutoLogin"]["MemberName"] = tm.getAll().FirstOrDefault(m => m.Account == Account).MemberName.ToString();
                    result = "登入成功";
                    if (RememberMe)
                        Response.Cookies["AutoLogin"].Expires = DateTime.Now.AddMonths(1);
                }
                else
                    result = "帳號已被封鎖 或 尚未啟用請確認信箱";
            }
            else            
                result = "帳號密碼錯誤";            
            return result;
        }

        //<----------------登出---------------------->
        public void Logout()
        {
            Response.Cookies["AutoLogin"].Expires = DateTime.Now.AddSeconds(-1);
        }

        //<-------------------驗證帳號是否重複------------------->
        public JsonResult CheckAccount(string Account)
        {
            bool result = tm.getAll().FirstOrDefault(m => m.Account == Account) == null;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateMember(tMembers form)
        {
            tMembers aa = new tMembers()
            {
                MemberName = form.MemberName,
                Account = form.Account,
                Password = form.Password,
                RegisteredDate = DateTime.Now,
                Authority = "Member",
                Status = 1
            };
            tm.create(aa);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        //<------------------照片上傳-------------------->
        public JsonResult Imgupload(HttpPostedFileBase photo)
        {
            string filepath = "";
            int MemberID = int.Parse(Request.Cookies["AutoLogin"]["MemberID"]);

            if (photo.ContentLength>0)
            {
                filepath = $"/MemberImg/Member{MemberID}.jpg";
                photo.SaveAs(Server.MapPath("~") + filepath);

                db.tMembers.FirstOrDefault(m => m.MemberID == MemberID).ImgURL = filepath;
                db.SaveChanges();
            }
            return Json(filepath, JsonRequestBehavior.AllowGet);
        }

        //<-----------------------個人頁面載入---------------------->
        public JsonResult ShowMember(int MemberID)
        {
            var list = tm.getAll().Where(m => m.MemberID == MemberID).Select(m => new
            {
                MemberName=m.MemberName,
                ImgURL=m.ImgURL,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ShowCategory(int MemberID)
        {
            var list = db.tCustomizeTopic.Where(m => m.MemberID == MemberID).GroupBy(m => m.Category, (t, tn) => new { Category = t});
            return Json(list, JsonRequestBehavior.AllowGet);
        }



        public JsonResult ShowTopic(string Category, int MemberID)
        {
            var list = db.tCustomizeTopic.Where(m => m.Category == Category && m.MemberID == MemberID).Select(m => new
            {
                No = m.No,
                Question=m.Question,
                Answer=m.Answer,
            });
            return Json(list, JsonRequestBehavior.AllowGet);

        }
    }
}