using UnityEngine;

public class RookPiece : ChessPiece
{
    public override bool CanMoveTo(int targetX, int targetY)
    {
        // ����ֻ�ܺ���������ƶ�
        if (targetX != boardX && targetY != boardY)
        {
            return false;
        }

        // ����м��Ƿ��м�����з������赲��������Խ�ӣ�
        if (targetX == boardX)
        {
            // ��ֱ�ƶ�
            int step = (targetY > boardY) ? 1 : -1;
            for (int yCheck = boardY + step; yCheck != targetY; yCheck += step)
            {
                var piece = ChessBoardManager.Instance.GetPieceAtPosition(boardX, yCheck);
                if (piece != null)
                {
                    // ��;�������� -> �赲
                    return false;
                }
            }
        }
        else
        {
            // ˮƽ�ƶ�
            int step = (targetX > boardX) ? 1 : -1;
            for (int xCheck = boardX + step; xCheck != targetX; xCheck += step)
            {
                var piece = ChessBoardManager.Instance.GetPieceAtPosition(xCheck, boardY);
                if (piece != null)
                {
                    // ��;�������� -> �赲
                    return false;
                }
            }
        }

        // ͨ���˼�� -> �����ƶ�
        return true;
    }
}
