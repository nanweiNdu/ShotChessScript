using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Ԥ����")]
    public GameObject blackRookPrefab;    // ����BlackRook.prefab
    public GameObject redSoldierPrefab;   // ����RedSoldier.prefab

    private void Start()
    {
        // ������4���ڳ����ĸ���
        PlacePiece(blackRookPrefab, 0, 0); // ����
        PlacePiece(blackRookPrefab, 8, 0); // ����
        PlacePiece(blackRookPrefab, 0, 9); // ����
        PlacePiece(blackRookPrefab, 8, 9); // ����

        // ����10������ڵ�ͼ�ڲ������λ��
        // �������� (0,0),(8,0),(0,9),(8,9)
        // �Լ�������Խ�� 0..8, 0..9
        for (int i = 0; i < 10; i++)
        {
            int rx, ry;
            do
            {
                rx = Random.Range(0, 9);   // 0..8
                ry = Random.Range(0, 10);  // 0..9
            }
            while (IsCorner(rx, ry) || ChessBoardManager.Instance.GetPieceAtPosition(rx, ry) != null);

            PlacePiece(redSoldierPrefab, rx, ry);
        }
    }

    private bool IsCorner(int x, int y)
    {
        // �ĸ��ǣ� (0,0), (8,0), (0,9), (8,9)
        if ((x == 0 && y == 0) ||
            (x == 8 && y == 0) ||
            (x == 0 && y == 9) ||
            (x == 8 && y == 9))
            return true;
        return false;
    }

    private void PlacePiece(GameObject prefab, int x, int y)
    {
        // ʵ����
        var obj = Instantiate(prefab);
        // �ŵ�������������
        Vector3 pos = ChessBoardManager.Instance.BoardToWorldPos(x, y);
        obj.transform.position = pos;

        // �������ӽű����� & ע�ᵽ���̹�����
        ChessPiece piece = obj.GetComponent<ChessPiece>();
        piece.boardX = x;
        piece.boardY = y;
        ChessBoardManager.Instance.SetPieceAtPosition(x, y, piece);
    }
}
