using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FitnessSolution.Models;

namespace FitnessSolution.Data
{
    public class FitnessSolutionPlansContext : DbContext
    {
        public FitnessSolutionPlansContext (DbContextOptions<FitnessSolutionPlansContext> options)
            : base(options)
        {
        }

        public DbSet<FitnessSolution.Models.Diet> Diet { get; set; }

        public DbSet<FitnessSolution.Models.Exercice> Exercice { get; set; }

        public DbSet<FitnessSolution.Models.Recipe> Recipe { get; set; }

        public DbSet<FitnessSolution.Models.Workout> Workout { get; set; }
    }
}
