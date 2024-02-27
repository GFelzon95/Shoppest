using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Shoppest.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        public string? StreetAddress { get; set; }
        public string? Barangay { get; set; }
        public string? Province { get; set; }
        public string? Region { get; set; }
        [MaxLength(5)]
        public string? PostalCode { get; set; }

    }
}
