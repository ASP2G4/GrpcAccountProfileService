using GrpcAccountProfileService.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrpcAccountProfileService.Contexts
{
    public class AccountProfileContext(DbContextOptions<AccountProfileContext> options) : DbContext(options)
    {
        public DbSet<AccountProfile> AccountProfiles { get; set; }
        public DbSet<AccountProfileAddress> AccountProfileAddresses { get; set; }
    }
}
