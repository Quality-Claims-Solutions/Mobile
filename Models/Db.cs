using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mobile.Account;
using Mobile.Data;
using Mobile.Models.EntityModels;
using SampleMVC.Models.EntityModels;

namespace Mobile.Models
{
    public class Db : IdentityDbContext<ApplicationUser>
    {
        public Db(DbContextOptions<Db> options)
            : base(options)
        {
        }

        // Entity Models
        public DbSet<PhotosExpress> PhotosExpress { get; set; }
        public DbSet<User> User { get; set; }

    }
}
