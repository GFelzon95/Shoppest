using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shoppest.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }
		public string ApplicationUserId { get; set; }
		[ForeignKey(nameof(ApplicationUserId))]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }

		public DateTime OrderDate { get; set; }
		public DateTime ShippingDate { get; set; }
		public double OrderTotal { get; set; }

		public string? OrderStatus { get; set; }
		public string? PaymentStatus { get; set; }
		public string? TrackingNumber { get; set; }
		public string? Carrier { get; set; }

		public DateTime PaymentDate { get; set; }
		public DateOnly PaymentDueDate { get; set; }

		public string? PaymentIntentId { get; set; }

		[Required]
		public string Name { get; set; }
		[Required]
		public string PhoneNumber { get; set; }
		[Required]
		public string Region { get; set; }
		[Required]
		public string Province { get; set; }
		[Required]
		public string City { get; set; }
		[Required]
		public string Barangay { get; set; }
		[Required]
		public string StreetAddress { get; set; }
		[Required]
		public string PostalCode { get; set; }
	}
}
