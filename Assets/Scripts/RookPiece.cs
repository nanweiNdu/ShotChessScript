using UnityEngine;

public class RookPiece : ChessPiece
{
    public override bool CanMoveTo(int targetX, int targetY)
    {
        // 车：只能横向或纵向移动
        if (targetX != boardX && targetY != boardY)
        {
            return false;
        }

        // 检查中间是否有己方或敌方棋子阻挡（车不能越子）
        if (targetX == boardX)
        {
            // 垂直移动
            int step = (targetY > boardY) ? 1 : -1;
            for (int yCheck = boardY + step; yCheck != targetY; yCheck += step)
            {
                var piece = ChessBoardManager.Instance.GetPieceAtPosition(boardX, yCheck);
                if (piece != null)
                {
                    // 中途遇到棋子 -> 阻挡
                    return false;
                }
            }
        }
        else
        {
            // 水平移动
            int step = (targetX > boardX) ? 1 : -1;
            for (int xCheck = boardX + step; xCheck != targetX; xCheck += step)
            {
                var piece = ChessBoardManager.Instance.GetPieceAtPosition(xCheck, boardY);
                if (piece != null)
                {
                    // 中途遇到棋子 -> 阻挡
                    return false;
                }
            }
        }

        // 通过了检查 -> 可以移动
        return true;
    }
}
