using System;
using System.Collections.Generic;
using UnityEngine;

namespace Resources.Script.ChessPieces
{
    public enum ChessType
    {
        Pawn = 'p',
        Rook = 'r',
        Knight = 'n',
        Bishop = 'b',
        Queen = 'q',
        King = 'k'
    }
    public enum Party
    {
        White = 1,
        Black = -1
    }
    public class Pieces : MonoBehaviour
    {
        public ChessType type;
        public Party party;
        public Location position;
        public bool killed;
        public List<Location> moveSet;
        public bool updated = true;
        public bool validating;
        

        public virtual List<Location> GetMovement(bool validate = true)
        {
            return new List<Location>();
        }
        public virtual void MoveTo(Location location)
        {
            Pieces dest = Grid.instance.GetValue(location.x, location.y);
            Pieces currPiece = gameObject.GetComponent<Pieces>();
            Location curr = position;
            // remove piece
            if (dest != null)
            {
                dest.killed = true;
                dest.moveSet.Clear();
                dest.GetComponent<MeshRenderer>().enabled = false;
            }
            // move and remove in matrix
            Grid.instance.SetValue(location, currPiece);
            position = location;
            Grid.instance.SetValue(curr, null);
            Log.instance.Push(new History(currPiece, curr, location, dest));
        }
        
        public void TransformTo(Location location)
        {
            Location curr = GetCurrentLocation();
            
            // transform.DOMove( transform.position + new Vector3(
            //     (location.y - curr.y) * 0.0209f,
            //     (location.x - curr.x) * 0.0208f,
            //     0
            //     ),.5f).SetEase(Ease.InBack);
            transform.position += new Vector3(
                (location.y - curr.y) * 0.0209f,
                (location.x - curr.x) * 0.0208f,
                0
            );

        }

        public static Location GetXY(Vector3 vec)
        {
            return new Location(Mathf.FloorToInt(vec.y / 0.0208f), Mathf.FloorToInt(vec.x / 0.0208f));
        }

        public Location GetCurrentLocation()
        {
            Vector3 vec = gameObject.transform.position;
            return new Location(
                Mathf.FloorToInt(vec.y / 0.0208f),
                Mathf.FloorToInt(vec.x / 0.0208f)
            );
        }

        protected bool CellBeAttacked(Location location)
        {
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j) {
                    if (i == 0 && j == 0) continue;
                    int x = location.x + i, y = location.y + j;
                    while (Math.Min(x, y) >= 0 && Math.Max(x, y) < 8)
                    {
                        Pieces p = Grid.instance.GetValue(x, y);
                        if (p != null)
                        {
                            if (p.party != party)
                            {
                                if (p.type == ChessType.Queen) return true;
                                if ((i == 0 || j == 0) && p.type == ChessType.Rook) return true;
                                if (Math.Abs(i) == Math.Abs(j) && p.type == ChessType.Bishop) return true;
                                if (x == location.x + i && y == location.y + j)
                                {
                                    if(p.type == ChessType.King) return true;
                                    if (i == (int) party && Math.Abs(j) == 1 && p.type == ChessType.Pawn)
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;
                        }
                        x += i;
                        y += j;
                    }
                }
            }
            // Knight
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if(Math.Abs(i) == Math.Abs(j) || i*j==0) continue;
                    int toX = location.x + i;
                    int toY = location.y + j;
                    Pieces p = Grid.instance.GetValue(toX, toY);
                    if (Math.Min(toX, toY) >= 0 && Math.Max(toX, toY) < 8 &&
                        p != null && p.type == ChessType.Knight && p.party != party)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool MoveCheck(Location location)
        {
            Location l = moveSet.Find(m => m.Equals(location));
            return l != null;
        }
        
        protected bool IlLegalMove(Location location)
        {
            validating = true;
            MoveTo(location);
            foreach (var k in Board.instance.kings)
            {
                if (k.party == party && k.InCheck())
                {
                    //Debug.Log("king be attack when " + gameObject.name + " moved");
                    Board.instance.Undo(false);
                    validating = false;
                    return true;
                }
            }
            Board.instance.Undo(false);
            validating = false;
            return false;
        }

        public void UpdateMoveSet(bool validate = true)
        {
            //Debug.Log("moveset of "+gameObject + " at "+position.x + ":" + position.y + " size: "+moveSet.Count);
            if(killed) return;
            moveSet = GetMovement(validate);
            //Debug.Log(moveSet.Count + " move legal of "+ gameObject.name);
            updated = true;
        }

        public void TransformToPosition()
        {
            TransformTo(position);
        }
    }
}
