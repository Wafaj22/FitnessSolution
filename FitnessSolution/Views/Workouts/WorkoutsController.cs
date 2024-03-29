﻿using System;
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
using Microsoft.AspNetCore.Authorization;
using FitnessSolution.Helpers;

namespace FitnessSolution.Views.Workouts
{
    public class WorkoutsController : BlobsController
    {
        private readonly FitnessSolutionPlansContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public WorkoutsController(FitnessSolutionPlansContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Workouts
        [Authorize]
        public async Task<IActionResult> Index()
        {
            _context.Workout.ForEachAsync(item => item.WorkoutImageName = GetSingleBlob("workout", item.WorkoutImageName)).Wait();
            return View(await _context.Workout.ToListAsync());
        }

        // GET: Workouts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workout
                .Include(s => s.Exercices)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.WorkoutId == id);
            if (workout == null)
            {
                return NotFound();
            }

            workout.WorkoutImageName = GetSingleBlob("workout", workout.WorkoutImageName);
            return View(workout);
        }

        // GET: Workouts/Create
        [AuthorizeRoles(Constants.ROLE_TRAINER, Constants.ROLE_ADMIN)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Constants.ROLE_TRAINER, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Create([Bind("WorkoutId,WorkoutTitle,WorkoutDescription,Type,WorkoutImageFile")] Workout workout)
        {
            if (ModelState.IsValid)
            {
                //Save image to wwwroot/image
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(workout.WorkoutImageFile.FileName);
                string extension = Path.GetExtension(workout.WorkoutImageFile.FileName);
                string content = workout.WorkoutImageFile.ContentType;
                workout.WorkoutImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/uploads/", fileName);
              
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await workout.WorkoutImageFile.CopyToAsync(fileStream);
                }

                SimpleUploadFile("workout", workout.WorkoutImageName, path, content);

                //Insert record
                _context.Add(workout);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                Notification nt = new Notification()
                {
                    Plan = workout.WorkoutTitle,
                    Specification = workout.Type,
                    Type = "Workout",
                    Id = workout.WorkoutId.ToString(),
                };

                await new ServicebusController().AddNotification(nt);
                return RedirectToAction("Create", "Exercices");
            }
            return View(workout);
        }

        // GET: Workouts/Edit/5
        [AuthorizeRoles(Constants.ROLE_TRAINER, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workout.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }
            workout.WorkoutImageName = GetSingleBlob("workout", workout.WorkoutImageName);

            return View(workout);
        }

        // POST: Workouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Constants.ROLE_TRAINER, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Edit(int id, [Bind("WorkoutId,WorkoutTitle,WorkoutDescription,Type,WorkoutImageName")] Workout workout)
        {
            if (id != workout.WorkoutId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutExists(workout.WorkoutId))
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
            return View(workout);
        }

        // GET: Workouts/Delete/5
        [AuthorizeRoles(Constants.ROLE_TRAINER, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workout
                .FirstOrDefaultAsync(m => m.WorkoutId == id);
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Constants.ROLE_TRAINER, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workout = await _context.Workout.FindAsync(id);
            _context.Workout.Remove(workout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutExists(int id)
        {
            return _context.Workout.Any(e => e.WorkoutId == id);
        }
    }
}
