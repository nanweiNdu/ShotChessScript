using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 我方棋子攻击或移动指令管理器
/// </summary>
public class ChessGameManager : MonoBehaviour
{
    public static ChessGameManager Instance;

    public WeaponManager weaponManager;
   public ChessPiece selectedPiece = null; // 当前选中的棋子
    private int turnCounter = 0; // 回合计数器

    [Header("子弹设置")]
    public GameObject bulletPrefab; // 子弹预制体
    public float bulletSpeed = 10f;   // 子弹速度（可调）
    public float bulletMaxDistance = 30f; // 子弹最大飞行距离（可调）
    public int bulletMaxCollisions = 10;   // 子弹最大可碰撞次数（可调）

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 当某个棋子被点击时调用
    /// </summary>
    public void OnPieceClicked(ChessPiece piece)
    {
        if (selectedPiece == null)
        {
            // 没有选中棋子时，选中当前点击的棋子
            SelectPiece(piece);
        }
        else
        {
            // 如果再次点击，取消选中原棋子并选中新的棋子
            DeselectPiece();
            SelectPiece(piece);
        }
    }

    /// <summary>
    /// 选中指定的棋子
    /// </summary>
    private void SelectPiece(ChessPiece piece)
    {
        if (selectedPiece != null)
        {
            selectedPiece.isSelected = false;
        }

        selectedPiece = piece;
        selectedPiece.isSelected = true;
        Debug.Log($"选中棋子: {selectedPiece.pieceType}");
    }

    /// <summary>
    /// 取消选中当前棋子
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
    /// 鼠标右键触发攻击方法
    /// </summary>
    private void TryTriggerAttack(Vector3 targetPosition)
    {
        if (selectedPiece == null || selectedPiece.camp != ChessPiece.Camp.Red)
        {
            Debug.LogWarning("只有选中的红方棋子可以执行攻击！");
            return;
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("子弹预制体未设置！");
            return;
        }

        Vector3 direction;
        FireBullet(targetPosition, out direction);

        Debug.Log($"红方棋子发射子弹，目标位置: {targetPosition}，方向: {direction}");
      //  AttributeManager.Instance.NotifyAttributeChange("DamageBout", 0, 0, 0);
        // 增加回合计数器
      //  IncrementTurnCounter();
    }
    /// <summary>
    /// 平射射程内发射子弹
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="direction"></param>
    private void FireBullet(Vector3 targetPosition, out Vector3 direction)
    {
        // 1) 计算子弹的朝向
        Vector3 attackerPosXZ = new Vector3(selectedPiece.transform.position.x, 0, selectedPiece.transform.position.z);
        Vector3 targetPosXZ = new Vector3(targetPosition.x, 0, targetPosition.z);
        direction = (targetPosXZ - attackerPosXZ).normalized;
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

        Vector3 pos = selectedPiece.transform.position;
        pos.y += 10;
        // 2) 实例化子弹
        GameObject bulletObj = Instantiate(bulletPrefab, pos, rot);
        if (bulletObj == null)
        {
            Debug.LogError("子弹实例化失败！");
            return;
        }
        // 3) 初始化 Bullet 脚本
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
            Debug.LogError("子弹预制体上未挂载 Bullet 脚本！");
        }
    }


    private void FireCurveBullet(Vector3 targetPosition)
    {
        // 2) 实例化子弹
        GameObject bulletObj = Instantiate(bulletPrefab, selectedPiece.transform.position, Quaternion.identity);
        if (bulletObj == null)
        {
            Debug.LogError("子弹实例化失败！");
            return;
        }
     
        // 3) 初始化 Bullet 脚本
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
            Debug.LogError("子弹预制体上未挂载 Bullet 脚本！");
        }
        Debug.Log("子弹生成完毕");
    }

    private void Update()
    {
        // 左键检测：选中棋子或移动棋子
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
                        Debug.LogWarning("点击位置超出棋盘范围！");
                    }
                }
            }
        }

        // 右键检测：发射子弹
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // 只要在棋盘区域，便执行攻击
                if (hit.collider.CompareTag("ChessBoard"))
                {

                    if(weaponManager.WeaponBulletNum <=0)
                    {
                        return;
                    }

                    // 获取相交点坐标
                    Vector3 hitPoint = hit.point;

                    // 检查 ChessGameManager 实例和 selectedPiece 是否存在
                    // 获取 selectedPiece 的位置

                    // 应用旋转// 获取 selectedPiece 的位置
                    Vector3 selectedPiecePosition = selectedPiece.transform.position;

                    // 计算焦点和 selectedPiece 的距离
                    float distance = Vector3.Distance(selectedPiecePosition, hitPoint);
                    int i = (int)(distance / 11);
                    if (i < weaponManager.WeaponRange)
                    {
                        Debug.Log($"右键点击的棋盘位置: {hit.point}");
                        TryTriggerAttack(hit.point);
                    }
                    else
                    {
                        // 半径为 i * 10
                        float radius = (i +1 - weaponManager.WeaponRange) * 5;

                        // 随机选取一个圆内的点
                        Vector3 randomPoint = GetRandomPointInCircle(hit.point, radius);

                        Debug.Log($"随机选取的圆内点: {randomPoint}");

                        // 发射曲线子弹到随机点
                        FireCurveBullet(randomPoint);
                    }
                    selectedPiece.spineCtrl.PlayAttackAnimation();
                    AttributeManager.Instance.NotifyAttributeChange("DamageBout", 0, 0, 0);
                }
            }
        }
    }

    /// <summary>
    /// 处理棋盘格子的点击逻辑（移动棋子）
    /// </summary>
    private void HandleBoardClick(int x, int y)
    {
        if (selectedPiece == null)
        {
            Debug.LogWarning("当前没有选中的棋子！");
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
            Debug.Log("目标格子已被占据，请直接点击棋子执行操作！");
        }
    }
    /// <summary>
    /// 计算可移动点中距离鼠标落点最近的点
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="chessPieces"></param>
    /// <returns></returns>
    private (int x, int y) GetNearestPointTo(int x, int y, List<(int x, int y)> chessPieces)
    {
        if (chessPieces == null || chessPieces.Count == 0)
        {
            Debug.LogWarning("chessPieces 列表为空或未初始化！");
            return (-1, -1); // 返回无效值，表示没有找到最近的点
        }

        // 使用 LINQ 查找距离最近的点
        var nearestPoint = chessPieces
            .OrderBy(p => Mathf.Pow(p.x - x, 2) + Mathf.Pow(p.y - y, 2)) // 按平方距离排序
            .First(); // 获取最近的点

        return nearestPoint;
    }
    /// <summary>
    /// 移动选中的棋子到目标位置
    /// </summary>
    private void MoveSelectedPieceToPosition(int x, int y)
    {
        Vector3 targetWorldPos = ChessBoardManager.Instance.BoardToWorldPos(x, y);
        selectedPiece.transform.position = targetWorldPos;

        // 更新棋盘数据
        ChessBoardManager.Instance.SetPieceAtPosition(x, y, selectedPiece);
        ChessBoardManager.Instance.SetPieceAtPosition(selectedPiece.boardX, selectedPiece.boardY, null);

        selectedPiece.boardX = x;
        selectedPiece.boardY = y;

        Debug.Log($"移动棋子到棋盘坐标 ({x}, {y})，世界坐标 {targetWorldPos}");

        if (selectedPiece.camp == ChessPiece.Camp.Red)
        {
            IncrementTurnCounter();
        }

        selectedPiece.isSelected = false;
        selectedPiece = null;
    }

    /// <summary>
    /// 增加回合计数器
    /// </summary>
    private void IncrementTurnCounter()
    {
        turnCounter++;
        Debug.Log($"回合计数器更新: 当前回合数为 {turnCounter}");

        // 通知场景中的 Warrior 脚本，做一些回合更新
        foreach (Warrior warrior in FindObjectsOfType<Warrior>())
        {
            warrior.OnGlobalTurnIncreased();
        }
    }

    /// <summary>
    /// 以指定的圆心和半径，随机选取一个圆内的点
    /// </summary>
    /// <param name="center">圆心</param>
    /// <param name="radius">半径</param>
    /// <returns>圆内的随机点</returns>
    private Vector3 GetRandomPointInCircle(Vector3 center, float radius)
    {
        // 随机生成半径范围内的距离
        float randomRadius = Random.Range(0, radius);

        // 随机生成角度（0 到 360 度）
        float randomAngle = Random.Range(0, 2 * Mathf.PI);

        // 计算圆内随机点的 X 和 Z 坐标
        float x = center.x + randomRadius * Mathf.Cos(randomAngle);
        float z = center.z + randomRadius * Mathf.Sin(randomAngle);

        // 返回随机点（Y 轴保持与圆心相同）
        return new Vector3(x, center.y, z);
    }
}
