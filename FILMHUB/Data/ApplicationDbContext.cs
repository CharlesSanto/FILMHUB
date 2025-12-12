using Microsoft.EntityFrameworkCore;
using FILMHUB.Models;

namespace FILMHUB.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserMovie> UserMovies { get; set; }
}