using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory p_scopeFactory;
        private readonly IMapper p_mapper;

        public EventProcessor(IServiceScopeFactory _scopeFactory, IMapper _mapper)
        {
            p_scopeFactory = _scopeFactory;
            p_mapper = _mapper;
        }
        public void ProcessEvent(string _message)
        {
            var eventType = DetermineEvent(_message);

            switch(eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(_message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string _notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(_notificationMessage);

            switch(eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform published event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }
        private void AddPlatform(string _platformPublishedMessage)
        {
            using(var scope = p_scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(_platformPublishedMessage);

                try
                {
                    var platform = p_mapper.Map<Platform>(platformPublishedDto);

                    if(!repo.ExternalPlatformExists(platform.ExternalID))
                    {
                        repo.CreatePlatform(platform);

                        repo.SaveChanges();

                        System.Console.WriteLine("--> Platform added");
                    }
                    else
                    {
                        System.Console.WriteLine("--> Platform already exists...");
                    }
                }
                catch(Exception ex)
                {
                    System.Console.WriteLine($"--> Could not add platform to DB {ex.Message}");
                }
            }
        }
    }
    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}