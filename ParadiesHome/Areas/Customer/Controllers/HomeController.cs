using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ParadiesHome.Data;
using ParadiesHome.Models;
using ParadiesHome.Models.ViewModels;

namespace ParadiesHome.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        public HomeController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            //await _emailSender.SendEmailAsync()
            return View();
        }

        //POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(Contact contact)
        {
            await _emailSender.SendEmailAsync(contact.Email, contact.Subject, contact.Message);

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Gallery()
        {
            IndexViewModel indexVM = new IndexViewModel()
            {
                Gallery = await _db.Gallery.OrderByDescending(c => c.Id).Take(9).ToListAsync()
            };
            
            return View(indexVM);
        }

        [HttpGet]
        public async Task<IActionResult> Blog()
        {

            var postList = await _db.Post.ToListAsync();
                //CommentCount = await _db.CommentCount.ToListAsync()
            //var postList = await _db.Post.ToListAsync();
            for (int i = 0; i < postList.Count(); i++)
            {
                var commentCount = await _db.CommentCount.Where(c => c.PostId == postList[i].PostId).ToListAsync();
                var likeCount = await _db.LikeCount.Where(l => l.PostId == postList[i].PostId).ToListAsync();
                foreach (var item in commentCount)
                {
                    HttpContext.Session.SetInt32("cmtCount" + i, item.Count);
                }

                foreach (var item in likeCount)
                {
                    HttpContext.Session.SetInt32("likesCount" + i, item.Count);
                }
            }
            return View(postList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
