using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Web.Models
{
    public class LeaveTypeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Leave Type Name")]
        [Required(ErrorMessage = "Leave Type Name is required.")]
        public string Name { get; set; }

        [Display(Name = "Default Number of Days")]
        [Range(1, 150, ErrorMessage = "Please enter a valid number.")]
        [Required(ErrorMessage = "Default Number of Days is required.")]
        public int DefaultDays { get; set; }

        [Display(Name = "Date Created")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Date Modified")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime DateModified { get; set; }
    }
}
