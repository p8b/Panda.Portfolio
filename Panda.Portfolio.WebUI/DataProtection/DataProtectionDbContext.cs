using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Panda.Portfolio.WebUI.DataProtection
{
    public class DataProtectionDbContext : DbContext, IDataProtectionKeyContext
    {
        public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

        public DataProtectionDbContext(DbContextOptions<DataProtectionDbContext> options)
            : base(options)
        {
        }
    }
}