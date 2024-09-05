using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Browncklus : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject bulletPrefabs;
    [SerializeField] GameObject dangerLine;



 
    void LongRangeAttack()
    {
        photonView.RPC("LongRangeAttackPun", RpcTarget.AllBuffered); 

    }
    [PunRPC]
    void LongRangeAttackPun()
    {
        StartCoroutine(AttackCor());
    }
    IEnumerator AttackCor()
    {
        dangerLine.SetActive(true);

        // 2초 대기
        yield return new WaitForSeconds(1f);

        dangerLine.SetActive(false);

        // 총알 생성
        GameObject bullet = Instantiate(bulletPrefabs, transform.position + new Vector3(0,1.5f,0), Quaternion.identity);
        bullet.transform.rotation = Quaternion.LookRotation(transform.forward);
      //  SingleAudioManager.instance.PlaySound(transform.position, 11, UnityEngine.Random.Range(1f, 1.7f);
    }
}
