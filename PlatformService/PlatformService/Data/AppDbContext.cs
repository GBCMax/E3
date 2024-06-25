using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> _options) : base(_options)
		{

		}
		public DbSet<Platform> Platforms { get; set; }
	}
}
