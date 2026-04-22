using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mobile.Account;
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
        public DbSet<CarrierCompany> CarrierCompany { get; set; }
        public DbSet<CarrierLocation> CarrierLocation { get; set; }
        public DbSet<CarrierProgramCode> CarrierProgramCode { get; set; }

        public DbSet<Log> Log { get; set; }

        public DbSet<Hertz_Fleet_US> Hertz_Fleet_US { get; set; }
        public DbSet<Hertz_Fleet_Canada> Hertz_Fleet_Canada { get; set; }

        public DbSet<HertzRentalPhoto> HertzRentalPhoto { get; set; }
        public DbSet<HertzRentalPhoto_Attachment> HertzRentalPhoto_Attachment { get; set; }

        public DbSet<PhotosExpress> PhotosExpress { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<User_AssociationMatchup> User_AssociationMatchup { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Entity<Hertz_Fleet_US>().HasNoKey();

        //    //modelBuilder.Entity<Hertz_Fleet_Canada>().HasNoKey();
        //}
    }
}
