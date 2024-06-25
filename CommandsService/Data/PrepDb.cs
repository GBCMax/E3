using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder _builder)
        {
            using (var serviceScope = _builder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpcClient.ReturnAllPlatforms();

                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo _repo, IEnumerable<Platform> _platforms)
        {
            System.Console.WriteLine("--> Seeding new platforms...");

            foreach (var platform in _platforms)
            {
                if(!_repo.ExternalPlatformExists(platform.ExternalID))
                {
                    _repo.CreatePlatform(platform);
                }
                _repo.SaveChanges();
            }
        }
    }
}