using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 10f; // 총알의 속도
    public float lifetime = 5f; // 총알의 생명 시간

    private void Start()
    {
        // 총알이 생성된 후 일정 시간이 지나면 자동으로 파괴되도록 설정
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // 총알을 발사 방향으로 이동
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject); // 충돌 시 총알을 파괴
    }
}
