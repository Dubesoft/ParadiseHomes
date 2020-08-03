using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParadiesHome.Data;
using ParadiesHome.Models;

namespace ParadiesHome.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BlogController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        public BlogController(ApplicationDbContext db,
                              UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int Id)
        {
            List<Comments> comments = new List<Comments>();

            comments = await _db.Comments.Where(p => p.PostId == Id).ToListAsync();
            return PartialView(comments);
            //}
        }


        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] Comments model)
        {
            var UserId = "";
            var Email = "";
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                UserId = "0";
                Email = "Anonymous";
            }
            else
            {
                UserId = user.Id;
                Email = user.Email;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    CommentCount CoummentCountFromDb = await _db.CommentCount.Where(c => c.PostId == model.PostId).FirstOrDefaultAsync();

                    if (CoummentCountFromDb == null)
                    {
                        var commentCountModel = new CommentCount
                        {
                            UserId = UserId,
                            PostId = model.PostId
                        };

                        _db.CommentCount.Add(commentCountModel);
                    }
                    else
                    {
                        CoummentCountFromDb.Count = CoummentCountFromDb.Count + 1;
                    }

                    model.DateCreated = DateTime.UtcNow;
                    model.UserId = UserId;
                    model.Email = Email;
                    _db.Comments.Add(model);
                    await _db.SaveChangesAsync();

                }
                return Json(new { status = "ok" });
            }
            catch (Exception e)
            {

                return Json(new { status = e.Message });
            }
        }

        public async Task<IActionResult> AddLike([FromBody] Comments model)
        {
            var UserId = "";
            var Email = "";
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                UserId = "0";
                Email = "Anonymous";
            }
            else
            {
                UserId = user.Id;
                Email = user.Email;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    LikeCount LikeCountFromDb = await _db.LikeCount.Where(l => l.PostId == model.PostId).FirstOrDefaultAsync();
                    if (LikeCountFromDb == null)
                    {
                        var LikeCountModel = new LikeCount()
                        {
                            UserId = UserId,
                            PostId = model.PostId
                        };
                        _db.LikeCount.Add(LikeCountModel);
                    }
                    else
                    {
                        LikeCountFromDb.Count = LikeCountFromDb.Count + 1;
                    }
                    await _db.SaveChangesAsync();
                }
                return Json(new { status = "ok" });
            }
            catch (Exception e)
            {
                return Json(new { status = e.Message });
            }
        }

        
    }
}