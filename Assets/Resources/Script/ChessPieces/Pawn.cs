using System.Collections.Generic;
using UnityEngine;

namespace Resources.Script.ChessPieces
{
    public class Pawn : Pieces
    {
        private GameObject promoteModal;

        void Start()
        { 
            type = ChessType.Pawn;
            moveSet = GetMovement();
        }
        

        private Location EnPassant(int x, int y, History lastTurn)
        {
            if (lastTurn == null) return null;
            if (lastTurn.piece.type != ChessType.Pawn) return null;
            
            int offsetY = Mathf.Abs(y - lastTurn.to.y);
            if (lastTurn.from.x == 1 && lastTurn.to.x == 3 && x == 3 && offsetY == 1)
            {
                return new Location(lastTurn.to.x -1,lastTurn.to.y);
            }
            if (lastTurn.from.x == 6 && lastTurn.to.x == 4 && x == 4 && offsetY == 1)
            {
                return new Location(lastTurn.to.x + 1,lastTurn.to.y);
            }

            return null;
        }
        public override List<Location> GetMovement(bool validate = true)
        {
            int direction = party == Party.White ? 1 : -1;
            int start = party == Party.White ? 1 : 6;
            List<Location> moveSet = new List<Location>();
            Location curr = position; 
            // attack left & right
            if (position == null)
            {
                return moveSet;
            }
            Pieces atkRight = Grid.instance.GetValue(curr.x + direction, curr.y + 1);
            Pieces atkLeft = Grid.instance.GetValue(curr.x + direction, curr.y - 1);
            if (atkRight != null && atkRight.party != party)
            {
                moveSet.Add(new Location(curr.x + direction, curr.y + 1));
            }
            if (atkLeft != null && atkLeft.party != party)
            {
                moveSet.Add(new Location(curr.x + direction, curr.y - 1));
            }
            // en passant
            History lastTurn = Log.instance.Size() > 0 ? Log.instance.Peek() : null;
            Location enPos = EnPassant(curr.x, curr.y, lastTurn);
            if (lastTurn != null && enPos != null)
            {
                moveSet.Add(enPos);
            }
            // move 2 cell at start
            if (curr.x == start)
            {
                if (Grid.instance.GetValue(curr.x + direction, curr.y) == null)
                {
                    moveSet.Add(new Location(curr.x + direction, curr.y));
                    if (Grid.instance.GetValue(curr.x + direction*2, curr.y) == null)
                        moveSet.Add(new Location(curr.x + direction*2, curr.y));
                }
            
            }
            else if (Grid.instance.GetValue(curr.x + direction, curr.y) == null)
            {
                // otherwise move 1 cell
                moveSet.Add(new Location(curr.x + direction, curr.y));
            }

            if(validate) moveSet.RemoveAll(IlLegalMove);
            return moveSet;
        }

        public override void MoveTo(Location location)
        {
            Pieces dest;
            Pieces currPiece = gameObject.GetComponent<Pieces>();
            Location curr = position;
            History lastTurn = Log.instance.Size() > 0 ? Log.instance.Peek() : null;
            MoveType moveType = MoveType.Normal;
            // check is en passant
            Location enPos = EnPassant(curr.x, curr.y, lastTurn);
            if (lastTurn != null && enPos != null && enPos.Equals(location))
            {
                
                dest = lastTurn.piece.party == Party.White ?
                    Grid.instance.GetValue(location.x + 1, location.y) :
                    Grid.instance.GetValue(location.x - 1, location.y);
                moveType = MoveType.Enpassant;
            }
            else
            {
                dest = Grid.instance.GetValue(location.x, location.y);
            }
            // remove piece
            if (dest != null)
            {
                dest.killed = true;
                dest.moveSet.Clear();
                dest.GetComponent<MeshRenderer>().enabled = false;
            }
            // promote
            // pawn promote
            if (validating == false && (location.x == 7 || location.x == 0))
            {
                if (GameManager.instance.modePve)
                {
                    GameManager.instance.promoteController.ConvertPiece(this,ChessType.Queen);
                }
                else
                {
                    GameManager.instance.promoteController.Active();   
                }
                moveType = MoveType.Promote;
            }

            // move and remove in matrix
            Grid.instance.SetValue(location, currPiece);
            position = location;
            Grid.instance.SetValue(curr, null);
            Log.instance.Push(new History(currPiece, curr, location, dest,moveType));
        }
    }
}
