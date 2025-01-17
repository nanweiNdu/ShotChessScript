using UnityEngine;

public class PiecesSpawner : MonoBehaviour
{
    [Header("棋子贴图（Sprite）")]
    public Sprite pieceSprite;

    private void Start()
    {
        // 如果你已经在棋盘上挂了一个 ChessBoardManager 脚本，
        // 并且它有 BoardToWorldPos(x, y) 方法，
        // 就可以直接使用这个方法来计算每个格子在世界空间的坐标。

        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                // 1. 生成一个新的空物体，用于显示「棋子贴图」
                GameObject pieceObj = new GameObject($"ChessPiece_{x}_{y}");

                // 2. 设置它的在世界中的位置
                //    假设 ChessBoardManager.Instance.BoardToWorldPos(x, y) 能给出 (x,y) 在世界坐标的映射
                Vector3 worldPos = ChessBoardManager.Instance.BoardToWorldPos(x, y);
                pieceObj.transform.position = worldPos;

                // 3. 给这个空物体添加一个 SpriteRenderer，用来渲染 png
                SpriteRenderer sr = pieceObj.AddComponent<SpriteRenderer>();
                sr.sprite = pieceSprite;   // 指定你在 Inspector 拖入的「棋子精灵」

                // 4. 你可以根据需要，调整 Sorting Layer 或者 Order in Layer
                // sr.sortingOrder = 1;
            }
        }
    }
}
