using Microsoft.EntityFrameworkCore;
using UniqloMvc.Models;

namespace UniqloMvc.DataAccess
{
    public class UniqloDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public UniqloDbContext(DbContextOptions<UniqloDbContext> opt) : base(opt)
        {
            
        }
    }
}
