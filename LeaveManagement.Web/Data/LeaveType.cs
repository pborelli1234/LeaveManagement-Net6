using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Web.Data
{
    public class LeaveType //: BaseEntity
    {
        public int Id { get; set; }
        [Display(Name = "Leave Type Name")]
        public string Name { get; set; }

        [Display(Name = "Default Number of Days")]
        public int DefaultDays { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
