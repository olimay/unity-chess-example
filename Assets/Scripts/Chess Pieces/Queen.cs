using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        // Rank
        for (int i = currentY - 1; i >= 0; i--) {
            if (board[currentX, i] == null) {
                r.Add(new Vector2Int(currentX, i));
            } else {
                if (board[currentX, i].team != team) {
                    r.Add(new Vector2Int(currentX, i));
                }
                break;
            }
        }
        for (int i = currentY + 1; i < tileCountY; i++) {
            if (board[currentX, i] == null) {
                r.Add(new Vector2Int(currentX, i));
            } else {
                if (board[currentX, i].team != team) {
                    r.Add(new Vector2Int(currentX, i));
                }
                break;
            }
        }
        // File
        for (int j = currentX - 1; j >= 0; j--) {
            if (board[j, currentY] == null) {
                r.Add(new Vector2Int(j, currentY));
            } else {
                if (board[j, currentY].team != team) {
                    r.Add(new Vector2Int(j, currentY));
                }
                break;
            }
        }
        for (int j = currentX + 1; j < tileCountX; j++) {
            if (board[j, currentY] == null) {
                r.Add(new Vector2Int(j, currentY));
            } else {
                if (board[j, currentY].team != team) {
                    r.Add(new Vector2Int(j, currentY));
                }
                break;
            }
        }

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
