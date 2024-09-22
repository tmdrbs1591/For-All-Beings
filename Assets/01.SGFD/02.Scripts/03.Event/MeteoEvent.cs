using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoEvent : MonoBehaviourPun
{
    [SerializeField] GameObject eventTextPanel;
    [SerializeField] TextAnim textanim;

    [SerializeField] private List<Transform> meteoSpawnPos = new List<Transform>();
    [SerializeField] private GameObject meteoPrefab; // ���׿� ������

    private int maxMeteos = 20; // �ִ� ���� ����
    private int spawnedMeteos = 0; // ���� ������ ���׿� ����

    public bool eventClear;

    // Update is called once per frame
    void Update()
    {
    }

    [PunRPC]
    public void EventStart()
    {
        // ���׿� ���� �ʱ�ȭ
        spawnedMeteos = 0;

        eventClear = false;
        // �޽��� �гΰ� ���׿� ���� �ڷ�ƾ ȣ��
        photonView.RPC("MessagePanel", RpcTarget.All);
        StartCoroutine(SpawnMeteos()); // �ڷ�ƾ ����
    }

    [PunRPC]
    void MessagePanel()
    {
        eventTextPanel.SetActive(false);
        eventTextPanel.SetActive(true);
        textanim.textToShow = "���׿��� �������ϴ�! ���ϼ���!";
    }

    IEnumerator SpawnMeteos()
    {
        int excludedIndex = Random.Range(0, meteoSpawnPos.Count); // ������ �ε��� ����
        while (spawnedMeteos < maxMeteos)
        {
            // ���� �ε����� �������� �����ؼ� ��� Ŭ���̾�Ʈ�� ����
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, meteoSpawnPos.Count);
            } while (randomIndex == excludedIndex); // ������ �ε����� ����

            photonView.RPC("SpawnMeteo", RpcTarget.All, randomIndex);

            // 1�� ���
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

        // ���޵� �ε����� ���� ���� ��ġ ����
        Transform spawnPosition = meteoSpawnPos[randomIndex];

        // ���׿� ����
        Instantiate(meteoPrefab, spawnPosition.position, Quaternion.identity);
        AudioManager.instance.PlaySound(transform.position, 18, UnityEngine.Random.Range(1f, 1.7f), 0.3f);

        // ������ ���׿� ���� ����
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
            // �ٱ��� ��ġ�� ��� ����
            for (int i = 0; i < 20; i++)
            {
                GameObject gold = PhotonNetwork.Instantiate("Gold", transform.position, Quaternion.identity);
                Gold goldComponent = gold.GetComponent<Gold>();
                goldComponent.isget = false;

                // ��� "Player" �±װ� ���� ������Ʈ�� ã��
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                if (players.Length > 0)
                {
                    // �������� Ÿ���� ����
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
