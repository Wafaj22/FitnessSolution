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
    public class Exercice
    {
        [Key]        
        public int ExerciceId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [Column(TypeName = "nvarchar(50)")]
        [DisplayName("Title")]
        public string ExerciceTitle { get; set; }

        [Required]
        [StringLength(4000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [Column(TypeName = "nvarchar(4000)")]
        [DisplayName("Description")]
        public string ExerciceDescription { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(24)")]
        public string Level { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image Name")]
        public string ExerciceImageName { get; set; }

        [Required]
        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile ExerciceImageFile { get; set; }

        //Foreign key for Workout
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }
    }
}
