using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlantBoss : MonoBehaviour
{
    private Enemy enemyScript; // Enemy 스크립트를 참조하기 위한 변수
    private float timer = 0f; // 시간을 추적하기 위한 변수
    private bool isTracking = false; // 추적이 시작되었는지 여부를 나타내는 변수
    private bool isFiring = false; // 발사를 시작했는지 여부를 나타내는 변수

    private NavMeshAgent agent; // 보스의 NavMeshAgent 컴포넌트
    public BoxCollider boxCollider; // 보스의 BoxCollider 컴포넌트

    public GameObject effect;
    [SerializeField] GameObject bulletPrefabs;
    [SerializeField] GameObject dangerLine;
    [SerializeField] GameObject fireEffect;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 가져오기
        enemyScript = GetComponent<Enemy>(); // Enemy 스크립트를 가져옵니다

        boxCollider.enabled = false;
        enemyScript.enabled = false; // 처음에는 Enemy 스크립트를 비활성화합니다

        boxCollider = GetComponent<BoxCollider>(); // BoxCollider 컴포넌트 가져오기
        StartCoroutine(EffectOn());
        StartCoroutine(DelayPatternStart()); // 5초 후에 패턴 시작
    }

    // 패턴 시작을 5초 지연시키는 코루틴
    private IEnumerator DelayPatternStart()
    {
        yield return new WaitForSeconds(10f); // 5초 대기
        while (true)
        {
            Pattern();
            yield return null; // 다음 프레임까지 대기
        }
    }

    void Update()
    {
        timer += Time.deltaTime; // 타이머를 증가시킵니다

        if (timer >= 10f && !isTracking) // 10초가 지나면 Enemy 스크립트를 활성화합니다
        {
            enemyScript.enabled = true;
            boxCollider.enabled = true;
            isTracking = true; // 추적이 시작되었음을 표시합니다
        }
    }

    void Pattern() // 보스 패턴
    {
        if (enemyScript.currentHP > enemyScript.maxHP / 2)
        {
            if (!isFiring) // 발사가 시작되지 않았을 때만 호출
            {
                InvokeRepeating("Fire", 0f, 5f); // 반 이하일 때 5초마다 Fire 메서드를 호출하여 총알을 발사합니다.
                isFiring = true; // 발사 시작 상태로 설정
            }
        }
        else
        {
            if (isFiring) // HP가 절반 이하로 떨어지면 발사를 중지
            {
                CancelInvoke("Fire"); // 발사 반복 호출을 취소합니다.
                fireEffect.SetActive(true);
                isFiring = false; // 발사 중지 상태로 설정
                SecondPazeShoot();
            }
        }
    }

    void Fire()
    {
        StartCoroutine(FreezeFire());
    }

    public IEnumerator FreezeFire()
    {
        // 보스의 NavMeshAgent를 멈춤
        agent.isStopped = true;
        dangerLine.SetActive(true);

        // 2초 대기
        yield return new WaitForSeconds(2f);

        dangerLine.SetActive(false);

        // 총알 생성
        GameObject bullet = Instantiate(bulletPrefabs, transform.position, Quaternion.identity);
        bullet.transform.rotation = Quaternion.LookRotation(transform.forward);
        SingleAudioManager.instance.PlaySound(transform.position, 11, UnityEngine.Random.Range(1f, 1.7f), 1f);


        // 보스의 NavMeshAgent를 다시 활성화
        agent.isStopped = false;
    }


    public void SecondPazeShoot()
    {
        StartCoroutine(SpellStartCor());
    }

    IEnumerator SpellStartCor()
    {
        for (int j = 0; j < 5; j++)
        {
            yield return new WaitForSeconds(1f); // 발사 간격 기다림
            StartCoroutine(SpellStart());
        }
      
    }

    IEnumerator SpellStart()
    {
        float angleStep = 360f / 36;


        Debug.Log("asdasd");
        for (int i = 0; i < 200; i++)
        {
            Quaternion rotation = Quaternion.Euler(0f, angleStep * i, 0f); // 총알의 회전 각도 계산

            GameObject bullet = Instantiate(bulletPrefabs, transform.position, rotation); // 총알 생성
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

            SingleAudioManager.instance.PlaySound(transform.position, 11, UnityEngine.Random.Range(1f, 1.7f), 0.4f);

            // 총알의 방향과 속도 설정
            Vector3 shootDirection = bullet.transform.forward;
            bulletRigidbody.velocity = shootDirection * 10;

            Destroy(bullet, 2f);

            yield return new WaitForSeconds(0.2f); // 발사 간격 기다림
        }
    }


    IEnumerator EffectOn()
    {
        yield return new WaitForSeconds(2f);
        effect.SetActive(true);
    }

    private void OnDestroy()
    {
        SongManager.instance.InGameSongPlay();
    }
}
