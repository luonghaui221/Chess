using System;
using System.Collections;
using System.Collections.Generic;
using Resources.Script;
using Resources.Script.ChessPieces;
using Unity.VisualScripting;
using UnityEngine;

public class Log : MonoBehaviour
{
    public static Log instance;
    public List<History> log;
    public int top = -1;
    
    private void Awake()
    {
        instance = this;
        log = new List<History>();
    }

    public void Push(History history)    
    {
        // log.Add(history);
        top++;
        log.Add(history);
    }

    public History Peek()
    {
        if (IsEmpty()) return null;
        return log[top];
    }

    public History Pop()
    {
        History h = Peek();
        log.RemoveAt(Size() - 1);
        top--;
        return h;
    }

    public  bool IsEmpty()
    {
        return top == -1;
    }
    
    public int Size()
    {
        // return log.Count;
        return top+1;
    }

    public bool Contains(ChessType type, Party party)
    {
        return log.Exists(h =>
        {
            Pieces piece  = h.piece.GetComponent<Pieces>();
            return piece.party == party && piece.type == type;
        });
    }
    public bool Contains(ChessType type, Party party, Location from)
    {
        return log.Exists(h =>
        {
            Pieces piece  = h.piece.GetComponent<Pieces>();
            return piece.party == party && piece.type == type && h.from.Equals(from);
        });
    }

    public void Clear()
    {
        log.Clear();
        top = -1;
    }
}
