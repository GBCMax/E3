using CommandsService.Models;

namespace CommandsService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext p_context;
        public CommandRepo(AppDbContext _context)
        {
            p_context = _context;
        }
        public void CreateCommand(int _platformId, Command _command)
        {
            if(_command == null)
            {
                throw new ArgumentNullException(nameof(_command));
            }
            _command.PlatformId = _platformId;
            p_context.Commands.Add(_command);
        }

        public void CreatePlatform(Platform _platform)
        {
            if(_platform == null)
            {
                throw new ArgumentNullException(nameof(_platform));
            }
            p_context.Platforms.Add(_platform);
        }

        public bool ExternalPlatformExists(int _externalPlatformId)
        {
            return p_context.Platforms.Any(_p => _p.ExternalID == _externalPlatformId);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return p_context.Platforms.ToList();
        }

        public Command GetCommand(int _platformId, int _commandId)
        {
            return p_context.Commands
                .Where(_c => _c.PlatformId == _platformId && _c.Id == _commandId)
                .FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsForPlatform(int _platformId)
        {
            return p_context.Commands
                .Where(_c => _c.PlatformId == _platformId)
                .OrderBy(_c => _c.Platform.Name);
        }

        public bool PlatformExists(int _platformId)
        {
            return p_context.Platforms.Any(_p => _p.Id == _platformId);
        }

        public bool SaveChanges()
        {
            return (p_context.SaveChanges() >= 0);
        }
    }
}