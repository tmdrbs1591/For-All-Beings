using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoEvent : MonoBehaviourPun
{
    [SerializeField] GameObject eventTextPanel;
    [SerializeField] TextAnim textanim;

    [SerializeField] private List<Transform> meteoSpawnPos = new List<Transform>();
    [SerializeField] private GameObject meteoPrefab; // 메테오 프리팹

    private int maxMeteos = 20; // 최대 스폰 개수
    private int spawnedMeteos = 0; // 현재 스폰된 메테오 개수

    public bool eventClear;

    // Update is called once per frame
    void Update()
    {
    }

    [PunRPC]
    public void EventStart()
    {
        // 메테오 개수 초기화
        spawnedMeteos = 0;

        eventClear = false;
        // 메시지 패널과 메테오 스폰 코루틴 호출
        photonView.RPC("MessagePanel", RpcTarget.All);
        StartCoroutine(SpawnMeteos()); // 코루틴 시작
    }

    [PunRPC]
    void MessagePanel()
    {
        eventTextPanel.SetActive(false);
        eventTextPanel.SetActive(true);
        textanim.textToShow = "메테오가 떨어집니다! 피하세요!";
    }

    IEnumerator SpawnMeteos()
    {
        int excludedIndex = Random.Range(0, meteoSpawnPos.Count); // 제외할 인덱스 결정
        while (spawnedMeteos < maxMeteos)
        {
            // 랜덤 인덱스를 서버에서 결정해서 모든 클라이언트에 전달
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, meteoSpawnPos.Count);
            } while (randomIndex == excludedIndex); // 제외할 인덱스를 제외

            photonView.RPC("SpawnMeteo", RpcTarget.All, randomIndex);

            // 1초 대기
            yield return new WaitForSeconds(1f);
        }

        EventClear();
    }

    [PunRPC]
    void SpawnMeteo(int randomIndex)
    {
        if (meteoSpawnPos.Count == 0)
        {
            Debug.LogWarning("No spawn positions set for meteos!");
            return;
        }

        // 전달된 인덱스에 따라 스폰 위치 결정
        Transform spawnPosition = meteoSpawnPos[randomIndex];

        // 메테오 생성
        Instantiate(meteoPrefab, spawnPosition.position, Quaternion.identity);
        AudioManager.instance.PlaySound(transform.position, 18, UnityEngine.Random.Range(1f, 1.7f), 0.3f);

        // 스폰된 메테오 개수 증가
        spawnedMeteos++;
    }

    void EventClear()
    {
        eventClear = true;
        StageManager.instance.photonView.RPC("EventCheck", RpcTarget.All);
        AudioManager.instance.PlaySound(transform.position,18, Random.Range(1f, 1f), 1f);
        SpawnGold();

    }

    void SpawnGold()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 바구니 위치에 골드 생성
            for (int i = 0; i < 20; i++)
            {
                GameObject gold = PhotonNetwork.Instantiate("Gold", transform.position, Quaternion.identity);
                Gold goldComponent = gold.GetComponent<Gold>();
                goldComponent.isget = false;

                // 모든 "Player" 태그가 붙은 오브젝트를 찾음
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                if (players.Length > 0)
                {
                    // 랜덤으로 타겟을 선택
                    GameObject randomPlayer = players[Random.Range(0, players.Length)];
                    goldComponent.target = randomPlayer.transform;
                }
                else
                {
                    Debug.LogWarning("No players found with the 'Player' tag.");
                }
            }
        }
    }
}
