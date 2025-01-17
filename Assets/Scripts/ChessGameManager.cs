using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// �ҷ����ӹ������ƶ�ָ�������
/// </summary>
public class ChessGameManager : MonoBehaviour
{
    public static ChessGameManager Instance;

    public WeaponManager weaponManager;
   public ChessPiece selectedPiece = null; // ��ǰѡ�е�����
    private int turnCounter = 0; // �غϼ�����

    [Header("�ӵ�����")]
    public GameObject bulletPrefab; // �ӵ�Ԥ����
    public float bulletSpeed = 10f;   // �ӵ��ٶȣ��ɵ���
    public float bulletMaxDistance = 30f; // �ӵ������о��루�ɵ���
    public int bulletMaxCollisions = 10;   // �ӵ�������ײ�������ɵ���

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// ��ĳ�����ӱ����ʱ����
    /// </summary>
    public void OnPieceClicked(ChessPiece piece)
    {
        if (selectedPiece == null)
        {
            // û��ѡ������ʱ��ѡ�е�ǰ���������
            SelectPiece(piece);
        }
        else
        {
            // ����ٴε����ȡ��ѡ��ԭ���Ӳ�ѡ���µ�����
            DeselectPiece();
            SelectPiece(piece);
        }
    }

    /// <summary>
    /// ѡ��ָ��������
    /// </summary>
    private void SelectPiece(ChessPiece piece)
    {
        if (selectedPiece != null)
        {
            selectedPiece.isSelected = false;
        }

        selectedPiece = piece;
        selectedPiece.isSelected = true;
        Debug.Log($"ѡ������: {selectedPiece.pieceType}");
    }

    /// <summary>
    /// ȡ��ѡ�е�ǰ����
    /// </summary>
    private void DeselectPiece()
    {
        if (selectedPiece != null)
        {
            selectedPiece.isSelected = false;
            selectedPiece = null;
        }
    }

    /// <summary>
    /// ����Ҽ�������������
    /// </summary>
    private void TryTriggerAttack(Vector3 targetPosition)
    {
        if (selectedPiece == null || selectedPiece.camp != ChessPiece.Camp.Red)
        {
            Debug.LogWarning("ֻ��ѡ�еĺ췽���ӿ���ִ�й�����");
            return;
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("�ӵ�Ԥ����δ���ã�");
            return;
        }

        Vector3 direction;
        FireBullet(targetPosition, out direction);

        Debug.Log($"�췽���ӷ����ӵ���Ŀ��λ��: {targetPosition}������: {direction}");
      //  AttributeManager.Instance.NotifyAttributeChange("DamageBout", 0, 0, 0);
        // ���ӻغϼ�����
      //  IncrementTurnCounter();
    }
    /// <summary>
    /// ƽ������ڷ����ӵ�
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="direction"></param>
    private void FireBullet(Vector3 targetPosition, out Vector3 direction)
    {
        // 1) �����ӵ��ĳ���
        Vector3 attackerPosXZ = new Vector3(selectedPiece.transform.position.x, 0, selectedPiece.transform.position.z);
        Vector3 targetPosXZ = new Vector3(targetPosition.x, 0, targetPosition.z);
        direction = (targetPosXZ - attackerPosXZ).normalized;
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

        Vector3 pos = selectedPiece.transform.position;
        pos.y += 10;
        // 2) ʵ�����ӵ�
        GameObject bulletObj = Instantiate(bulletPrefab, pos, rot);
        if (bulletObj == null)
        {
            Debug.LogError("�ӵ�ʵ����ʧ�ܣ�");
            return;
        }
        // 3) ��ʼ�� Bullet �ű�
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetProperties(
                direction,
                weaponManager.WeaponRange * 10,
                bulletSpeed*4f,
                selectedPiece.gameObject,
                bulletMaxCollisions,
                  weaponManager.WeaponDamage
            );
        }
        else
        {
            Debug.LogError("�ӵ�Ԥ������δ���� Bullet �ű���");
        }
    }


    private void FireCurveBullet(Vector3 targetPosition)
    {
        // 2) ʵ�����ӵ�
        GameObject bulletObj = Instantiate(bulletPrefab, selectedPiece.transform.position, Quaternion.identity);
        if (bulletObj == null)
        {
            Debug.LogError("�ӵ�ʵ����ʧ�ܣ�");
            return;
        }
     
        // 3) ��ʼ�� Bullet �ű�
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.isCurvedFire = true;
            bulletScript.SetCurveProperties(
                targetPosition,
                 bulletSpeed,
                 weaponManager.WeaponDamage
            );
        }
        else
        {
            Debug.LogError("�ӵ�Ԥ������δ���� Bullet �ű���");
        }
        Debug.Log("�ӵ��������");
    }

    private void Update()
    {
        // �����⣺ѡ�����ӻ��ƶ�����
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("ChessPiece"))
                {
                    ChessPiece clickedPiece = hit.collider.GetComponent<ChessPiece>();
                    OnPieceClicked(clickedPiece);
                }
                else if (hit.collider.CompareTag("ChessBoard") && selectedPiece != null)
                {
                    if (ChessBoardManager.Instance.TryGetBoardCoordFromWorld(hit.point, out int x, out int y))
                    {
                        HandleBoardClick(x, y);
                    }
                    else
                    {
                        Debug.LogWarning("���λ�ó������̷�Χ��");
                    }
                }
            }
        }

        // �Ҽ���⣺�����ӵ�
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // ֻҪ���������򣬱�ִ�й���
                if (hit.collider.CompareTag("ChessBoard"))
                {

                    if(weaponManager.WeaponBulletNum <=0)
                    {
                        return;
                    }

                    // ��ȡ�ཻ������
                    Vector3 hitPoint = hit.point;

                    // ��� ChessGameManager ʵ���� selectedPiece �Ƿ����
                    // ��ȡ selectedPiece ��λ��

                    // Ӧ����ת// ��ȡ selectedPiece ��λ��
                    Vector3 selectedPiecePosition = selectedPiece.transform.position;

                    // ���㽹��� selectedPiece �ľ���
                    float distance = Vector3.Distance(selectedPiecePosition, hitPoint);
                    int i = (int)(distance / 11);
                    if (i < weaponManager.WeaponRange)
                    {
                        Debug.Log($"�Ҽ����������λ��: {hit.point}");
                        TryTriggerAttack(hit.point);
                    }
                    else
                    {
                        // �뾶Ϊ i * 10
                        float radius = (i +1 - weaponManager.WeaponRange) * 5;

                        // ���ѡȡһ��Բ�ڵĵ�
                        Vector3 randomPoint = GetRandomPointInCircle(hit.point, radius);

                        Debug.Log($"���ѡȡ��Բ�ڵ�: {randomPoint}");

                        // ���������ӵ��������
                        FireCurveBullet(randomPoint);
                    }
                    selectedPiece.spineCtrl.PlayAttackAnimation();
                    AttributeManager.Instance.NotifyAttributeChange("DamageBout", 0, 0, 0);
                }
            }
        }
    }

    /// <summary>
    /// �������̸��ӵĵ���߼����ƶ����ӣ�
    /// </summary>
    private void HandleBoardClick(int x, int y)
    {
        if (selectedPiece == null)
        {
            Debug.LogWarning("��ǰû��ѡ�е����ӣ�");
            return;
        }

        List<(int x, int y)> chessPieces = ChessBoardManager.Instance.GetPieceCanMove(selectedPiece.boardX,selectedPiece.boardY,selectedPiece.pieceType);
        var chessPoint = GetNearestPointTo(x, y, chessPieces);
        Debug.Log(chessPoint);
        ChessPiece targetPiece = ChessBoardManager.Instance.GetPieceAtPosition(chessPoint.x, chessPoint.y);
        if (targetPiece == null)
        {
            MoveSelectedPieceToPosition(chessPoint.x, chessPoint.y);
            AttributeManager.Instance.NotifyAttributeChange("CommonBout",0,0,0);
        }
        else
        {
            Debug.Log("Ŀ������ѱ�ռ�ݣ���ֱ�ӵ������ִ�в�����");
        }
    }
    /// <summary>
    /// ������ƶ����о�������������ĵ�
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="chessPieces"></param>
    /// <returns></returns>
    private (int x, int y) GetNearestPointTo(int x, int y, List<(int x, int y)> chessPieces)
    {
        if (chessPieces == null || chessPieces.Count == 0)
        {
            Debug.LogWarning("chessPieces �б�Ϊ�ջ�δ��ʼ����");
            return (-1, -1); // ������Чֵ����ʾû���ҵ�����ĵ�
        }

        // ʹ�� LINQ ���Ҿ�������ĵ�
        var nearestPoint = chessPieces
            .OrderBy(p => Mathf.Pow(p.x - x, 2) + Mathf.Pow(p.y - y, 2)) // ��ƽ����������
            .First(); // ��ȡ����ĵ�

        return nearestPoint;
    }
    /// <summary>
    /// �ƶ�ѡ�е����ӵ�Ŀ��λ��
    /// </summary>
    private void MoveSelectedPieceToPosition(int x, int y)
    {
        Vector3 targetWorldPos = ChessBoardManager.Instance.BoardToWorldPos(x, y);
        selectedPiece.transform.position = targetWorldPos;

        // ������������
        ChessBoardManager.Instance.SetPieceAtPosition(x, y, selectedPiece);
        ChessBoardManager.Instance.SetPieceAtPosition(selectedPiece.boardX, selectedPiece.boardY, null);

        selectedPiece.boardX = x;
        selectedPiece.boardY = y;

        Debug.Log($"�ƶ����ӵ��������� ({x}, {y})���������� {targetWorldPos}");

        if (selectedPiece.camp == ChessPiece.Camp.Red)
        {
            IncrementTurnCounter();
        }

        selectedPiece.isSelected = false;
        selectedPiece = null;
    }

    /// <summary>
    /// ���ӻغϼ�����
    /// </summary>
    private void IncrementTurnCounter()
    {
        turnCounter++;
        Debug.Log($"�غϼ���������: ��ǰ�غ���Ϊ {turnCounter}");

        // ֪ͨ�����е� Warrior �ű�����һЩ�غϸ���
        foreach (Warrior warrior in FindObjectsOfType<Warrior>())
        {
            warrior.OnGlobalTurnIncreased();
        }
    }

    /// <summary>
    /// ��ָ����Բ�ĺͰ뾶�����ѡȡһ��Բ�ڵĵ�
    /// </summary>
    /// <param name="center">Բ��</param>
    /// <param name="radius">�뾶</param>
    /// <returns>Բ�ڵ������</returns>
    private Vector3 GetRandomPointInCircle(Vector3 center, float radius)
    {
        // ������ɰ뾶��Χ�ڵľ���
        float randomRadius = Random.Range(0, radius);

        // ������ɽǶȣ�0 �� 360 �ȣ�
        float randomAngle = Random.Range(0, 2 * Mathf.PI);

        // ����Բ�������� X �� Z ����
        float x = center.x + randomRadius * Mathf.Cos(randomAngle);
        float z = center.z + randomRadius * Mathf.Sin(randomAngle);

        // ��������㣨Y �ᱣ����Բ����ͬ��
        return new Vector3(x, center.y, z);
    }
}
