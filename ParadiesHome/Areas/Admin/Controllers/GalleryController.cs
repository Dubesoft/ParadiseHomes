using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParadiesHome.Data;
using ParadiesHome.Models;

namespace ParadiesHome.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GalleryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GalleryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Get
        public async Task<IActionResult> Index()
        {
            return View(await _db.Gallery.ToListAsync());
        }

        //Get - Create
        public IActionResult Create()
        {
            return View();
        }

        //Post - create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Gallery gallery)
        {
            if(ModelState.IsValid)
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
                    gallery.Picture = p1;
                }

                gallery.DateCreated = DateTime.UtcNow;
                gallery.IsActive = true;
                _db.Gallery.Add(gallery);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(gallery);
        }

        //Get - Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var gallery = await _db.Gallery.FindAsync(id);

            if(gallery == null)
            {
                return NotFound();
            }

            return View(gallery);
        }

        //Post - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Gallery gallery)
        {
            if(gallery.Id == 0)
            {
                return NotFound();
            }

            var galleryFromDb = await _db.Gallery.Where(c => c.Id == gallery.Id).FirstOrDefaultAsync();

            if(ModelState.IsValid)
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
                    galleryFromDb.Picture = p1;
                }
                galleryFromDb.Name = gallery.Name;
                galleryFromDb.Description = gallery.Description;
                galleryFromDb.IsActive = gallery.IsActive;
                galleryFromDb.DateCreated = DateTime.UtcNow;

                //_db.Gallery.Update(gallery);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(gallery);
        }

        //Get - Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var gallery = await _db.Gallery.FindAsync(id);

            if(gallery == null)
            {
                return NotFound();
            }

            return View(gallery);
        }

        //Post - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var gallery = await _db.Gallery.SingleOrDefaultAsync(c => c.Id == id);
            _db.Gallery.Remove(gallery);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Get - Detail
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var gallery = await _db.Gallery.FindAsync(id);

            if(gallery == null)
            {
                return NotFound();
            }

            return View(gallery);
        }
    }
}