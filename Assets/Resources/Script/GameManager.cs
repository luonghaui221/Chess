using System.Collections.Generic;
using Resources.Script.ChessPieces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

namespace Resources.Script
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public PromoteController promoteController;
        public Party _turn = Party.White;
        private King wKing;
        private King bKing;
        private bool Updating;
        public Party Turn => _turn;
        public Dictionary<Pieces, List<Location>> moveSet;
        public List<GameObject> piecesPrefab;
        public GameObject highLightPrefab;
        public bool modePve;
        
        [HideInInspector] public Dictionary<Pieces, Location> piecesPos;
        [HideInInspector] public List<Pieces> keysInDict;
        [SerializeField] private GameObject endGame;
        [SerializeField] private Text message;

        public int totalMoveOfParty
        {
            get
            {
                int count = 0;
                foreach (var kvp in piecesPos)
                {
                    if (kvp.Key.party == _turn)
                    {
                        count += kvp.Key.moveSet.Count;
                    }
                }

                return count;
            }
        }
        [HideInInspector] public List<GameObject> highlightInstance;
        private List<Renderer> outlines;
        private void Awake()
        {
            instance = this;
            highlightInstance = new List<GameObject>();
            piecesPos = new Dictionary<Pieces, Location>();
            outlines = new List<Renderer>();
        }

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            PositionChange();
        }

        private void Init()
        {
            piecesPrefab.ForEach(c =>
            {
                Location location = Pieces.GetXY(c.gameObject.transform.position);
                GameObject g = Instantiate(c);
                Pieces p = g.GetComponent<Pieces>();
                if (g.name.Contains("Pawn"))
                {
                    for (int i = 1; i <= 7; i++)
                    {
                        GameObject gpawn = Instantiate(c);
                        Pieces pawn = gpawn.GetComponent<Pieces>();
                        pawn.transform.position += new Vector3(i * 0.0209f,0,0);
                        Location pawnPos = Pieces.GetXY(pawn.transform.position);
                        pawn.position = pawnPos;
                        Grid.instance.SetValue(pawnPos, pawn);
                        piecesPos.Add(pawn,pawnPos);
                    }
                }
                p.position = location;
                Grid.instance.SetValue(location, p);
                piecesPos.Add(p,location);
            });
            keysInDict = new List<Pieces>(piecesPos.Keys);
            foreach (var king in FindObjectsOfType<King>())
            {
                if (king.party == Party.White) wKing = king;
                else bKing = king;
            }
            modePve = GlobalStorage.modePve;
        }

        private void PositionChange()
        {
            foreach (var k in keysInDict)
            {
                if (k.killed == false && k.validating == false &&
                    Updating == false && !k.position.Equals(piecesPos[k]))
                {
                    piecesPos[k] = k.position;
                    k.UpdateMoveSet(false);
                }
            }
        }

        public void UpdateAllMoveSetOfParty(Party party)
        {
            Updating = true;
            foreach (var k in keysInDict)
            {
                if (k.party == party)
                {
                    piecesPos[k] = k.position;
                    k.UpdateMoveSet();
                }
            }

            Updating = false;
        }
        
        public void CheckMate()
        {
            King selected = _turn == Party.White ? wKing : bKing;
            if (selected.InCheck())
            {
                UpdateAllMoveSetOfParty(selected.party);
                if (totalMoveOfParty == 0)
                {
                    string color = _turn == Party.White ? "đen" : "trắng";
                    message.text = "Quân " + color + " đã thắng!";
                    endGame.SetActive(true);
                }
            }
        }
        
        public void SwapTurn()
        {
            //Debug.Log("swap turn");
            _turn = _turn == Party.White ? Party.Black : Party.White;
            
        }

        public void HighLight(Pieces p)
        {
            if(p.killed) return;
            ClearHighLight();
            p.gameObject.GetComponent<Outline>().eraseRenderer = false;
            p.moveSet.ForEach(v =>
            {
                Pieces value = Grid.instance.GetValue(v.x, v.y);
                if (value != null)
                {
                    Renderer r = value.GetComponent<Renderer>();
                    outlines.Add(r);
                    r.material.EnableKeyword("_EMISSION");
                    r.material.SetColor("_EmissionColor", new Color(0.867f, 0.325f, 0.325f, 0.5f));
                }
                else
                {
                    GameObject go = Instantiate(highLightPrefab);
                    go.transform.position += new Vector3(
                        v.y * 0.0209f,
                        v.x * 0.0208f,
                        0
                    );
                    highlightInstance.Add(go);
                }
            });
        }

        public void ClearHighLight()
        {
            highlightInstance.ForEach(Destroy);
            outlines.ForEach(ol => ol.material.DisableKeyword("_EMISSION"));
            foreach (var kvp in piecesPos)
            {

                kvp.Key.GetComponent<Outline>().eraseRenderer = true;
            }
            highlightInstance.Clear();
            outlines.Clear();
        }

        public void ClearPiece()
        {
            foreach (var kvp in piecesPos)
            {
                Destroy(kvp.Key.gameObject);
            }
            piecesPos.Clear();
        }

        public void MoveDeprecated()
        {
            keysInDict.ForEach(k => k.updated = false);
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
            ClearPiece();
        }
    }
}