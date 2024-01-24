using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Data
{
    public class LeaveType : BaseEntity
    {
        public string Name { get; set; }
        public int DefaultDays { get; set; }
    }
}
