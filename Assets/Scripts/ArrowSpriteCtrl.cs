using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��ʸ�������ֿ���
/// </summary>
public class ArrowSpriteCtrl : MonoBehaviour
{
    /// <summary>
    /// ��ͷSpr
    /// </summary>
    [SerializeField]
    private SpriteRenderer ArrowSpr;
    /// <summary>
    /// ɢ��Ŀ������
    /// </summary>
    [SerializeField]
    private SpriteRenderer TargetRegionSpr;
    /// <summary>
    /// ���ڻ������ߵ� LineRenderer
    /// </summary>
    [SerializeField]
    private LineRenderer lineRenderer;
    // ��¼��ǰ����״̬
    private bool isCurveMode = false;
    private void Awake()
    {
        // ���� LineRenderer ����
        lineRenderer.startWidth = 2f; // �����
        lineRenderer.endWidth = 2f;   // �յ���
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // ����Ĭ�ϲ���
        lineRenderer.startColor = Color.red; // �����ɫ
        lineRenderer.endColor = Color.red;   // �յ���ɫ
        lineRenderer.positionCount = 2; // ������
    }
    private void Update()
    {
        if(ChessGameManager.Instance.selectedPiece != null && ChessGameManager.Instance.selectedPiece.camp == ChessPiece.Camp.Red)
        {
            GetPosition();
        }
        else
        {
            ArrowSpr.gameObject.SetActive(false);
            TargetRegionSpr.gameObject.SetActive(false);
            ClearCurve();
        }

    }
    private void GetPosition()
    {
        // ��ȡ�����
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not found.");
            return;
        }

        // ����Ļ���λ�÷���һ������
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // �������ߣ�������ͼ��
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            // ������еĶ���ı�ǩ�Ƿ��� "Chessboard"
            if (hitInfo.collider.CompareTag("ChessBoard"))
            {
                // ��ȡ�ཻ������
                Vector3 hitPoint = hitInfo.point;

                // ��� ChessGameManager ʵ���� selectedPiece �Ƿ����
                if (ChessGameManager.Instance != null && ChessGameManager.Instance.selectedPiece != null)
                {
                    // ��ȡ selectedPiece ��λ��

                    // Ӧ����ת// ��ȡ selectedPiece ��λ��
                    Vector3 selectedPiecePosition = ChessGameManager.Instance.selectedPiece.transform.position;

                    // ���㽹��� selectedPiece �ľ���
                    float distance = Vector3.Distance(selectedPiecePosition, hitPoint);
                    int i = (int)(distance / 11);
                    if (i < ChessGameManager.Instance.weaponManager.WeaponRange)
                    {
                        if (isCurveMode)
                        {
                            ClearCurve(); // ֻ���л���ֱ��ģʽʱ��������
                            isCurveMode = false;
                        }
                        ArrowSpr.gameObject.SetActive(true);
                        TargetRegionSpr.gameObject.SetActive(false);
                        UpdateArrowDirection(hitPoint, selectedPiecePosition);
                        UpdateLineRenderer(selectedPiecePosition, hitPoint);
                    }
                    else
                    {
                        // �л�������ģʽ
                        if (!isCurveMode)
                        {
                            ClearCurve();
                            isCurveMode = true;
                        }
                        ArrowSpr.gameObject.SetActive(true);
                        TargetRegionSpr.gameObject.SetActive(true);
                        TargetRegionSpr.gameObject.transform.localScale = new Vector3((i + 1 - ChessGameManager.Instance.weaponManager.WeaponRange) * 30, (i + 1 - ChessGameManager.Instance.weaponManager.WeaponRange) * 30, 0);
                       DrawCurve(selectedPiecePosition, hitPoint);
                    }
                }
                else
                {
                    Debug.LogWarning("ChessGameManager or selectedPiece is null.");
                }
            }
        }
    }
    /// <summary>
    /// ���¼�ͷ�ķ���ʹ��ָ�� ChessGameManager.Instance.selectedPiece �� ChessBoard �ཻ��ķ���
    /// </summary>
    private void UpdateArrowDirection(Vector3 hitPoint, Vector3 selectedPiecePosition)
    {

        // �����ͷ�ķ���
        Vector3 direction = (hitPoint - selectedPiecePosition).normalized;

        // ���Է���� Y ������ֻ�� XZ ƽ�����
        direction.y = 0;
        direction.Normalize();

        // ����Ŀ����ת����Χ�� Y ����ת��
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // ��ӹ̶��� X ����תΪ -90 ��
        Quaternion fixedXRotation = Quaternion.Euler(-90, targetRotation.eulerAngles.y, 0);
        hitPoint.y += 1;
        // Ӧ����ת
        ArrowSpr.gameObject.transform.SetPositionAndRotation(hitPoint, fixedXRotation);
        TargetRegionSpr.gameObject.transform.position = hitPoint;
    }
        
    /// <summary>
    /// ���� LineRenderer ������ ArrowSpr �� selectedPiece
    /// </summary>
    private void UpdateLineRenderer(Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.enabled = true; // ���� LineRenderer
                                     // ���õ�����Ϊ 2
        if (lineRenderer.positionCount != 2)
        {
            lineRenderer.positionCount = 2;
        }
        startPosition.y += 10;
        lineRenderer.SetPosition(0, startPosition); // �������
        lineRenderer.SetPosition(1, endPosition);   // �����յ�
    }
    /// <summary>
    /// ����һ���� selectedPiecePosition �� hitPoint ������
    /// </summary>
    private void DrawCurve(Vector3 startPosition, Vector3 endPosition)
    {
        // ���ÿ��Ƶ�Ϊ�����յ���е㣬����΢̧�� Y ��
        Vector3 controlPoint = (startPosition + endPosition) / 2;
        controlPoint.y += 50; // ������ߵĻ���
        startPosition.y += 10;
        // �������ߵ�
        int segmentCount = 20; // ���߶�����ֵԽ������Խƽ��
        Vector3[] curvePoints = new Vector3[segmentCount + 1];

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            curvePoints[i] = CalculateBezierPoint(t, startPosition, controlPoint, endPosition);
        }

        // ���� LineRenderer
        lineRenderer.enabled = true;
        lineRenderer.positionCount = curvePoints.Length;
        lineRenderer.SetPositions(curvePoints);

        // ���¼�ͷλ�úͷ���ʹ��ָ���յ�
        UpdateArrowDirection(endPosition, curvePoints[curvePoints.Length - 2]);
    }

    /// <summary>
    /// ���㱴���������ϵĵ�
    /// </summary>
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0; // (1 - t)^2 * P0
        point += 2 * u * t * p1; // 2(1 - t)t * P1
        point += tt * p2;        // t^2 * P2

        return point;
    }
    /// <summary>
    /// �����������
    /// </summary>
    private void ClearCurve()
    {
        lineRenderer.positionCount = 0; // ��յ�����
        //lineRenderer.enabled = false;  // ���� LineRenderer
    }
}
