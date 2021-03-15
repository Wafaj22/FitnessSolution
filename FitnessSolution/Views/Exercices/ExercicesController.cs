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

namespace FitnessSolution.Views.Exercices
{
    public class ExercicesController : Controller
    {
        private readonly FitnessSolutionPlansContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ExercicesController(FitnessSolutionPlansContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Exercices
        public async Task<IActionResult> Index()
        {
            var fitnessSolutionPlansContext = _context.Exercice.Include(e => e.Workout);
            return View(await fitnessSolutionPlansContext.ToListAsync());
        }

        // GET: Exercices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercice = await _context.Exercice
                .Include(e => e.Workout)
                .FirstOrDefaultAsync(m => m.ExerciceId == id);
            if (exercice == null)
            {
                return NotFound();
            }

            return View(exercice);
        }

        // GET: Exercices/Create
        public IActionResult Create()
        {
            ViewData["WorkoutId"] = new SelectList(_context.Set<Workout>(), "WorkoutId", "WorkoutId");
            return View();
        }

        // POST: Exercices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExerciceId,ExerciceTitle,ExerciceDescription,Level,ExerciceImageFile,WorkoutId")] Exercice exercice)
        {
            if (ModelState.IsValid)
            {
                //Save image to wwwroot/image
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(exercice.ExerciceImageFile.FileName);
                string extension = Path.GetExtension(exercice.ExerciceImageFile.FileName);
                exercice.ExerciceImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/uploads/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await exercice.ExerciceImageFile.CopyToAsync(fileStream);
                }
                //Insert record
                _context.Add(exercice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkoutId"] = new SelectList(_context.Set<Workout>(), "WorkoutId", "WorkoutId", exercice.WorkoutId);
            return View(exercice);
        }

        // GET: Exercices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercice = await _context.Exercice.FindAsync(id);
            if (exercice == null)
            {
                return NotFound();
            }
            ViewData["WorkoutId"] = new SelectList(_context.Set<Workout>(), "WorkoutId", "WorkoutId", exercice.WorkoutId);
            return View(exercice);
        }

        // POST: Exercices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExerciceId,ExerciceTitle,ExerciceDescription,Level,ExerciceImageName,WorkoutId")] Exercice exercice)
        {
            if (id != exercice.ExerciceId)
            {
                return NotFound();
            }

            var itemToUpdate = await _context.Exercice.FirstOrDefaultAsync(s => s.ExerciceId == id);
            if (await TryUpdateModelAsync<Exercice>(itemToUpdate,"",s => s.ExerciceTitle, s => s.ExerciceDescription, s => s.Level))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    ModelState.AddModelError("", "Unable to save changes. " + "Try again, and if the problem persists, " + "see your system administrator.");
                }
            }
            ViewData["WorkoutId"] = new SelectList(_context.Set<Workout>(), "WorkoutId", "WorkoutId", exercice.WorkoutId);
            return View(exercice);
        }

        // GET: Exercices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercice = await _context.Exercice
                .Include(e => e.Workout)
                .FirstOrDefaultAsync(m => m.ExerciceId == id);
            if (exercice == null)
            {
                return NotFound();
            }

            return View(exercice);
        }

        // POST: Exercices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercice = await _context.Exercice.FindAsync(id);
            _context.Exercice.Remove(exercice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciceExists(int id)
        {
            return _context.Exercice.Any(e => e.ExerciceId == id);
        }
    }
}
