using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace LeaveManagement.Data
{
    public class LeaveAllocation : BaseEntity
    {          
        [ForeignKey("LeaveTypeId")]
        public LeaveType LeaveType { get; set; }
        public int LeaveTypeId { get; set; }

        public string EmployeeId { get; set; }

        public int NumberOfDays { get; set; }
        public int Period { get; set; }

    }
}
