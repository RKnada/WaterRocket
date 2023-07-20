using UnityEngine;

public class WindController : MonoBehaviour
{
    public float windSpeed = 5f;
    public Vector3 windDirection = new Vector3(1f, 0f, 0f); // �⺻ ǳ�� (���������� ����)

    void Update()
    {
        // ǳ��� ǳ�ӿ� ���� ������Ʈ ȸ��
        if (windDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(windDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 180f);
        }

        // ǳ�ӿ� ���� ������Ʈ �̵�
        transform.position += windDirection.normalized * windSpeed * Time.deltaTime;
    }
}