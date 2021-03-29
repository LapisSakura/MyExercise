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


        public void Logout()
        {
            Response.Cookies["AutoLogin"].Expires = DateTime.Now.AddSeconds(-1);
        }


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


        public JsonResult Imgupload(HttpPostedFileBase photo)
        {
            var MemberID = Request.Cookies["AutoLogin"]["MemberID"];

            if (photo.ContentLength>0)
            {
                string filepath = $"/MemberImg/Member{MemberID}.jpg";
                photo.SaveAs(Server.MapPath("~") + filepath);

                db.tMembers.FirstOrDefault(m => m.MemberID == 2).ImgURL = filepath;
                db.SaveChanges();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


        public JsonResult ShowMember(int MemberID)
        {
            return Json(tm.getAll().FirstOrDefault(m => m.MemberID == MemberID).ImgURL??"null", JsonRequestBehavior.AllowGet);
        }
    }
}