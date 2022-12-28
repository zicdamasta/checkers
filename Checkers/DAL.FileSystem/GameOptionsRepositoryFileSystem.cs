using System.Text.Json;
using Domain;

namespace DAL.FileSystem;

public class GameOptionsRepositoryFileSystem : IGameOptionsRepository
{
    private const string FileExtension = "json";
    private readonly string _optionsDirectory = "." + Path.DirectorySeparatorChar + "options";

    public string Name { get; set; } = "FileSystem";
    public List<CheckersOption> GetGameOptionsList()
    {
        Helpers.CheckOrCreateDirectory(_optionsDirectory);

        var gameOptions = new List<CheckersOption>();

        // var path = Path.GetFullPath(_optionsDirectory);
        // Console.WriteLine(path);

        foreach (var fileName in Directory.GetFileSystemEntries(_optionsDirectory, "*." + FileExtension))
        {
            var json = File.ReadAllText(fileName);
            var gameOption = JsonSerializer.Deserialize<CheckersOption>(json);
            if (gameOption?.Name != null)
            {
                gameOptions.Add(gameOption);
            }
        }

        return gameOptions;
    }

    public CheckersOption GetGameOptions(Guid id)
    {
        var fileContent = File.ReadAllText(Helpers.GetFileName(_optionsDirectory, id.ToString(), FileExtension));
        var options = JsonSerializer.Deserialize<CheckersOption>(fileContent);
        if (options == null)
        {
            throw new NullReferenceException($"Could not deserialize: {fileContent}");
        }

        return options;
    }

    public CheckersOption SaveGameOptions(CheckersOption option)

    {
        if (option.Id == Guid.Empty || option.Name != "Default")
        {
            option.Id = Guid.NewGuid();
        }
        
        Helpers.CheckOrCreateDirectory(_optionsDirectory);

        var fileContent = System.Text.Json.JsonSerializer.Serialize(option);

        File.WriteAllText(Helpers.GetFileName(_optionsDirectory, option.Id.ToString(), FileExtension), fileContent);
        
        return option;
    }
    
    public void OptionsNameIsUnique(CheckersOption option)
    {
        var gameOptions = GetGameOptionsList();
        if (gameOptions.Any(o => o.Name == option.Name && o.Id != option.Id))
        {
            throw new ArgumentException($"Name {option.Name} is already in use");
        }
    }

    public void DeleteGameOptions(Guid id)
    {
        File.Delete(Helpers.GetFileName(_optionsDirectory, id.ToString(), FileExtension));
    }
    
}