using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class PlayerGold : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] public int coin;
    [SerializeField] PlayerStats playerStats;
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        coinText.text = coin.ToString();
    }
    private void OnCollisionEnter(Collision collision)
    {
        var GoldCom = collision.gameObject.GetComponent<Gold>();
        if (collision.gameObject.CompareTag("Gold") && GoldCom.isget)
        {
            playerStats.currentUltimategauge++;
            AudioManager.instance.PlaySound(transform.position, 6, Random.Range(1f, 0.9f), 0.4f);
            var ps = GetComponent<PlayerStats>();
            ps.currentXp += 10;
            coin++;
            ps.LV_UP();
            Destroy(collision.gameObject);
        }
    }
}
