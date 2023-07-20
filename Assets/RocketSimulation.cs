using UnityEngine;

public class RocketSimulation : MonoBehaviour
{
    public float rocketMass;                // 로켓 통의 질량 (단위: kg)
    public float waterMass_upper;           // [상부연료통] 내 물의 질량 (단위 : kg)
    public float waterMass_lower;           // [하부연료통] 내 물의 질량 (단위 : kg)
    public float upperPressure;             // [상부연료통] 내 CO2 압력 (단위: atm)
    public float lowerPressure;             // [하부연료통] 내 공기 압력 (단위: atm)

    public bool pThrust = false;           // 일차 추진 여부 (일차 추진이 발생하면 true, 그렇지 않으면 false, 기본값은 false)
    public bool sThrust = false;           // 이차 역추진 여부 (이차 역추진이 발생하면 true, 그렇지 않으면 false, 기본값은 false)

    public float secondaryThrustTime;       // 일차 추진 이후(pThrust = true가 된 이후) [상부연료통] 내부의 모터 작용으로 발포비타민이 물에 녹은 시점까지의 시간
                                            // (pThrust = true가 되고 secondaryThrustTime초 후 sThrust = true가 됨)

    public float vitaminReactionTime;       // 발포비타민이 물에 녹은 시점(secondaryThrustTime) 이후 발포비타민이 모두 반응하여 CO2가 발생하는 시점까지의 시간

    public float pressureLimit;             // [상부연료통] 분출구가 열리게 하는 [상부연료통] 내부의 CO2 압력의 임계점

    public float fuelRadius;                //연료통 반지름(단위 : cm)

    public float FlightTime;

    public float windSpeed;                   // 풍속
    public float windDirection;               // 풍향
    public new Rigidbody rigidbody;
    public GameObject CenterofMass;           // 로켓의 질량 중심
    public float CoM_Position;                // 로켓의 질량 중심의 위치
    public Vector3 torqueDirection;

    public float ThrustPosition;

    public bool stable;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //InvokeRepeating("Torque", 0, 0.1f);
    }
    private void Update()
    {
        // 'Space' 키를 누르면 [하부연료통] 내부에 공기를 주입하여 압력을 증가시킴
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IncreaseLowerPressure();
        }

        // 'L' 키를 누르면 [하부연료통]의 분출구가 열리며, [하부연료통] 내부의 공기에 의한 압력으로 물이 분출구로 강하게 분사되어 일차 추진이 시작됨
        if (Input.GetKeyDown(KeyCode.L))
        {
            OpenLowerOutlet();
        }

        // 이차 추진을 위한 시간 체크
        if (pThrust && !sThrust && FlightTime >= secondaryThrustTime)
        {
            InitiateSecondaryThrust();
        }

        // 비타민 반응을 위한 시간 체크
        if (sThrust && Time.time >= secondaryThrustTime + vitaminReactionTime)
        {
            ReactVitamin();
        }

        if(waterMass_lower != 0 && waterMass_upper != 0)
        {
            CenterofMass.GetComponent<Transform>().position = new Vector3(0, GetComponent<Transform>().position.y + ((waterMass_lower * 0.5f) + (waterMass_upper * 0.755f) / (waterMass_lower + waterMass_upper)), 0);
        }
    }

    private void IncreaseLowerPressure()
    {
        lowerPressure += 0.1f;
        Debug.Log("공기 주입 - " + lowerPressure + "L");
    }

    private void OpenLowerOutlet()
    {
        rigidbody.velocity += transform.up * ((9.8f + (lowerPressure * Mathf.PI * Mathf.Pow(fuelRadius, 2))) / rocketMass);
        Debug.Log("발1사");
        pThrust = true;

        float velocity = rigidbody.velocity.y;
        velocity = velocity + Mathf.Log((rocketMass + waterMass_lower + waterMass_upper) / (rocketMass + waterMass_upper));
        rigidbody.velocity = transform.up * velocity;

        InvokeRepeating("PrimaryThrust", 0, 0.1f);
    }

    private void InitiateSecondaryThrust()
    {
        float velocity = rigidbody.velocity.y;
        velocity = velocity + Mathf.Log((rocketMass + waterMass_upper) / rocketMass);
        rigidbody.velocity = transform.up * velocity;

        Debug.Log("발2사");
        sThrust = true;
        InvokeRepeating("SecondaryThrust", 0, 0.1f);
    }

    private void ReactVitamin()
    {
        // TODO: 비타민 반응을 위한 코드를 구현해야 함 (상부연료통 내에서 CO2를 생성함)
    }

    private void PrimaryThrust()
    {
        FlightTime += 0.1f;
        if (waterMass_lower > 0)
        {
            waterMass_lower += -0.01f;
        }

        else
        {
            waterMass_lower = 0;
        }
    }

    private void SecondaryThrust()
    {
        if (waterMass_upper > 0)
        {
            waterMass_upper += -0.01f;

            float velocity = rigidbody.velocity.y;
            velocity = velocity + Mathf.Log((rocketMass + waterMass_lower + waterMass_upper) / (rocketMass + waterMass_upper));
            rigidbody.velocity = transform.up * velocity;
        }

        else
        {
            waterMass_upper = 0;
        }
    }

    /*private void Torque()
    {
        if (pThrust == true)
        {
            stable = GetComponent<Score>().stable;

            torqueDirection = transform.position - CenterofMass.transform.position;
            Vector3 position = rigidbody.position;
            ThrustPosition = rigidbody.position.y;

            if (position.y >= ThrustPosition - 5f && position.y <= ThrustPosition + 1.5f && pThrust == true && stable == false)
            {
                rigidbody.AddTorque(Random.Range(-1, 1), 0, 0);
                rigidbody.AddTorque(0, Random.Range(-1, 1), 0);
                rigidbody.AddTorque(0, 0, Random.Range(-1, 1));
            }
        }
    }*/
}