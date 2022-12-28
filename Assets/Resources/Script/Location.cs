using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location
{
    public int x;
    public int y;
    public Location(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        var item = obj as Location;
        if(item == null) return false;
        return this.x == item.x && this.y == item.y;
    }

    public override int GetHashCode()
    {
        return (x + "" + y).GetHashCode();
    }

    public bool Illegal()
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8) return true;
        return false;
    }

    public Location Clone()
    {
        return new Location(x, y);
    }
}
