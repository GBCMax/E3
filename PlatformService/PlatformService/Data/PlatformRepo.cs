using PlatformService.Models;

namespace PlatformService.Data
{
	public class PlatformRepo : IPlatformRepo
	{
		private readonly AppDbContext p_context;
		public PlatformRepo(AppDbContext _context)
		{
			p_context = _context;
		}
		public void CreatePlatform(Platform _platform)
		{
			ArgumentNullException.ThrowIfNull(_platform);

			p_context.Platforms.Add(_platform);
		}

		public IEnumerable<Platform> GetAllPlatforms()
		{
			return p_context.Platforms.ToList();
		}

		public Platform GetPlatformById(int _id)
		{
			return p_context.Platforms.FirstOrDefault(_p => _p.Id == _id);
		}

		public bool SaveChanges()
		{
			return (p_context.SaveChanges() >= 0);
		}
	}
}
