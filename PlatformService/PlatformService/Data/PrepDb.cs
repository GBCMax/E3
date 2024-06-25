using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
	public static class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder _app, bool _isProd)
		{
			using (var serviceScope = _app.ApplicationServices.CreateScope())
			{
				SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), _isProd);
			}
		}
		private static void SeedData(AppDbContext _context, bool _isProd)
		{
			if(_isProd)
			{
				System.Console.WriteLine("--> Attemping to apply migrations...");
				try
				{
					_context.Database.Migrate();
				}
				catch(Exception ex)
				{
					System.Console.WriteLine($"Could not run migrations: {ex.Message}");
				}
			}

			if (!_context.Platforms.Any())
			{
				Console.WriteLine("--> Seeding data...");

				_context.Platforms.AddRange(
					new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free"},
					new Platform() { Name = "Sql Server Express", Publisher = "Microsoft", Cost = "Free" },
					new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
				);

				_context.SaveChanges();
			}
			else
			{
				Console.WriteLine("--> We already have data");
			}
		}
	}
}
