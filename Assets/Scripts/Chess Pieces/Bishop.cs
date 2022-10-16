using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int[] directionsX = {-1, 1};
        int[] directionsY = {-1, 1};

        foreach (int dx in directionsX) {
            foreach (int dy in directionsY) {
                for (int x = currentX + dx, y = currentY + dy; x >= 0 && y >= 0 && x < tileCountX && y < tileCountY;
                    x += dx, y += dy) {
                    if (board[x,y] == null || (board[x,y].team != team)) {
                        r.Add(new Vector2Int(x,y));
                    }
                    if (board[x,y]) {
                        break;
                    }
                }
            }
        }

        return r;
    }
}
