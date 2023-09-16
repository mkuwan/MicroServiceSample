using CommandService.Models;

namespace CommandService.Data;

public class CommandRepository: ICommandRepository
{
    private readonly AppDbContext _appDbContext;
    public CommandRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public bool SaveChanges()
    {
        return _appDbContext.SaveChanges() >= 0;
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _appDbContext.Platforms.ToList();
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform == null)
            throw new ArgumentNullException(nameof(platform));

        _appDbContext.Platforms.Add(platform);
    }

    public bool PlatformExits(int platformId)
    {
        return _appDbContext.Platforms.Any(x => x.Id == platformId);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _appDbContext.Commands
            .Where(x => x.PlatformId == platformId)
            .OrderBy(x => x.Platform.Name);
    }

    public Command? GetCommand(int platformId, int commandId)
    {
        return _appDbContext.Commands
            .FirstOrDefault(x => x.PlatformId == platformId && x.Id == commandId);
    }

    public void CreateCommand(int platformId, Command command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        command.PlatformId = platformId;

        _appDbContext.Commands.Add(command);
    }
}