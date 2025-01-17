using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���̹�������
/// 1. ���沢�������������е��������ã�board[x, y]����
/// 2. �ṩ���ӷ��á��Ƴ�����ѯ�Ȼ������ܡ�
/// 3. ����������������������֮���ת���߼������ݾ����������ã���
/// </summary>
public class ChessBoardManager : MonoBehaviour
{
    /// <summary>
    /// ������Singleton���������������ű���ͨ�� ChessBoardManager.Instance ������
    /// </summary>
    public static ChessBoardManager Instance;

    /// <summary>
    /// ���̴�С��9 �� �� 10 ��
    /// �����±꣺
    ///   x ȡֵ [0..8]
    ///   y ȡֵ [0..9]
    /// </summary>
    private ChessPiece[,] board = new ChessPiece[9, 10];

    /// <summary>
    /// ÿ�������� "X" ����Ŀ�� & "Y" ����(ʵ����Z��)�ĸ߶�
    /// ������ϣ�� (0,0) ~ (8,9) ���ǵ� -42 ~ 42, ��Ҫ�ֱ���㡣
    ///   ����: 8 �� => 84 / 8 = 10.5
    ///   ����: 9 �� => 84 / 9 = 9.333...��Լ 9.3333��
    /// </summary>
    [Header("���̸��Ӵ�С����")]
    [Tooltip("һ�������� X ��������絥λ��С")]
    public float cellWidth = 10f;
    [Tooltip("һ�������� Y(ʵ��Z) ��������絥λ��С")]
    public float cellHeight = 10f;

    /// <summary>
    /// ����ԭ�� (0,0) �����������е�λ�á�
    /// ��������������Ϊ (-42, 0, -42)��ʹ�� (x=0, y=0) => (-42, -42)
    /// </summary>
    [Header("���������������е�ƫ��")]
    public Vector3 boardOrigin = new Vector3(-45f, 0, -50f);

    private void Awake()
    {
        // ��ʼ������
        Instance = this;

        // ����ѡ������Ϸ��ʼʱ����ջ��ʼ�� board ����
        ClearBoard();
    }

    /// <summary>
    /// ����������ݣ�������λ����Ϊ null
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
    /// ���ĳ���������� (x, y) �Ƿ��ںϷ���Χ��
    /// </summary>
    /// <param name="x">������ [0..8]</param>
    /// <param name="y">������ [0..9]</param>
    /// <returns>true ��ʾ�����̷�Χ�ڣ�false ��ʾԽ��</returns>
    public bool IsValidPosition(int x, int y)
    {
        return (x >= 0 && x < 9 && y >= 0 && y < 10);
    }

    /// <summary>
    /// �������������ȡ��ǰ���ڵ����ӣ�����Ϊ null ��ʾû�����ӣ�
    /// </summary>
    /// <param name="x">�������� x</param>
    /// <param name="y">�������� y</param>
    /// <returns>��Ӧλ�õ� ChessPiece���� null</returns>
    public ChessPiece GetPieceAtPosition(int x, int y)
    {
        if (!IsValidPosition(x, y))
        {
            return null;
        }
        return board[x, y];
    }

    /// <summary>
    /// ��ȡ���ƶ�Ŀ�ĵ�
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
    /// ��ĳ���������õǼǵ����̵�ָ��������
    /// �����ԭ�����������ӣ������������Ƴ����������ⲿ�߼��д������ӡ���
    /// </summary>
    /// <param name="x">�������� x</param>
    /// <param name="y">�������� y</param>
    /// <param name="piece">Ҫ���õ��������ã���Ϊ null</param>
    public void SetPieceAtPosition(int x, int y, ChessPiece piece)
    {
        if (IsValidPosition(x, y))
        {
            board[x, y] = piece;
        }
        else
        {
            Debug.LogWarning($"SetPieceAtPosition: ����({x},{y})������Χ��");
        }
    }

    /// <summary>
    /// ���������� (x, y) ת��Ϊ Unity �������� (worldX, worldY, worldZ)
    /// ���ڸ������ڳ����ж�λ���ƶ�
    /// </summary>
    /// <param name="x">�������� x</param>
    /// <param name="y">�������� y</param>
    /// <returns>��Ӧ����������</returns>
    public Vector3 BoardToWorldPos(int x, int y)
    {
        // ���� (0,0) => ����(-42, -42)��(8,9) => ����(42,42)
        // ���ÿ�� X => 10.5�� Y => 9.3333
        float worldX = boardOrigin.x + x * cellWidth;
        float worldZ = boardOrigin.z + y * cellHeight;

        // worldY �����ӵĸ߶ȣ�һ���ڵ��棬���� 0��
        float worldY = boardOrigin.y;

        return new Vector3(worldX, worldY, worldZ);
    }

    /// <summary>
    /// ����ѡ���������Ҫ�������ѡ���ӣ������ʵ�֡��������� �� �������ꡱ���߼���
    /// ʾ�������ο����ɸ���ʵ�����������
    /// </summary>
    public bool TryGetBoardCoordFromWorld(Vector3 worldPos, out int x, out int y)
    {
        // �����������ȥ boardOrigin��Ȼ��ֱ���� cellWidth �� cellHeight
        float localX = (worldPos.x - boardOrigin.x) / cellWidth;
        float localY = (worldPos.z - boardOrigin.z) / cellHeight;

        // �������������ȡ��
        x = Mathf.RoundToInt(localX);
        y = Mathf.RoundToInt(localY);

        // ����Ƿ��ڷ�Χ��
        if (!IsValidPosition(x, y))
        {
            return false;
        }
        return true;
    }
}
