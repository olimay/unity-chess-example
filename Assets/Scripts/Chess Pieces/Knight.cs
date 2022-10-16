using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        // knight can move to eight different squares
        Vector2Int[] moves = {
            new Vector2Int(currentX+2, currentY+1), // PI/6
            new Vector2Int(currentX+1, currentY+2), // PI/3
            new Vector2Int(currentX-1, currentY+2), // 2PI/3
            new Vector2Int(currentX-2, currentY+1), // 5PI/6
            new Vector2Int(currentX-2, currentY-1), // 7PI/6
            new Vector2Int(currentX-1, currentY-2), // 
            new Vector2Int(currentX+1, currentY-2), 
            new Vector2Int(currentX+2, currentY-1)
        };
        for (int i = 0; i < moves.Length; i++) {
            if (moves[i].x >= 0 && moves[i].x < tileCountX &&
                moves[i].y >= 0 && moves[i].y < tileCountY &&
                (board[moves[i].x, moves[i].y] == null ||
                 board[moves[i].x, moves[i].y].team != team)) {
                    r.Add(moves[i]);
                }
        }
        return r;
    }
}
