using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessSolution.Data;
using FitnessSolution.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using FitnessSolution.Controllers;

namespace FitnessSolution.Views.Diets
{
    public class DietsController : BlobsController
    {
        private readonly FitnessSolutionPlansContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DietsController(FitnessSolutionPlansContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Diets
        public async Task<IActionResult> Index()
        {
            _context.Diet.ForEachAsync(item => item.DietImageName = GetSingleBlob("diet", item.DietImageName)).Wait();
            return View(await _context.Diet.ToListAsync());
        }

        // GET: Diets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diet = await _context.Diet
                .Include(s => s.Recipes)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DietId == id);

            if (diet == null)
            {
                return NotFound();
            }
            diet.DietImageName = GetSingleBlob("diet", diet.DietImageName);
            return View(diet);
        }

        // GET: Diets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Diets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DietId,DietTitle,DietImageFile,DietDescription,Type")] Diet diet)
        {
            if (ModelState.IsValid)
            {
                //Save image to wwwroot/image
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(diet.DietImageFile.FileName);
                string extension = Path.GetExtension(diet.DietImageFile.FileName);
                string content = diet.DietImageFile.ContentType;
                diet.DietImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/uploads/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await diet.DietImageFile.CopyToAsync(fileStream);
                }
                
                SimpleUploadFile("diet", diet.DietImageName, path, content);

                //Insert record
                _context.Add(diet);
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine(diet.DietId);
                //return RedirectToAction(nameof(Index));
                Notification nt = new Notification()
                {
                    Plan = diet.DietTitle,
                    Specification = diet.Type,
                    Type = "Diet",
                    Id = diet.DietId,
                };

                await new ServicebusController().AddNotification(nt);
                return RedirectToAction("Create", "Recipes");
            }
            return View(diet);
        }

        // GET: Diets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diet = await _context.Diet.FindAsync(id);
            if (diet == null)
            {
                return NotFound();
            }
            diet.DietImageName = GetSingleBlob("diet", diet.DietImageName);

            return View(diet);
        }

        // POST: Diets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DietId,DietTitle,DietDescription,Type,DietImageName")] Diet diet)
        {
            if (id != diet.DietId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DietExists(diet.DietId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(diet);
        }

        // GET: Diets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diet = await _context.Diet
                .FirstOrDefaultAsync(m => m.DietId == id);
            if (diet == null)
            {
                return NotFound();
            }

            return View(diet);
        }

        // POST: Diets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diet = await _context.Diet.FindAsync(id);
            _context.Diet.Remove(diet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DietExists(int id)
        {
            return _context.Diet.Any(e => e.DietId == id);
        }
    }
}
