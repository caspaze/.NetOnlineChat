using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineChat.Models.Chats;
using OnlineChat.Models.Users;
using OnlineChat.Models.Messages;
using Microsoft.AspNetCore.Authorization;
using OnlineChat.ViewModels;
using Microsoft.AspNetCore.SignalR;
using OnlineChat.Hubs;
using Microsoft.AspNetCore.Http;
using System.IO;
using OnlineChat.Models.Documents;
using System.Security.Claims;
using OnlineChat.Models;
using log4net;

namespace OnlineChat.Controllers.Chats
{
    public class ChatController : Controller
    {
        UserDAO repos;
        ChatDAO repos1;
        MessageDAO repos3;
        LogFactory logFactory;
        private ILog log;
        IHubContext<ChatHub> hubContext;
        public ChatController(UserDAO _repos,ChatDAO _repos1, MessageDAO _repos3, IHubContext<ChatHub> hubContext, LogFactory _logFactory)
		{
            repos = _repos;
            repos1 = _repos1;
            repos3 = _repos3;
            logFactory = _logFactory;
            log = logFactory.GetLogger();
            this.hubContext = hubContext;
        }
        [Authorize]
        public async Task<IActionResult> ForLogged(string name, string email)
        {
            User user = repos.GetUserOnEmail(email);
            Chat chat = repos1.GetChatOnName(name);
            return RedirectToAction("Chat", "Chat", new { chat.GUID });
        }
       
        [TempData]
        public int Count { get; set; }
        public IActionResult Chat(string GUID)
        {
            ViewBag.GUID = GUID;
            string name = repos1.GetOnGUID(GUID).Name;
            ViewBag.ChatName = name;

            if (User.Identity.IsAuthenticated)
            {

                User user1 = repos.GetUserOnEmail(this.User.FindFirstValue(ClaimTypes.Name));
                if (user1 == null)
                {
                    user1 = repos.GetUserOnEmail(this.User.FindFirstValue(ClaimTypes.Name));
                }
                List<Chat> mychats = repos.GetChats(user1.UserId);
                bool Join = true;
                foreach (Chat mychat in mychats)
                {
                    if (mychat.Name == name)
                        Join = false;
                }
                ViewBag.Join = Join;
                ViewBag.Login = user1.UserName;
                ViewBag.UserId = user1.UserId;
            }
            else
            {
                ViewBag.Join = false;
            }
            Chat myChat = repos1.GetChatOnName(name);
            ViewBag.Name = name;
            ViewBag.Id = myChat.Id;
            List < Message > messages = repos1.GetMessages(myChat.Id);
            List<Message> texts = new List<Message>();
            List<string> logins = new List<string>();
            int kolvo = Count;
            TempData.Keep("Count");
            int i = 0;
            int messagesBegin = messages.Count() - kolvo-10;
            int k = 0;
            foreach(Message mes in messages)
            {
                
                if (mes.UserId!=0&&k>messagesBegin)
                {
                    if(mes.DocId!=0)
                    mes.Doc = repos3.GetDoc(mes.DocId);
                    texts.Add(mes);
                    User user = repos.Get(mes.UserId);
                    logins.Add(user.UserName);
                    
                }
                k++;
            }
            bool load = messages.Count() > kolvo+10;
            ViewBag.Load = load;
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Auth = true;
            }
            else
            {
                ViewBag.Auth = false;
            }
            ViewBag.Messages = messages;
            ViewBag.Texts = texts;
            ViewBag.Names = logins;
            return View();
        }
        public async Task<IActionResult> Loadmore(CreateModel model)
        {
           
            Count += 10;
            TempData.Keep("Count");
            Chat chat = repos1.GetOnGUID(model.Name);
            return RedirectToAction("Chat", "Chat", new { chat.GUID });
        }
        public async Task<IActionResult> JoinChat(CreateModel model)
        {
            User user = repos.Get(Convert.ToInt32(model.Email));
            Chat chat = repos1.GetOnGUID(model.Name);
            repos1.Join(user.Email,chat);
            return RedirectToAction("Chat", "Chat", new { chat.GUID });
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            Chat chat = repos1.GetOnGUID(model.Name);
            byte[] post = null;
            if(model.File!=null)
                using (var binaryReader = new BinaryReader(model.File.OpenReadStream()))
                {
                    post = binaryReader.ReadBytes((int)model.File.Length);
                }
            User user = repos.Get(Convert.ToInt32(model.Email));
            Message message = new Message { Color=0 , ChatId=chat.Id,Text=model.Text,UserId=user.UserId };
            if (this.User.FindFirstValue(ClaimTypes.Role) == "Admin")
            {
                message.Color = 1;
            }
                string login = user.UserName;
            if (model.File == null)
            {
                repos3.Create(message);
            }
            else
            {

                Document doc = new Document { GUID = Guid.NewGuid().ToString(), Name=model.File.FileName,Data=post};
                repos3.CreateWithDoc(message,doc);
            }
            message.Doc = repos3.GetDoc(message.DocId);
            log.Info("Created new chat " + '\n'
                + "Chat name: " + chat.Name + '\n'
                + "Chat creator: " + model.Name);
            await hubContext.Clients.Group(chat.GUID).SendAsync("ReceiveMessage", login, message);
            return RedirectToAction("Chat", "Chat", new { chat.GUID });
        }
        private byte[] GetByteArrayFromFile(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyToAsync(target);
                return target.ToArray();
            }
        }

        public FileResult GetBytes(string GUID)
        {
            Document doc = repos3.GetDocument(GUID);
            byte[] mas = doc.Data;
            string type = "";
            for (int i = doc.Name.LastIndexOf('.'); i < doc.Name.Length; i++)
            {
                type += GUID[i];
            }
            string file_type = "application/"+type;
            string file_name = doc.Name ;
            return File(mas, file_type, file_name);
        }
        }
}
