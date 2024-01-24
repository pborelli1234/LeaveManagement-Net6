using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Common.Models
{
    public class LeaveAllocationVM
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Number of Days")]
        [Required]
        [Range(1, 50, ErrorMessage = "Invalid number entered")]
        public int NumberOfDays { get; set; }

        [Display(Name = "Allocation Period")]
        [Required]
        public int Period { get; set; }

        public LeaveTypeViewModel? LeaveType { get; set; }
    }
}
