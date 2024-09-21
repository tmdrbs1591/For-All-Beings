using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 10f; // �Ѿ��� �ӵ�
    public float lifetime = 5f; // �Ѿ��� ���� �ð�

    private void Start()
    {
        // �Ѿ��� ������ �� ���� �ð��� ������ �ڵ����� �ı��ǵ��� ����
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // �Ѿ��� �߻� �������� �̵�
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // �浹 �� �Ѿ��� �ı�
        }
    }
}
