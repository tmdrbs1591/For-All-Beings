using Inventory.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public Button increaseGrenadeButton;
    public Button increaseAppleButton;
    public Button increaseHealthPotionButton;


    public ItemSO GrenadeToAdd;
    public ItemSO AppleToAdd;
    public ItemSO HealthPotionToAdd;


    [SerializeField]
    private InventorySO inventoryData;

    private PlayerGold playerGold;

    private void Awake()
    {
        playerGold = GetComponentInParent<PlayerGold>();
        increaseGrenadeButton.onClick.AddListener(GrenadItemAdd);  // ��ư Ŭ�� �� ������ �߰� �޼��� ȣ��
        increaseAppleButton.onClick.AddListener(AppleItemAdd);  // ��ư Ŭ�� �� ������ �߰� �޼��� ȣ��
        increaseHealthPotionButton.onClick.AddListener(HealthpotionItemAdd);  // ��ư Ŭ�� �� ������ �߰� �޼��� ȣ��
    }

    public void ToggleShop()
    {
        gameObject.SetActive(!gameObject.activeSelf);
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

            // �κ��丮�� ������ �߰�
            int reminder = inventoryData.AddItem(newItem.item, newItem.quantity);

            UseGolde(30);

            TutorialManager.instance.LastEventTutorial();
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

            // �κ��丮�� ������ �߰�
            int reminder = inventoryData.AddItem(newItem.item, newItem.quantity);
            UseGolde(5);

            TutorialManager.instance.LastEventTutorial();

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

            // �κ��丮�� ������ �߰�
            int reminder = inventoryData.AddItem(newItem.item, newItem.quantity);
            UseGolde(10);

            TutorialManager.instance.LastEventTutorial();


        }
    }
    private void UseGolde(int count)
    {
        playerGold.coin -= count;
    }
}
