using Inventory.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public Button increaseGrenadeButton;
    public Button increaseAppleButton;
    public Button increaseHealthPotionButton;
    public Button attackPowerUpButton;
    public Button attacSpeedUpButton;
    public Button moveSpeedUPButton;


    public ItemSO GrenadeToAdd;
    public ItemSO AppleToAdd;
    public ItemSO HealthPotionToAdd;

    [SerializeField] GameObject dontmoneyPanel;

    [SerializeField] PlayerStats playerstat;

    [SerializeField]
    private InventorySO inventoryData;

    private PlayerGold playerGold;

    private void Awake()
    {
        playerGold = GetComponentInParent<PlayerGold>();
        increaseGrenadeButton.onClick.AddListener(GrenadItemAdd);  // ��ư Ŭ�� �� ������ �߰� �޼��� ȣ��
        increaseAppleButton.onClick.AddListener(AppleItemAdd);  // ��ư Ŭ�� �� ������ �߰� �޼��� ȣ��
        increaseHealthPotionButton.onClick.AddListener(HealthpotionItemAdd);  // ��ư Ŭ�� �� ������ �߰� �޼��� ȣ��

        attackPowerUpButton.onClick.AddListener(AttackPowerUpAdd);  // ��ư Ŭ�� �� ������ �߰� �޼��� ȣ��
        attacSpeedUpButton.onClick.AddListener(AttackSpeedUpAdd);  // ��ư Ŭ�� �� ������ �߰� �޼��� ȣ��
        moveSpeedUPButton.onClick.AddListener(MoveSpeedUpAdd);  // ��ư Ŭ�� �� ������ �߰� �޼��� ȣ��
    }

    public void ToggleShop()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void AttackPowerUpAdd()
    {
        if (GrenadeToAdd != null && playerGold.coin >= 30)
        {

            playerstat.attackPower += 10;

            SingleAudioManager.instance.PlaySound(transform.position, 16, UnityEngine.Random.Range(1f, 1.5f), 1f);

            UseGolde(30);
            if (TutorialManager.instance != null)
                TutorialManager.instance.LastEventTutorial();
        }
       else if (playerGold.coin < 30)
        {
            dontmoneyPanel.SetActive(false);
            dontmoneyPanel.SetActive(true);

        }
    }

    private void AttackSpeedUpAdd()
    {
        if (GrenadeToAdd != null && playerGold.coin >= 30)
        {

            playerstat.attackCoolTime += 0.05f;

            SingleAudioManager.instance.PlaySound(transform.position, 16, UnityEngine.Random.Range(1f, 1.5f), 1f);

            UseGolde(30);
            if (TutorialManager.instance != null)
                TutorialManager.instance.LastEventTutorial();
        }
        else if (playerGold.coin < 30)
        {
            dontmoneyPanel.SetActive(false);
            dontmoneyPanel.SetActive(true);

        }
    }

    private void MoveSpeedUpAdd()
    {
        if (GrenadeToAdd != null && playerGold.coin >= 30)
        {

            playerstat.speed += 1f;
            SingleAudioManager.instance.PlaySound(transform.position, 16, UnityEngine.Random.Range(1f, 1.5f), 1f);

            UseGolde(30);
            if (TutorialManager.instance != null)
                TutorialManager.instance.LastEventTutorial();
        }
        else if (playerGold.coin < 30)
        {
            dontmoneyPanel.SetActive(false);
            dontmoneyPanel.SetActive(true);

        }
    }
    private void GrenadItemAdd()
    {
        if (GrenadeToAdd != null && playerGold.coin >= 30)
        {
            // ItemSO�� ������� InventoryItem�� ����
            InventoryItem newItem = new InventoryItem
            {
                item = GrenadeToAdd,
                quantity = 1,  // �߰��� ���� (�⺻��: 1)
                itemState = new List<ItemParameter>()  // �⺻ ������ ���� ���� (�ʿ信 ���� ���� ����)
            };
            SingleAudioManager.instance.PlaySound(transform.position, 15, UnityEngine.Random.Range(1f, 1.5f), 1f);

            // �κ��丮�� ������ �߰�
            int reminder = inventoryData.AddItem(newItem.item, newItem.quantity);

            UseGolde(30);
            if (TutorialManager.instance != null)
                TutorialManager.instance.LastEventTutorial();
        }
        else if (playerGold.coin < 30)
        {
            dontmoneyPanel.SetActive(false);
            dontmoneyPanel.SetActive(true);

        }
    }

    private void AppleItemAdd()
    {
        if (AppleToAdd != null && playerGold.coin >= 5)
        {
            // ItemSO�� ������� InventoryItem�� ����
            InventoryItem newItem = new InventoryItem
            {
                item = AppleToAdd,
                quantity = 1,  // �߰��� ���� (�⺻��: 1)
                itemState = new List<ItemParameter>()  // �⺻ ������ ���� ���� (�ʿ信 ���� ���� ����)
            };
            SingleAudioManager.instance.PlaySound(transform.position, 15, UnityEngine.Random.Range(1f, 1.5f), 1f);

            // �κ��丮�� ������ �߰�
            int reminder = inventoryData.AddItem(newItem.item, newItem.quantity);
            UseGolde(5);
            if (TutorialManager.instance != null)
                TutorialManager.instance.LastEventTutorial();

        }
        else if (playerGold.coin < 5)
        {
            dontmoneyPanel.SetActive(false);
            dontmoneyPanel.SetActive(true);

        }

    }
    private void HealthpotionItemAdd()
    {
        if (AppleToAdd != null && playerGold.coin >= 10)
        {
            // ItemSO�� ������� InventoryItem�� ����
            InventoryItem newItem = new InventoryItem
            {
                item = HealthPotionToAdd,
                quantity = 1,  // �߰��� ���� (�⺻��: 1)
                itemState = new List<ItemParameter>()  // �⺻ ������ ���� ���� (�ʿ信 ���� ���� ����)
            };
            SingleAudioManager.instance.PlaySound(transform.position, 15, UnityEngine.Random.Range(1f, 1.5f), 1f);

            // �κ��丮�� ������ �߰�
            int reminder = inventoryData.AddItem(newItem.item, newItem.quantity);
            UseGolde(10);
            if (TutorialManager.instance != null)
                TutorialManager.instance.LastEventTutorial();


        }
        else if (playerGold.coin < 10)
        {
            dontmoneyPanel.SetActive(false);
            dontmoneyPanel.SetActive(true);

        }
    }
    private void UseGolde(int count)
    {
        playerGold.coin -= count;
    }
}
