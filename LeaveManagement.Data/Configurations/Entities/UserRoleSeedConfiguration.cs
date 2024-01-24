using LeaveManagement.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Data.Configurations.Entities
{
    public class UserRoleSeedConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "6911ADA1-2DE3-4E2C-8084-CA408DA6D60B",
                    UserId  = "314fca64-98e6-47aa-837b-e3d8c0dbfe83"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "FFB2BF7E-B323-4A5B-AC7C-0A88B8007B6C",
                    UserId = "814fca94-98e6-47aa-4402-e3d8c0ebfe83"
                }
            );
        }
    }
}