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
using Microsoft.WindowsAzure.Storage.Table;
using FitnessSolution.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.AspNetCore.Authorization;

namespace FitnessSolution.Views.Diets
{
    public class DietsController : BlobsController
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        private CloudTable recipesTable, dietTable;

        public DietsController(FitnessSolutionPlansContext context, IWebHostEnvironment hostEnvironment)
        {
            this._hostEnvironment = hostEnvironment;
            recipesTable = TableManager.GetStorageTableAsync(TableManager.RECIPES_TABLE).Result;
            dietTable = TableManager.GetStorageTableAsync(TableManager.DIETS_TABLE).Result;
        }

        // GET: Diets
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var dietList = await RetrieveAllDiets();
            dietList.ForEach(item => item.DietImageName = GetSingleBlob("diet", item.DietImageName));
            return View(dietList);
        }

        // GET: Diets/Details/5
        [Authorize]
        public async Task<IActionResult> Details(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diet = await RetrieveDiet(id);
            if (diet == null)
            {
                return NotFound();
            }
            return View(diet);
        }

        // GET: Diets/Create
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Diets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Create([Bind("DietTitle,DietImageFile,DietDescription,Type")] DietEntity diet)
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

                var dietId = Guid.NewGuid().ToString();
                diet.PartitionKey = dietId;
                diet.RowKey = dietId;
                await InsertOrMergeDiet(diet);

                System.Diagnostics.Debug.WriteLine(diet.RowKey);
                //return RedirectToAction(nameof(Index));
                Notification nt = new Notification()
                {
                    Plan = diet.DietTitle,
                    Specification = diet.Type,
                    Type = "Diet",
                    Id = diet.PartitionKey,
                };

                await new ServicebusController().AddNotification(nt);
                return RedirectToAction("Create", "Recipes");
            }
            return View(diet);
        }

        // GET: Diets/Edit/5
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Edit(String id)
        {
            var diet = await RetrieveDiet(id);
            if (diet == null)
            {
                return NotFound();
            }

            return View(diet);
        }

        // POST: Diets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Edit(String id, [Bind("PartitionKey,RowKey,DietTitle,DietDescription,Type,DietImageName")] DietEntity diet)
        {
            if (id != diet.PartitionKey)
            {
                return NotFound();
            }

            if (diet.DietTitle != null && diet.DietDescription != null && diet.Type != null)
            {
                await InsertOrMergeDiet(diet);
                return RedirectToAction(nameof(Index));
            }
            return View(diet);
        }

        // GET: Diets/Delete/5
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Delete(String id)
        {
            var diet = await RetrieveDiet(id);
            if (diet == null)
            {
                return NotFound();
            }

            return View(diet);
        }

        // POST: Diets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> DeleteConfirmed(String id)
        {
            var diet = await RetrieveDiet(id);
            await DeleteDiet(diet);
            return RedirectToAction(nameof(Index));
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

        public async Task<DietEntity> RetrieveDiet(string dietId)
        {
            try
            {
                TableQuery<RecipeEntity> recipeQuery = new TableQuery<RecipeEntity>();
                TableQuery<DietEntity> dietQuery = new TableQuery<DietEntity>();

                string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, dietId);
                recipeQuery = recipeQuery.Where(filter);
                dietQuery = dietQuery.Where(filter);

                var recipesTask = await recipesTable.ExecuteQuerySegmentedAsync(recipeQuery, null);
              
                var diet = dietTable.ExecuteQuerySegmentedAsync(dietQuery, null).Result.FirstOrDefault();
                var recipes = recipesTask.Results;
                recipes.ForEach(item => item.RecipeImageName = GetSingleBlob("recipe", item.RecipeImageName));

                diet.Recipes = recipes;
                diet.DietImageName = GetSingleBlob("diet", diet.DietImageName);

                return diet;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task<DietEntity> InsertOrMergeDiet(DietEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await dietTable.ExecuteAsync(insertOrMergeOperation);
                DietEntity insertedDiet = result.Result as DietEntity;

                return insertedDiet;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task DeleteDiet(DietEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                var batchOperation = new TableBatchOperation();
                var deleteRecipesQuery = new TableQuery<RecipeEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, entity.RowKey))
                        .Select(new string[] { "RowKey" });
                foreach (var e in await recipesTable.ExecuteQuerySegmentedAsync(deleteRecipesQuery, null))
                    batchOperation.Delete(e);

                await recipesTable.ExecuteBatchAsync(batchOperation);

                // Create the Delete table operation
                TableOperation deleteOperation = TableOperation.Delete(entity);
                // Execute the operation.
                TableResult result = await dietTable.ExecuteAsync(deleteOperation);
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
