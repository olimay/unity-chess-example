using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int[] directionX = {-1, 0, 1};
        int[] directionY = {-1, 0, 1};
        foreach (int dx in directionX) {
            foreach (int dy in directionY) {
                int targetX = currentX + dx;
                int targetY = currentY + dy;
                if ((Mathf.Pow(dx,2)+Mathf.Pow(dy,2) > 0) &&
                    targetX >= 0 && targetY >= 0 &&
                    targetX < tileCountX && targetY < tileCountY &&
                    (board[targetX,targetY] == null ||
                    board[targetX,targetY].team != team)) {
                    r.Add(new Vector2Int(targetX,targetY));
                }
            }
        }
        return r;
    }
}