using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            var playerstats = other.GetComponent<PlayerStats>();
            playerstats.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, 10);

            Debug.Log("으악!!!!!!!!!!!!!!!!!! 메테오다 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }
}
