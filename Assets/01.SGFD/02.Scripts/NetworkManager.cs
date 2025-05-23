﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    public string type;

    public CharImage charImage;
    public CharManager charManager;
    public GameObject fadeImage;

    [Header("DisconnectPanel")]
    public TMP_InputField NickNameInput;
    public GameObject DisconnectPanel;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public InputField RoomInput;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text ListText;
    public Text RoomInfoText;
    public TMP_Text[] ChatText;
    public InputField ChatInput;

    [Header("ETC")]
    public TMP_Text StatusText;
    public PhotonView PV;

    [Header("Spawn Positions")]
    public Transform[] spawnPositions;

    private List<RoomInfo> myList = new List<RoomInfo>();
    private int currentPage = 1, maxPage, multiple;

    public GameObject startGameButton;

    [SerializeField] Transform playerLisContent;
    [SerializeField] GameObject playerListItemPrefab;

    [SerializeField] Transform hpBarLisContent;
    [SerializeField] GameObject hpBarListItemPrefab;

    private Dictionary<string, GameObject> playerObjects = new Dictionary<string, GameObject>(); // 플레이어 오브젝트 관리 딕셔너리

    [SerializeField] GameObject timeManager;
    [SerializeField] GameObject massagePanel;
    [SerializeField] SongManager songManager;

    private void Start()
    {
        PhotonNetwork.Disconnect();

        if (type == "Tutorial")
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            PV.RPC("StartGameRPC", RpcTarget.All);
            PV.RPC("StatusTextfalse", RpcTarget.All);
          
        }
        else
        {
            Debug.LogError("Not connected to Photon master client or not in a room.");
        }
    }
    [PunRPC]
    public void StatusTextfalse()
    {
        StatusText.gameObject.SetActive(false);
    }
    public void Spawn()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            Vector3 spawnPosition = spawnPositions[playerIndex % spawnPositions.Length].position;
            GameObject playerObject = PhotonNetwork.Instantiate(CharManager.instance.currentCharacter.ToString(), spawnPosition, Quaternion.identity);

            timeManager.SetActive(true);

            StartCoroutine(MessagePanel());

            playerObjects[PhotonNetwork.LocalPlayer.NickName] = playerObject; // 플레이어 오브젝트 저장
        }
        else
        {
            Debug.LogError("Not connected to Photon server.");
        }
    }

    [PunRPC]
    void StartGameRPC()
    {
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        Spawn();
        Fade();
        yield return new WaitForSeconds(2f);
        DisconnectPanel.SetActive(false);
        RoomPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        yield return new WaitForSeconds(2f);
        fadeImage.SetActive(false);

        // 방을 시작하면서 방 목록에서 방을 제거합니다.
        RemoveRoomFromList();
    }

    void RemoveRoomFromList()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public void Fade()
    {
        StartCoroutine(FadeCor());
    }

    public IEnumerator FadeCor()
    {
        fadeImage.SetActive(true);
        yield return new WaitForSeconds(4f);
        fadeImage.SetActive(false);
    }

    #region RoomList Management

    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count);
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        myList.Clear(); // 모든 방 목록을 초기화합니다.

        foreach (RoomInfo room in roomList)
        {
            // 방이 목록에서 제거된 경우, 리스트에서 제거합니다.
            if (!room.RemovedFromList)
            {
                if (!myList.Contains(room))
                {
                    myList.Add(room);
                }
                else
                {
                    myList[myList.IndexOf(room)] = room;
                }
            }
        }

        // 페이지를 새로고침합니다.
        MyListRenewal();
    }

    #endregion

    #region Photon Callbacks

    void Awake()
    {

        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
        instance = this;


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Send();
        }
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + " 로비 / " + PhotonNetwork.CountOfPlayers + " 접속";
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
    }

    public void CreateRoom()
    {
        string roomName = string.IsNullOrEmpty(RoomInput.text) ? "Room" + Random.Range(0, 100) : RoomInput.text;
        RoomOptions roomOptions = new RoomOptions();

        // Tutorial 타입일 경우 방 인원수를 1명으로 설정
        if (type == "Tutorial")
        {
            roomOptions.MaxPlayers = 1;
        }
        else
        {
            roomOptions.MaxPlayers = 4; // 기본 인원수
        }

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }


    public void JoinRandomRoom()
    {
        // Tutorial 타입일 경우, 방을 직접 생성하고 방장이 됩니다.
        if (type == "Tutorial")
        {
            CreateRoom();
        }
        else
        {
            // 일반 경우는 랜덤 방에 참가
            PhotonNetwork.JoinRandomRoom();
        }
    }


    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        RoomPanel.SetActive(true);
        RoomRenewal();

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Count(); i++)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab, playerLisContent);
            playerItem.GetComponent<PlayerListItem>().Setup(players[i]);
            charImage = playerItem.GetComponent<CharImage>();
            playerObjects[players[i].NickName] = playerItem; // 플레이어 오브젝트 저장
        }
        for (int i = 0; i < players.Count(); i++)
        {
            GameObject playerhpItem = Instantiate(hpBarListItemPrefab, hpBarLisContent);
            playerhpItem.GetComponent<HpbarListItem>().Setup(players[i]);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }

        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomInput.text = "";
        CreateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomInput.text = "";
        CreateRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + "[System]", "<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
        GameObject playerItem = Instantiate(playerListItemPrefab, playerLisContent);
        playerItem.GetComponent<PlayerListItem>().Setup(newPlayer);
        playerObjects[newPlayer.NickName] = playerItem; // 플레이어 오브젝트 저장
        GameObject playerHpItem = Instantiate(hpBarListItemPrefab, hpBarLisContent);
        playerHpItem.GetComponent<HpbarListItem>().Setup(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + "[System]", "<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");

        if (playerObjects.ContainsKey(otherPlayer.NickName))
        {
            Destroy(playerObjects[otherPlayer.NickName]);
            playerObjects.Remove(otherPlayer.NickName);
        }
    }

    void RoomRenewal()
    {
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "명";
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }

    IEnumerator MessagePanel()
    {
        yield return new WaitForSeconds(2);
        massagePanel.SetActive(true);
        songManager.InGameSongPlay();
        yield return new WaitForSeconds(3.5f);
        massagePanel.SetActive(false);
    }

    #endregion

    #region Chat

    public void Send()
    {
        if (!string.IsNullOrEmpty(ChatInput.text))
        {
            string message = ChatInput.text;
            PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName, message);
            ChatInput.text = "";
        }

        ChatInput.ActivateInputField();
        ChatInput.Select();
    }

    [PunRPC]
    void ChatRPC(string playerName, string msg)
    {
        for (int i = 0; i < ChatText.Length - 1; i++)
        {
            ChatText[i].text = ChatText[i + 1].text;
        }

        ChatText[ChatText.Length - 1].text = playerName + " : " + msg;

        // 채팅을 보낸 플레이어의 오브젝트에서 PlayerChat 컴포넌트의 이미지 활성화
        if (playerObjects.ContainsKey(playerName))
        {
            PhotonView playerPhotonView = playerObjects[playerName].GetComponent<PhotonView>();
            if (playerPhotonView != null)
            {
                playerPhotonView.RPC("ActivateChatImage", RpcTarget.All, msg); // 이미지 활성화 및 채팅 메시지 설정
            }
        }
    }

    #endregion
}
