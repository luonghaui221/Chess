using System;
using System.Collections.Generic;
using Resources.Script;
using Resources.Script.ChessPieces;
using UnityEngine;

public class King : Pieces
{
    void Start()
    {
        type = ChessType.King;
        moveSet = GetMovement();
    }
    public bool InCheck()
    {
        return CellBeAttacked(position);
    }

    private bool IsCastling(int toPosY)
    {
        int y = toPosY < 4 ? 0 : 7;
        int x = party == Party.White ? 0 : 7;
        int side = toPosY < 4 ? -1 : 1;
        bool kingNotHaveMoved = !Log.instance.Contains(ChessType.King, party);
        bool rookNotHaveMoved = !Log.instance.Contains(ChessType.Rook, party,new Location(x,y));
        bool kingCannotBeAttacked = CellBeAttacked(new Location(x, 4+side)) == false && 
                                                   CellBeAttacked(new Location(x, 4+side*2)) ==false;
        bool cellAreEmpty = Grid.instance.GetValue(x, 4 + side) == null &&
                           Grid.instance.GetValue(x, 4 + side * 2) == null;
        bool notInCheck = !InCheck();
        return cellAreEmpty && kingCannotBeAttacked && rookNotHaveMoved && kingNotHaveMoved && notInCheck;
    }
    
    public override void MoveTo(Location location)
    {
        Pieces dest = Grid.instance.GetValue(location.x, location.y);
        Pieces currPiece = gameObject.GetComponent<Pieces>();
        Location curr = position;
        MoveType moveType = MoveType.Normal;
        // remove piece
        if (dest != null)
        {
            dest.killed = true;
            dest.moveSet.Clear();
            dest.GetComponent<MeshRenderer>().enabled = false;
        }
        
        // move rook if is castling queen side
        if ((location.x == 0 || location.x == 7) && location.y == 2 && IsCastling(location.y))
        {
            //Debug.Log("move rook");
            Pieces rook = Grid.instance.GetValue(location.x, 0);
            rook.MoveTo(new Location(location.x, 3));
            dest = rook;
            moveType = MoveType.Castling;
        }
        // king side
        if ((location.x == 0 || location.x == 7) && location.y == 6 && IsCastling(location.y))
        {
            //Debug.Log("move rook");
            Pieces rook = Grid.instance.GetValue(location.x, 7);
            rook.MoveTo(new Location(location.x, 5));
            dest = rook;
            moveType = MoveType.Castling;
        }
        
        // move and remove in matrix
        Grid.instance.SetValue(location.x, location.y, currPiece);
        Grid.instance.SetValue(curr.x, curr.y, null);
        position = new Location(location.x,location.y);
        Log.instance.Push(new History(currPiece, new Location(curr.x, curr.y), location.Clone(), dest, moveType));
    }
    public override List<Location> GetMovement(bool validate = true)
    {
        List<Location> moveSet = new List<Location>();

        Location curr = position;
        
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j) {
                if (i == 0 && j == 0) continue;
                int x = curr.x + i, y = curr.y + j;
                if(Math.Min(x, y) >= 0 && Math.Max(x, y) < 8)
                {
                    Pieces p = Grid.instance.GetValue(x, y);
                    if (p != null)
                    {
                        if(p.party != party) moveSet.Add(new Location(x,y));
                    }
                    else
                    {
                        moveSet.Add(new Location(x,y));   
                    }
                }
            }
        }
        // castling
        int toPosX = party == Party.White ? 0 : 7;
        if (IsCastling(2))
        {
            moveSet.Add(new Location(toPosX,2));
        }

        if (IsCastling(6))
        {
            moveSet.Add(new Location(toPosX,6));
        }

        if(validate) moveSet.RemoveAll(IlLegalMove);
        return moveSet;
    }
}
