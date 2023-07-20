using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    //점수(Score)
    public float score_Total; //총합 점수(Total Score)

    //점수 부여 항목
    public float score_DAR; //0.1초당 평균 rotation 변화량(Delta Average Rotation)
    public float score_DMR; //최대 rotation 변화량(Delta Maximum Rotation)
    public float score_SC; //안정 상태 여부(Stable Condition)
    public float score_SBL; //착륙 직전 속도(Speed Before Landing)

    //중요도(Importance) - 가중치 역할
    public float iDAR; //DAR의 중요도(Importance of DAR)
    public float iDMR; //DMR의 중요도(Importance of DMR)
    public float iSC; //SC의 중요도(Importance of SC)
    public float iSBL; //SBL의 중요도(Importance of SBL)

    //시스템 정보
    public float rotation_I; //초기 rotation(Initial Rotation)
    public float rotation_C; //현재 rotation(Current Rotation)
    public float rotation_Max; //최대 rotation(Maximum Rotation)
    public bool stable; //안정 여부

    Transform objectTransform; //물체 transform
    Rigidbody objectRigidboy; //물체 rigidbody

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
                stable = true; //2초간 안정 상태 유지시 안정 상태
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
