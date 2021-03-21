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
using FitnessSolution.Helpers;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.AspNetCore.Authorization;

namespace FitnessSolution.Views.Recipies
{
    public class RecipesController : BlobsController
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private CloudTable recipesTable, dietTable;

        public RecipesController(FitnessSolutionPlansContext context, IWebHostEnvironment hostEnvironment)
        {
            this._hostEnvironment = hostEnvironment;
            recipesTable = TableManager.GetStorageTableAsync(TableManager.RECIPES_TABLE).Result;
            dietTable = TableManager.GetStorageTableAsync(TableManager.DIETS_TABLE).Result;
        }

        // GET: Recipes
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var recipes = await RetrieveAllRecipes();
            recipes.ForEach(item => item.RecipeImageName = GetSingleBlob("recipe", item.RecipeImageName));

            return View(recipes);
        }

        // GET: Recipes/Details/5
        [Authorize]
        public async Task<IActionResult> Details(String id)
        {
            var recipe = await RetrieveRecipe(id);
            if (recipe == null)
            {
                return NotFound();
            }

            recipe.RecipeImageName = GetSingleBlob("recipe", recipe.RecipeImageName);

            return View(recipe);
        }

        // GET: Recipes/Create
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public IActionResult Create()
        {
            var diets = RetrieveAllDiets().Result;
            var selectLists = new SelectList(diets, "PartitionKey", "PartitionKey");
            ViewData["DietId"] = selectLists;

            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Create([Bind("RecipeTitle,RecipeDescription,Type,RecipeImageFile,PartitionKey")] RecipeEntity recipe)
        {
            if (ModelState.IsValid)
            {
                //Save image to wwwroot/image
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(recipe.RecipeImageFile.FileName);
                string extension = Path.GetExtension(recipe.RecipeImageFile.FileName);
                string content = recipe.RecipeImageFile.ContentType;
                recipe.RecipeImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/uploads/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await recipe.RecipeImageFile.CopyToAsync(fileStream);
                }

                bool result = SimpleUploadFile("recipe", recipe.RecipeImageName, path, content);

                //Insert record
                var recipeId = Guid.NewGuid().ToString();
                recipe.RowKey = recipeId;
                await InsertOrMergeRecipe(recipe);
                return RedirectToAction(nameof(Index));
            }
            //ViewData["DietId"] = new SelectList(_context.Diet, "DietId", "DietId", recipe.DietId);
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Edit(String id)
        {
            var recipe = await RetrieveRecipe(id);
            if (recipe == null)
            {
                return NotFound();
            }
            var diets = RetrieveAllDiets().Result;
            var selectLists = new SelectList(diets, "DietTitle", "PartitionKey", recipe.PartitionKey);
            ViewData["DietId"] = selectLists;

            recipe.RecipeImageName = GetSingleBlob("recipe", recipe.RecipeImageName);

            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Edit(String id, [Bind("RowKey,PartitionKey,RecipeTitle,RecipeDescription,Type,RecipeImageName")] RecipeEntity recipe)
        {
            if (id != recipe.PartitionKey)
            {
                return NotFound();
            }

            if (recipe.RecipeTitle != null && recipe.RecipeDescription != null && recipe.Type != null)
            {
                await InsertOrMergeRecipe(recipe);
                return RedirectToAction(nameof(Index));
            }

            var diets = RetrieveAllDiets().Result;
            var selectLists = new SelectList(diets, "PartitionKey", "PartitionKey", recipe.PartitionKey);
            ViewData["DietId"] = selectLists;
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Delete(String id)
        {
            var recipe = await RetrieveRecipe(id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Constants.ROLE_NUTRITIONIST, Constants.ROLE_ADMIN)]
        public async Task<IActionResult> DeleteConfirmed(String id)
        {
            var recipe = await RetrieveRecipe(id);
            await DeleteRecipe(recipe);
            return RedirectToAction(nameof(Index));
        }

        public async Task<List<DietEntity>> RetrieveAllDiets()
        {
            try
            {
                TableQuery<DietEntity> dietQuery = new TableQuery<DietEntity>();
                var diets = await dietTable.ExecuteQuerySegmentedAsync(dietQuery, null);
                return diets.Results;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task<List<RecipeEntity>> RetrieveAllRecipes()
        {
            try
            {
                TableQuery<RecipeEntity> recipeQuery = new TableQuery<RecipeEntity>();
                var recipes = await recipesTable.ExecuteQuerySegmentedAsync(recipeQuery, null);
                return recipes.Results;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task<RecipeEntity> RetrieveRecipe(string recipeId)
        {
            try
            {
                TableQuery<RecipeEntity> recipeQuery = new TableQuery<RecipeEntity>();
                TableQuery<DietEntity> dietQuery = new TableQuery<DietEntity>();

                string recipeFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, recipeId);
                recipeQuery = recipeQuery.Where(recipeFilter);
                var recipeTask = await recipesTable.ExecuteQuerySegmentedAsync(recipeQuery, null);
                var recipe = recipeTask.FirstOrDefault();

                string dietFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, recipe.PartitionKey);
                dietQuery = dietQuery.Where(dietFilter);
                var diet = dietTable.ExecuteQuerySegmentedAsync(dietQuery, null).Result.FirstOrDefault();
                diet.DietImageName = GetSingleBlob("diet", diet.DietImageName);
                recipe.DietEntity = diet;
                return recipe;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task<DietEntity> InsertOrMergeRecipe(RecipeEntity entity)
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
                TableResult result = await recipesTable.ExecuteAsync(insertOrMergeOperation);
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

        public async Task DeleteRecipe(RecipeEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                // Create the Delete table operation
                TableOperation deleteOperation = TableOperation.Delete(entity);
                // Execute the operation.
                TableResult result = await recipesTable.ExecuteAsync(deleteOperation);
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
