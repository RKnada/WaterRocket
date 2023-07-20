using UnityEngine;

public class DualThrusterRocket : MonoBehaviour
{
    public float gravityAcceleration = 9.8f; // 중력 가속도
    public float airResistance = 1f; // 공기 저항

    private bool pThrust = false; // 일차 추진 여부
    private bool sThrust = false; // 이차 추진 여부

    private float lowerFuelPressure; // 하부 연료통 압력

    public float secondaryThrustTime; // 이차 추진 시간
    private float timeSinceLaunch; // 발사 후 경과 시간

    private Rigidbody rocketRigidbody; // 로켓의 리지드바디

    public float rocketMass = 10f; // 로켓의 질량

    public float upperFuelCapacity = 5f; // 상부 연료통 물의 양
    public float lowerFuelCapacity = 50f; // 하부 연료통 물의 양

    float airAmount; // 하부 연료통 주입된 공기 양


    private void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 공기 주입(Space)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InjectAir();
        }

        // 일차 추진(R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("하부연료통 분사 - 일차 추진");
            PrimaryThrust();
        }

        // 이차 추진
        if (pThrust && timeSinceLaunch >= secondaryThrustTime)
        {
            if (upperFuelCapacity > 0)
            {
                SecondaryThrust();
            }
        }

        // 물리 업데이트
        if (pThrust)
        {
            UpdatePhysics();
        }

        timeSinceLaunch += Time.deltaTime;
    }

    private void InjectAir()
    {
        airAmount += 1f;
        Debug.Log("공기 주입 - " + airAmount + "L");
    }

    private void PrimaryThrust()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, CalculateThrustForce(), 0);

        pThrust = true;
    }

    private void SecondaryThrust()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, CalculateSecondary(), 0);

        sThrust = true;
    }

    private void UpdatePhysics()
    {
        // 중력 적용
        Vector3 gravity = Vector3.down * gravityAcceleration;
        rocketRigidbody.AddForce(gravity);

        // 공기 저항 적용
        Vector3 airResistanceForce = -rocketRigidbody.velocity.normalized * airResistance;
        rocketRigidbody.AddForce(airResistanceForce);
    }

    private float CalculateThrustForce()
    {
        // 일차 추진 속도 계산
        float thrustForce = airAmount * lowerFuelCapacity / rocketMass;
        return thrustForce;
    }

    private float CalculateSecondary()
    {
        // 이차 역추진 속도 계산
        float secondThrustForce = upperFuelCapacity / rocketMass;
        return secondThrustForce;
    }
}