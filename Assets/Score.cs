using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    //����(Score)
    public float score_Total; //���� ����(Total Score)

    //���� �ο� �׸�
    public float score_DAR; //0.1�ʴ� ��� rotation ��ȭ��(Delta Average Rotation)
    public float score_DMR; //�ִ� rotation ��ȭ��(Delta Maximum Rotation)
    public float score_SC; //���� ���� ����(Stable Condition)
    public float score_SBL; //���� ���� �ӵ�(Speed Before Landing)

    //�߿䵵(Importance) - ����ġ ����
    public float iDAR; //DAR�� �߿䵵(Importance of DAR)
    public float iDMR; //DMR�� �߿䵵(Importance of DMR)
    public float iSC; //SC�� �߿䵵(Importance of SC)
    public float iSBL; //SBL�� �߿䵵(Importance of SBL)

    //�ý��� ����
    public float rotation_I; //�ʱ� rotation(Initial Rotation)
    public float rotation_C; //���� rotation(Current Rotation)
    public float rotation_Max; //�ִ� rotation(Maximum Rotation)
    public bool stable; //���� ����

    Transform objectTransform; //��ü transform
    Rigidbody objectRigidboy; //��ü rigidbody

    float TotalDeltaRotation;

    public float TotalTime;
    int TotalTime_U;

    public bool pThrust;
    public bool sThrust;

    public float ThrustPosition;

    public void Start()
    {
        objectTransform = GetComponent<Transform>();
        Vector3 rotation = objectTransform.rotation.eulerAngles;
        rotation_I = rotation.x + rotation.z;

        InvokeRepeating("DAR", 0, 0.1f);
        InvokeRepeating("DMR", 0, 0.1f);
        InvokeRepeating("SC", 0, 0.1f);
        InvokeRepeating("SBL", 0, 0.1f);
    }

    void DAR()
    {
        if (stable == false)
        {
            Vector3 rotation = objectTransform.rotation.eulerAngles;
            rotation_C = rotation.x + rotation.z;

            if (pThrust == true)
            {
                TotalTime += 0.1f;
                TotalDeltaRotation += Mathf.Abs(rotation_I - rotation_C);

                score_DAR = -1 * iDAR * (TotalDeltaRotation / TotalTime);
            }
        }
    }

    void DMR()
    {
        if (stable == false)
        {
            Vector3 rotation = objectTransform.rotation.eulerAngles;
            rotation_C = rotation.x + rotation.z;

            if (rotation_Max == 0)
            {
                rotation_Max = Mathf.Abs(rotation_I - rotation_C);
            }

            if (rotation_Max < Mathf.Abs(rotation_I - rotation_C))
            {
                rotation_Max = Mathf.Abs(rotation_I - rotation_C);
            }
        }

        if (stable == true)
        {
            score_DMR = -1 * iDAR * rotation_Max;
        }
    }

    void SC()
    {
        if (stable == true)
        {
            if(Mathf.Abs(rotation_I - rotation_C) <= 0.1f)
            {
                score_SC = iSC;
            }

            if (Mathf.Abs(rotation_I - rotation_C) > 0.1f)
            {
                score_SC = -1 * iSC;
            }
        }
    }

    void SBL()
    {
        Vector3 position = objectTransform.position;
        if (pThrust == true)
        {
            objectRigidboy = GetComponent<Rigidbody>();

            float velocity = Mathf.Abs(objectRigidboy.velocity.x + objectRigidboy.velocity.y + objectRigidboy.velocity.z);
            if (position.y >= ThrustPosition && pThrust == true && velocity != 0 && stable == false)
            {
                score_SBL = -1 * iSBL * velocity;
            }
        }
    }

    public void Update()
    {
        pThrust = GetComponent<RocketSimulation>().pThrust;
        sThrust = GetComponent<RocketSimulation>().sThrust;
        Vector3 rotation = objectTransform.rotation.eulerAngles;
        Vector3 position = objectTransform.position;
        rotation_C = rotation.x + rotation.z;
        
        if (position.y >= ThrustPosition - 5f && position.y <= ThrustPosition + 1.5f && pThrust == true)
        {
            TotalTime_U += 1;
            if (TotalTime_U == 120)
            {
                stable = true; //2�ʰ� ���� ���� ������ ���� ����
            }
        }

        else
        {
            TotalTime_U = 0;
        }

        if (pThrust == false)
        {
            ThrustPosition = objectTransform.position.y;
        }
    }
}
