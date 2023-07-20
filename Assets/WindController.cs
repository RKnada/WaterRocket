using UnityEngine;

public class WindController : MonoBehaviour
{
    public float windSpeed = 5f;
    public Vector3 windDirection = new Vector3(1f, 0f, 0f); // 기본 풍향 (오른쪽으로 향함)

    void Update()
    {
        // 풍향과 풍속에 따라 오브젝트 회전
        if (windDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(windDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 180f);
        }

        // 풍속에 따라 오브젝트 이동
        transform.position += windDirection.normalized * windSpeed * Time.deltaTime;
    }
}