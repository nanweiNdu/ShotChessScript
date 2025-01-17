using UnityEngine;

/// <summary>
/// ���ӵ���ָ���ٶ��� XZ ƽ���Ϸ��У���;�������зǷ����ߵ�����
/// </summary>
public class Bullet : MonoBehaviour
{
    private Vector3 direction;    // �ӵ����з���ֻ�� XZ ƽ�棩
    private float speed;          // �ӵ��ٶ�
    private float maxDistance;    // �ӵ������о���
    private Vector3 startPosition;
    private GameObject shooter;   // �����ӵ��Ķ��󣨺췽���ӣ�
    private int currentCollisions = 0;
    private int maxCollisions = 10; // Ĭ�������ײ����
    private int Damage; //�˺���
    public bool isCurvedFire = false; // �Ƿ�����
    /// <summary>
    /// �����ӵ��ķ�������
    /// </summary>
    /// <param name="direction">Ŀ�귽��</param>
    /// <param name="maxDistance">������</param>
    /// <param name="speed">�����ٶ�</param>
    /// <param name="shooter">������</param>
    /// <param name="maxCollisions">������ײ����</param>
    /// <param name="damage">�˺�</param>
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
            // ���ӵ���Ŀ�귽���ƶ�
            transform.position += direction * speed * Time.deltaTime;
            // ����ӵ��Ƿ񳬹������о���
            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (t <= 1f)
            {
                // ���㵱ǰ t ��Ӧ�����ߵ�λ��
                Vector3 positionOnCurve = CalculateBezierPoint(t, startPoint, controlPoint, endPoint);

                // ���������ڵ�ǰ t �����߷���
                Vector3 tangent = CalculateBezierTangent(t, startPoint, controlPoint, endPoint);

                // ���¼�ʸ��λ�úͳ���
                transform.position = positionOnCurve;
                transform.rotation = Quaternion.LookRotation(tangent);

                // ���� t��ģ��ʱ���ƽ�
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
        Debug.Log("����ײ");
        ChessPiece piece = other.GetComponent<ChessPiece>();
        if (piece != null && piece.camp == ChessPiece.Camp.Black)
        {
            Debug.Log($"�ӵ���������: {piece.pieceType}������ݻ١�");
            Destroy(piece.gameObject);

            currentCollisions++;

            if (currentCollisions >= maxCollisions)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private Vector3 startPoint;      // ���
    private Vector3 controlPoint;   // ���Ƶ�
    private Vector3 endPoint;       // �յ�
    private float t = 0f;           // ���ߵĵ�ǰ���� t��0 �� 1��

    /// <summary>
    /// ���ü�ʸ�������ƶ�����
    /// </summary>
    /// <param name="damage">�˺�</param>
    /// <param name="endPoint">�յ�</param>
    /// <param name="speed">�ƶ��ٶ�</param>
    public void SetCurveProperties(Vector3 endPoint, float speed,int damage)
    {
 
        this.endPoint = endPoint;
        this.speed = speed/5;
        startPoint = transform.position;
        // ������Ƶ㣺�е� + Y ��ƫ��
        this.controlPoint = (startPoint + endPoint) / 2;
        this.controlPoint.y += 50;
        Damage = damage;
        // ����ʸ���������λ��
        transform.position = startPoint;
    }


    /// <summary>
    /// ���㱴���������ϵĵ�
    /// </summary>
    /// <param name="t">���߲�����0 �� 1��</param>
    /// <param name="p0">���</param>
    /// <param name="p1">���Ƶ�</param>
    /// <param name="p2">�յ�</param>
    /// <returns>�����ϵĵ�</returns>
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    /// <summary>
    /// ���㱴�������ߵ����߷���
    /// </summary>
    /// <param name="t">���߲�����0 �� 1��</param>
    /// <param name="p0">���</param>
    /// <param name="p1">���Ƶ�</param>
    /// <param name="p2">�յ�</param>
    /// <returns>���ߵ����߷���</returns>
    private Vector3 CalculateBezierTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;

        // һ�׵�����ʽ
        return 2 * u * (p1 - p0) + 2 * t * (p2 - p1);
    }
}
