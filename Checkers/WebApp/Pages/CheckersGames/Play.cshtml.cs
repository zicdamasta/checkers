using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL;
using DAL.Db;
using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.CheckersGames
{
    public class PlayModel : PageModel
    {
        private readonly IGameOptionsRepository _optionsRepo;
        private readonly IGameRepository _gameRepo;
        public CheckersGame CheckersGame { get; set; } = default!;
        public CheckersBrain CheckersBrain { get; set; } = default!;
        public CheckersAI CheckersAi { get; set; } = default!;
        public EPlayerType PlayerType { get; set; } = default!;

        public PlayModel(
            IGameOptionsRepository optionsRepo, 
            IGameRepository gameRepo)
        {
            _optionsRepo = optionsRepo;
            _gameRepo = gameRepo;
        }

        public Task<IActionResult> OnGetAsync(Guid id, int? moveFromRow, int? moveFromCol, int? moveToRow, int? moveToCol)
        {
            var gameId = id;
            CheckersGame = _gameRepo.GetSavedGame(gameId);
            var gameOptions = _optionsRepo.GetGameOptions(CheckersGame.CheckerOptionId);
            
            var lastStateJson = CheckersGame.GetLastGameState()?.SerializedGameState;
            var state = lastStateJson == null ? null : JsonSerializer.Deserialize<CheckersLocalState>(lastStateJson);
            CheckersBrain = new CheckersBrain(gameOptions, state);
            
            CheckersAi = new CheckersAI(CheckersBrain);
            
            PlayerType = GetPlayerType(CheckersGame, CheckersBrain);

            var goodMove = false;
            if (PlayerType == EPlayerType.Human)
            {
                if (moveFromRow != null && moveFromCol != null && moveToRow != null && moveToCol != null)
                {
                    var move = new Move(new Coordinates(moveFromRow.Value, moveFromCol.Value), new Coordinates(moveToRow.Value, moveToCol.Value));
                    goodMove = CheckersBrain.MakeMove(move);
                } 
            }
            else
            {
                CheckersAi.MakeGoodMove();
                goodMove = true;
            }
            if (goodMove)
            {
                SaveState(CheckersGame, CheckersBrain.GetLocalState());
            }

            


            if (CheckersBrain.IsGameOver())
            {
                _gameRepo.SetWinner(CheckersGame.Id, CheckersBrain.GetWinner().ToString());
                return Task.FromResult<IActionResult>(Page());
            }
            
            return Task.FromResult<IActionResult>(Page());
        }

        private void SaveState(CheckersGame game, CheckersLocalState state)
        {
            var savedState = _gameRepo.SaveState(game, state);
            _gameRepo.AddStateToSavedGame(game.Id, savedState.Id);
        }
        
        public EPlayerType GetPlayerType(CheckersGame game, CheckersBrain brain)
        {
            var playerPieceColor = brain.GetPlayerPieceColor();
            var gameOptions = _optionsRepo.GetGameOptions(game.CheckerOptionId);
            return (playerPieceColor == EGamePiece.White) ? gameOptions.WhitePieces : gameOptions.BlackPieces;
        }
    }
}