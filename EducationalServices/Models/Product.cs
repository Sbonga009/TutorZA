using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EducationalServices.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        public string Weight { get; set; }
        public string Picture { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class Cart
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Weight { get; set; }
        public string Picture { get; set; }
        public string UserId { get; set; }
    }
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DueDate { get; set; } 
        [Column(TypeName = "datetime2")]
        public DateTime deliveryDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DeliveredOn { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime PickupDate { get; set; }
        public string Status { get; set; }
        public int Code { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal ProductCost { get; set; }
        public decimal TotalAmount { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string DriverEmail { get; set; }
        public int DeliveredBy { get; set; }
        public bool IsDeliveryRescheduled { get; set; }
        public string PaymentID { get; set; }

    }
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }
        public Orders Orders { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Weight { get; set; }
        public string Picture { get; set; }
        

    }
}