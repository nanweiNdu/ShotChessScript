using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("预制体")]
    public GameObject blackRookPrefab;    // 拖入BlackRook.prefab
    public GameObject redSoldierPrefab;   // 拖入RedSoldier.prefab

    private void Start()
    {
        // 先生成4个黑车在四个角
        PlacePiece(blackRookPrefab, 0, 0); // 左下
        PlacePiece(blackRookPrefab, 8, 0); // 右下
        PlacePiece(blackRookPrefab, 0, 9); // 左上
        PlacePiece(blackRookPrefab, 8, 9); // 右上

        // 生成10个红兵在地图内部的随机位置
        // 不可以是 (0,0),(8,0),(0,9),(8,9)
        // 以及不可以越界 0..8, 0..9
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
        // 四个角： (0,0), (8,0), (0,9), (8,9)
        if ((x == 0 && y == 0) ||
            (x == 8 && y == 0) ||
            (x == 0 && y == 9) ||
            (x == 8 && y == 9))
            return true;
        return false;
    }

    private void PlacePiece(GameObject prefab, int x, int y)
    {
        // 实例化
        var obj = Instantiate(prefab);
        // 放到棋盘世界坐标
        Vector3 pos = ChessBoardManager.Instance.BoardToWorldPos(x, y);
        obj.transform.position = pos;

        // 设置棋子脚本坐标 & 注册到棋盘管理器
        ChessPiece piece = obj.GetComponent<ChessPiece>();
        piece.boardX = x;
        piece.boardY = y;
        ChessBoardManager.Instance.SetPieceAtPosition(x, y, piece);
    }
}
