using UnityEngine;

public class DualThrusterRocket : MonoBehaviour
{
    public float gravityAcceleration = 9.8f; // �߷� ���ӵ�
    public float airResistance = 1f; // ���� ����

    private bool pThrust = false; // ���� ���� ����
    private bool sThrust = false; // ���� ���� ����

    private float lowerFuelPressure; // �Ϻ� ������ �з�

    public float secondaryThrustTime; // ���� ���� �ð�
    private float timeSinceLaunch; // �߻� �� ��� �ð�

    private Rigidbody rocketRigidbody; // ������ ������ٵ�

    public float rocketMass = 10f; // ������ ����

    public float upperFuelCapacity = 5f; // ��� ������ ���� ��
    public float lowerFuelCapacity = 50f; // �Ϻ� ������ ���� ��

    float airAmount; // �Ϻ� ������ ���Ե� ���� ��


    private void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // ���� ����(Space)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InjectAir();
        }

        // ���� ����(R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("�Ϻο����� �л� - ���� ����");
            PrimaryThrust();
        }

        // ���� ����
        if (pThrust && timeSinceLaunch >= secondaryThrustTime)
        {
            if (upperFuelCapacity > 0)
            {
                SecondaryThrust();
            }
        }

        // ���� ������Ʈ
        if (pThrust)
        {
            UpdatePhysics();
        }

        timeSinceLaunch += Time.deltaTime;
    }

    private void InjectAir()
    {
        airAmount += 1f;
        Debug.Log("���� ���� - " + airAmount + "L");
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
        // �߷� ����
        Vector3 gravity = Vector3.down * gravityAcceleration;
        rocketRigidbody.AddForce(gravity);

        // ���� ���� ����
        Vector3 airResistanceForce = -rocketRigidbody.velocity.normalized * airResistance;
        rocketRigidbody.AddForce(airResistanceForce);
    }

    private float CalculateThrustForce()
    {
        // ���� ���� �ӵ� ���
        float thrustForce = airAmount * lowerFuelCapacity / rocketMass;
        return thrustForce;
    }

    private float CalculateSecondary()
    {
        // ���� ������ �ӵ� ���
        float secondThrustForce = upperFuelCapacity / rocketMass;
        return secondThrustForce;
    }
}