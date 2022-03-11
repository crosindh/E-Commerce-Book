using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Book.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name ="Order Date")]
        public DateTime OrderDate { get; set; }
        
        [Display(Name = "Shipping Date")]
        public DateTime? ShippingDate { get; set; }
       
        [Display(Name = "Order Total")]
        public double OrderTotal { get; set; }
       
        [Display(Name = "Order Status")]
        public string? OrderStatus { get; set; }
       
        [Display(Name = "Order Payment")]
        public string? PaymentStatus { get; set; }
       
        [Display(Name = "Tracking Number")]
        public string? TrackingNumber { get; set; }
        
        public string? Carrier { get; set; }

        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Payment Due Date")]
        public DateTime PaymentDueDate { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        [Required]
        public string Name { get; set; }
       
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
       
        [Required]
        [Display(Name = "Street Address")]
        public string? StreetAddress { get; set; }
        
        [Required]
        public string? City { get; set; }
        
        [Required]
        public string? State { get; set; }
        
        [Required]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }




    }
}
