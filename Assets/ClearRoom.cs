using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ClearRoom : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; // �÷��̾���� ��ġ�� ���� ����Ʈ �迭
    private List<int> availableSpawnIndices; // ��� ������ ���� ����Ʈ �ε��� ����Ʈ
    private HashSet<int> usedSpawnIndices; // �̹� ���� �ε��� ����

    public GameObject originalCamera;
    public GameObject clearCamera;

    private void Start()
    {
        // ��� ������ ���� ����Ʈ �ε��� ����Ʈ �ʱ�ȭ
        availableSpawnIndices = new List<int>();
        usedSpawnIndices = new HashSet<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawnIndices.Add(i);
        }

        // ������ Ŭ���̾�Ʈ�� ClearPlayerTransform�� ȣ��
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

            // ��� ������ ���� ����Ʈ�� ã�� ������ �ݺ�
            do
            {
                spawnIndex = Random.Range(0, spawnPoints.Length);
            } while (usedSpawnIndices.Contains(spawnIndex)); // �̹� ���� �ε����� �ٽ� ����

            // �÷��̾ ���õ� ���� ����Ʈ�� �̵�
            players[i].transform.position = spawnPoints[spawnIndex].position;
            players[i].transform.rotation = Quaternion.Euler(0, 180, 0); // ȸ���� ����

            // ����� ���� ����Ʈ�� ����Ʈ�� �߰�
            currentUsedIndices.Add(spawnIndex);
            usedSpawnIndices.Add(spawnIndex);

            // PlayerStats ������Ʈ�� �ִ��� Ȯ�� �� isFreeze ����
            var playerstat = players[i].GetComponent<PlayerStats>();
            if (playerstat != null)
            {
                playerstat.isFreeze = true; // �÷��̾� �̵� ���߱�
            }
        }

        // ���� �ڵ�� ����
        GameObject[] canvases = GameObject.FindGameObjectsWithTag("Canvas");
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(false);
        }

        clearCamera.SetActive(true);
        originalCamera.SetActive(false);

        // ���� ����Ʈ ���� ����
        photonView.RPC("SyncPlayerPositions", RpcTarget.OthersBuffered, currentUsedIndices);
    }

    [PunRPC]
    private void SyncPlayerPositions(List<int> spawnIndices)
    {
        // ���� �ε����� ������Ʈ
        foreach (int index in spawnIndices)
        {
            usedSpawnIndices.Add(index);
        }

        // �� �÷��̾��� ��ġ ������Ʈ
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            // ��� �÷��̾ ���� ��ġ�� ������Ʈ�ϵ�, ���� �ε����� ���� ����
            if (i < spawnIndices.Count)
            {
                int spawnIndex = spawnIndices[i];

                // �̹� ���� �ε������� Ȯ�� �� ��ġ ����
                if (!usedSpawnIndices.Contains(spawnIndex))
                {
                    players[i].transform.position = spawnPoints[spawnIndex].position;
                    players[i].transform.rotation = Quaternion.Euler(0, 180, 0);
                    usedSpawnIndices.Add(spawnIndex); // �ε��� �߰�
                }
            }
        }
    }

}
