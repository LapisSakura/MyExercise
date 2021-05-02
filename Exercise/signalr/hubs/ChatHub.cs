using Exercise.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Exercise.signalr.hubs
{
    public class ChatHub : Hub
    {
        dbTeam2_FinalEntities db = new dbTeam2_FinalEntities();


        //<------------------私聊------------------------>
        public void Send(int id,string message)
        {
            var list = db.tMembers.FirstOrDefault(m => m.MemberID == id);
            var cookie = Context.Request.GetHttpContext().Request.Cookies["AutoLogin"]["MemberName"];
            Clients.Client(list.Cid).addNewMessageToPage(cookie, message);
            Clients.Client(Context.ConnectionId).addNewMessageToPage("", message);
        }
        //<----------------進入廣場聊天室 全體跳出上線通知-------------------------->
        public void Login_sq_chatroom(int id)
        {
            var list = db.tMembers.FirstOrDefault(m => m.MemberID == id);
            var cookie = Context.Request.GetHttpContext().Request.Cookies["AutoLogin"]["MemberName"];

            tSqChatroom tsc = new tSqChatroom();
            tsc.MemberID = id;
            db.tSqChatroom.Add(tsc);
            db.SaveChanges();
            Clients.All.login(cookie,list.ImgURL);
        }
        //<----------------廣場聊天室-------------------------->
        public void Sendall(int id,string message)
        {
            var list = db.tMembers.FirstOrDefault(m => m.MemberID == id);
            var MemberName = Context.Request.GetHttpContext().Request.Cookies["AutoLogin"]["MemberName"];
            var MemberID = Context.Request.GetHttpContext().Request.Cookies["AutoLogin"]["MemberID"];
            Clients.All.SqChatroomMsg(MemberName, message,list.ImgURL,MemberID);

        }
        //<-----------------登入------------------------->
        public override Task OnConnected()
        {
            string cid = Context.ConnectionId;
            var cookie = Context.Request.GetHttpContext().Request.Cookies["AutoLogin"]["MemberID"];
            db.tMembers.FirstOrDefault(m => m.MemberID.ToString() == cookie).Cid = cid;
            db.SaveChanges();

            return base.OnConnected();
        }
        //<----------------登出-------------------------->
        public override Task OnDisconnected(bool stopCalled)
        {
            var cookie = Context.Request.GetHttpContext().Request.Cookies["AutoLogin"]["MemberID"];
            db.tMembers.FirstOrDefault(m => m.MemberID.ToString() == cookie).Cid = null;
            db.SaveChanges();

            return base.OnDisconnected(stopCalled);
        }

    }
}