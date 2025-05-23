using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Pun.Demo.Cockpit.Forms;
using UnityEngine.Experimental.GlobalIllumination;

[System.Serializable]
public class StageInfo
{
    public Transform spawnPos;
    public Transform[] monsterSpawnPos;
    public Transform portalPos;
}

[System.Serializable]
public class BossMonster
{
    public string BossName;
    public GameObject bossObj;
}

[System.Serializable]
public class StageIcon
{
    public string iconName;
    public GameObject icon;
}

[System.Serializable]
public class EventStageInfo
{
    public Transform SpawnPos;
    public Transform PortalPos;
}

[System.Serializable]
public class MonsterInfo
{
    public int spawnStage;
    public GameObject monsterPrefab;
}

public class StageManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static StageManager instance;

    [Header("StageInfo")]
    public List<StageInfo> stageInfos = new List<StageInfo>();

    [Header("Shop")]
    public List<Transform> shopPosition = new List<Transform>();

    [Header("Boss")]
    public List<StageInfo> bossPosition = new List<StageInfo>();
    public List<BossMonster> bossMonsters = new List<BossMonster>();

    [Header("EventStage")]
    public List<EventStageInfo> eventStage = new List<EventStageInfo>();
    public int stagePercentage = 5;
    public AcornEvent arconEvent;
    public OstrichEvent ostrichEvent;
    public Flag flag;
    public MeteoEvent meteoEvent;
    [SerializeField] private bool isEvent;

    [Header("Monster")]
    [SerializeField] private List<MonsterInfo> monsterInfos = new List<MonsterInfo>();
    private List<GameObject> currentSpawnMonsters = new List<GameObject>();
    private int killCount = 0;
    private int totalMonsters = 0;
    private int currentStageMonsterListLength = 1;
    [SerializeField] private TextMeshProUGUI totalMonstersText;

    [Header("StageCount")]
    public int currentStage = 0;
    public int lastStage;

    [Header("Portal")]
    public GameObject portalObj;

    [SerializeField] TMP_Text stageText;

    private float stageCooldown = 2f; // 다음 스테이지로 이동할 수 있는 쿨다운 시간 (초)
    private float lastStageChangeTime = 0f; // 마지막 스테이지 변경 시간

    [Header("StageBar")]
    [SerializeField] public List<StageIcon> stageIcons = new List<StageIcon>();
    [SerializeField] public List<Transform> stagePoss = new List<Transform>();
    [SerializeField] private Image stageBar;

    [Header("StatUp")]
    [SerializeField] private float hpUp = 0;
    [SerializeField] private float attackUp = 0;
    [SerializeField] private float bossHpUp = 0;
    [SerializeField] private float bossAttackUp = 0;

    [Header("Camera")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject cutSceneCamera;

    [SerializeField] private GameObject eventMessagePanel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 전환되어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateStageIcons();
    }

    public void NextStage()
    {
        // 쿨다운 확인
        if (Time.time - lastStageChangeTime < stageCooldown)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트에서만 스테이지 변경 로직을 수행
            //if (currentStage == 10)
            //{
            //    currentStageMonsterListLength++;

            //    if (currentStageMonsterListLength > monsterInfos.Length)
            //    {
            //        currentStageMonsterListLength = monsterInfos.Length;
            //    }
            //}

            // 스테이지 증가
            currentStage++;

            stageBar.fillAmount = 0;

            // stageBar를 부드럽게 0.25까지 증가시키는 코루틴 호출
            StartCoroutine(FillStageBar(0.28f, 1.5f)); // 1.5초 동안 0.25까지 증가

            Transform targetPosition = null;

            currentSpawnMonsters.Clear();
            killCount = 0;
            totalMonsters = 0; // totalMonsters 초기화

            // 5, 15, 25, 35 스테이지일 때는 ShopPosition으로 이동
            if (currentStage > 0 && currentStage % 10 == 5)
            {
                Debug.Log("상점 스테이지 입장합니다. 현재 스테이지 : " + currentStage);
                int shopIndex = Random.Range(0, shopPosition.Count);
                targetPosition = shopPosition[shopIndex];
               AudioManager.instance.PlaySound(transform.position, 17, Random.Range(1f, 1f), 1f);
            }
            // 10, 20, 30, 40, 50 스테이지일 때는 BossPosition으로 이동
            else if (currentStage  > 0 && currentStage % 10 == 0)
            {
                Debug.Log("보스 스테이지 입장합니다. 현재 스테이지 : " + currentStage);
                int bossIndex = Random.Range(0, bossPosition.Count);
                int bossMonsterIndex = Random.Range(0, bossMonsters.Count);
                targetPosition = bossPosition[bossIndex].spawnPos;
                AudioManager.instance.PlaySound(transform.position, 17, Random.Range(1f, 1f), 1f);



                photonView.RPC("CutSceneCamera", RpcTarget.All); // 컷씬 시작

                SongManager.instance.BossSongPlay();// 보스 노래로


                // 포탈 위치와 상태 설정을 Co_PortalSet 코루틴으로 대체
                StartCoroutine(Co_PortalSet(false, bossPosition[bossIndex].portalPos));

                // spawnPos 대신 monsterSpawnPos를 사용하여 올바르게 반복문을 돌도록 수정
                foreach (Transform t in bossPosition[bossIndex].monsterSpawnPos)
                {
                    GameObject boss = PhotonNetwork.Instantiate(bossMonsters[bossMonsterIndex].bossObj.name, t.position, t.rotation);
                    boss.GetComponent<Enemy>().photonView.RPC("StatUp", RpcTarget.All, bossHpUp * currentStage, attackUp * currentStage);
                    currentSpawnMonsters.Add(boss);
                    totalMonsters++;
                }
                isEvent = false;
                eventMessagePanel.SetActive(false);
            }
            else
            {
                int isEventStage = Random.Range(0, stagePercentage);

                if (isEventStage == 0 && !isEvent && currentStage % 10 != 0)
                {
                    isEvent = true;

                    AudioManager.instance.PlaySound(transform.position, 17, Random.Range(1f, 1f), 1f);

                    int randomIndex = Random.Range(0, eventStage.Count);
                    targetPosition = eventStage[randomIndex].SpawnPos;

                    StartCoroutine(EventSoundCor());

                    // 포탈 위치와 상태 설정을 Co_PortalSet 코루틴으로 대체
                    StartCoroutine(Co_PortalSet(false, eventStage[randomIndex].PortalPos));

                    StartCoroutine(Co_StartEventStage(randomIndex));
                }
                else
                {
                    AudioManager.instance.PlaySound(transform.position, 17, Random.Range(1f, 1f), 1f);

                    // 일반 스테이지일 때는 stageInfos에서 랜덤하게 spawnPos 선택
                    int randomIndex = Random.Range(0, stageInfos.Count);
                    while (randomIndex == lastStage)
                    {
                        randomIndex = Random.Range(0, stageInfos.Count);
                    }

                    targetPosition = stageInfos[randomIndex].spawnPos;

                    int monsterIndex;
                    List<GameObject> monsterPrefabs = new List<GameObject>();

                    foreach(var obj in monsterInfos)
                    {
                        if(obj.spawnStage <= currentStage)
                        {
                            monsterPrefabs.Add(obj.monsterPrefab);
                        }
                    }

                    foreach (Transform t in stageInfos[randomIndex].monsterSpawnPos)
                    {
                        monsterIndex = Random.Range(0, monsterPrefabs.Count);
                        GameObject monster = PhotonNetwork.Instantiate(monsterPrefabs[monsterIndex].name, t.position, t.rotation);
                        monster.GetComponent<Enemy>().photonView.RPC("StatUp", RpcTarget.All, hpUp * currentStage, attackUp * currentStage);
                        currentSpawnMonsters.Add(monster);
                        totalMonsters++; // 몬스터 수 증가
                    }

                    // 포탈 위치와 상태 설정을 Co_PortalSet 코루틴으로 대체
                    StartCoroutine(Co_PortalSet(false, stageInfos[randomIndex].portalPos));

                    lastStage = randomIndex;
                    isEvent = false;
                }
            }

            if (targetPosition != null)
            {
                lastStageChangeTime = Time.time; // 현재 시간을 마지막 변경 시간으로 설정

                // 모든 클라이언트에서 RPC 호출하여 currentStage와 함께 MovePlayer 메서드 실행
                photonView.RPC("MovePlayer", RpcTarget.All, currentStage, targetPosition.position, targetPosition.rotation);
            }
            else
            {
                Debug.LogError("Target position is null.");
            }

            // 스테이지 아이콘 업데이트
            photonView.RPC("UpdateStageIcons", RpcTarget.All);

            // totalMonsters 값 동기화
            photonView.RPC("SyncTotalMonsters", RpcTarget.All, totalMonsters);

            // 스테이지 설정이 끝난 후 추가 동기화 호출
            photonView.RPC("SyncStageData", RpcTarget.All, currentStage, killCount, totalMonsters);
        }
        photonView.RPC("UpdateText", RpcTarget.All);
    }

    [PunRPC]
    private void SyncTotalMonsters(int total)
    {
        totalMonsters = total;
        totalMonstersText.text = "" + (totalMonsters - killCount);
    }

    [PunRPC]
    private void SyncStageData(int stage, int kills, int total)
    {
        currentStage = stage;
        killCount = kills;
        totalMonsters = total;
        totalMonstersText.text = "" + (totalMonsters - killCount);
    }

    [PunRPC]
    public void UpdateText()
    {
        stageText.text = $"{currentStage} 스테이지";
        totalMonstersText.text = "" + (totalMonsters - killCount);
    }

    IEnumerator FillStageBar(float targetValue, float duration)
    {
        float startValue = stageBar.fillAmount;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            stageBar.fillAmount = Mathf.Lerp(startValue, targetValue, time / duration);
            yield return null;
        }

        stageBar.fillAmount = targetValue;
    }

    [PunRPC]
    public void MovePlayer(int stage, Vector3 position, Quaternion rotation)
    {
        currentStage = stage;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            StartCoroutine(TP(player, position, rotation));
        }
        else
        {
            Debug.LogError("Player not found.");
        }
    }

    IEnumerator Co_PortalSet(bool state, Transform PoatalPos)
    {
        yield return new WaitForSeconds(1.3f);
        // 포탈 위치 설정
        photonView.RPC("SetPortalPosition", RpcTarget.All, PoatalPos.position);
        // 포탈 비활성화
        photonView.RPC("SetPortalState", RpcTarget.All, state);
    }

    IEnumerator TP(GameObject player, Vector3 position, Quaternion rotation)
    {
        NetworkManager.instance.Fade();
        yield return new WaitForSeconds(1.3f);
        player.transform.position = position;
        player.transform.rotation = rotation;
    }

    [PunRPC]
    private void UpdateStageIcons()
    {
        foreach (var icon in stageIcons)
        {
            icon.icon.SetActive(false);
        }

        int defaulIconCount = 0;
        int stagePosUpandDown = 0;
        bool isShop = false;
        bool isBoss = false;

        for (int i = 0; i < stagePoss.Count; i++)
        {
            if (i == 0)
            {
                stagePosUpandDown = -1;
            }
            else if (i > 0)
            {
                stagePosUpandDown++;
            }
            Debug.Log("현재 표시할 스테이지 : " + (currentStage + stagePosUpandDown));

            if ((currentStage + stagePosUpandDown) % 10 == 0 && i != 4 && (currentStage + stagePosUpandDown) > 0)
            {
                stageIcons[5].icon.transform.position = stagePoss[i].position;
                stageIcons[5].icon.SetActive(true);
                isBoss = true;
            }
            else if ((currentStage + stagePosUpandDown) % 10 == 5 && i != 4 && (currentStage + stagePosUpandDown) > 0)
            {
                stageIcons[4].icon.transform.position = stagePoss[i].position;
                stageIcons[4].icon.SetActive(true);
                isShop = true;
            }
            else
            {
                if (i == 4)
                {
                    if (isShop == true)
                    {
                        stageIcons[5].icon.transform.position = stagePoss[i].position;
                        stageIcons[5].icon.SetActive(true);
                    }
                    else if (isBoss == true)
                    {
                        stageIcons[4].icon.transform.position = stagePoss[i].position;
                        stageIcons[4].icon.SetActive(true);
                    }
                    else if (!isShop && !isBoss)
                    {
                        int nearest5 = FindNearestMultiple(currentStage, 5);
                        int nearest10 = FindNearestMultiple(currentStage, 10);

                        if (Mathf.Abs(currentStage - nearest5) < Mathf.Abs(currentStage - nearest10))
                        {
                            stageIcons[5].icon.transform.position = stagePoss[i].position;
                            stageIcons[5].icon.SetActive(true);
                        }
                        else
                        {
                            stageIcons[4].icon.transform.position = stagePoss[i].position;
                            stageIcons[4].icon.SetActive(true);
                        }
                    }
                }
                else
                {
                    stageIcons[defaulIconCount].icon.transform.position = stagePoss[i].position;
                    stageIcons[defaulIconCount].icon.SetActive(true);
                    defaulIconCount++;
                }
            }
        }
    }

    int FindNearestMultiple(int num, int multiple)
    {
        int lower = (num / multiple) * multiple;
        int upper = lower + multiple;

        if (Mathf.Abs(num - lower) < Mathf.Abs(num - upper))
        {
            return lower;
        }
        else
        {
            return upper;
        }
    }

    public void KillMonster()
    {
        killCount++;
        stageBar.fillAmount = (float)killCount / totalMonsters;

        if (killCount >= totalMonsters)
        {
            portalObj.SetActive(true);
        }

        photonView.RPC("UpdateKillCount", RpcTarget.Others, killCount);
    }

    [PunRPC]
    private void SetPortalState(bool state)
    {
        portalObj.SetActive(state);
    }

    [PunRPC]
    private void SetPortalPosition(Vector3 position)
    {
        portalObj.transform.position = position;
    }

    [PunRPC]
    public void MonsterDied()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            killCount++;
            if (killCount >= totalMonsters)
            {
                photonView.RPC("SetPortalState", RpcTarget.All, true);
            }
            photonView.RPC("UpdateKillCount", RpcTarget.All, killCount); // killCount 동기화
            photonView.RPC("UpdateText", RpcTarget.All);
        }
    }

    [PunRPC]
    public void EventCheck()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (flag.isClear || arconEvent.isEventEnd || meteoEvent.eventClear)
            {
                photonView.RPC("SetPortalState", RpcTarget.All, true);
                flag.isClear = false;
                arconEvent.isEventEnd = false;
                meteoEvent.eventClear = false;
            }
        }
    }

    IEnumerator Co_StartEventStage(int eventStage)
    {
        yield return new WaitForSeconds(2);
        if (eventStage == 0)
        {
            arconEvent.EventStart();
        }
        else if (eventStage == 1)
        {
            ostrichEvent.photonView.RPC("EventStart", RpcTarget.All);
        }
        else if(eventStage == 2)
        {
            meteoEvent.photonView.RPC("EventStart", RpcTarget.All);
        }
    }

public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        stream.SendNext(currentStage);
        stream.SendNext(killCount);
        stream.SendNext(totalMonsters);
    }
    else
    {
        currentStage = (int)stream.ReceiveNext();
        killCount = (int)stream.ReceiveNext();
        totalMonsters = (int)stream.ReceiveNext();
    }
}


    [PunRPC]
    private void UpdateKillCount(int newKillCount)
    {
        killCount = newKillCount;
        totalMonstersText.text = "남은 몬스터 수 : " + (totalMonsters - killCount);
    }
    [PunRPC]
    void CutSceneCamera()
    {
        StartCoroutine(CutSceneCameraCor());
    }
    IEnumerator CutSceneCameraCor()
    {
        yield return new WaitForSeconds(2f);


        // Canvas 태그를 가진 모든 객체를 가져옵니다
        GameObject[] canvases = GameObject.FindGameObjectsWithTag("Canvas");

        // 모든 Canvas 객체를 비활성화합니다
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(false);
        }

        // 카메라 전환
        playerCamera.SetActive(false);
        cutSceneCamera.SetActive(true);

        // 6초 대기
        yield return new WaitForSeconds(8f);

        // 카메라 원상 복구
        playerCamera.SetActive(true);
        cutSceneCamera.SetActive(false);

        // 모든 Canvas 객체를 다시 활성화합니다  
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(true);
        }
    }
    IEnumerator EventSoundCor()
    {
        yield return new WaitForSeconds(2f);
        AudioManager.instance.PlaySound(transform.position, 14, Random.Range(1.2f, 1.2f), 1f);

    }
}
