# Checkers Game

A checkers (draughts) game implementation in C# .NET 9.0 with both console and web interfaces. Features configurable board sizes (4x4 to 32x32), Human vs Human, Human vs AI, and AI vs AI gameplay with Minimax algorithm.

## Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQLite (included with .NET)

### Running the Game

**Console Application (recommended for first-time users):**
```bash
cd /Users/aleksandrsmirnov/git/checkers
dotnet run --project ConsoleApp
```

The console app provides a menu-driven interface where you can:
- Create new games with custom board sizes
- Choose between Human and AI players
- Save and load games
- Switch between Database and FileSystem storage

**Web Application:**
```bash
cd /Users/aleksandrsmirnov/git/checkers
dotnet run --project WebApp
```

Then navigate to the URL shown in the console (typically `http://localhost:5000`).

## Configuration

### Environment Variables

**DATABASE_URL** - Path to SQLite database file (optional for ConsoleApp, required for WebApp)

Example:
```bash
export DATABASE_URL=/Users/yourusername/checkers.db
dotnet run --project ConsoleApp
```

If not set, ConsoleApp uses a default path. WebApp uses the connection string in `WebApp/appsettings.json`.

### Database Setup

**Initialize the database (first time only):**
```bash
dotnet ef database update --project DAL.Db --startup-project ConsoleApp
```

This creates the SQLite database with the required schema (CheckersOption, CheckersGame, CheckersGameState tables).

### Persistence Options

The application supports two storage strategies:

1. **Database (SQLite)** - Uses Entity Framework Core
   - Required for WebApp
   - Optional for ConsoleApp (selected via menu)

2. **FileSystem (JSON)** - Stores data in local directories
   - ConsoleApp only
   - Creates `./options/`, `./games/`, `./states/` directories automatically
   - No database setup required

## Game Features

### Board Configuration
- Customizable board sizes: 4x4 to 32x32 (width and height must be even)
- Named game configurations for quick setup

### Player Types
- **Human** - Interactive play via keyboard (console) or mouse (web)
- **AI** - Computer opponent using Minimax algorithm (depth 3)

### Game Rules
- Standard checkers rules (draughts)
- Diagonal movement only
- Mandatory captures (forced eating)
- Chain captures (multiple jumps in one turn)
- King promotion when reaching opposite end
- Kings can move unlimited distance diagonally

### Additional Features
- Save/load games mid-play
- Game history tracking with timestamps
- Winner detection and recording
- Auto-refresh for AI turns in web interface

## Console Controls

- **Arrow Keys** - Navigate the board
- **Enter** - Select piece, then select destination
- **Menu Navigation** - Use arrow keys and Enter to navigate menus

## Development

### Build Commands

```bash
# Build entire solution
dotnet build Checkers.sln

# Clean build artifacts
dotnet clean

# Restore NuGet packages
dotnet restore
```

### Database Migrations

**Create a new migration:**
```bash
dotnet ef migrations add MigrationName --project DAL.Db --startup-project ConsoleApp
```

**Apply migrations:**
```bash
dotnet ef database update --project DAL.Db --startup-project ConsoleApp
```

**Remove last migration:**
```bash
dotnet ef migrations remove --project DAL.Db --startup-project ConsoleApp
```

## Project Structure

- **Domain** - Core entities and enums
- **GameBrain** - Game logic, move validation, AI implementation
- **DAL** - Data access interfaces
- **DAL.Db** - Entity Framework Core/SQLite implementation
- **DAL.FileSystem** - JSON file-based storage
- **ConsoleUI** - Console rendering with Spectre.Console
- **MenuSystem** - Reusable console menu framework
- **ConsoleApp** - Main CLI application
- **WebApp** - ASP.NET Core Razor Pages web interface
- **MenuTestApp** - Menu system testing utility

## Technology Stack

- .NET 9.0
- Entity Framework Core 9.0
- SQLite
- Spectre.Console (console UI)
- ASP.NET Core Razor Pages
- Bootstrap (web UI)

## Architecture

The project follows a layered architecture:
- **Domain Layer** - Entities and enums
- **Data Access Layer** - Repository pattern with two implementations
- **Business Logic Layer** - Game rules, AI, UI logic
- **Presentation Layer** - Console and web applications
