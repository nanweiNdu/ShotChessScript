using UnityEngine;

public class PiecesSpawner : MonoBehaviour
{
    [Header("������ͼ��Sprite��")]
    public Sprite pieceSprite;

    private void Start()
    {
        // ������Ѿ��������Ϲ���һ�� ChessBoardManager �ű���
        // �������� BoardToWorldPos(x, y) ������
        // �Ϳ���ֱ��ʹ���������������ÿ������������ռ�����ꡣ

        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                // 1. ����һ���µĿ����壬������ʾ��������ͼ��
                GameObject pieceObj = new GameObject($"ChessPiece_{x}_{y}");

                // 2. ���������������е�λ��
                //    ���� ChessBoardManager.Instance.BoardToWorldPos(x, y) �ܸ��� (x,y) �����������ӳ��
                Vector3 worldPos = ChessBoardManager.Instance.BoardToWorldPos(x, y);
                pieceObj.transform.position = worldPos;

                // 3. ��������������һ�� SpriteRenderer��������Ⱦ png
                SpriteRenderer sr = pieceObj.AddComponent<SpriteRenderer>();
                sr.sprite = pieceSprite;   // ָ������ Inspector ����ġ����Ӿ��项

                // 4. ����Ը�����Ҫ������ Sorting Layer ���� Order in Layer
                // sr.sortingOrder = 1;
            }
        }
    }
}
