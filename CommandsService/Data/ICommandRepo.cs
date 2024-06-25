using CommandsService.Models;

namespace CommandsService.Data
{
    public interface ICommandRepo
    {
        bool SaveChanges();

        //Platforms
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform _platform);
        bool PlatformExists(int _platformId);
        bool ExternalPlatformExists(int _externalPlatformId);

        //Commands
        IEnumerable<Command> GetCommandsForPlatform(int _platformId);
        Command GetCommand(int _platformId, int _commandId);
        void CreateCommand(int _platformId, Command _command);
    }
}