using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Resources.Script.ChessPieces;

[Serializable]
public class Grid : MonoBehaviour
{
    
    public Pieces[,] gridArray;
    public static Grid instance;

    private void Awake()
    {
        gridArray = new Pieces[8, 8];
        instance = this;
    }

    public Pieces[,] Get()
    {
        return gridArray;
    }
    public void SetValue(int x, int y, Pieces value)
    {
        if (x >= 0 && y >= 0 && x < 8 && y < 8)
        {
            gridArray[x, y] = value;
        }
    }
    
    public void SetValue(Location vec, Pieces value)
    {
        int x = vec.x, y = vec.y;
        if (x >= 0 && y >= 0 && x < 8 && y < 8)
        {
            gridArray[x, y] = value;
        }
    }

    public Pieces GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < 8 && y < 8)
        {
            return gridArray[x, y];
        }
        else
        {
            return null;
        }
    }
    public Pieces GetValue(Location vec)
    {
        int x = vec.x, y = vec.y;
        if (x >= 0 && y >= 0 && x < 8 && y < 8)
        {
            return gridArray[x, y];
        }
        else
        {
            return null;
        }
    }

}
