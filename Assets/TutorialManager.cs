using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviourPunCallbacks
{
    public static TutorialManager instance;
    public TextAnim textAnim;
    public GameObject messagePanel;
    private PlayerCtrl playerController; // PlayerController ��ũ��Ʈ�� ������ ������ ����
    private PlayerStats playerStats; // PlayerController ��ũ��Ʈ�� ������ ������ ����

    [SerializeField] AudioSource audioSource;

    [SerializeField] GameObject monsterPrefabs;
    [SerializeField] Transform[] monsterSpawnPoint;
    [SerializeField] GameObject portal;
    [SerializeField] GameObject guidePanel;

    [SerializeField] Toggle questtoggle;
    [SerializeField] public TMP_Text questText;

    Animator anim;

    public bool isMoveTutorialClear = false;
    public bool isAttackTutorialClear = false;
    public bool isSkillTutorialClear = false;

    public int monsterkillCount;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        instance = this;
        // Player �±׸� ���� ���� ������Ʈ�� ã���ϴ�.
        GameObject player = GameObject.FindGameObjectWithTag("Player");


        // ���� ������Ʈ�� null�� �ƴϸ� PlayerController ��ũ��Ʈ�� �����ɴϴ�.
        if (player != null)
        {
            playerController = player.GetComponent<PlayerCtrl>();

        }
        playerStats = playerController.GetComponent<PlayerStats>();

        playerStats.skillCoolTime = 0.3f;

        playerStats.maxUltimategauge = 0;

        anim = playerController.GetComponent<Animator>();

        StartCoroutine(TutorialStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoveTutorialClear && Input.GetMouseButton(0))
        {
            StartCoroutine(AttackTutorialCor());
            isMoveTutorialClear = false;
        }

        if (isAttackTutorialClear && Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(SkillTutorialCor());
            isAttackTutorialClear = false;
        }

        if (isSkillTutorialClear && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(SkillUltimateCor());
            isSkillTutorialClear = false;
        }

        if (monsterkillCount >= 4)
        {
            portal.SetActive(true);
            StartCoroutine(PortalCor());
            Debug.Log("��� ���� óġ");
            monsterkillCount = 0;
        }
    }

    IEnumerator TutorialStart()
    {
 
        playerController.enabled = false;
        // ��� �ð�
        yield return new WaitForSeconds(3f);


         Text("�ȳ� �ݰ���!");
        yield return new WaitForSeconds(2f);
        Text("�⺻���� ���� ����� ������� !");
        yield return new WaitForSeconds(2f);
        Text("W,A,S,D �� ���� �̵��� �� �־�!");
        yield return new WaitForSeconds(2f);
        Text("�տ� ��ǥ �������� �̵��غ�!");
        yield return new WaitForSeconds(2f);
        messagePanel.SetActive(false);

        questText.text = "��ǥ �������� �̵��ϱ�";

        playerController.enabled = true;

    }

    void Text(string text)
    {

        textAnim.textToShow = text;
        messagePanel.SetActive(false);
        messagePanel.SetActive(true);
    }

    public void MoveTutorial()
    {
        StartCoroutine(MoveTutorialCor());
    }
    public IEnumerator MoveTutorialCor()
    {
        questtoggle.isOn = true;

        playerController.enabled = false;
        Text("���߾�!");
        yield return new WaitForSeconds(2f);
        Text("�� Ȥ�� õ���?");
        yield return new WaitForSeconds(2f);
        Text("�����ܰ�� �Ѿ ����?");
        yield return new WaitForSeconds(2f);
        Text("���콺 ��Ŭ���� ���� ������ �� �� �־�!");
        yield return new WaitForSeconds(2f);
        isMoveTutorialClear = true;
        messagePanel.SetActive(false);

        questtoggle.isOn = false;

        questText.text = "���콺 ��Ŭ���� ���� �����ϱ�";


        playerController.enabled = true ;
    }

    public IEnumerator AttackTutorialCor()
    {
        questtoggle.isOn = true;

        yield return new WaitForSeconds(2f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("���Ҿ�!");
        yield return new WaitForSeconds(2f);
        Text("[Q]�� ���� ��ų�� ����� �� �־�!!");
        yield return new WaitForSeconds(2f);
        messagePanel.SetActive(false);
        playerController.enabled = true;

        questtoggle.isOn = false;

        questText.text = "[Q]�� ���� ��ų ����ϱ�";

        isAttackTutorialClear = true;
    }

    public IEnumerator SkillTutorialCor()
    {
        questtoggle.isOn = true;
        yield return new WaitForSeconds(2f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("���Ҿ�!");
        yield return new WaitForSeconds(2f);
        Text("[R]�� ���� �ñر⸦ ����� ���־�!");
        yield return new WaitForSeconds(2f);
        messagePanel.SetActive(false);
        playerController.enabled = true;

        questText.text = "[R]�� ���� �ñر� ����ϱ�";


        questtoggle.isOn = false;

        isSkillTutorialClear = true;

    }

    public IEnumerator SkillUltimateCor()
    {
        questtoggle.isOn = true;

        yield return new WaitForSeconds(5f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("���Ҿ�!");
        yield return new WaitForSeconds(2f);
        Text("���� ������ ��ƺ���?");
        yield return new WaitForSeconds(2f);
        messagePanel.SetActive(false);
        playerController.enabled = true;

        questText.text =  "��� �� óġ�ϱ� " + monsterkillCount +"/ 4";

        questtoggle.isOn = false;

        MonsterSpawn();
    }
    public IEnumerator PortalCor()
    {
        questtoggle.isOn = true;

        yield return new WaitForSeconds(1f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("��Ż�� ���� �Ǿ���!");
        yield return new WaitForSeconds(2f);
        Text("��� �� ����?");
        yield return new WaitForSeconds(2f);
        messagePanel.SetActive(false);
        playerController.enabled = true;

        questText.text = "��Ż ����";


        questtoggle.isOn = false;


    }
    void MonsterSpawn()
    {
        for (int i = 0; i < monsterSpawnPoint.Length; i++)
        {
            PhotonNetwork.Instantiate(monsterPrefabs.name, monsterSpawnPoint[i].transform.position, Quaternion.identity);
        }
    }
    public void EventTutorial()
    {
        StartCoroutine(EventTutorialCor());
    }
    IEnumerator EventTutorialCor()
    {

        questtoggle.isOn = true;

        yield return new WaitForSeconds(1f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("�ٶ��㰡 ���丮�� �� ����Ʈ�Ⱦ�!");
        yield return new WaitForSeconds(2f);
        Text("�ٶ��㸦 ��������!");
        yield return new WaitForSeconds(2f);
        Text("[F]�� ���� ���丮�� �ݰ� �ٱ��Ͽ� ���� �� �־�!");
        yield return new WaitForSeconds(2f);

        questText.text = "�ٶ��� �����ֱ�";

        messagePanel.SetActive(false);
        playerController.enabled = true;

        questtoggle.isOn = false;
    }
    public void ShopEventTutorial()
    {
        StartCoroutine(ShopEventTutorialCor());
    }
    IEnumerator ShopEventTutorialCor()
    {
        questtoggle.isOn = true;

        yield return new WaitForSeconds(1f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("�����̾�!");
        yield return new WaitForSeconds(2f);
        Text("[F]�� ���� ������ ���� �����غ���!");
        yield return new WaitForSeconds(2f);

        messagePanel.SetActive(false);
        playerController.enabled = true;

        questText.text = "������ �����ϱ�";


        questtoggle.isOn = false;

    }

    public void LastEventTutorial()
    {
        StartCoroutine(LastEventTutorialCor());
    }
    IEnumerator LastEventTutorialCor()
    {
        questtoggle.isOn = true;

        yield return new WaitForSeconds(1f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("���Ҿ�!");
        yield return new WaitForSeconds(2f);
        Text("�����߾�!");
        yield return new WaitForSeconds(2f);

        messagePanel.SetActive(false);

        guidePanel.SetActive(true);


    }
}
