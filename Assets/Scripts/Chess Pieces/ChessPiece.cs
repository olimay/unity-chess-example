using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public class ChessPiece : MonoBehaviour
{
    public ChessTeam team;
    public int currentX;
    public int currentY;
    public ChessPieceType type;

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;
    public float moveDuration = 0.5f;
    public float scaleDuration = 0.5f;
    public virtual void SetPosition(Vector3 position, bool animate = true)
    {
        desiredPosition = position;
        if (!animate) {
            Debug.Log("skipping move animation");
            transform.position = desiredPosition;
        }
    }
    public virtual void SetScale(Vector3 scale, bool animate = true)
    {
        desiredScale = scale;
        Debug.Log("setting scale to " + desiredScale);
        if (!animate) {
            Debug.Log("skipping scale animation");
            transform.localScale = desiredScale;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // StartCoroutine(LerpPosition(transform.position, desiredPosition)); // this is the right way to do it, but it's flickering...
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 5/moveDuration * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, 5/scaleDuration * Time.deltaTime);
    }
    /*
    IEnumerator LerpPosition(Vector3 startPosition, Vector3 endPosition)
    {
        float t = 0;
        while (t < moveDuration) {
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition;
    }

    IEnumerator LerpScale()
    {
        float t = 0;
        Vector3 startScale = transform.localScale;
        while (t < scaleDuration) {
            transform.localScale = Vector3.Lerp(startScale, desiredScale, t);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = desiredScale;
    }
    // */
    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        // debug
        r.Add(new Vector2Int(3, 3));
        r.Add(new Vector2Int(3, 4));
        r.Add(new Vector2Int(4, 3));
        r.Add(new Vector2Int(4, 4));
        return r;
    }
}

