using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // ��ȡ�������
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // ���������ָ��������ķ�������
            Vector3 direction = mainCamera.transform.position - transform.position;

            // ����ˮƽ���ϵķ��򣨺��� Y �ᣩ
            Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);

            // ������������ĳ��Ȳ�Ϊ��
            if (horizontalDirection.sqrMagnitude > 0.0001f)
            {
                // ����ˮƽ�Ƕȣ��� Y �����ת��
                float horizontalAngle = Mathf.Atan2(horizontalDirection.x, horizontalDirection.z) * Mathf.Rad2Deg;

                // Ӧ��ӳ�����ˮƽ�Ƕ�
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

                // ��ȡ������� X ����ת�Ƕ�
                float cameraXRotation = mainCamera.transform.eulerAngles.x;

                // ��������� X ����ת�Ƕ�ӳ�䵽 [-180, 180] ��Χ��
                if (cameraXRotation > 180)
                {
                    cameraXRotation -= 360;
                }

                // ����������Ҫ�� X ����ת�Ƕȣ�ʹ����������� X ����ת�Ƕ�֮�͵��� 90 ��
                float verticalAngle = 90 - cameraXRotation;

                // Ӧ����ת������
                transform.rotation = Quaternion.Euler(verticalAngle, horizontalAngle/2, 0);
            }
        }
    }
}
