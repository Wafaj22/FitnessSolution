using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessSolution.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [Column(TypeName = "nvarchar(50)")]
        [DisplayName("Title")]
        public string RecipeTitle { get; set; }

        [Required]
        [StringLength(4000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [Column(TypeName = "nvarchar(4000)")]
        [DisplayName("Description")]
        public string RecipeDescription { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(24)")]
        public string Type { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image Name")]
        public string RecipeImageName { get; set; }

        [Required]
        [NotMapped]
        [DisplayName("Upload Image")]
        public IFormFile RecipeImageFile { get; set; }

        //Foreign key for Diet
        public int DietId { get; set; }
        public Diet Diet { get; set; }
    }
}
