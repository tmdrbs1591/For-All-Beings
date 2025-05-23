using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;  // 슬라이더를 사용하기 위한 네임스페이스

public class HpbarListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;        // 플레이어 이름을 표시할 텍스트
    [SerializeField] Slider hpSlider;      // HP를 표시할 슬라이더
    [SerializeField] TMP_Text levelText;   // 레벨을 표시할 텍스트
    Player player;

    public void Setup(Player _player)
    {
        player = _player;
        text.text = _player.NickName;
        UpdateHPBar(); // 초기 HP 및 레벨 업데이트
    }

    // HP 바와 레벨을 업데이트하는 메서드
    void UpdateHPBar()
    {
        PlayerStats playerStats = GetPlayerStatsByNickName(text.text);
        if (playerStats != null)
        {
            // HP 슬라이더의 값 설정 (비율로 표현)
            hpSlider.value = playerStats.curHp / playerStats.maxHp;

            // 레벨 텍스트 설정
            levelText.text = "LV." + playerStats.playerLevel.ToString();
        }
    }

    // 닉네임으로 PlayerStats를 찾는 메서드
    PlayerStats GetPlayerStatsByNickName(string nickName)
    {
        foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView photonView = playerObject.GetComponent<PhotonView>();
            if (photonView != null && photonView.Owner.NickName == nickName)
            {
                return playerObject.GetComponent<PlayerStats>();
            }
        }
        return null; // 플레이어를 찾지 못한 경우
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        UpdateHPBar();  // 매 프레임 HP와 레벨을 업데이트 (최신 상태 반영)
    }
}
