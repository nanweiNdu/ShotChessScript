using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    // 阵营 & 棋子类型
    public Camp camp;
    public PieceType pieceType;
    // 棋盘坐标
    public int boardX;
    public int boardY;
    // 是否选中
    public bool isSelected = false;

    public SpineAnimationController spineCtrl;
    public virtual bool CanMoveTo(int targetX, int targetY)
    {
        // 默认不允许，子类中具体实现
        return false;
    }
    /// <summary>
    /// 当鼠标在 3D 物体上点击（需有 Collider）时，会调用此方法
    /// 默认只允许 "黑方" 棋子被点击触发示例逻辑，你可自行调整
    /// </summary>
    

    // 枚举放在同一个脚本里也可以，或单独放 ChessDefine.cs
    public enum Camp
    {
        Red,
        Black
    }

    public enum PieceType
    {
        Soldier,// 兵/卒
        Rook,  // 车
        Knight,//(马),
        Cannon,//(炮),
        King,   //(将), ...
        Then    //士
    }


}
