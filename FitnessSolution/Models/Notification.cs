using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FitnessSolution.Models
{
    public class Notification
    {
        [Display(Name="Plan Title")]        
        public string Plan { get; set; }

        [Display(Name = "Plan")]        
        public string Type { get; set; }

        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Type")]
        public string Specification { get; set; }
    }
}
