using System.Collections;
using UnityEngine;
using Photon.Pun;
public class TutorialManager : MonoBehaviourPunCallbacks
{
    public static TutorialManager instance;
    public TextAnim textAnim;
    public GameObject messagePanel;
    private PlayerCtrl playerController; // PlayerController ��ũ��Ʈ�� ������ ������ ����
    private PlayerStats playerStats; // PlayerController ��ũ��Ʈ�� ������ ������ ����

    [SerializeField] GameObject monsterPrefabs;
    [SerializeField] Transform[] monsterSpawnPoint;
    [SerializeField] GameObject portal;

    Animator anim;

    public bool isMoveTutorialClear = false;
    public bool isAttackTutorialClear = false;
    public bool isSkillTutorialClear = false;

    public int monsterkillCount;


    // Start is called before the first frame update
    void Start()
    {
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

        if (isSkillTutorialClear && Input.GetKeyDown(KeyCode.Space))
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
        playerController.enabled = true ;
    }

    public IEnumerator AttackTutorialCor()
    {
        yield return new WaitForSeconds(2f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("���Ҿ�!");
        yield return new WaitForSeconds(2f);
        Text("Q�� ���� ��ų�� ����� �� �־�!!");
        yield return new WaitForSeconds(2f);
        messagePanel.SetActive(false);
        playerController.enabled = true;

        isAttackTutorialClear = true;
    }

    public IEnumerator SkillTutorialCor()
    {
        yield return new WaitForSeconds(2f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("���Ҿ�!");
        yield return new WaitForSeconds(2f);
        Text("Space�� ���� �ñر⸦ ����� ���־�!");
        yield return new WaitForSeconds(2f);
        messagePanel.SetActive(false);
        playerController.enabled = true;

        isSkillTutorialClear = true;

    }

    public IEnumerator SkillUltimateCor()
    {
        yield return new WaitForSeconds(5f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("���Ҿ�!");
        yield return new WaitForSeconds(2f);
        Text("���� ������ ��ƺ���?");
        yield return new WaitForSeconds(2f);
        messagePanel.SetActive(false);
        playerController.enabled = true;

        MonsterSpawn();
    }
    public IEnumerator PortalCor()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("��Ż�� ���� �Ǿ���!");
        yield return new WaitForSeconds(2f);
        Text("��� �� ����?");
        yield return new WaitForSeconds(2f);
        messagePanel.SetActive(false);
        playerController.enabled = true;

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
        yield return new WaitForSeconds(1f);
        anim.SetBool("isWalk", false);

        playerController.enabled = false;
        Text("�ٶ��㰡 ���丮�� �� ����Ʈ�Ⱦ�!");
        yield return new WaitForSeconds(2f);
        Text("�ٶ��㸦 ��������!");
        yield return new WaitForSeconds(2f);
        Text("[F]�� ���� ���丮�� �ݰ� �ٱ��Ͽ� ���� �� �־�!");
        yield return new WaitForSeconds(2f);

        messagePanel.SetActive(false);
        playerController.enabled = true;
    }
}
