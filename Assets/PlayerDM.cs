using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerDM : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text nickNameText;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            nickNameText.text = PhotonNetwork.NickName;
            nickNameText.color = Color.green;

        }
        else
        {
            nickNameText.text = photonView.Owner.NickName;
            nickNameText.color = Color.black;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
