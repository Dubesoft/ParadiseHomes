using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParadiesHome.Data;
using ParadiesHome.Models;

namespace ParadiesHome.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        public readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> userManager;

        public BlogController(ApplicationDbContext db,
                              UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await db.Post.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Post - create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    post.Picture = p1;
                }

                var user = await userManager.GetUserAsync(User);
                post.UserId = user.Id;
                post.PosterEmail = user.Email;
                post.DateCreated = DateTime.UtcNow;
                db.Post.Add(post);
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(post);
        }

        //Get - Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await db.Post.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }


        //Post - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Post post)
        {
            if (post.PostId == 0)
            {
                return NotFound();
            }

            var postFromDb = await db.Post.Where(p => p.PostId == post.PostId).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    postFromDb.Picture = p1;
                }
                postFromDb.Title = post.Title;
                postFromDb.Description = post.Description;
                postFromDb.DateCreated = DateTime.UtcNow;

                //_db.Gallery.Update(gallery);
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(post);
        }

        //Get - Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await db.Post.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        //Post - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var post = await db.Post.SingleOrDefaultAsync(p => p.PostId == id);
            db.Post.Remove(post);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Get - Detail
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await db.Post.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
    }
}