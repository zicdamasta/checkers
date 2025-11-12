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

            // if white player set maximizing to true
            var maximizing = _checkersBrain.GetPlayerPieceColor() == EGamePiece.White;
            var bestScore = maximizing ? int.MinValue : int.MaxValue;

            // Iterate through all possible moves
            foreach (var piece in possibleMoves)
            {
                foreach (var destination in piece.Value)
                {
                    var gameStateCopy = _checkersBrain.CopyGameState();
                    gameStateCopy.MakeMove(new Move(piece.Key, destination));
                    int score = AlphaBeta(gameStateCopy, !maximizing, 5, int.MinValue, int.MaxValue);

                    bool isBetterMove = maximizing ? (score > bestScore) : (score < bestScore);
                    if (isBetterMove)
                    {
                        bestMove = new Move(piece.Key, destination);
                        bestScore = score;
                    }
                }
            }

            // Make the best move
            _checkersBrain.MakeMove(bestMove);
        }

        private int AlphaBeta(CheckersBrain gameState, bool maximizingPlayer, int depth, int alpha, int beta)
        {
            if (gameState.IsGameOver() || depth == 0)
            {
                return gameState.GetScore();
            }

            Dictionary<Coordinates, List<Coordinates>> possibleMoves = gameState.GetPossibleMoves();

            if (maximizingPlayer)
            {
                int bestScore = int.MinValue;
                foreach (var piece in possibleMoves)
                {
                    foreach (var destination in piece.Value)
                    {
                        var gameStateCopy = gameState.CopyGameState();
                        gameStateCopy.MakeMove(new Move(piece.Key, destination));
                        int score = AlphaBeta(gameStateCopy, false, depth - 1, alpha, beta);
                        bestScore = Math.Max(bestScore, score);
                        alpha = Math.Max(alpha, score);
                        if (beta <= alpha)
                            break; // Beta cutoff - prune remaining moves
                    }
                    if (beta <= alpha)
                        break; // Prune remaining pieces
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                foreach (var piece in possibleMoves)
                {
                    foreach (var destination in piece.Value)
                    {
                        var gameStateCopy = gameState.CopyGameState();
                        gameStateCopy.MakeMove(new Move(piece.Key, destination));
                        int score = AlphaBeta(gameStateCopy, true, depth - 1, alpha, beta);
                        bestScore = Math.Min(bestScore, score);
                        beta = Math.Min(beta, score);
                        if (beta <= alpha)
                            break; // Alpha cutoff - prune remaining moves
                    }
                    if (beta <= alpha)
                        break; // Prune remaining pieces
                }
                return bestScore;
            }
        }
    }
}