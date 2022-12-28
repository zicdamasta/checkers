namespace GameBrain
{
    public class CheckersAI
    {
        private readonly CheckersBrain _checkersBrain;
        private readonly Random _random;

        public CheckersAI(CheckersBrain checkersBrain)
        {
            _checkersBrain = checkersBrain;
            _random = new Random();
        }

        public void MakeRandoMove()
        {
            // Find all possible moves for the current player
            Dictionary<Coordinates, List<Coordinates>> possibleMoves = _checkersBrain.GetPossibleMoves();

            // Select a random piece to move
            KeyValuePair<Coordinates, List<Coordinates>> randomPiece =
                possibleMoves.ElementAt(_random.Next(possibleMoves.Count));

            // Select a random move for that piece
            Coordinates randomDestination = randomPiece.Value.ElementAt(_random.Next(randomPiece.Value.Count));

            // Make the move
            _checkersBrain.MakeMove(new Move(randomPiece.Key, randomDestination));
        }

        public void MakeGoodMove()
        {
            // Find all possible moves for the current player
            Dictionary<Coordinates, List<Coordinates>> possibleMoves = _checkersBrain.GetPossibleMoves();
            
            Move bestMove = new Move();
            var bestScore = int.MinValue;

            // if white player set maximizing to true
            var maximizing = _checkersBrain.GetPlayerPieceColor() == EGamePiece.Black;
            
            // Iterate through all possible moves
            foreach (var piece in possibleMoves)
            {
                foreach (var destination in piece.Value)
                {
                    var gameStateCopy = _checkersBrain.CopyGameState();
                    gameStateCopy.MakeMove(new Move(piece.Key, destination));
                    int score = Minimax(gameStateCopy, maximizing, 3);
                    
                    if (score > bestScore)
                    {
                        bestMove = new Move(piece.Key, destination);
                        bestScore = score;
                    }
                }
            }

            // Make the best move
            _checkersBrain.MakeMove(bestMove);
        }

        private int Minimax(CheckersBrain gameState, bool maximizingPlayer, int depth)
        {
            if (gameState.IsGameOver() || depth == 0)
            {
                return maximizingPlayer ? gameState.GetScore() : -gameState.GetScore();
            }
            
            int bestScore = maximizingPlayer ? int.MinValue : int.MaxValue;
            Dictionary<Coordinates, List<Coordinates>> possibleMoves = gameState.GetPossibleMoves();
            
            foreach (var piece in possibleMoves)
            {
                foreach (var destination in piece.Value)
                {
                    var gameStateCopy = gameState.CopyGameState();
                    gameStateCopy.MakeMove(new Move(piece.Key, destination));
                    int score = Minimax(gameStateCopy, !maximizingPlayer, depth - 1 );
                    if (maximizingPlayer)
                    {
                        bestScore = Math.Max(bestScore, score);
                    }
                    else
                    {
                        bestScore = Math.Min(bestScore, score);
                    }
                }
            }

            return bestScore;
        }
    }
}