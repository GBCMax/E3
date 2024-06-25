using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{_platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo p_repository;
        private readonly IMapper p_mapper;
        public CommandsController(ICommandRepo _repository, IMapper _mapper)
        {
            p_repository = _repository;
            p_mapper = _mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int _platformId)
        {
            System.Console.WriteLine($"--> Hit GetCommandsForPlatform: {_platformId}");

            if (!p_repository.PlatformExists(_platformId))
            {
                return NotFound();
            }

            var commands = p_repository.GetCommandsForPlatform(_platformId);

            return Ok(p_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{_commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int _platformId, int _commandId)
        {
            System.Console.WriteLine($"--> Hit GetCommandForPlatform: {_platformId} / {_commandId}");

            if (!p_repository.PlatformExists(_platformId))
            {
                return NotFound();
            }

            var command = p_repository.GetCommand(_platformId, _commandId);

            if (command == null)
            {
                return NotFound();
            }

            return Ok(p_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int _platformId, CommandCreateDto _commandDto)
        {
            System.Console.WriteLine($"--> Hit CreateCommandForPlatform: {_platformId}");

            if (!p_repository.PlatformExists(_platformId))
            {
                return NotFound();
            }

            var command = p_mapper.Map<Command>(_commandDto);

            p_repository.CreateCommand(_platformId, command);
            p_repository.SaveChanges();

            var commandReadDto = p_mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), 
            new { _platformId = _platformId, _commandId = commandReadDto.Id },
            commandReadDto);
        }
    }
}