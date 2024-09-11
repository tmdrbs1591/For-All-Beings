using System.Collections;
using UnityEngine;
using Photon.Pun;
public class TutorialManager : MonoBehaviourPunCallbacks
{
    public static TutorialManager instance;
    public TextAnim textAnim;
    public GameObject messagePanel;
    private PlayerCtrl playerController; // PlayerController 스크립트의 참조를 저장할 변수
    private PlayerStats playerStats; // PlayerController 스크립트의 참조를 저장할 변수

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
        // Player 태그를 가진 게임 오브젝트를 찾습니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");


        // 게임 오브젝트가 null이 아니면 PlayerController 스크립트를 가져옵니다.
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
            Debug.Log("모든 몬스터 처치");
            monsterkillCount = 0;
        }
    }

    IEnumerator TutorialStart()
    {
 
        playerController.enabled = false;
        // 대기 시간
        yield return new WaitForSeconds(3f);

         Text("안녕 반가워!");
        yield return new WaitForSeconds(2f);
        Text("기본적인 게임 방식을 배워보자 !");
        yield return new WaitForSeconds(2f);
        Text("W,A,S,D 를 눌러 이동할 수 있어!");
        yield return new WaitForSeconds(2f);
        Text("앞에 목표 지점까지 이동해봐!");
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
        Text("잘했어!");
        yield return new WaitForSeconds(2f);
        Text("너 혹시 천재니?");
        yield return new WaitForSeconds(2f);
        Text("다음단계로 넘어가 볼까?");
        yield return new WaitForSeconds(2f);
        Text("마우스 좌클릭을 눌러 공격을 할 수 있어!");
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
        Text("좋았어!");
        yield return new WaitForSeconds(2f);
        Text("Q를 눌러 스킬을 사용할 수 있어!!");
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
        Text("좋았어!");
        yield return new WaitForSeconds(2f);
        Text("Space를 눌러 궁극기를 사용할 수있어!");
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
        Text("좋았어!");
        yield return new WaitForSeconds(2f);
        Text("이제 적들을 잡아볼까?");
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
        Text("포탈이 생성 되었어!");
        yield return new WaitForSeconds(2f);
        Text("헌번 들어가 볼까?");
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
        Text("다람쥐가 도토리를 다 떨어트렸어!");
        yield return new WaitForSeconds(2f);
        Text("다람쥐를 도와주자!");
        yield return new WaitForSeconds(2f);
        Text("[F]를 눌러 도토리를 줍고 바구니에 넣을 수 있어!");
        yield return new WaitForSeconds(2f);

        messagePanel.SetActive(false);
        playerController.enabled = true;
    }
}
