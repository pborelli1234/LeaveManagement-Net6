using LeaveManagement.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Web.Configurations.Entities
{
    public class UserSeedConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            var hasher = new PasswordHasher<Employee>();

            builder.HasData(
                new Employee
                {
                    Id = "314fca64-98e6-47aa-837b-e3d8c0dbfe83",
                    Email = "penny.borelli@multichoice.co.za",
                    NormalizedEmail = "PENNY.BORELLI@MULTICHOICE.CO.ZA",
                    Firstname = "System",
                    Lastname = "Admin",
                    PasswordHash = hasher.HashPassword(null, "Pr0v1dence")
                },
                new Employee
                {
                    Id = "814fca94-98e6-47aa-4402-e3d8c0ebfe83",
                    Email = "user@localhost.com",
                    NormalizedEmail = "USER@LOCALHOST.COM",
                    Firstname = "System",
                    Lastname = "User",
                    PasswordHash = hasher.HashPassword(null, "Pr0v1dence")
                }
            ); 
        }
    }
}