using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 棋盘管理器：
/// 1. 保存并管理棋盘上所有的棋子引用（board[x, y]）。
/// 2. 提供棋子放置、移除、查询等基础功能。
/// 3. 负责棋盘坐标与世界坐标之间的转换逻辑（根据具体需求配置）。
/// </summary>
public class ChessBoardManager : MonoBehaviour
{
    /// <summary>
    /// 单例（Singleton），方便在其他脚本中通过 ChessBoardManager.Instance 来访问
    /// </summary>
    public static ChessBoardManager Instance;

    /// <summary>
    /// 棋盘大小：9 列 × 10 行
    /// 数组下标：
    ///   x 取值 [0..8]
    ///   y 取值 [0..9]
    /// </summary>
    private ChessPiece[,] board = new ChessPiece[9, 10];

    /// <summary>
    /// 每个格子在 "X" 方向的宽度 & "Y" 方向(实际是Z轴)的高度
    /// 当我们希望 (0,0) ~ (8,9) 覆盖到 -42 ~ 42, 需要分别计算。
    ///   横向: 8 段 => 84 / 8 = 10.5
    ///   纵向: 9 段 => 84 / 9 = 9.333...（约 9.3333）
    /// </summary>
    [Header("棋盘格子大小配置")]
    [Tooltip("一个格子在 X 方向的世界单位大小")]
    public float cellWidth = 10f;
    [Tooltip("一个格子在 Y(实际Z) 方向的世界单位大小")]
    public float cellHeight = 10f;

    /// <summary>
    /// 棋盘原点 (0,0) 在世界坐标中的位置。
    /// 在这里我们设置为 (-42, 0, -42)，使得 (x=0, y=0) => (-42, -42)
    /// </summary>
    [Header("棋盘在世界坐标中的偏移")]
    public Vector3 boardOrigin = new Vector3(-45f, 0, -50f);

    private void Awake()
    {
        // 初始化单例
        Instance = this;

        // （可选）在游戏开始时，清空或初始化 board 数组
        ClearBoard();
    }

    /// <summary>
    /// 清空棋盘数据，将所有位置设为 null
    /// </summary>
    public void ClearBoard()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                board[x, y] = null;
            }
        }
    }

    /// <summary>
    /// 检查某个棋盘坐标 (x, y) 是否在合法范围内
    /// </summary>
    /// <param name="x">横坐标 [0..8]</param>
    /// <param name="y">纵坐标 [0..9]</param>
    /// <returns>true 表示在棋盘范围内；false 表示越界</returns>
    public bool IsValidPosition(int x, int y)
    {
        return (x >= 0 && x < 9 && y >= 0 && y < 10);
    }

    /// <summary>
    /// 根据棋盘坐标获取当前所在的棋子（可能为 null 表示没有棋子）
    /// </summary>
    /// <param name="x">棋盘坐标 x</param>
    /// <param name="y">棋盘坐标 y</param>
    /// <returns>对应位置的 ChessPiece，或 null</returns>
    public ChessPiece GetPieceAtPosition(int x, int y)
    {
        if (!IsValidPosition(x, y))
        {
            return null;
        }
        return board[x, y];
    }

    /// <summary>
    /// 获取可移动目的地
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public List<(int x,int y)> GetPieceCanMove(int x, int y,ChessPiece.PieceType pieceType)
    {
        List<(int x, int y)> item = new();

        if (pieceType == ChessPiece.PieceType.Soldier)
        {
            if(x - 1 >= 0)
            {
                if(GetPieceAtPosition(x - 1,y)== null)
                {
                    item.Add((x - 1, y));
                }      
            }
            if (y + 1 <= 9)
            {
                if (GetPieceAtPosition(x, y +1) == null)
                {
                    item.Add((x, y + 1));
                }
            }
            if (x + 1 <= 8)
            {
                if (GetPieceAtPosition(x +1, y) == null)
                {
                    item.Add((x + 1, y));
                }
            }
            return item;
        }
        return null;
    }


    /// <summary>
    /// 将某个棋子引用登记到棋盘的指定坐标上
    /// （如果原先那里有棋子，可能需先做移除；或者在外部逻辑中处理“吃子”）
    /// </summary>
    /// <param name="x">棋盘坐标 x</param>
    /// <param name="y">棋盘坐标 y</param>
    /// <param name="piece">要放置的棋子引用，可为 null</param>
    public void SetPieceAtPosition(int x, int y, ChessPiece piece)
    {
        if (IsValidPosition(x, y))
        {
            board[x, y] = piece;
        }
        else
        {
            Debug.LogWarning($"SetPieceAtPosition: 坐标({x},{y})超出范围！");
        }
    }

    /// <summary>
    /// 将棋盘坐标 (x, y) 转换为 Unity 世界坐标 (worldX, worldY, worldZ)
    /// 用于给棋子在场景中定位、移动
    /// </summary>
    /// <param name="x">棋盘坐标 x</param>
    /// <param name="y">棋盘坐标 y</param>
    /// <returns>对应的世界坐标</returns>
    public Vector3 BoardToWorldPos(int x, int y)
    {
        // 假设 (0,0) => 世界(-42, -42)，(8,9) => 世界(42,42)
        // 因此每步 X => 10.5， Y => 9.3333
        float worldX = boardOrigin.x + x * cellWidth;
        float worldZ = boardOrigin.z + y * cellHeight;

        // worldY 是棋子的高度（一般在地面，可设 0）
        float worldY = boardOrigin.y;

        return new Vector3(worldX, worldY, worldZ);
    }

    /// <summary>
    /// （可选）如果你需要鼠标点击来选格子，则可以实现“世界坐标 → 棋盘坐标”的逻辑。
    /// 示例仅供参考，可根据实际需求调整。
    /// </summary>
    public bool TryGetBoardCoordFromWorld(Vector3 worldPos, out int x, out int y)
    {
        // 把世界坐标减去 boardOrigin，然后分别除以 cellWidth 和 cellHeight
        float localX = (worldPos.x - boardOrigin.x) / cellWidth;
        float localY = (worldPos.z - boardOrigin.z) / cellHeight;

        // 四舍五入或向下取整
        x = Mathf.RoundToInt(localX);
        y = Mathf.RoundToInt(localY);

        // 检查是否在范围内
        if (!IsValidPosition(x, y))
        {
            return false;
        }
        return true;
    }
}
