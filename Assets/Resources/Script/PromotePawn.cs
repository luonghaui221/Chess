using Resources.Script.ChessPieces;
using UnityEngine;

public class PromotePawn : MonoBehaviour
{
    public ChessType Type;

    public GameObject PromotePiece;

    public GameObject queen;
    public GameObject bishop;
    public GameObject knight;
    public GameObject rook;
    public GameObject pawn;
    
    public void clickOnImage(int type){
        switch (type)
        {
            case 1:
                PromotePiece = rook;
                Type = ChessType.Rook;
                break;
            case 2:
                PromotePiece = knight;
                Type = ChessType.Knight;
                break;
            case 3:
                PromotePiece = bishop;
                Type = ChessType.Bishop;
                break;
            case 4:
                PromotePiece = queen;
                Type = ChessType.Queen;
                break;
        }
    }

}
