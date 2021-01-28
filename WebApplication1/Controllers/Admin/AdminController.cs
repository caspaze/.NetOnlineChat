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
using Newtonsoft.Json;
using OnlineChat.Models;
using log4net;

namespace OnlineChat.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        UserDAO repos;
        [TempData]
        public string Users { get; set; }
        LogFactory logFactory;
        private ILog log;
        public AdminController(UserDAO _repos, LogFactory _logFactory)
        {
            repos = _repos;
            logFactory = _logFactory;
            log = logFactory.GetLogger();
        }
        public IActionResult AdminPanel()
        {
            if (Users == null)
                ViewBag.Users = repos.GetAllUsersExceptYourself(this.User.FindFirstValue(ClaimTypes.Name));
            else
            {
                ViewBag.Users = JsonConvert.DeserializeObject<List<User>>(Users); ;
            }
            return View();
        }
        public async Task<IActionResult> AddAdmin(CreateModel model)
        {
            repos.AddAdmin(model.Name);
            log.Info("New Admin: " + model.Name + " " + model.Email);
            return  RedirectToAction("AdminPanel","Admin");
        }
        public async Task<IActionResult> RemoveAdmin(CreateModel model)
        {
            repos.RemoveAdmin(model.Name);
            log.Info("RemovedAdmin: " + model.Name + " " + model.Email);
            return RedirectToAction("AdminPanel", "Admin");
        }

        public async Task<IActionResult> Search(CreateModel model)
        {
            if(model.Text!=null)
          Users=JsonConvert.SerializeObject(repos.SearchUsers(model.Text, this.User.FindFirstValue(ClaimTypes.Name)));
            return RedirectToAction("AdminPanel", "Admin");
        }
    }
}
