using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEventType(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            default:
                // TODO
                break;
        }
    }

    private EventType DetermineEventType(string notificationMessage)
    {
        Console.WriteLine(">> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        switch (eventType.Event)
        {
            case "Platform_Published":
                Console.WriteLine(">> Platform Published Event Detected");
                return EventType.PlatformPublished;

            default:
                Console.WriteLine(">> Could not determine the event type");
                return EventType.Undetermined;
        }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);
                if (!repository.ExternalPlatformExist(platform.ExternalId))
                {
                    repository.CreatePlatform(platform);
                    repository.SaveChanges();
                    Console.WriteLine($">> Platform added!  platformId:={platform.Id}, externalId:={platform.ExternalId}");
                }
                else
                {
                    Console.WriteLine(">> Platform already exists...");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($">> Cloud not add Platform to DB {ex.Message}");
            }
        }
    }
}