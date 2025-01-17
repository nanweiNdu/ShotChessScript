using UnityEngine;

/// <summary>
/// 让子弹以指定速度在 XZ 平面上飞行，沿途击中所有非发射者的棋子
/// </summary>
public class Bullet : MonoBehaviour
{
    private Vector3 direction;    // 子弹飞行方向（只在 XZ 平面）
    private float speed;          // 子弹速度
    private float maxDistance;    // 子弹最大飞行距离
    private Vector3 startPosition;
    private GameObject shooter;   // 发射子弹的对象（红方棋子）
    private int currentCollisions = 0;
    private int maxCollisions = 10; // 默认最大碰撞次数
    private int Damage; //伤害量
    public bool isCurvedFire = false; // 是否曲射
    /// <summary>
    /// 设置子弹的飞行属性
    /// </summary>
    /// <param name="direction">目标方向</param>
    /// <param name="maxDistance">最大射程</param>
    /// <param name="speed">飞行速度</param>
    /// <param name="shooter">发射者</param>
    /// <param name="maxCollisions">最大可碰撞次数</param>
    /// <param name="damage">伤害</param>
    public void SetProperties(Vector3 direction, float maxDistance, float speed, GameObject shooter, int maxCollisions,int damage)
    {
        this.direction = direction.normalized;
        this.maxDistance = maxDistance;
        this.speed = speed;
        this.shooter = shooter;
        this.maxCollisions = maxCollisions;
        startPosition = transform.position;
        Damage = damage;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (!isCurvedFire)
        {
            // 让子弹朝目标方向移动
            transform.position += direction * speed * Time.deltaTime;
            // 检查子弹是否超过最大飞行距离
            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (t <= 1f)
            {
                // 计算当前 t 对应的曲线点位置
                Vector3 positionOnCurve = CalculateBezierPoint(t, startPoint, controlPoint, endPoint);

                // 计算曲线在当前 t 的切线方向
                Vector3 tangent = CalculateBezierTangent(t, startPoint, controlPoint, endPoint);

                // 更新箭矢的位置和朝向
                transform.position = positionOnCurve;
                transform.rotation = Quaternion.LookRotation(tangent);

                // 增加 t，模拟时间推进
                t += speed * Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("有碰撞");
        ChessPiece piece = other.GetComponent<ChessPiece>();
        if (piece != null && piece.camp == ChessPiece.Camp.Black)
        {
            Debug.Log($"子弹击中棋子: {piece.pieceType}，将其摧毁。");
            Destroy(piece.gameObject);

            currentCollisions++;

            if (currentCollisions >= maxCollisions)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private Vector3 startPoint;      // 起点
    private Vector3 controlPoint;   // 控制点
    private Vector3 endPoint;       // 终点
    private float t = 0f;           // 曲线的当前参数 t（0 到 1）

    /// <summary>
    /// 设置箭矢的曲线移动属性
    /// </summary>
    /// <param name="damage">伤害</param>
    /// <param name="endPoint">终点</param>
    /// <param name="speed">移动速度</param>
    public void SetCurveProperties(Vector3 endPoint, float speed,int damage)
    {
 
        this.endPoint = endPoint;
        this.speed = speed/5;
        startPoint = transform.position;
        // 计算控制点：中点 + Y 轴偏移
        this.controlPoint = (startPoint + endPoint) / 2;
        this.controlPoint.y += 50;
        Damage = damage;
        // 将箭矢放置在起点位置
        transform.position = startPoint;
    }


    /// <summary>
    /// 计算贝塞尔曲线上的点
    /// </summary>
    /// <param name="t">曲线参数（0 到 1）</param>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点</param>
    /// <param name="p2">终点</param>
    /// <returns>曲线上的点</returns>
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    /// <summary>
    /// 计算贝塞尔曲线的切线方向
    /// </summary>
    /// <param name="t">曲线参数（0 到 1）</param>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点</param>
    /// <param name="p2">终点</param>
    /// <returns>曲线的切线方向</returns>
    private Vector3 CalculateBezierTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;

        // 一阶导数公式
        return 2 * u * (p1 - p0) + 2 * t * (p2 - p1);
    }
}
