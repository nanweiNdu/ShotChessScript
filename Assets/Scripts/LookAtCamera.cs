using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // 获取主摄像机
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // 计算从物体指向摄像机的方向向量
            Vector3 direction = mainCamera.transform.position - transform.position;

            // 计算水平面上的方向（忽略 Y 轴）
            Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);

            // 如果方向向量的长度不为零
            if (horizontalDirection.sqrMagnitude > 0.0001f)
            {
                // 计算水平角度（绕 Y 轴的旋转）
                float horizontalAngle = Mathf.Atan2(horizontalDirection.x, horizontalDirection.z) * Mathf.Rad2Deg;

                // 应用映射规则到水平角度
                if (horizontalAngle > 180)
                {
                    horizontalAngle -= 360;
                }
                if (horizontalAngle >= 0)
                {
                    horizontalAngle -= 180;
                }
                else
                {
                    horizontalAngle += 180;
                }

                // 获取摄像机的 X 轴旋转角度
                float cameraXRotation = mainCamera.transform.eulerAngles.x;

                // 将摄像机的 X 轴旋转角度映射到 [-180, 180] 范围内
                if (cameraXRotation > 180)
                {
                    cameraXRotation -= 360;
                }

                // 计算物体需要的 X 轴旋转角度，使其与摄像机的 X 轴旋转角度之和等于 90 度
                float verticalAngle = 90 - cameraXRotation;

                // 应用旋转到物体
                transform.rotation = Quaternion.Euler(verticalAngle, horizontalAngle/2, 0);
            }
        }
    }
}
