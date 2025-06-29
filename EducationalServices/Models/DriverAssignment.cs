using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;
using EducationalServices.Models;

namespace EducationalServices.Models
{
    public class DriverAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssDrivId { get; set; }
        [DisplayName("Order Number")]
        public int OrderId { get; set; }
        public int DrivId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }

        [DataType(DataType.Date)]
        public string DeliveryDate { get; set; }
        [DataType(DataType.Time)]
        public string DeliveryTime { get; set; }
        public DateTime Created { get; set; }
        public bool IsActive { get; set; }
        [DisplayName("Due on")]
        public string GenDeliveryDate { get; set; }
        [DisplayName("Selected Time")]
        public string preffaredTime { get; set; }
        [DisplayName("Delivery Address")]
        public string DeliveryAddress { get; set; }
        [ForeignKey("OrderId")]
        public virtual Orders Orders { get; set; }
        [ForeignKey("DrivId")]
        public virtual Driver Driver { get; set; }
    }
}