using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Crystall : MonoBehaviourPunCallbacks
{
    [SerializeField] float currentHP;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentHP <= 0)
        {
            photonView.RPC("Destroy", RpcTarget.All);
        }
    }
    [PunRPC]
    public void Destroy()
    {
        Destroy(gameObject);
    }
    [PunRPC]
    public void TakeDamage()
    {
        currentHP -= 1;
        PhotonNetwork.Instantiate("Gold", transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
    }
}
