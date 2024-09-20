using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ClearRoom : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; // 플레이어들을 배치할 스폰 포인트 배열

    public GameObject originalCamera;
    public GameObject clearCamera;

    private void Start()
    {
    
    }
    public void CameraSet()
    {
        SingleAudioManager.instance.PlaySound(transform.position, 17, UnityEngine.Random.Range(1f, 1f), 0.3f);

        clearCamera.SetActive(true);
        originalCamera.SetActive(false);
        GameObject[] canvases = GameObject.FindGameObjectsWithTag("Canvas");
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(false);
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            var playerstat = players[i].GetComponent<PlayerStats>();
            playerstat.isFreeze = true;
        }

    }
    public void ClearPlayerTransform()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 spawnPosition = spawnPoints[playerIndex % spawnPoints.Length].position;
        GameObject playerObject = PhotonNetwork.Instantiate(CharManager.instance.currentCharacter.ToString() + "DM", spawnPosition, Quaternion.identity);
        playerObject.transform.rotation = Quaternion.Euler(0, 180, 0);

        SongManager.instance.ClearSongPlay();
        // 이후 코드는 동일

    }



}
