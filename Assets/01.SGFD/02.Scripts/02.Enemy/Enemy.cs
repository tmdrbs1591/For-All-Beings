using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] string Enemytype;
    [SerializeField] public float currentHP;
    [SerializeField] public  float maxHP;
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] float maxAttackSpeed;
    [SerializeField] float curAttackSpeed;
    [SerializeField] Slider hpBar;
    [SerializeField] Slider hpBar2;
    [SerializeField] GameObject attackBox;
    [SerializeField] GameObject gold; // 죽었을때 생성할 골드
    [SerializeField] float goldCount; // 골드 수

    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject DangerMarker;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] int songIndex;

    private float playerDistance;
    private float syncInterval = 0.1f; // 동기화 간격 (초)
    private float lastSyncTime;

    Animator anim;
    public GameObject playerObj;
    private PhotonView PV;
    NavMeshAgent agent;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
        if (hpBar != null)
        {
            hpBar.gameObject.SetActive(false);
            hpBar2.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StageManager.instance.photonView.RPC("MonsterDied", RpcTarget.All);
        }
    }


    protected virtual void Update()
    {
        if (playerObj == null)
        {
            // 중심 위치와 반지름을 설정하여 오버랩 서클 사용
            Collider[] colliders = Physics.OverlapSphere(transform.position, 20f);
            // 오버랩 서클에서 플레이어 태그를 가진 오브젝트를 찾기
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    playerObj = collider.gameObject;
                    break; // 플레이어를 찾았으므로 중지
                }
            }
        }

        if (playerObj != null)
        {
            Vector3 moveVec = (playerObj.transform.position - transform.position).normalized;
            anim.SetBool("isWalk", moveVec != Vector3.zero);

            if (Enemytype != "Box")
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveVec); // 목표 회전을 이동 방향 벡터로 설정
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3 * Time.deltaTime); // 현재 회전에서 목표 회전까지 부드럽게 회전

                agent.SetDestination(playerObj.transform.position); // 플레이어 방향으로 이동
            }

            playerDistance = Vector3.Distance(transform.position, playerObj.transform.position); // 거리 계산

            if (curAttackSpeed < 0 && playerDistance < attackRange)
            {
                curAttackSpeed = maxAttackSpeed;
                anim.SetTrigger("isAttack");
                Debug.Log("플레이어를 공격함");
            }
            else
            {
                curAttackSpeed -= Time.deltaTime;
            }

            // 동기화 간격에 따른 위치 동기화
            if (photonView.IsMine && Time.time - lastSyncTime > syncInterval && Enemytype != "Box")
            {
                photonView.RPC("SyncEnemyPosition", RpcTarget.Others, transform.position);
                lastSyncTime = Time.time;
            }
        }

        if (Enemytype != "Box")
        {
            // NavMeshAgent가 목적지에 도달했는지 확인
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                anim.SetBool("isWalk", false);
            }
            else
            {
                anim.SetBool("isWalk", true);
            }
        }

        if (hpBar != null || hpBar2 != null)
        {
            hpBar.value = Mathf.Lerp(hpBar.value, currentHP / maxHP, Time.deltaTime * 40f);
            hpBar2.value = Mathf.Lerp(hpBar2.value, currentHP / maxHP, Time.deltaTime * 5f);
        }

        Die();
    }

    void Attack()
    {
        photonView.RPC("RPC_AttackBoxActive", RpcTarget.All);
    }

    void Die()
    {
        if (currentHP <= 0)
        {
            for (int i = 0; i < goldCount; i++)
            {
                GameObject enemygold = Instantiate(gold, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                Gold goldComponent = enemygold.GetComponent<Gold>();
                goldComponent.isget = false;

                // 현재 플레이어 오브젝트를 타겟으로 설정
                goldComponent.target = playerObj.transform;
            }

            Destroy(gameObject);
        }
    }

    [PunRPC]
    void SyncEnemyPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (damage > 0)
        {
           // PhotonNetwork.Instantiate("BloodPtc", transform.position + new Vector3(0, -1.5f, 0), Quaternion.identity);

            if (hpBar != null || hpBar2 != null)
            {
                hpBar.gameObject.SetActive(true);
                hpBar2.gameObject.SetActive(true);
            }

            CameraShake.instance.Shake();
            AudioManager.instance.PlaySound(transform.position, songIndex, Random.Range(1.0f, 1.3f), 0.4f);
            anim.SetTrigger("isDamage");
            currentHP -= damage;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHP);
        }
        else
        {
            currentHP = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void RPC_AttackBoxActive()
    {
        StartCoroutine(AttackBoxActive());
    }

    [PunRPC]
    void SyncSpellStart(Vector3 firePosition, Quaternion fireRotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePosition, fireRotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        Vector3 shootDirection = bullet.transform.forward;
        bulletRigidbody.velocity = shootDirection * 10;
    }

    IEnumerator AttackBoxActive()
    {
        attackBox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        attackBox.SetActive(false);
    }

    [PunRPC]
    public void DecreaseMonsterCount()
    {
        StageManager.instance.photonView.RPC("DecreaseMonsterCount", RpcTarget.All);
    }

    [PunRPC]
    public void StatUp(float upHp, float upAttack)
    {
        maxHP += upHp;
        currentHP = maxHP;
        attackDamage += upAttack;
    }
}
