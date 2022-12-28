using System;
using System.Collections.Generic;
using System.Linq;
using Resources.Script.ChessPieces;
using UnityEngine;

namespace Resources.Script
{
    public class Bot : MonoBehaviour
    {
        private GameManager gameManager;
        [SerializeField] private int depth = 2;
        [SerializeField] private Board board;
        [SerializeField] private Log log;
        private float[,] pawnEvalWhite = {
            {0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f},
            {5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f},
            {1.0f,  1.0f,  2.0f,  3.0f,  3.0f,  2.0f,  1.0f,  1.0f},
            {0.5f,  0.5f,  1.0f,  2.5f,  2.5f,  1.0f,  0.5f,  0.5f},
            {0.0f,  0.0f,  0.0f,  2.0f,  2.0f,  0.0f,  0.0f,  0.0f},
            {0.5f, -0.5f, -1.0f,  0.0f,  0.0f, -1.0f, -0.5f,  0.5f},
            {0.5f,  1.0f,  1.0f,  -2.0f, -2.0f,  1.0f,  1.0f,  0.5f},
            {0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f}
        };

        private float[,] pawnEvalBlack;
        private float[,] knightEval = {
            {-5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f},
            {-4.0f, -2.0f,  0.0f,  0.0f,  0.0f,  0.0f, -2.0f, -4.0f},
            {-3.0f,  0.0f,  1.0f,  1.5f,  1.5f,  1.0f,  0.0f, -3.0f},
            {-3.0f,  0.5f,  1.5f,  2.0f,  2.0f,  1.5f,  0.5f, -3.0f},
            {-3.0f,  0.0f,  1.5f,  2.0f,  2.0f,  1.5f,  0.0f, -3.0f},
            {-3.0f,  0.5f,  1.0f,  1.5f,  1.5f,  1.0f,  0.5f, -3.0f},
            {-4.0f, -2.0f,  0.0f,  0.5f,  0.5f,  0.0f, -2.0f, -4.0f},
            {-5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f}
        };

        private float[,] bishopEvalWhite = {
            { -2.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -2.0f},
            { -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -1.0f},
            { -1.0f,  0.0f,  0.5f,  1.0f,  1.0f,  0.5f,  0.0f, -1.0f},
            { -1.0f,  0.5f,  0.5f,  1.0f,  1.0f,  0.5f,  0.5f, -1.0f},
            { -1.0f,  0.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.0f, -1.0f},
            { -1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f, -1.0f},
            { -1.0f,  0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.5f, -1.0f},
            { -2.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -2.0f}
        };

        private float[,] bishopEvalBlack;

        private readonly float[,] rookEvalWhite = {
            {  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f},
            {  0.5f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.5f},
            { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f},
            { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f},
            { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f},
            { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f},
            { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f},
            {  0.0f,  0.0f, 0.0f,  0.5f,  0.5f,  0.0f,  0.0f,  0.0f}
        };

        private float[,] rookEvalBlack;

        private float[,] evalQueen = {
            { -2.0f, -1.0f, -1.0f, -0.5f, -0.5f, -1.0f, -1.0f, -2.0f},
            { -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -1.0f},
            { -1.0f,  0.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -1.0f},
            { -0.5f,  0.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -0.5f},
            {  0.0f,  0.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -0.5f},
            { -1.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -1.0f},
            { -1.0f,  0.0f,  0.5f,  0.0f,  0.0f,  0.0f,  0.0f, -1.0f},
            { -2.0f, -1.0f, -1.0f, -0.5f, -0.5f, -1.0f, -1.0f, -2.0f}
        };

        private float[,] kingEvalWhite = {

            { -3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
            { -3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
            { -3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
            { -3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f},
            { -2.0f, -3.0f, -3.0f, -4.0f, -4.0f, -3.0f, -3.0f, -2.0f},
            { -1.0f, -2.0f, -2.0f, -2.0f, -2.0f, -2.0f, -2.0f, -1.0f},
            {  2.0f,  2.0f,  0.0f,  0.0f,  0.0f,  0.0f,  2.0f,  2.0f },
            {  2.0f,  3.0f,  1.0f,  0.0f,  0.0f,  1.0f,  3.0f,  2.0f }
        };

        private float[,] kingEvalBlack;
        private int count;

        private void Awake()
        {
            gameManager = GetComponent<GameManager>();
            pawnEvalBlack = ReverseRows(pawnEvalWhite);
            bishopEvalBlack = ReverseRows(bishopEvalWhite);
            rookEvalBlack = ReverseRows(rookEvalWhite);
            kingEvalBlack = ReverseRows(kingEvalWhite);
            board.onBotTurn += MakeBestMove;
        }

        private float GetPieceValue(Pieces piece, int x,int y) {
            if (piece == null) {
                return 0;
            }
            var absoluteValue = GetAbsoluteValue(piece, piece.party == Party.White, x ,y);
            return piece.party == Party.White ? absoluteValue : -absoluteValue;
        }
    
        public void MakeBestMove()
        {
            count = 0;
            Dictionary<Pieces, List<Location>> moveSet = new Dictionary<Pieces, List<Location>>();
            foreach (var key in gameManager.keysInDict)
            {
                if (key.party == gameManager.Turn && !key.killed)
                {
                    key.UpdateMoveSet();
                    moveSet.Add(key,key.moveSet);
                }
            }
            Dictionary<Pieces,Location> bestMove = MinimaxRoot(moveSet, true);
            if (bestMove.Count > 0)
            {
                bestMove.First().Key.MoveTo(bestMove.First().Value);
                bestMove.First().Key.TransformTo(bestMove.First().Value);
                if (log.Peek()?.type == MoveType.Castling)
                {
                    log.Peek().catched.TransformTo(log.Peek().catched.position);
                }
                board.PlayAudio();
                gameManager.ClearHighLight();
                gameManager.MoveDeprecated();
                gameManager.SwapTurn();
                gameManager.CheckMate();
            }
        }

    
    
        private Dictionary<Pieces,Location> MinimaxRoot(Dictionary<Pieces, List<Location>> newGameMoves,bool isMaximisingPlayer)
        {
            int bestMove = -9999;
            Location bestMoveFound = null;
            Pieces piece = null;

            foreach (var kvp in newGameMoves)
            {
                var listLocation = kvp.Value.ToList();
                foreach(Location location in listLocation)
                {
                    if(kvp.Key.updated == false) kvp.Key.UpdateMoveSet();
                    kvp.Key.MoveTo(location);
                    gameManager.SwapTurn();
                    gameManager.MoveDeprecated();
                    var value = Minimax(depth - 1, -10000, 10000, !isMaximisingPlayer);
                    board.Undo();
                    if (value >= bestMove)
                    {
                        bestMove = value;
                        bestMoveFound = location;
                        piece = kvp.Key;
                    }
                }
            }
            Dictionary<Pieces, Location> dictionary = new Dictionary<Pieces, Location>();
            if(piece != null) dictionary.Add(piece,bestMoveFound);
            return dictionary;
        }
    
        private int Minimax(int d,int alpha,int beta,bool isMaximisingPlayer)
        {
            count++;
            //Debug.Log("call minimax "+count+ " times");
            if (d == 0) {
                //Debug.Log("minimax is break: ");
                return -EvaluateBoard();
            }
 
            IDictionary<Pieces,List<Location>> newGameMoves = new Dictionary<Pieces, List<Location>>();
            foreach (var p in gameManager.keysInDict)
            {
                if (p.party == gameManager.Turn && !p.killed)
                {
                    List<Location> locations = p.GetMovement();
                    if (locations.Count > 0)
                    {
                        newGameMoves.Add(p,locations);
                    }
                }
            }

            if (isMaximisingPlayer) {
                var bestMove = -9999;
                for (int i = 0, len = newGameMoves.Count; i < len; i++)
                {
                    var kvp = newGameMoves.ElementAt(i);
                    foreach(Location location in kvp.Value)
                    {
                        if(kvp.Key.updated == false) kvp.Key.UpdateMoveSet();
                        kvp.Key.MoveTo(location);
                        gameManager.SwapTurn();
                        gameManager.MoveDeprecated();
                        bestMove = Math.Max(bestMove,Minimax(d - 1, alpha, beta, false));
                        board.Undo();
                        alpha = Math.Max(alpha, bestMove);
                        if (beta <= alpha) {
                            return bestMove;
                        }
                    }
                }
                return bestMove;
            } else {
                var bestMove = 9999;
                for (int i = 0, len = newGameMoves.Count; i < len; i++)
                {
                    var kvp = newGameMoves.ElementAt(i);
                    foreach(Location location in kvp.Value)
                    {
                        if(kvp.Key.updated == false) kvp.Key.UpdateMoveSet();
                        kvp.Key.MoveTo(location);
                        gameManager.SwapTurn();
                        gameManager.MoveDeprecated();
                        bestMove = Math.Min(bestMove,Minimax(d - 1, alpha, beta, true));
                        board.Undo();
                        alpha = Math.Min(beta, bestMove);
                        if (beta <= alpha) {
                            return bestMove;
                        }
                    }
                }
                return bestMove;
            }
        }
    
        private int EvaluateBoard() {
            int totalEvaluation = 0;
            for (var i = 7; i >=0; i--) {
                for (var j = 0; j < 8; j++)
                {
                    Pieces piece = Grid.instance.GetValue(i, j);
                    totalEvaluation = totalEvaluation + (int)GetPieceValue(piece, i ,j);
                }
            }
            return totalEvaluation;
        }
    
        private float GetAbsoluteValue(Pieces piece, bool isWhite, int x, int y)
        {
            if (piece.type == ChessType.Pawn)
            {
                return 10 + (isWhite ? pawnEvalWhite[y, x] : pawnEvalBlack[y, x]);
            }
            if (piece.type == ChessType.Rook)
            {
                return 50 + (isWhite ? rookEvalWhite[y, x] : rookEvalBlack[y, x]);
            }
            if (piece.type == ChessType.Knight)
            {
                return 30 + knightEval[y, x];
            }
            if (piece.type == ChessType.Bishop)
            {
                return 30 + (isWhite ? bishopEvalWhite[y, x] : bishopEvalBlack[y, x]);
            }
            if (piece.type == ChessType.Queen)
            {
                return 90 + evalQueen[y, x];
            }
            if (piece.type == ChessType.King)
            {
                return 900 + (isWhite ? kingEvalWhite[y, x] : kingEvalBlack[y, x]);
            }
            throw new Exception("Unknown piece type: " + piece.type);
        }

        private T[,] ReverseRows<T>(T[,] array)
        {
            int n = array.GetLength(0);
            T[,] result = array.Clone() as T[,];

            for (int i = 0; i < n/2; i++)
            {
                SwapRows(result, i, n - i - 1); 
            }
            return result;
        }

        private void SwapRows<T>(T[,] array, int row1, int row2)
        {
            if (row1 == row2) return;
            int n = array.GetLength(0), m = array.GetLength(1);
            int byteLen = Buffer.ByteLength(array) / (n * m);

            T[] buffer = new T[m];
            // copy row1 to buffer
            Buffer.BlockCopy(array, row1 * m * byteLen, buffer, 0, m * byteLen);
            // copy row2 to row1
            Buffer.BlockCopy(array, row2 * m * byteLen, array, row1 * m * byteLen, m * byteLen);
            // copy buffer to row2
            Buffer.BlockCopy(buffer, 0, array, row2 * m * byteLen, m * byteLen);
        }
    }
}
