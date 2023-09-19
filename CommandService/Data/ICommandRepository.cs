using CommandService.Models;

namespace CommandService.Data;

public interface ICommandRepository
{
    bool SaveChanges();

    // Platforms
    IEnumerable<Platform> GetAllPlatforms();

    void CreatePlatform(Platform platform);

    bool PlatformExits(int platformId);

    bool ExternalPlatformExist(int externalPlatformId);


    // Commands
    IEnumerable<Command> GetCommandsForPlatform(int platformId);

    Command? GetCommand(int platformId, int commandId);

    void CreateCommand(int platformId, Command command);

}