using System;
using System.Collections.Generic;
using UnityEngine;

namespace Resources.Script.ChessPieces
{
    public class Knight : Pieces
    {
        void Start()
        {
            type = ChessType.Knight;
            moveSet = GetMovement();
        }
        public override List<Location> GetMovement(bool validate = true)
        {
            List<Location> moveSet = new List<Location>();
            Location curr = position;

            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if(Math.Abs(i) == Math.Abs(j) || i*j==0) continue;
                    int toX = curr.x + i;
                    int toY = curr.y + j;
                    Pieces value = Grid.instance.GetValue(toX, toY);
                    if (Math.Min(toX, toY) >= 0 && Math.Max(toX, toY) < 8)
                    {
                        if (value != null)
                        {
                            if (value.party != party) moveSet.Add(new Location(toX, toY));
                        }
                        else
                        {
                            moveSet.Add(new Location(toX, toY));
                        }
                    
                    }
                }
            
            }

            if(validate) moveSet.RemoveAll(IlLegalMove);
            return moveSet;
        }
    }
}
