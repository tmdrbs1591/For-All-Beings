using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ClearRoom : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; // 플레이어들을 배치할 스폰 포인트 배열
    private List<int> availableSpawnIndices; // 사용 가능한 스폰 포인트 인덱스 리스트
    private HashSet<int> usedSpawnIndices; // 이미 사용된 인덱스 집합

    public GameObject originalCamera;
    public GameObject clearCamera;

    private void Start()
    {
        // 사용 가능한 스폰 포인트 인덱스 리스트 초기화
        availableSpawnIndices = new List<int>();
        usedSpawnIndices = new HashSet<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawnIndices.Add(i);
        }

        // 마스터 클라이언트만 ClearPlayerTransform을 호출
        if (PhotonNetwork.IsMasterClient)
        {
            ClearPlayerTransform();
        }
    }
    public void ClearPlayerTransform()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int playerCount = Mathf.Min(players.Length, spawnPoints.Length - usedSpawnIndices.Count);

        List<int> currentUsedIndices = new List<int>();

        for (int i = 0; i < playerCount; i++)
        {
            int spawnIndex;

            // 사용 가능한 스폰 포인트를 찾을 때까지 반복
            do
            {
                spawnIndex = Random.Range(0, spawnPoints.Length);
            } while (usedSpawnIndices.Contains(spawnIndex)); // 이미 사용된 인덱스면 다시 선택

            // 플레이어를 선택된 스폰 포인트로 이동
            players[i].transform.position = spawnPoints[spawnIndex].position;
            players[i].transform.rotation = Quaternion.Euler(0, 180, 0); // 회전값 맞춤

            // 사용한 스폰 포인트를 리스트에 추가
            currentUsedIndices.Add(spawnIndex);
            usedSpawnIndices.Add(spawnIndex);

            // PlayerStats 컴포넌트가 있는지 확인 후 isFreeze 설정
            var playerstat = players[i].GetComponent<PlayerStats>();
            if (playerstat != null)
            {
                playerstat.isFreeze = true; // 플레이어 이동 멈추기
            }
        }

        // 이후 코드는 동일
        GameObject[] canvases = GameObject.FindGameObjectsWithTag("Canvas");
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(false);
        }

        clearCamera.SetActive(true);
        originalCamera.SetActive(false);

        // 스폰 포인트 정보 전송
        photonView.RPC("SyncPlayerPositions", RpcTarget.OthersBuffered, currentUsedIndices);
    }

    [PunRPC]
    private void SyncPlayerPositions(List<int> spawnIndices)
    {
        // 사용된 인덱스를 업데이트
        foreach (int index in spawnIndices)
        {
            usedSpawnIndices.Add(index);
        }

        // 각 플레이어의 위치 업데이트
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            // 모든 플레이어에 대해 위치를 업데이트하되, 사용된 인덱스에 따라 조정
            if (i < spawnIndices.Count)
            {
                int spawnIndex = spawnIndices[i];

                // 이미 사용된 인덱스인지 확인 후 위치 설정
                if (!usedSpawnIndices.Contains(spawnIndex))
                {
                    players[i].transform.position = spawnPoints[spawnIndex].position;
                    players[i].transform.rotation = Quaternion.Euler(0, 180, 0);
                    usedSpawnIndices.Add(spawnIndex); // 인덱스 추가
                }
            }
        }
    }

}
