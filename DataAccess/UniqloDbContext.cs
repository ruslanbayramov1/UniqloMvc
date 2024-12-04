using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.Models;

namespace UniqloMvc.DataAccess
{
    public class UniqloDbContext : IdentityDbContext<User>
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public UniqloDbContext(DbContextOptions<UniqloDbContext> opt) : base(opt)
        {
            
        }
    }
}
