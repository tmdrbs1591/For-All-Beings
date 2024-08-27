using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    [SerializeField] public Transform target; // 골드가 따라갈 타겟
    [SerializeField] private float speed = 3.0f; // 이동 속도
    [SerializeField] private float minDistance = 0.5f; // 타겟과의 최소 거리
    public bool isget; // 먹을 수 있는 상태인지
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Jump();
    }

    void Update()
    {
        if (isget && target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > minDistance)
            {
                FollowTarget();
            }
            else
            {
                // 타겟과 너무 가까울 때 부드럽게 멈추도록 설정
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            Invoke("Get", 1f);
        }
    }

    void Get()
    {
        isget = true;
    }

    void Jump()
    {
        // 랜덤한 방향 벡터 생성 (XZ 평면)
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        // 랜덤한 힘의 크기 설정 (예를 들어 1에서 3 사이의 랜덤한 값)
        float randomForceMagnitude = Random.Range(3f, 6f);

        // 랜덤한 방향과 크기의 임펄스 힘 적용
        rb.AddForce(randomDirection * randomForceMagnitude, ForceMode.Impulse);
    }

    void FollowTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed; // 직접 위치를 설정하는 대신 속도를 조정
    }
}
