using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    // ��Ӫ & ��������
    public Camp camp;
    public PieceType pieceType;
    // ��������
    public int boardX;
    public int boardY;
    // �Ƿ�ѡ��
    public bool isSelected = false;

    public SpineAnimationController spineCtrl;
    public virtual bool CanMoveTo(int targetX, int targetY)
    {
        // Ĭ�ϲ����������о���ʵ��
        return false;
    }
    /// <summary>
    /// ������� 3D �����ϵ�������� Collider��ʱ������ô˷���
    /// Ĭ��ֻ���� "�ڷ�" ���ӱ��������ʾ���߼���������е���
    /// </summary>
    

    // ö�ٷ���ͬһ���ű���Ҳ���ԣ��򵥶��� ChessDefine.cs
    public enum Camp
    {
        Red,
        Black
    }

    public enum PieceType
    {
        Soldier,// ��/��
        Rook,  // ��
        Knight,//(��),
        Cannon,//(��),
        King,   //(��), ...
        Then    //ʿ
    }


}
