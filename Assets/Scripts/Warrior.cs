using UnityEngine;

public class Warrior : MonoBehaviour
{
    private int actionCounter = 0; // �ж�����������ʼΪ 0
    private const int maxActionCounter = 3; // ����ж�������ֵ
    private ChessPiece chessPiece; // ��ǰ���ӵ� ChessPiece �ű�����

    private void Awake()
    {
        // ��ȡ������ͬһ�����ϵ� ChessPiece �ű�
        chessPiece = GetComponent<ChessPiece>();
        if (chessPiece == null)
        {
            Debug.LogError("Warrior ��Ҫ�������� ChessPiece ����Ķ����ϣ�");
        }
    }

    /// <summary>
    /// ��ȫ�ֻغϼ���������ʱ����
    /// </summary>
    public void OnGlobalTurnIncreased()
    {
        if (chessPiece == null)
            return;

        // �����ж�������
        actionCounter = Mathf.Min(actionCounter + 1, maxActionCounter);
        Debug.Log($"��ɫսʿ�ж����������ӣ�{actionCounter}");

        // ����Ƿ���Ҫ�Զ��ƶ�
        if (actionCounter == 2)
        {
            TryMoveRight();
            actionCounter = 0; // ����ж�������
        }
    }

    /// <summary>
    /// ���������ƶ�һ��
    /// </summary>
    private void TryMoveRight()
    {
        int targetX = chessPiece.boardX + 1; // �Ҳ���ӵ� x ����
        int targetY = chessPiece.boardY; // ���� y ���겻��

        // ���Ŀ������Ƿ������̷�Χ��
        if (!ChessBoardManager.Instance.IsValidPosition(targetX, targetY))
        {
            Debug.Log("�Ҳ�û�и��ӣ�սʿ�޷��ƶ���");
            return;
        }

        // ���Ŀ������ϵ�����
        ChessPiece targetPiece = ChessBoardManager.Instance.GetPieceAtPosition(targetX, targetY);

        if (targetPiece == null)
        {
            // Ŀ�����Ϊ�գ������ƶ�
            MoveTo(targetX, targetY);
        }
        else if (targetPiece.camp == chessPiece.camp)
        {
            // Ŀ�����Ϊ�ѷ����޷��ƶ�
            Debug.Log("�Ҳ���ӱ��Ѿ�ռ�ݣ�սʿ�޷��ƶ���");
        }
        else
        {
            // Ŀ�����Ϊ�з����ݻٵз����Ӳ�ռ����λ��
            Debug.Log($"�Ҳ���ӱ��з�����ռ�ݣ��ݻٵз����ӣ�{targetPiece.pieceType}");
            Destroy(targetPiece.gameObject);
            MoveTo(targetX, targetY);
        }
    }

    /// <summary>
    /// ִ���ƶ�����
    /// </summary>
    private void MoveTo(int targetX, int targetY)
    {
        // ��ȡĿ��λ�õ���������
        Vector3 targetWorldPos = ChessBoardManager.Instance.BoardToWorldPos(targetX, targetY);

        // ��������λ��
        chessPiece.transform.position = targetWorldPos;

        // ��������״̬
        ChessBoardManager.Instance.SetPieceAtPosition(targetX, targetY, chessPiece);
        ChessBoardManager.Instance.SetPieceAtPosition(chessPiece.boardX, chessPiece.boardY, null);

        // �������ӵ���������
        chessPiece.boardX = targetX;
        chessPiece.boardY = targetY;

        Debug.Log($"սʿ�ƶ��� ({targetX}, {targetY})");
    }
}
