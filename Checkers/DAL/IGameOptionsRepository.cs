using Domain;

namespace DAL;

public interface IGameOptionsRepository

{
    public string Name { get; set; }
    List<CheckersOption> GetGameOptionsList();

    CheckersOption GetGameOptions(Guid id);

    CheckersOption SaveGameOptions(CheckersOption option);
    
    public void OptionsNameIsUnique(CheckersOption option);

    void DeleteGameOptions(Guid id);
}