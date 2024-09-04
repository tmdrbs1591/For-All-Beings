using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ArcherUltimateArrow : MonoBehaviourPunCallbacks
{

    public PhotonView PV;

    public float _damage = 15f; // 데미지 값

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        StartCoroutine(DestroyCor());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(DamageCor(other));
            // DestroyArrowDelayed() 메서드는 필요하지 않은 경우 주석 처리
        }
    }

    IEnumerator DamageCor(Collider _other)
    {
        var enemyPhotonView = _other.gameObject.GetComponent<PhotonView>();
        if (enemyPhotonView != null && PV.IsMine)
        {
            for (int i = 0; i < 5; i++)
            {
                enemyPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, _damage + 50);

                // 데미지 텍스트 생성 RPC 호출
                PV.RPC("SpawnDamageText", RpcTarget.AllBuffered, _other.transform.position, _damage + 50);

                Debug.Log("Hit the enemy!");

                yield return new WaitForSeconds(0.05f);
            }
        }
    }
    [PunRPC]
    void SpawnDamageText(Vector3 position, float damage)
    {
        GameObject damageTextObj = Instantiate(Resources.Load<GameObject>("DamageText"), position + new Vector3(1, 2.5f, 0), Quaternion.identity);
        TMP_Text damageText = damageTextObj.GetComponent<TMP_Text>();
        if (damageText != null)
        {
            damageText.text = damage.ToString();
        }
        // Destroy(damageTextObj, 2f); // 주석 처리: 필요 시 활성화
    }
    IEnumerator DestroyCor()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
