using Resources.Script;
using Resources.Script.ChessPieces;
using UnityEngine;

public class PromoteController : MonoBehaviour
{
    [SerializeField] private PromotePawn whitePanel;
    [SerializeField] private PromotePawn blackPanel;
    private PromotePawn panel;
    
    private void Update()
    {
        if (panel != null)
        {
            if(panel.PromotePiece == null) return;
            History h = Log.instance.Peek();
            if (h.piece.type != ChessType.Pawn) return;
            ConvertPiece(h.piece, panel.Type);
            panel.PromotePiece = null;
            DeActive();
        }
    }

    public Pieces ConvertPiece(Pieces pieces, ChessType toType)
    {
        if (pieces == null) return null;
        if(pieces.type == ChessType.King || toType == ChessType.King) return null;
        if (pieces.type == toType) return null;
        GameObject toGameObject = null;
        GameObject fromGameObject = pieces.gameObject;
        Location oldPos = pieces.position.Clone();
        Party oldParty = pieces.party;
        Vector3 oldTransform = pieces.transform.position;

        switch (toType)
        {
            case ChessType.Rook:
                toGameObject = oldParty == Party.White ? whitePanel.rook : blackPanel.rook;
                break;
            case ChessType.Bishop:
                toGameObject = oldParty == Party.White ? whitePanel.bishop : blackPanel.bishop;
                break;
            case ChessType.Knight:
                toGameObject = oldParty == Party.White ? whitePanel.knight : blackPanel.knight;
                break;
            case ChessType.Queen:
                toGameObject = oldParty == Party.White ? whitePanel.queen : blackPanel.queen;
                break;
            case ChessType.Pawn:
                toGameObject = oldParty == Party.White ? whitePanel.pawn : blackPanel.pawn;
                break;
        }
        
        oldTransform.z = toGameObject.transform.position.z;
        GameManager.instance.keysInDict.Remove(pieces);
        GameManager.instance.piecesPos.Remove(pieces);
        
        GameObject g = Instantiate(toGameObject, oldTransform, Quaternion.Euler(90,0,180));
        pieces = g.GetComponent<Pieces>();
        pieces.position = oldPos;
        Log.instance.log.ForEach(h =>
        {
            if (h.piece == fromGameObject.GetComponent<Pieces>())
            {
                h.piece = pieces;
            }
        });
        Destroy(fromGameObject);
        GameManager.instance.piecesPos.Add(pieces,pieces.position);
        GameManager.instance.keysInDict.Add(pieces);
        return pieces;
    }
    
    public void Active()
    {
        if (GameManager.instance.Turn == Party.White)
        {
            whitePanel.gameObject.SetActive(true);
            blackPanel.gameObject.SetActive(false);
            panel = whitePanel;
        }
        else
        {
            whitePanel.gameObject.SetActive(false);
            blackPanel.gameObject.SetActive(true);
            panel = blackPanel;
        }
        
    }

    public void DeActive()
    {
        whitePanel.gameObject.SetActive(false);
        blackPanel.gameObject.SetActive(false);
    }

    public bool ActiveInHierachy()
    {
        return whitePanel.gameObject.activeInHierarchy || blackPanel.gameObject.activeInHierarchy;
    }
}
