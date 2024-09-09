using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialChar : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도를 조절하는 변수
    [SerializeField] private float jumpPower = 10f;

    [SerializeField] Transform cameraPos;
    [SerializeField] TMP_Text nickNameText;

    [SerializeField] private GameObject AttackPtc1;
    [SerializeField] private GameObject AttackPtc2;
    [SerializeField] private GameObject AttackPtc3;
    [SerializeField] private GameObject DashPtc;
    [SerializeField] private GameObject SkillPtc;

    [SerializeField] private GameObject ultimatePtc;

    [SerializeField] private GameObject SkillPanel;

    [SerializeField] Vector3 attackBoxSize;
    [SerializeField] Transform attackBoxPos;
    [SerializeField] Slider hpBar;

    public bool isNeverDie;

    [Header("쿨타임")]
    private float attacklCurTime;

    private float skilllCurTime;
    [SerializeField] private float dashCoolTime = 5f; // 대시 쿨타임 설정
    private float dashCurTime;

    [Header("UI")]
    [SerializeField] private Image skillFilled; // 스킬 쿨타임을 표시할 이미지
    [SerializeField] private TMP_Text skillText; // 스킬 쿨타임을 표시할 텍스트
    [SerializeField] private Image dashFiled; // 대시 쿨타임을 표시할 이미지
    [SerializeField] private TMP_Text dashText; // 대시 쿨타임을 표시할 텍스트

    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject playerUICanvas;

    [Header("Shop")]
    [SerializeField] public bool isShop;

    [Header("사운드")]
    [SerializeField] private AudioSource wakkAudioSource;

    [Header("카메라")]
    [SerializeField] Camera MiniMapCamera;
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera ultimateCutSceneCamera;

    private float hAxis; // 수평 입력 값
    private float vAxis; // 수직 입력 값
    private bool jumpDown;
    private bool isDash;
    private bool isStop;
    private bool isSkill;

    private Animator anim; // 애니메이터 컴포넌트

    private int maxAttackCount = 3;
    private int curAttackCount = 0;
    private Coroutine attackCoroutine;

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float interpolationFactor = 30f; // 보간 계수

    public PlayerStats playerStats;

    protected void Awake()
    {
        wakkAudioSource = GetComponent<AudioSource>();
        playerStats = GetComponent<PlayerStats>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerStats.curHp = playerStats.maxHp;

        playerCamera = Camera.main;

        if (!isLocalPlayer())
        {
            // 다른 플레이어의 캔버스를 비활성화
            playerCanvas.SetActive(false);
            playerUICanvas.SetActive(false);
            MiniMapCamera.gameObject.SetActive(false);
        }
        else
        {
            nickNameText.text = "PlayerName"; // 플레이어 이름 설정
            nickNameText.color = Color.green;

            var CM = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
            CM.target = transform;
        }
    }

    protected void Update()
    {
        if (!isLocalPlayer()) return;
        if (playerStats.isDie)
            return;
        GetInput();
        Move();
        Attack();
        Dash();
        Skill();
        Jump();
        UpdateSkillUI(); // 스킬 UI 업데이트 메서드 호출
        UpdateDashUI(); // 대시 UI 업데이트 메서드 호출

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeAttackPower(playerStats.attackPower + 1f); // attackPower 증가 함수 호출
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("sasdasdasdasdsssssssssssssssss");
            anim.SetTrigger("isSleep");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartUltimate();
        }
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jumpDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        if (isStop || isSkill || isNeverDie) // 공격이나 스킬 중엔 못 움직이게
            return;

        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        transform.position += moveVec * playerStats.speed * Time.deltaTime;

        anim.SetBool("isWalk", moveVec != Vector3.zero);

        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 걷는 소리가 재생 중이 아니면 재생
            if (!wakkAudioSource.isPlaying)
            {
                wakkAudioSource.Play();
            }
        }
        else
        {
            // 캐릭터가 멈추면 걷는 소리 중지
            if (wakkAudioSource.isPlaying)
            {
                wakkAudioSource.Stop();
            }
        }
    }

    void Jump()
    {
        if (jumpDown)
        {
            anim.SetTrigger("isJump");
        }
    }

    void Attack()
    {
        if (attacklCurTime <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                int randomIndex = UnityEngine.Random.Range(0, 5); // 0부터 4까지의 범위를 가지는 랜덤 인덱스 생성

                if (randomIndex == 0)
                {
                    SingleAudioManager.instance.PlaySound(transform.position, 6, UnityEngine.Random.Range(1f, 1f), 0.3f);
                }
                else if (randomIndex == 1)
                {
                    SingleAudioManager.instance.PlaySound(transform.position, 7, UnityEngine.Random.Range(1f, 1f), 0.3f);
                }
                else
                {
                    SingleAudioManager.instance.PlaySound(transform.position, 8, UnityEngine.Random.Range(1f, 1f), 0.3f);
                }
                StartCoroutine(IsStop(0.2f));
                AudioManager.instance.PlaySound(transform.position, 0, Random.Range(1f, 0.9f), 0.4f);
                // Damage 처리 로직 필요
                attacklCurTime = playerStats.attackCoolTime;
                StartCoroutine(PlayerAttackAnim(curAttackCount));
                curAttackCount = (curAttackCount + 1) % maxAttackCount;
            }
        }
        else
        {
            attacklCurTime -= Time.deltaTime;
        }
    }

    IEnumerator PlayerAttackAnim(int attackIndex)
    {
        switch (attackIndex)
        {
            case 0:
                anim.SetTrigger("isAttack1");
                StartCoroutine(EffectSetActive(0.5f, AttackPtc1));
                break;
            case 1:
                anim.SetTrigger("isAttack2");
                StartCoroutine(EffectSetActive(0.5f, AttackPtc2));
                break;
            case 2:
                anim.SetTrigger("isAttack3");
                StartCoroutine(EffectSetActive(0.5f, AttackPtc3));
                break;
        }
        yield return null;
    }

    IEnumerator EffectSetActive(float time, GameObject effectObject)
    {
        effectObject.SetActive(true);
        yield return new WaitForSeconds(time);
        effectObject.SetActive(false);
    }

    void Damage(float damage)
    {
        Collider[] colliders = Physics.OverlapBox(attackBoxPos.position, attackBoxSize / 2f);
        foreach (Collider collider in colliders)
        {
            if (collider != null && collider.CompareTag("Enemy"))
            {
                var enemyScript = collider.gameObject.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    // Damage 처리 로직 필요
                    if (!isNeverDie)
                        playerStats.currentUltimategauge++;

                    enemyScript.playerObj = this.gameObject;
                    // SpawnDamageText 처리 로직 필요
                    if (enemyScript.currentHP - damage <= 0)
                    {
                        Debug.Log("Enemy defeated");
                    }
                }
            }
        }
    }

    public void ChangeAttackPower(float newAttackPower)
    {
        Debug.Log("Attack Power Changed");
        playerStats.attackPower = newAttackPower;
    }

    void Dash()
    {
        if (dashCurTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                dashCurTime = dashCoolTime;

                anim.SetTrigger("isDash");
                // 대쉬 이펙트 활성화
                StartCoroutine(EffectSetActive(0.3f, DashPtc));
                isDash = true;

                Vector3 dashDirection = new Vector3(hAxis, 0, vAxis).normalized;
                rigid.velocity = dashDirection * 20;
                StartCoroutine(IsStop(0.3f));

                // 대시 후 속도를 원래대로 복원
                rigid.velocity = Vector3.zero;
                isDash = false;
            }
        }
        else
        {
            dashCurTime -= Time.deltaTime;
        }
    }

    void Skill()
    {
        if (skilllCurTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                skilllCurTime = playerStats.skillCoolTime;
                anim.SetTrigger("isSkill");
                // 스킬 이펙트 활성화
                StartCoroutine(EffectSetActive(0.5f, SkillPtc));
                // 스킬 처리 로직 필요
            }
        }
        else
        {
            skilllCurTime -= Time.deltaTime;
        }
    }

    IEnumerator IsStop(float time)
    {
        isStop = true;
        yield return new WaitForSeconds(time);
        isStop = false;
    }

    void UpdateSkillUI()
    {
        if (skilllCurTime > 0)
        {
            skillFilled.fillAmount = 1 - (skilllCurTime / playerStats.skillCoolTime);
            skillText.text = Mathf.RoundToInt(skilllCurTime).ToString();
        }
        else
        {
            skillFilled.fillAmount = 0;
            skillText.text = "Ready";
        }
    }

    void UpdateDashUI()
    {
        if (dashCurTime > 0)
        {
            dashFiled.fillAmount = 1 - (dashCurTime / dashCoolTime);
            dashText.text = Mathf.RoundToInt(dashCurTime).ToString();
        }
        else
        {
            dashFiled.fillAmount = 0;
            dashText.text = "Ready";
        }
    }

    void StartUltimate()
    {
        anim.SetTrigger("isUltimate");
        StartCoroutine(EffectSetActive(0.5f, ultimatePtc));
        // 궁극기 처리 로직 필요
    }

    bool isLocalPlayer()
    {
        // 로컬 플레이어 체크 로직 (예: PhotonNetwork.IsConnectedAndReady)
        return true; // 테스트용
    }
}
