
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Dinner> Dinners { get; set; }

        public virtual DbSet<Rsvp> Rsvp { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
            //Database.EnsureCreatedAsync().Wait();
        }

       
     

    }
}
