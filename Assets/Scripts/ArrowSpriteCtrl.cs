using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 箭矢弹道表现控制
/// </summary>
public class ArrowSpriteCtrl : MonoBehaviour
{
    /// <summary>
    /// 箭头Spr
    /// </summary>
    [SerializeField]
    private SpriteRenderer ArrowSpr;
    /// <summary>
    /// 散射目标区域
    /// </summary>
    [SerializeField]
    private SpriteRenderer TargetRegionSpr;
    /// <summary>
    /// 用于绘制连线的 LineRenderer
    /// </summary>
    [SerializeField]
    private LineRenderer lineRenderer;
    // 记录当前绘制状态
    private bool isCurveMode = false;
    private void Awake()
    {
        // 设置 LineRenderer 属性
        lineRenderer.startWidth = 2f; // 起点宽度
        lineRenderer.endWidth = 2f;   // 终点宽度
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 设置默认材质
        lineRenderer.startColor = Color.red; // 起点颜色
        lineRenderer.endColor = Color.red;   // 终点颜色
        lineRenderer.positionCount = 2; // 两个点
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
        // 获取主相机
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not found.");
            return;
        }

        // 从屏幕鼠标位置发射一条射线
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // 发射射线，不限制图层
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            // 检查命中的对象的标签是否是 "Chessboard"
            if (hitInfo.collider.CompareTag("ChessBoard"))
            {
                // 获取相交点坐标
                Vector3 hitPoint = hitInfo.point;

                // 检查 ChessGameManager 实例和 selectedPiece 是否存在
                if (ChessGameManager.Instance != null && ChessGameManager.Instance.selectedPiece != null)
                {
                    // 获取 selectedPiece 的位置

                    // 应用旋转// 获取 selectedPiece 的位置
                    Vector3 selectedPiecePosition = ChessGameManager.Instance.selectedPiece.transform.position;

                    // 计算焦点和 selectedPiece 的距离
                    float distance = Vector3.Distance(selectedPiecePosition, hitPoint);
                    int i = (int)(distance / 11);
                    if (i < ChessGameManager.Instance.weaponManager.WeaponRange)
                    {
                        if (isCurveMode)
                        {
                            ClearCurve(); // 只在切换到直线模式时清理曲线
                            isCurveMode = false;
                        }
                        ArrowSpr.gameObject.SetActive(true);
                        TargetRegionSpr.gameObject.SetActive(false);
                        UpdateArrowDirection(hitPoint, selectedPiecePosition);
                        UpdateLineRenderer(selectedPiecePosition, hitPoint);
                    }
                    else
                    {
                        // 切换到曲线模式
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
    /// 更新箭头的方向，使其指向 ChessGameManager.Instance.selectedPiece 和 ChessBoard 相交点的方向。
    /// </summary>
    private void UpdateArrowDirection(Vector3 hitPoint, Vector3 selectedPiecePosition)
    {

        // 计算箭头的方向
        Vector3 direction = (hitPoint - selectedPiecePosition).normalized;

        // 忽略方向的 Y 分量，只在 XZ 平面计算
        direction.y = 0;
        direction.Normalize();

        // 计算目标旋转（仅围绕 Y 轴旋转）
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // 添加固定的 X 轴旋转为 -90 度
        Quaternion fixedXRotation = Quaternion.Euler(-90, targetRotation.eulerAngles.y, 0);
        hitPoint.y += 1;
        // 应用旋转
        ArrowSpr.gameObject.transform.SetPositionAndRotation(hitPoint, fixedXRotation);
        TargetRegionSpr.gameObject.transform.position = hitPoint;
    }
        
    /// <summary>
    /// 更新 LineRenderer 以连接 ArrowSpr 和 selectedPiece
    /// </summary>
    private void UpdateLineRenderer(Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.enabled = true; // 启用 LineRenderer
                                     // 设置点数量为 2
        if (lineRenderer.positionCount != 2)
        {
            lineRenderer.positionCount = 2;
        }
        startPosition.y += 10;
        lineRenderer.SetPosition(0, startPosition); // 设置起点
        lineRenderer.SetPosition(1, endPosition);   // 设置终点
    }
    /// <summary>
    /// 绘制一条从 selectedPiecePosition 到 hitPoint 的曲线
    /// </summary>
    private void DrawCurve(Vector3 startPosition, Vector3 endPosition)
    {
        // 设置控制点为起点和终点的中点，并稍微抬高 Y 轴
        Vector3 controlPoint = (startPosition + endPosition) / 2;
        controlPoint.y += 50; // 提高曲线的弧度
        startPosition.y += 10;
        // 生成曲线点
        int segmentCount = 20; // 曲线段数，值越高曲线越平滑
        Vector3[] curvePoints = new Vector3[segmentCount + 1];

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            curvePoints[i] = CalculateBezierPoint(t, startPosition, controlPoint, endPosition);
        }

        // 更新 LineRenderer
        lineRenderer.enabled = true;
        lineRenderer.positionCount = curvePoints.Length;
        lineRenderer.SetPositions(curvePoints);

        // 更新箭头位置和方向，使其指向终点
        UpdateArrowDirection(endPosition, curvePoints[curvePoints.Length - 2]);
    }

    /// <summary>
    /// 计算贝塞尔曲线上的点
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
    /// 清除曲线数据
    /// </summary>
    private void ClearCurve()
    {
        lineRenderer.positionCount = 0; // 清空点数据
        //lineRenderer.enabled = false;  // 隐藏 LineRenderer
    }
}
