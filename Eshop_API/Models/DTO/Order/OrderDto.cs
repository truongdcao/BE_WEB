using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshop_API.Helpers.Orders;

namespace eshop_api.Models.DTO.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; } // TNXREF
        public string Status { get; set; }
        public double Total { get; set; }
        public string Note { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime CheckedAt { get; set; }
        public string CheckedBy { get; set; }
        public string CheckedComment { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public int? AddressId { get; set; }
        public int DeliveryTime { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentURL{get;set;}
    }
}