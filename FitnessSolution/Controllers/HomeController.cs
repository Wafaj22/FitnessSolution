using FitnessSolution.Data;
using FitnessSolution.Helpers;
using FitnessSolution.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessSolution.Controllers
{
    public class HomeController : BlobsController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FitnessSolutionPlansContext _context;

        private CloudTable dietTable;

        public HomeController(FitnessSolutionPlansContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
            dietTable = TableManager.GetStorageTableAsync(TableManager.DIETS_TABLE).Result;
        }

        public async Task<IActionResult> Index()
        {
            dynamic mymodel = new ExpandoObject();

            var diets = RetrieveAllDiets().Result;
            diets.ForEach(item => item.DietImageName = GetSingleBlob("diet", item.DietImageName));

            _context.Workout.ForEachAsync(item => item.WorkoutImageName = GetSingleBlob("workout", item.WorkoutImageName)).Wait();

            mymodel.Workouts = await _context.Workout.ToListAsync();
            mymodel.Diets = diets;
            return View(mymodel);
            //return View();
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

        public async Task<List<DietEntity>> RetrieveAllDiets()
        {
            try
            {
                TableQuery<DietEntity> recipeQuery = new TableQuery<DietEntity>();
                var recipes = await dietTable.ExecuteQuerySegmentedAsync(recipeQuery, null);
                return recipes.Results;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
