using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace LeaveManagement.Web.Data
{
    public class LeaveAllocation //: BaseEntity
    {
        public int Id { get; set; }
          
        [ForeignKey("LeaveTypeId")]
        public LeaveType LeaveType { get; set; }
        public int LeaveTypeId { get; set; }

        public int EmployeeId { get; set; }

        public int NumberOfDays { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

    }
}
