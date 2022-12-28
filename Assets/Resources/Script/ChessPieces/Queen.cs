using System;
using System.Collections.Generic;
using UnityEngine;

namespace Resources.Script.ChessPieces
{
    public class Queen : Pieces
    {
        void Start()
        {
            type = ChessType.Queen;
            moveSet = GetMovement();
        }
        public override List<Location> GetMovement(bool validate = true){
            List<Location> moveSet = new List<Location>();
            Location curr = position;
        
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j) {
                    if (i == 0 && j == 0) continue;
                    int x = curr.x + i, y = curr.y + j;
                    while (Math.Min(x, y) >= 0 && Math.Max(x, y) < 8)
                    {
                        Pieces p = Grid.instance.GetValue(x, y);
                        if (p != null)
                        {
                            if(p.party != party) moveSet.Add(new Location(x,y));
                            break;
                        }
                        moveSet.Add(new Location(x,y));
                        x += i;
                        y += j;
                    }
                }
            }

            if(validate) moveSet.RemoveAll(IlLegalMove);
            return moveSet;
        }
    }
}
