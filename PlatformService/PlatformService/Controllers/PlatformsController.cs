using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlatformsController : ControllerBase
	{
		private readonly IPlatformRepo p_repository;
		private readonly IMapper p_mapper;
		private readonly ICommandDataClient p_commandDataClient;
        private readonly IMessageBusClient p_messageBusClient;

        public PlatformsController(IPlatformRepo _repository, IMapper _mapper, ICommandDataClient _commandDataClient, IMessageBusClient _messageBusClient)
		{
			p_repository = _repository;
			p_mapper = _mapper;
			p_commandDataClient = _commandDataClient;
			p_messageBusClient = _messageBusClient;
		}

		[HttpGet("GetAll")]
		public ActionResult<IEnumerable<PlatformReadDto>> GetPlatfroms()
		{
			Console.WriteLine("--> Getting platforms...");

			var platformItem = p_repository.GetAllPlatforms();

			return Ok(p_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
		}

		[HttpGet("{_id}", Name = "GetPlatformById")]
		public ActionResult<PlatformReadDto> GetPlatformById(int _id)
		{
			var platformItem = p_repository.GetPlatformById(_id);

			if(platformItem == null)
			{
				return NotFound();
			}

			return Ok(p_mapper.Map<PlatformReadDto>(platformItem));
		}

		[HttpPost("Create")]
		public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto _platformCreateDto)
		{
			var platformModel = p_mapper.Map<Platform>(_platformCreateDto);

			p_repository.CreatePlatform(platformModel);

			p_repository.SaveChanges();

			var platformReadDto = p_mapper.Map<PlatformReadDto>(platformModel);

			//sync message
			try
			{
				await p_commandDataClient.SendPlatformToCommand(platformReadDto);
			}
			catch(Exception ex)
			{
				Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
			}

			//async message
			try
			{
				var platformPublishedDto = p_mapper.Map<PlatformPublishedDto>(platformReadDto);
				platformPublishedDto.Event = "Platform_Published";
				p_messageBusClient.PublishNewPlatform(platformPublishedDto);
			}
			catch(Exception ex)
			{
				Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
			}

			return CreatedAtRoute(nameof(GetPlatformById), new { _id = platformReadDto.Id }, platformReadDto);
		}
	}
}
