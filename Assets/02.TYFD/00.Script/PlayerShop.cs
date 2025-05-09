using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShop : MonoBehaviourPunCallbacks
{
    private ShopUI shopUI; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public bool isShop;    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private PlayerCtrl playerCtrl;
    private ArcherCtrl archerCtrl;
    private DragoonCtrl dragoonCtrl;
    [SerializeField] public GameObject KeyUI;

    private void Awake()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
        archerCtrl = GetComponent<ArcherCtrl>();
        dragoonCtrl = GetComponent<DragoonCtrl>();
        shopUI = GetComponentInChildren<ShopUI>(true);      // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isShop) // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            ToggleShop();

        }
    }

    public void OnIncreaseStatButton(string statType, float amount)
    {
        switch (statType)
        {
            case "attackPower":
                if (playerCtrl != null)
                {
                    playerCtrl.playerStats.attackPower += amount;
                }
                else
                {
                    Debug.LogWarning("playerCtrl is null.");
                }

                if (archerCtrl != null)
                {
                    archerCtrl.playerStats.attackPower += amount;
                }
                else
                {
                    Debug.LogWarning("archerCtrl is null.");
                }
                if (dragoonCtrl != null)
                {
                    dragoonCtrl.playerStats.attackPower += amount;
                }
                else
                {
                    Debug.LogWarning("dragoonCtrl is null");
                }
                break;


            default:
                Debug.LogError("Unknown stat type: " + statType);
                break;
        }
    }

    public void ToggleShop() // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    {
        if (shopUI != null)
        {
            shopUI.ToggleShop();
            if (playerCtrl != null)
                playerCtrl.isShop = !playerCtrl.isShop;
            if (archerCtrl != null)
                archerCtrl.isShop = !archerCtrl.isShop;
            if (dragoonCtrl != null)
                dragoonCtrl.isShop = !dragoonCtrl.isShop;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shop"))
        {
            isShop = true;
            if (photonView.IsMine)
                KeyUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Shop"))
        {
            if (photonView.IsMine)
                KeyUI.SetActive(false);
            isShop = false;
            if (playerCtrl != null)
            {
                if (playerCtrl.isShop)
                {
                    ToggleShop();
                    playerCtrl.isShop = false;
                }
            }
            if (archerCtrl != null)
            {
                if (archerCtrl.isShop)
                {
                    ToggleShop();
                    archerCtrl.isShop = false;
                }
            }
            if (dragoonCtrl != null)
            {
                if (dragoonCtrl.isShop)
                {
                    ToggleShop();
                    dragoonCtrl.isShop = false;
                }
            }
        }
    }
}
