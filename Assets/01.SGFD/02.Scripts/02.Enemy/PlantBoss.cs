using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlantBoss : MonoBehaviour
{
    private Enemy enemyScript; // Enemy ��ũ��Ʈ�� �����ϱ� ���� ����
    private float timer = 0f; // �ð��� �����ϱ� ���� ����
    private bool isTracking = false; // ������ ���۵Ǿ����� ���θ� ��Ÿ���� ����
    private bool isFiring = false; // �߻縦 �����ߴ��� ���θ� ��Ÿ���� ����

    private NavMeshAgent agent; // ������ NavMeshAgent ������Ʈ
    public BoxCollider boxCollider; // ������ BoxCollider ������Ʈ

    public GameObject effect;
    [SerializeField] GameObject bulletPrefabs;
    [SerializeField] GameObject dangerLine;
    [SerializeField] GameObject fireEffect;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent ������Ʈ ��������
        enemyScript = GetComponent<Enemy>(); // Enemy ��ũ��Ʈ�� �����ɴϴ�

        boxCollider.enabled = false;
        enemyScript.enabled = false; // ó������ Enemy ��ũ��Ʈ�� ��Ȱ��ȭ�մϴ�

        boxCollider = GetComponent<BoxCollider>(); // BoxCollider ������Ʈ ��������
        StartCoroutine(EffectOn());
        StartCoroutine(DelayPatternStart()); // 5�� �Ŀ� ���� ����
    }

    // ���� ������ 5�� ������Ű�� �ڷ�ƾ
    private IEnumerator DelayPatternStart()
    {
        yield return new WaitForSeconds(10f); // 5�� ���
        while (true)
        {
            Pattern();
            yield return null; // ���� �����ӱ��� ���
        }
    }

    void Update()
    {
        timer += Time.deltaTime; // Ÿ�̸Ӹ� ������ŵ�ϴ�

        if (timer >= 10f && !isTracking) // 10�ʰ� ������ Enemy ��ũ��Ʈ�� Ȱ��ȭ�մϴ�
        {
            enemyScript.enabled = true;
            boxCollider.enabled = true;
            isTracking = true; // ������ ���۵Ǿ����� ǥ���մϴ�
        }
    }

    void Pattern() // ���� ����
    {
        if (enemyScript.currentHP > enemyScript.maxHP / 2)
        {
            if (!isFiring) // �߻簡 ���۵��� �ʾ��� ���� ȣ��
            {
                InvokeRepeating("Fire", 0f, 5f); // �� ������ �� 5�ʸ��� Fire �޼��带 ȣ���Ͽ� �Ѿ��� �߻��մϴ�.
                isFiring = true; // �߻� ���� ���·� ����
            }
        }
        else
        {
            if (isFiring) // HP�� ���� ���Ϸ� �������� �߻縦 ����
            {
                CancelInvoke("Fire"); // �߻� �ݺ� ȣ���� ����մϴ�.
                fireEffect.SetActive(true);
                isFiring = false; // �߻� ���� ���·� ����
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
        // ������ NavMeshAgent�� ����
        agent.isStopped = true;
        dangerLine.SetActive(true);

        // 2�� ���
        yield return new WaitForSeconds(2f);

        dangerLine.SetActive(false);

        // �Ѿ� ����
        GameObject bullet = Instantiate(bulletPrefabs, transform.position, Quaternion.identity);
        bullet.transform.rotation = Quaternion.LookRotation(transform.forward);
        SingleAudioManager.instance.PlaySound(transform.position, 11, UnityEngine.Random.Range(1f, 1.7f), 1f);


        // ������ NavMeshAgent�� �ٽ� Ȱ��ȭ
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
            yield return new WaitForSeconds(1f); // �߻� ���� ��ٸ�
            StartCoroutine(SpellStart());
        }
      
    }

    IEnumerator SpellStart()
    {
        float angleStep = 360f / 36;


        Debug.Log("asdasd");
        for (int i = 0; i < 200; i++)
        {
            Quaternion rotation = Quaternion.Euler(0f, angleStep * i, 0f); // �Ѿ��� ȸ�� ���� ���

            GameObject bullet = Instantiate(bulletPrefabs, transform.position, rotation); // �Ѿ� ����
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

            SingleAudioManager.instance.PlaySound(transform.position, 11, UnityEngine.Random.Range(1f, 1.7f), 0.4f);

            // �Ѿ��� ����� �ӵ� ����
            Vector3 shootDirection = bullet.transform.forward;
            bulletRigidbody.velocity = shootDirection * 10;

            Destroy(bullet, 2f);

            yield return new WaitForSeconds(0.2f); // �߻� ���� ��ٸ�
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
