using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int direction = (team == ChessTeam.WhiteTeam) ? 1 : -1;
        // One forward
        if ((currentY + direction) > 0 && (currentY + direction) < tileCountY
            && board[currentX, currentY + direction] == null) {
            r.Add(new Vector2Int(currentX, currentY + direction));
        }
        // Two forward
        if (board[currentX, currentY + direction] == null &&
            board[currentX, currentY + 2*direction] == null &&
            ((team == ChessTeam.WhiteTeam && currentY == 1) ||
            (team == ChessTeam.BlackTeam && currentY == 6))) {
            r.Add(new Vector2Int(currentX, currentY + 2*direction));
        }
        // Attack one diagonal forward
        if (currentX > 0 && board[currentX - 1, currentY + direction] != null &&
            board[currentX - 1, currentY + direction].team != team) {
            r.Add(new Vector2Int(currentX - 1, currentY + direction));
        }
        if (currentX < (tileCountX-1) && board[currentX + 1, currentY + direction] != null &&
            board[currentX + 1, currentY + direction].team != team) {
            r.Add(new Vector2Int(currentX + 1, currentY + direction));
        }
        // todo: enpassant
        // todo: promotion
        return r;
    }
}
