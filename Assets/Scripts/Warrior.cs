using UnityEngine;

public class Warrior : MonoBehaviour
{
    private int actionCounter = 0; // 行动计数器，初始为 0
    private const int maxActionCounter = 3; // 最大行动计数器值
    private ChessPiece chessPiece; // 当前棋子的 ChessPiece 脚本引用

    private void Awake()
    {
        // 获取挂载在同一对象上的 ChessPiece 脚本
        chessPiece = GetComponent<ChessPiece>();
        if (chessPiece == null)
        {
            Debug.LogError("Warrior 需要挂载在有 ChessPiece 组件的对象上！");
        }
    }

    /// <summary>
    /// 当全局回合计数器增加时调用
    /// </summary>
    public void OnGlobalTurnIncreased()
    {
        if (chessPiece == null)
            return;

        // 增加行动计数器
        actionCounter = Mathf.Min(actionCounter + 1, maxActionCounter);
        Debug.Log($"黑色战士行动计数器增加：{actionCounter}");

        // 检查是否需要自动移动
        if (actionCounter == 2)
        {
            TryMoveRight();
            actionCounter = 0; // 清空行动计数器
        }
    }

    /// <summary>
    /// 尝试向右移动一格
    /// </summary>
    private void TryMoveRight()
    {
        int targetX = chessPiece.boardX + 1; // 右侧格子的 x 坐标
        int targetY = chessPiece.boardY; // 保持 y 坐标不变

        // 检查目标格子是否在棋盘范围内
        if (!ChessBoardManager.Instance.IsValidPosition(targetX, targetY))
        {
            Debug.Log("右侧没有格子，战士无法移动！");
            return;
        }

        // 检查目标格子上的内容
        ChessPiece targetPiece = ChessBoardManager.Instance.GetPieceAtPosition(targetX, targetY);

        if (targetPiece == null)
        {
            // 目标格子为空，正常移动
            MoveTo(targetX, targetY);
        }
        else if (targetPiece.camp == chessPiece.camp)
        {
            // 目标格子为友方，无法移动
            Debug.Log("右侧格子被友军占据，战士无法移动！");
        }
        else
        {
            // 目标格子为敌方，摧毁敌方棋子并占据其位置
            Debug.Log($"右侧格子被敌方棋子占据，摧毁敌方棋子：{targetPiece.pieceType}");
            Destroy(targetPiece.gameObject);
            MoveTo(targetX, targetY);
        }
    }

    /// <summary>
    /// 执行移动操作
    /// </summary>
    private void MoveTo(int targetX, int targetY)
    {
        // 获取目标位置的世界坐标
        Vector3 targetWorldPos = ChessBoardManager.Instance.BoardToWorldPos(targetX, targetY);

        // 更新棋子位置
        chessPiece.transform.position = targetWorldPos;

        // 更新棋盘状态
        ChessBoardManager.Instance.SetPieceAtPosition(targetX, targetY, chessPiece);
        ChessBoardManager.Instance.SetPieceAtPosition(chessPiece.boardX, chessPiece.boardY, null);

        // 更新棋子的棋盘坐标
        chessPiece.boardX = targetX;
        chessPiece.boardY = targetY;

        Debug.Log($"战士移动到 ({targetX}, {targetY})");
    }
}
