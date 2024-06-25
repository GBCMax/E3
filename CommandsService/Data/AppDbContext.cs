using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> _options) : base(_options)
        {
            
        }

        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Platform>()
                .HasMany(_p => _p.Commands)
                .WithOne(_p => _p.Platform)
                .HasForeignKey(_p => _p.PlatformId);

            modelBuilder
                .Entity<Command>()
                .HasOne(_p => _p.Platform)
                .WithMany(_p => _p.Commands)
                .HasForeignKey(_p => _p.PlatformId);
        }
    }
}