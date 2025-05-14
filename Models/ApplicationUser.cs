using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SmartInventorySystem.Models
{
    /// <summary>
    /// Application user that extends the base IdentityUser
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Additional user properties can be added here
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Display(Name = "Department")]
        public string? Department { get; set; }

        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [Display(Name = "Employee ID")]
        public string? EmployeeId { get; set; }

        [Display(Name = "Profile Picture")]
        public string? ProfilePicturePath { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLoginDate { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}