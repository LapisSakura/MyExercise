using Exercise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exercise.Controllers
{
    public class HomeController : Controller
    {
        dbTeam2_FinalEntities db = new dbTeam2_FinalEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Chatroom()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public JsonResult getChatroom()
        {
            var list = db.tSqChatroom.Select(m => new
            {
                MemberID = m.MemberID,
                MemberName = db.tMembers.FirstOrDefault(t=>t.MemberID==m.MemberID).MemberName,
                ImgURL = db.tMembers.FirstOrDefault(t => t.MemberID == m.MemberID).ImgURL,
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}