using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialChar : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] private float rotationSpeed = 10f; // ȸ�� �ӵ��� �����ϴ� ����
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

    [Header("��Ÿ��")]
    private float attacklCurTime;

    private float skilllCurTime;
    [SerializeField] private float dashCoolTime = 5f; // ��� ��Ÿ�� ����
    private float dashCurTime;

    [Header("UI")]
    [SerializeField] private Image skillFilled; // ��ų ��Ÿ���� ǥ���� �̹���
    [SerializeField] private TMP_Text skillText; // ��ų ��Ÿ���� ǥ���� �ؽ�Ʈ
    [SerializeField] private Image dashFiled; // ��� ��Ÿ���� ǥ���� �̹���
    [SerializeField] private TMP_Text dashText; // ��� ��Ÿ���� ǥ���� �ؽ�Ʈ

    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject playerUICanvas;

    [Header("Shop")]
    [SerializeField] public bool isShop;

    [Header("����")]
    [SerializeField] private AudioSource wakkAudioSource;

    [Header("ī�޶�")]
    [SerializeField] Camera MiniMapCamera;
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera ultimateCutSceneCamera;

    private float hAxis; // ���� �Է� ��
    private float vAxis; // ���� �Է� ��
    private bool jumpDown;
    private bool isDash;
    private bool isStop;
    private bool isSkill;

    private Animator anim; // �ִϸ����� ������Ʈ

    private int maxAttackCount = 3;
    private int curAttackCount = 0;
    private Coroutine attackCoroutine;

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float interpolationFactor = 30f; // ���� ���

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
            // �ٸ� �÷��̾��� ĵ������ ��Ȱ��ȭ
            playerCanvas.SetActive(false);
            playerUICanvas.SetActive(false);
            MiniMapCamera.gameObject.SetActive(false);
        }
        else
        {
            nickNameText.text = "PlayerName"; // �÷��̾� �̸� ����
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
        UpdateSkillUI(); // ��ų UI ������Ʈ �޼��� ȣ��
        UpdateDashUI(); // ��� UI ������Ʈ �޼��� ȣ��

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeAttackPower(playerStats.attackPower + 1f); // attackPower ���� �Լ� ȣ��
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
        if (isStop || isSkill || isNeverDie) // �����̳� ��ų �߿� �� �����̰�
            return;

        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        transform.position += moveVec * playerStats.speed * Time.deltaTime;

        anim.SetBool("isWalk", moveVec != Vector3.zero);

        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // �ȴ� �Ҹ��� ��� ���� �ƴϸ� ���
            if (!wakkAudioSource.isPlaying)
            {
                wakkAudioSource.Play();
            }
        }
        else
        {
            // ĳ���Ͱ� ���߸� �ȴ� �Ҹ� ����
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
                int randomIndex = UnityEngine.Random.Range(0, 5); // 0���� 4������ ������ ������ ���� �ε��� ����

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
                // Damage ó�� ���� �ʿ�
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
                    // Damage ó�� ���� �ʿ�
                    if (!isNeverDie)
                        playerStats.currentUltimategauge++;

                    enemyScript.playerObj = this.gameObject;
                    // SpawnDamageText ó�� ���� �ʿ�
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
                // �뽬 ����Ʈ Ȱ��ȭ
                StartCoroutine(EffectSetActive(0.3f, DashPtc));
                isDash = true;

                Vector3 dashDirection = new Vector3(hAxis, 0, vAxis).normalized;
                rigid.velocity = dashDirection * 20;
                StartCoroutine(IsStop(0.3f));

                // ��� �� �ӵ��� ������� ����
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
                // ��ų ����Ʈ Ȱ��ȭ
                StartCoroutine(EffectSetActive(0.5f, SkillPtc));
                // ��ų ó�� ���� �ʿ�
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
        // �ñر� ó�� ���� �ʿ�
    }

    bool isLocalPlayer()
    {
        // ���� �÷��̾� üũ ���� (��: PhotonNetwork.IsConnectedAndReady)
        return true; // �׽�Ʈ��
    }
}
