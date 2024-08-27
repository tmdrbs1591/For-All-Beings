using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    [SerializeField] public Transform target; // ��尡 ���� Ÿ��
    [SerializeField] private float speed = 3.0f; // �̵� �ӵ�
    [SerializeField] private float minDistance = 0.5f; // Ÿ�ٰ��� �ּ� �Ÿ�
    public bool isget; // ���� �� �ִ� ��������
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
                // Ÿ�ٰ� �ʹ� ����� �� �ε巴�� ���ߵ��� ����
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
        // ������ ���� ���� ���� (XZ ���)
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        // ������ ���� ũ�� ���� (���� ��� 1���� 3 ������ ������ ��)
        float randomForceMagnitude = Random.Range(3f, 6f);

        // ������ ����� ũ���� ���޽� �� ����
        rb.AddForce(randomDirection * randomForceMagnitude, ForceMode.Impulse);
    }

    void FollowTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed; // ���� ��ġ�� �����ϴ� ��� �ӵ��� ����
    }
}
