using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EducationalServices.Models
{
    public class Module
    {
        [Key]
        public int ModuleId { get; set; }
        public string Subject { get; set; }
        public string SubjectCode { get; set; }
        public string Description { get; set; }     
        public int DurationInHours { get; set; }
        public string Difficulty { get; set; }
        public decimal Price { get; set; }
    }
    public class TutorModule
    {
        [Key]
        public int TutorModuleId { get; set; }
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
        [ForeignKey("User")]
        public string TutorId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
    public class StudentModule
    {
        [Key]
        public int StudentModuleId { get; set; }
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
        [ForeignKey("User")]
        public string StudentId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string Status { get; set; }
        public decimal TotalPaid { get; set; }
    }
}