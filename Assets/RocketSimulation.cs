using UnityEngine;

public class RocketSimulation : MonoBehaviour
{
    public float rocketMass;                // ���� ���� ���� (����: kg)
    public float waterMass_upper;           // [��ο�����] �� ���� ���� (���� : kg)
    public float waterMass_lower;           // [�Ϻο�����] �� ���� ���� (���� : kg)
    public float upperPressure;             // [��ο�����] �� CO2 �з� (����: atm)
    public float lowerPressure;             // [�Ϻο�����] �� ���� �з� (����: atm)

    public bool pThrust = false;           // ���� ���� ���� (���� ������ �߻��ϸ� true, �׷��� ������ false, �⺻���� false)
    public bool sThrust = false;           // ���� ������ ���� (���� �������� �߻��ϸ� true, �׷��� ������ false, �⺻���� false)

    public float secondaryThrustTime;       // ���� ���� ����(pThrust = true�� �� ����) [��ο�����] ������ ���� �ۿ����� ������Ÿ���� ���� ���� ���������� �ð�
                                            // (pThrust = true�� �ǰ� secondaryThrustTime�� �� sThrust = true�� ��)

    public float vitaminReactionTime;       // ������Ÿ���� ���� ���� ����(secondaryThrustTime) ���� ������Ÿ���� ��� �����Ͽ� CO2�� �߻��ϴ� ���������� �ð�

    public float pressureLimit;             // [��ο�����] ���ⱸ�� ������ �ϴ� [��ο�����] ������ CO2 �з��� �Ӱ���

    public float fuelRadius;                //������ ������(���� : cm)

    public float FlightTime;

    public float windSpeed;                   // ǳ��
    public float windDirection;               // ǳ��
    public new Rigidbody rigidbody;
    public GameObject CenterofMass;           // ������ ���� �߽�
    public float CoM_Position;                // ������ ���� �߽��� ��ġ
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
        // 'Space' Ű�� ������ [�Ϻο�����] ���ο� ���⸦ �����Ͽ� �з��� ������Ŵ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IncreaseLowerPressure();
        }

        // 'L' Ű�� ������ [�Ϻο�����]�� ���ⱸ�� ������, [�Ϻο�����] ������ ���⿡ ���� �з����� ���� ���ⱸ�� ���ϰ� �л�Ǿ� ���� ������ ���۵�
        if (Input.GetKeyDown(KeyCode.L))
        {
            OpenLowerOutlet();
        }

        // ���� ������ ���� �ð� üũ
        if (pThrust && !sThrust && FlightTime >= secondaryThrustTime)
        {
            InitiateSecondaryThrust();
        }

        // ��Ÿ�� ������ ���� �ð� üũ
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
        Debug.Log("���� ���� - " + lowerPressure + "L");
    }

    private void OpenLowerOutlet()
    {
        rigidbody.velocity += transform.up * ((9.8f + (lowerPressure * Mathf.PI * Mathf.Pow(fuelRadius, 2))) / rocketMass);
        Debug.Log("��1��");
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

        Debug.Log("��2��");
        sThrust = true;
        InvokeRepeating("SecondaryThrust", 0, 0.1f);
    }

    private void ReactVitamin()
    {
        // TODO: ��Ÿ�� ������ ���� �ڵ带 �����ؾ� �� (��ο����� ������ CO2�� ������)
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