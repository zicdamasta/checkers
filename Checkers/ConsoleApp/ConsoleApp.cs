using System.Data;
using System.Text.Json;
using ConsoleUI;
using DAL;
using DAL.Db;
using DAL.FileSystem;
using Domain;
using GameBrain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Action = MenuSystem.Action;


var dataSource = Environment.GetEnvironmentVariable("DATABASE_URL");

var dbOptions =
    new DbContextOptionsBuilder<AppDbContext>()
        // .UseLoggerFactory(ConsoleApp.Helpers.MyLoggerFactory)
        .UseSqlite($"Data Source={dataSource}")
        .Options;

var ctx = new AppDbContext(dbOptions);

//dotnet ef migrations add InitialCreate --project DAL.db --startup-project ConsoleApp 
//dotnet ef database update --project DAL.Db --startup-project ConsoleApp
//ctx.Database.EnsureDeleted();
ctx.Database.Migrate();

IGameOptionsRepository optionsRepositoryFileSystem = new GameOptionsRepositoryFileSystem();
IGameRepository gameRepositoryFileSystem = new GameRepositoryFileSystem();

IGameOptionsRepository optionsRepositoryDatabase = new GameOptionsRepositoryDb(ctx);
IGameRepository gameRepositoryDatabase = new GameRepositoryDatabase(ctx);

var optionsRepo = optionsRepositoryFileSystem;
var gameRepo = gameRepositoryFileSystem;

var gameOptions = new CheckersOption();
CheckForDefaultOptions();


var editOptionsMenu = new Menu(MenuLevel.Level2Plus, "==============> EDIT OPTIONS <==============");

var loadOptionsMenu = new Menu(MenuLevel.Level2Plus, "==============> LOAD OPTIONS <==============");


var menu = new Menu(MenuLevel.Level0, "==============> MAIN MENU <==============");
menu.AddMenuItem(new MenuItem("New game", NewGameMenu));
menu.AddMenuItem(new MenuItem("Saved games", SavedGamesMenu));
menu.AddMenuItem(new MenuItem("Swap Persistence Engine ", SwapPersistenceEngine));

Action SavedGamesMenu()
{
    var savedGameMenu = new Menu(MenuLevel.Level1, "==============> SAVE MENU <==============");
    savedGameMenu.AddMenuItem(new MenuItem("Select from saved", LoadGameMenu));
    savedGameMenu.AddMenuItem(new MenuItem("List all games", ListGamesMenu));
    savedGameMenu.AddMenuItem(new MenuItem("Delete saved game", DeleteSavedGame));
    return savedGameMenu.RunMenu();
}

Action SwapPersistenceEngine()
{
    optionsRepo = optionsRepo == optionsRepositoryDatabase ? optionsRepositoryFileSystem : optionsRepositoryDatabase;
    gameRepo = gameRepo == gameRepositoryDatabase ? gameRepositoryFileSystem : gameRepositoryDatabase;
    gameOptions = new CheckersOption();
    Console.WriteLine("Persistence engine: " + optionsRepo.Name);
    Console.ReadKey();
    return Action.None;
}

Action ListGamesMenu()
{
    var games = gameRepo.GetSavedGames();
    var table = new Table();
    table.AddColumn("Id").Centered();
    table.AddColumn("Name").Centered();
    table.AddColumn("StartedAt").Centered();
    table.AddColumn("GameOverAt").Centered();
    table.AddColumn("GameWonByPlayer").Centered();
    table.AddColumn("WhiteName").Centered();
    table.AddColumn("WhitePieces").Centered();
    table.AddColumn("BlackName").Centered();
    table.AddColumn("BlackPieces").Centered();
    table.AddColumn("CheckerOption").Centered();

    foreach (var game in games)
    {
        table.AddRow(
            game.Id.ToString(),
            game.Name,
            game.StartedAt.ToString(),
            game.GameOverAt?.ToString() ?? "",
            game.GameWonByPlayer ?? "",
            game.WhiteName,
            game.WhitePieces.ToString(),
            game.BlackName,
            game.BlackPieces.ToString(),
            game.CheckerOption?.Name ?? ""
        );
    }

    Console.Clear();
    AnsiConsole.Write(table);
    Console.ReadKey();
    return Action.None;
}

Action DeleteSavedGame()
{
    var deleteGamesMenu = new Menu(MenuLevel.Level2Plus, "==============> DELETE GAMES <==============");
    var games = gameRepo.GetSavedGames();

    if (games.Count == 0)
    {
        AnsiConsole.MarkupLine("[red]No games to delete[/]");
        Console.ReadKey();
        return Action.None;
    }

    deleteGamesMenu.ClearMenuItems();
    foreach (var game in games)
    {
        deleteGamesMenu.AddMenuItem(new MenuItem(game.Name, DeleteGame(game)));
    }

    Console.Clear();
    return deleteGamesMenu.RunMenu();
}

Func<Action> DeleteGame(CheckersGame game)
{
    return () =>
    {
        gameRepo.DeleteGameStatesByGameId(game.Id);
        gameRepo.DeleteGame(game.Id);
        Console.WriteLine(game.Id + " deleted successfully!");
        Console.ReadKey();
        return Action.Back;
    };
}

Action NewGameMenu()
{
    var newGameMenu = new Menu(MenuLevel.Level1, "==============> SELECT OPTIONS <==============");
    newGameMenu.AddMenuItem(new MenuItem("New game. Custom options", CustomOptionsMenu));
    newGameMenu.AddMenuItem(new MenuItem("New game. Saved options", LoadOptionsMenu));
    newGameMenu.AddMenuItem(new MenuItem("Delete options", DeleteOptionsMenu));
    return newGameMenu.RunMenu();
}


void RefreshEditGameOptionsMenu()
{
    editOptionsMenu.ClearMenuItems();
    editOptionsMenu.AddMenuItem(new MenuItem($"Options name: {gameOptions.Name}", EditOptionsName));
    editOptionsMenu.AddMenuItem(new MenuItem($"Board height: {gameOptions.GameBoardHeight}", EditGameBoardHeight));
    editOptionsMenu.AddMenuItem(new MenuItem($"Board width: {gameOptions.GameBoardWidth}", EditGameBoardWidth));
    editOptionsMenu.AddMenuItem(new MenuItem($"White side: {gameOptions.WhitePieces}",
        EditSides(ESide.White)));
    editOptionsMenu.AddMenuItem(new MenuItem($"Black side: {gameOptions.BlackPieces}",
        EditSides(ESide.Black)));
    editOptionsMenu.AddMenuItem(new MenuItem($"START GAME", ValidateOptionsAndStartNewGame));
}

Action ValidateOptionsAndStartNewGame()
{
    try
    {
        optionsRepo?.OptionsNameIsUnique(gameOptions);
    }
    catch (Exception)
    {
        Console.WriteLine("Options name is not unique");
        Console.ReadKey();
        return Action.None;
    }

    var savedOptions = optionsRepo?.SaveGameOptions(gameOptions);
    if (savedOptions != null) LoadGameOption(savedOptions);
    NewGame();
    return Action.ReturnToMainMenu;
}

Action CustomOptionsMenu()
{
    RefreshEditGameOptionsMenu();
    return editOptionsMenu.RunMenu();
}

Action LoadOptionsMenu()
{
    var options = optionsRepo.GetGameOptionsList();

    if (options.Count == 0)
    {
        AnsiConsole.MarkupLine("[red]No options to load[/]");
        Console.ReadKey();
        return Action.None;
    }

    loadOptionsMenu.ClearMenuItems();
    foreach (var option in options)
    {
        loadOptionsMenu.AddMenuItem(new MenuItem(option.Name, SelectGameOption(option)));
    }

    Console.Clear();
    return loadOptionsMenu.RunMenu();
}

Action LoadGameMenu()
{
    var savedGames = gameRepo.GetSavedGames();
    var loadGameMenu = new Menu(MenuLevel.Level2Plus, "==============> LOAD GAME <==============");
    // if saved games is empty, add a menu item that says so
    if (savedGames.Count == 0)
    {
        loadGameMenu.AddMenuItem(new MenuItem("No saved games", None));
    }

    foreach (var savedGame in savedGames.Where(savedGame => savedGame.GameWonByPlayer == null))
    {
        loadGameMenu.AddMenuItem(new MenuItem(savedGame.Name, ContinueGame(savedGame)));
    }

    return loadGameMenu.RunMenu();
}

void CreateDefaultOptions()
{
    gameOptions.Id = Guid.NewGuid();
    gameOptions.Name = "Default";
    optionsRepo.SaveGameOptions(gameOptions);
}

void CheckForDefaultOptions()
{
    var options = optionsRepo.GetGameOptionsList();

    if (options.Count == 0)
    {
        CreateDefaultOptions();
    }

    foreach (var option in options)
    {
        // if default options are not found, create them
        if (option.Name == "Default")
        {
            // if default options are found, set them as the current options
            gameOptions = option;
            return;
        }

        CreateDefaultOptions();
    }
}

Action DeleteOptionsMenu()
{
    var deleteOptionsMenu = new Menu(MenuLevel.Level2Plus, "==============> DELETE OPTIONS <==============");
    var options = optionsRepo.GetGameOptionsList();

    if (options.Count == 0)
    {
        AnsiConsole.MarkupLine("[red]No options to delete[/]");
        Console.ReadKey();
        return Action.None;
    }

    deleteOptionsMenu.ClearMenuItems();
    foreach (var option in options)
    {
        deleteOptionsMenu.AddMenuItem(new MenuItem(option.Name, DeleteGameOption(option)));
    }

    Console.Clear();
    return deleteOptionsMenu.RunMenu();
}

Func<Action> DeleteGameOption(CheckersOption option)
{
    return () =>
    {
        var gamesCount = gameRepo.CountGamesByOptionsId(option.Id);
        var confirm =
            AnsiConsole.Confirm(
                $"Are you sure you want to delete. This will delete {gamesCount} linked games as well.");
        if (confirm)
        {
            optionsRepo.DeleteGameOptions(option.Id);
            gameRepo.DeleteGameByOptionsId(option.Id);
            Console.WriteLine(option.Id + " deleted successfully!");
            Console.ReadKey();
        }

        return Action.Back;
    };
}


Func<Action> EditSides(ESide color)
{
    return () =>
    {
        var colorSideMenu = new Menu(MenuLevel.Level2Plus, $"==============> {color} SIDE <==============");
        colorSideMenu.AddMenuItem(new MenuItem("Human", () =>
        {
            gameOptions.SetSide(color, EPlayerType.Human);
            RefreshEditGameOptionsMenu();
            return Action.Back;
        }));
        colorSideMenu.AddMenuItem(new MenuItem("AI", () =>
        {
            gameOptions.SetSide(color, EPlayerType.Ai);
            RefreshEditGameOptionsMenu();
            return Action.Back;
        }));

        Console.Clear();
        return colorSideMenu.RunMenu();
    };
}


Action EditGameBoardWidth()
{
    Console.Clear();
    Console.WriteLine("Enter new board width: ");
    var boardWidth = AnsiConsole.Prompt(
        new TextPrompt<int>("Enter board [green]width[/]: ")
            .PromptStyle("green")
            .ValidationErrorMessage("[red]That's not a valid width[/]")
            .Validate(height =>
            {
                return height switch
                {
                    <= 3 => ValidationResult.Error("[red]Board minimum width is 4[/]"),
                    >= 32 => ValidationResult.Error("[red]Board maximum width is 32[/]"),
                    _ => ValidationResult.Success()
                };
            }));

    gameOptions.GameBoardWidth = boardWidth;
    RefreshEditGameOptionsMenu();
    return Action.None;
}

Action EditGameBoardHeight()
{
    Console.Clear();
    Console.WriteLine("Enter new board height: ");
    var boardHeight = AnsiConsole.Prompt(
        new TextPrompt<int>("Enter board [green]height[/]: ")
            .PromptStyle("green")
            .ValidationErrorMessage("[red]That's not a valid height[/]")
            .Validate(height =>
            {
                if (height <= 3)
                    return ValidationResult.Error("[red]Board minimum height is 4[/]");
                if (height >= 32)
                    return ValidationResult.Error("[red]Board maximum height is 32[/]");
                if (height % 2 != 0)
                    return ValidationResult.Error("[red]Board height must be even integer[/]");
                return ValidationResult.Success();
            }));

    gameOptions.GameBoardHeight = boardHeight;
    RefreshEditGameOptionsMenu();
    return Action.None;
}

Action EditOptionsName()
{
    Console.Clear();
    var newName = AnsiConsole.Prompt(
        new TextPrompt<string>("Enter name for options: ")
            .PromptStyle("green")
            .ValidationErrorMessage("[red]That's not a valid name[/]")
            .Validate(name => !string.IsNullOrWhiteSpace(name) && name.Length is <= 20 and >= 3));

    gameOptions.Name = newName.Trim();
    RefreshEditGameOptionsMenu();
    return Action.None;
}

Action None()
{
    return Action.None;
}

Func<Action> SelectGameOption(CheckersOption option)
{
    return () =>
    {
        LoadGameOption(option);
        NewGame();
        return Action.ReturnToMainMenu;
    };
}

void LoadGameOption(CheckersOption option)
{
    gameOptions = option;
}


void SaveGame(CheckersGame game)
{
    gameRepo?.SaveGame(game);
}

void SaveState(CheckersGame game, CheckersLocalState state)
{
    var savedState = gameRepo?.SaveState(game, state);
    if (savedState != null) gameRepo?.AddStateToSavedGame(game.Id, savedState.Id);
}


void NewGame()
{
    var gameName = AnsiConsole.Prompt(
        new TextPrompt<string>("Enter name for game: ")
            .PromptStyle("green")
            .ValidationErrorMessage("[red]That's not a valid name[/]")
            .Validate(name => !string.IsNullOrWhiteSpace(name) && name.Length is <= 20 and >= 3));
    var checkersGame = new CheckersGame()
    {
        Id = Guid.NewGuid(),
        Name = gameName,
        StartedAt = DateTime.Now,
        WhiteName = "Player 1",
        BlackName = "Player 2",
        CheckersGameStates = new List<CheckersGameState>(),
        CheckerOptionId = gameOptions.Id,
        CheckerOption = gameOptions
    };
    gameOptions.AddCheckersGame(checkersGame);

    Console.WriteLine(checkersGame);
    SaveGame(checkersGame);
    StartGame(checkersGame);
}

Func<Action> ContinueGame(CheckersGame checkersGame)
{
    return () =>
    {
        var options = optionsRepo.GetGameOptions(checkersGame.CheckerOptionId);
        LoadGameOption(options);
        StartGame(checkersGame);
        return Action.Back;
    };
}

void StartGame(CheckersGame checkersGame)
{
    var ui = new UserInterface();

    // deserialize last state
    var lastStateJson = checkersGame.GetLastGameState()?.SerializedGameState;
    var state = lastStateJson == null ? null : JsonSerializer.Deserialize<CheckersLocalState>(lastStateJson);

    var gameBrain = new CheckersBrain(gameOptions, state);
    var ai = new CheckersAI(gameBrain);
    while (!gameBrain.IsGameOver())
    {
        var playerPieceColor = gameBrain.GetPlayerPieceColor();
        var playerType = (playerPieceColor == EGamePiece.White) ? gameOptions.WhitePieces : gameOptions.BlackPieces;
        bool successFulMove;

        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Backspace)
        {
            return;
        }

        if (playerType == EPlayerType.Human)
        {
            var move = ui.GetMove(gameBrain.GetBoard(), playerPieceColor);
            if (move.From is { Col: -1, Row: -1 })
            {
                return;
            }

            successFulMove = gameBrain.MakeMove(move);
        }
        else
        {
            UserInterface.DrawBoard(gameBrain.GetBoard(), gameBrain.GameBoardHeight, gameBrain.GameBoardWidth,
                new Coordinates(-1, -1));
            ai.MakeGoodMove();
            successFulMove = true;
        }

        if (successFulMove)
        {
            if (checkersGame != null) SaveState(checkersGame, gameBrain.GetLocalState());
        }
    }


    UserInterface.DrawBoard(gameBrain.GetBoard(), gameBrain.GameBoardHeight, gameBrain.GameBoardWidth,
        new Coordinates(-1, -1));
    AnsiConsole.MarkupLine($"[green]Winner is {gameBrain.GetWinner()}[/]");
    if (checkersGame != null)
    {
        gameRepo?.SetWinner(checkersGame.Id, gameBrain.GetWinner().ToString());
    }

    Console.ReadKey();
}

menu.RunMenu();