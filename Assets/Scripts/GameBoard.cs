using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessTeam {
    WhiteTeam,
    BlackTeam
}

public class GameBoard : MonoBehaviour
{

    // constants
    public static Vector2Int INVALID_TILE = -Vector2Int.one;
    private const float Y_OFFSET_DEFAULT = 0.2f;

    // defaults

    [Header("Art Stuff")]
    public Material tileMaterial;

    [Header("Board Attributes")]
    public bool boardActive = false;
    public int tileCountX = 8;
    public int tileCountY = 8;
    public float tileSize = 1.00f;
    public float yOffset = Y_OFFSET_DEFAULT;
    public float capturedSize = 1.5f;
    public float capturedSpacing = 0.3725f;
    public Vector3 boardCenter = Vector3.zero;
    public float dragOffset = 1.0f;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabsWhite;
    [SerializeField] private GameObject[] prefabsBlack;

    // Logic
    private ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> capturedWhite = new List<ChessPiece>();
    private List<ChessPiece> capturedBlack = new List<ChessPiece>();
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    // Lifecycle

    // Awake
    void Awake()
    {
        GenerateAllTiles(tileSize, tileCountX, tileCountY);
        SpawnAllPieces();
        PositionAllPieces();
        boardActive = true;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentCamera || !boardActive) {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight"))) {
            // Get indexes of tile I've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // no hover to hover
            if (currentHover == INVALID_TILE) {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            } 

            // hover tile to hover a different tile
            if (currentHover != hitPosition) {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover))
                    ? LayerMask.NameToLayer("Highlight")
                    : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            if (Input.GetMouseButtonDown(0)) {
                if (chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    // is it our turn?
                    if (true)
                    {
                        // Debug.Log("click and drag from "+hitPosition.x+","+hitPosition.y);
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];
                        // get list of all available moves and highlight tiles
                        availableMoves = currentlyDragging.GetAvailableMoves(
                            ref chessPieces,
                            tileCountX,
                            tileCountY
                        );
                        HighlightTiles();
                    }
                }
            }

            if (currentlyDragging != null && Input.GetMouseButtonUp(0)) {
                Vector2Int previousPosition = new Vector2Int(
                    currentlyDragging.currentX,
                    currentlyDragging.currentY
                );
                // Debug.Log("stop dragging at "+hitPosition.x+","+hitPosition.y);
                bool validMove = MoveTo(currentlyDragging,hitPosition.x,hitPosition.y);
                if (!validMove) {
                    // Debug.Log("invalid move");
                    currentlyDragging.SetPosition(
                        GetTileCenter(
                            previousPosition.x,
                            previousPosition.y
                        ),
                        false
                    );
                } 
                currentlyDragging = null;
                RemoveHighlightTiles();
            }

        } else {
            // hover to no hover
            if (currentHover != INVALID_TILE) {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover))
                    ? LayerMask.NameToLayer("Highlight")
                    : LayerMask.NameToLayer("Tile");
                currentHover = INVALID_TILE;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0)) {
                currentlyDragging.SetPosition(
                    GetTileCenter(
                        currentlyDragging.currentX,
                        currentlyDragging.currentY
                    ),
                    false
                );
                currentlyDragging = null;
                RemoveHighlightTiles();
            }
        } // raycast

        if (currentlyDragging) {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance)) {
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
            }
        }
    }

    // logic    
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3(tileCountX/2 * tileSize, 0, tileCountY/2 * tileSize) + boardCenter;
        this.tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++) {
            for (int y = 0; y < tileCountY; y++) {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("x:{0} y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh(); 
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = this.tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y+1) * tileSize) - bounds;
        vertices[2] = new Vector3((x+1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x+1) * tileSize, yOffset, (y+1) * tileSize) - bounds;

        int[] triangles = new int[] {0, 1, 2, 1, 3, 2};
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[tileCountX, tileCountY];

        // White Team
        chessPieces[0,0] = SpawnSinglePiece(ChessPieceType.Rook, ChessTeam.WhiteTeam);
        chessPieces[1,0] = SpawnSinglePiece(ChessPieceType.Knight, ChessTeam.WhiteTeam);
        chessPieces[2,0] = SpawnSinglePiece(ChessPieceType.Bishop, ChessTeam.WhiteTeam);
        chessPieces[3,0] = SpawnSinglePiece(ChessPieceType.Queen, ChessTeam.WhiteTeam);
        chessPieces[4,0] = SpawnSinglePiece(ChessPieceType.King, ChessTeam.WhiteTeam);
        chessPieces[5,0] = SpawnSinglePiece(ChessPieceType.Bishop, ChessTeam.WhiteTeam);
        chessPieces[6,0] = SpawnSinglePiece(ChessPieceType.Knight, ChessTeam.WhiteTeam);
        chessPieces[7,0] = SpawnSinglePiece(ChessPieceType.Rook, ChessTeam.WhiteTeam);
        for (int i = 0; i < tileCountX; i++) {
            chessPieces[i,1] = SpawnSinglePiece(ChessPieceType.Pawn, ChessTeam.WhiteTeam);
        }

        // Black Team
        chessPieces[0,7] = SpawnSinglePiece(ChessPieceType.Rook, ChessTeam.BlackTeam);
        chessPieces[1,7] = SpawnSinglePiece(ChessPieceType.Knight, ChessTeam.BlackTeam);
        chessPieces[2,7] = SpawnSinglePiece(ChessPieceType.Bishop, ChessTeam.BlackTeam);
        chessPieces[3,7] = SpawnSinglePiece(ChessPieceType.Queen, ChessTeam.BlackTeam);
        chessPieces[4,7] = SpawnSinglePiece(ChessPieceType.King, ChessTeam.BlackTeam);
        chessPieces[5,7] = SpawnSinglePiece(ChessPieceType.Bishop, ChessTeam.BlackTeam);
        chessPieces[6,7] = SpawnSinglePiece(ChessPieceType.Knight, ChessTeam.BlackTeam);
        chessPieces[7,7] = SpawnSinglePiece(ChessPieceType.Rook, ChessTeam.BlackTeam);
        for (int i = 0; i < tileCountX; i++) {
            chessPieces[i,6] = SpawnSinglePiece(ChessPieceType.Pawn, ChessTeam.BlackTeam);
        }
    }

    private ChessPiece SpawnSinglePiece(ChessPieceType type, ChessTeam team)
    {
        GameObject[] prefabs;
        switch (team) {
            case ChessTeam.WhiteTeam: prefabs = prefabsWhite; break;
            case ChessTeam.BlackTeam: prefabs = prefabsBlack; break;
            default: throw new System.Exception("invalid team!");
        }
        ChessPiece cp = Instantiate(prefabs[(int)type -1], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        // cp.GetComponent<MeshRender>().material = material; // <= if we wanted to change the material manually
        return cp;
    }

    // Positioning
    private void PositionAllPieces()
    {
        for (int x = 0; x < tileCountX; x++) {
            for (int y = 0; y < tileCountY; y++) {
                if (chessPieces[x,y] != null) {
                    PositionSinglePiece(x, y, false);
                }
            }
        }
    }

    private void PositionSinglePiece(int x, int y, bool animate = true)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), animate);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
       return (
           new Vector3(x * tileSize, yOffset, y * tileSize)
           - bounds
           + new Vector3(tileSize / 2, 0, tileSize / 2)
       );
    }
    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }

    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        } 
        availableMoves.Clear();
    }
/*
(ContainsValidMove(ref availableMoves, currentHover))
                ? LayerMask.NameToLayer("Highlight")
                : LayerMask.NameToLayer("Tile");
*/
    // Operations
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++) {
            if (moves[i].x == pos.x && moves[i].y == pos.y) {
                return true;
            }
        }
        return false;
    }

    private bool MoveTo(ChessPiece piece, int x, int y)
    {
        if (!ContainsValidMove(ref availableMoves, new Vector2(x,y))) return false;

        Vector2Int previousPosition = new Vector2Int(piece.currentX, piece.currentY);
        // todo: check if move is valid
        // is there another piece at target position?
        if (chessPieces[x,y] != null) {
            // Debug.Log("another piece in target square!");
            ChessPiece otherPiece = chessPieces[x, y];
            // if same team, move is invalid
            if (piece.team == otherPiece.team) return false;
            // if other team, take piece in target square
            int capCount = 0;
            Vector3 direction = Vector3.back;
            int xstart = 8;
            int zstart = -1;
            switch (otherPiece.team) {
                case ChessTeam.WhiteTeam: {
                    capturedWhite.Add(otherPiece);
                    capCount = capturedWhite.Count;
                    direction = Vector3.back;
                    xstart = -1;
                    zstart = 8;
                }  break;
                case ChessTeam.BlackTeam: {
                    capturedBlack.Add(otherPiece);
                    capCount = capturedBlack.Count;
                    direction = Vector3.forward;
                }  break;
            }
            otherPiece.SetPosition(
                new Vector3(xstart * tileSize, yOffset, zstart * tileSize)
                - bounds    // center of the board
                + new Vector3(tileSize / 2, 0, tileSize / 2) // center of the square
                + (direction * capturedSpacing * capCount)
            );
            otherPiece.SetScale(Vector3.one * capturedSize);
        }
        chessPieces[x, y] = piece;
        chessPieces[previousPosition.x, previousPosition.y] = null;
        PositionSinglePiece(x, y);
        // Debug.Log("moved");
        return true; // move was valid and successful
    }

    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < tileCountX; x++) {
            for (int y = 0; y < tileCountY; y++) {
                if (tiles[x,y] == hitInfo) {
                    return new Vector2Int(x, y);
                }
            }
        }
        return INVALID_TILE;
    }

} // GameBoard class
    