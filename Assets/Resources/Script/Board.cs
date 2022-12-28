using System;
using System.Collections;
using Resources.Script.ChessPieces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Resources.Script
{
    public class Board : MonoBehaviour
    {
        public static Board instance;
        
        private AudioSource audioSource;
        private Grid grid;
        private Log log;
        
        [HideInInspector] public Pieces selectedPiece;
        [HideInInspector] public King[] kings;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PromoteController promoteController;
        [SerializeField] private GameObject transition;
        public Action onBotTurn;
        private void Awake()
        {
            instance = this;
            grid = GetComponent<Grid>();
            log = GetComponent<Log>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            kings = FindObjectsOfType<King>();
            transition.GetComponent<Transition>().StartScene();
        }

        void Update()
        {   
            if (Input.GetMouseButtonDown(0))
            {
                Location to = SelectPiece();
                if(to == null) return;
                MoveExcution(to);
                gameManager.CheckMate();
                gameManager.MoveDeprecated();
            }
        }

        private void MoveExcution(Location to)
        {
            if (selectedPiece.MoveCheck(to))
            {
                selectedPiece.MoveTo(to);
                selectedPiece.TransformTo(to);
                if (log.Peek().type == MoveType.Castling)
                {
                    log.Peek().catched.TransformTo(log.Peek().catched.position);
                }
                audioSource.Play();
                gameManager.ClearHighLight();
                selectedPiece = null;
                gameManager.SwapTurn();
                if (gameManager.modePve && gameManager.Turn == Party.Black)
                {
                    onBotTurn?.Invoke();
                }
            }
        }

        public void UndoMove()
        {
            History h = Undo();
            if (h.type == MoveType.Castling)
            {
                h.catched.TransformToPosition();
            }
            h?.piece.GetComponent<Pieces>().TransformTo(h.from);
        }

        public History Undo(bool swapTurn = true)
        {
            History lastTurn = log.Size() > 0 ? log.Pop() : null;
            if (lastTurn == null) return null;
            if (selectedPiece != null)
            {
                gameManager.ClearHighLight();
            }
            lastTurn.piece.updated = false;
            grid.SetValue(lastTurn.from, lastTurn.piece);
            if (lastTurn.catched != null)
            {
                Pieces p = lastTurn.catched.GetComponent<Pieces>();
                switch (lastTurn.type)
                {
                    case MoveType.Enpassant:
                        int offset = p.party == Party.White ? 1 : -1;
                        Location pawnLocation = new Location(lastTurn.to.x + offset, lastTurn.to.y);
                        p.position = pawnLocation;
                        grid.SetValue(pawnLocation, lastTurn.catched);
                        grid.SetValue(lastTurn.to, null);
                        break;
                    case MoveType.Promote:
                        if (!lastTurn.piece.validating)
                        {
                            lastTurn.piece = promoteController.ConvertPiece(lastTurn.piece,ChessType.Pawn);
                            grid.SetValue(lastTurn.to, lastTurn.catched);
                        }
                        break;
                    case MoveType.Castling:
                        grid.SetValue(lastTurn.to, null);
                        if (lastTurn.piece.validating)
                        {
                            Undo(false);
                        }
                        else
                        {
                            Undo(true);
                        }
                        break;
                    default:
                        grid.SetValue(lastTurn.to, lastTurn.catched);
                        break;
                }
                p.killed = false;
                p.updated = false;
                p.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                grid.SetValue(lastTurn.to.x, lastTurn.to.y, null);
            }
            GameManager.instance.MoveDeprecated();
            lastTurn.piece.position = lastTurn.from;
            
            if(swapTurn && lastTurn.type != MoveType.Castling) gameManager.SwapTurn();
            return lastTurn;
        }

        
        
        public Location SelectPiece()
        {
            if (promoteController.ActiveInHierachy()) return null;
            if (gameManager.modePve && gameManager.Turn == Party.Black) return null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1.0f))
            {
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.green, 10.0f);
                if (hit.transform.gameObject.CompareTag("board"))
                {
                    Location to = Pieces.GetXY(hit.point);
                    if (selectedPiece == null)
                    {
                        selectedPiece = grid.GetValue(to.x, to.y);
                        if (selectedPiece != null && selectedPiece.party == gameManager.Turn)
                        {
                            if (!selectedPiece.updated)
                            {
                                selectedPiece.UpdateMoveSet();
                            }
                            gameManager.HighLight(selectedPiece);
                            return null;
                        }
                        return null;
                    }
                    Pieces cellClick = grid.GetValue(to.x, to.y);
                    if (cellClick != null && cellClick.party == gameManager.Turn)
                    {
                        selectedPiece = cellClick;
                        if (!selectedPiece.updated)
                        {
                            selectedPiece.UpdateMoveSet();
                        }
                        gameManager.HighLight(selectedPiece);
                        return null;
                    }
                    return to;
                }
            }
            return null;
        }

        public void PlayAudio()
        {
            audioSource.Play();
        }
    }
}