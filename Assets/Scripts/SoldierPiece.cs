public class SoldierPiece : ChessPiece
{
 
    public override bool CanMoveTo(int targetX, int targetY)
    {
        // 这里直接返回 false，表示不允许移动
        return false;
    }
}
