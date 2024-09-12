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
        increaseGrenadeButton.onClick.AddListener(GrenadItemAdd);  // 버튼 클릭 시 아이템 추가 메서드 호출
        increaseAppleButton.onClick.AddListener(AppleItemAdd);  // 버튼 클릭 시 아이템 추가 메서드 호출
        increaseHealthPotionButton.onClick.AddListener(HealthpotionItemAdd);  // 버튼 클릭 시 아이템 추가 메서드 호출
    }

    public void ToggleShop()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void GrenadItemAdd()
    {
        if (GrenadeToAdd != null && playerGold.coin >= 30)
        {
            // ItemSO를 기반으로 InventoryItem을 생성
            InventoryItem newItem = new InventoryItem
            {
                item = GrenadeToAdd,
                quantity = 1,  // 추가할 수량 (기본값: 1)
                itemState = new List<ItemParameter>()  // 기본 아이템 상태 설정 (필요에 따라 수정 가능)
            };

            // 인벤토리에 아이템 추가
            int reminder = inventoryData.AddItem(newItem.item, newItem.quantity);

            UseGolde(30);

            TutorialManager.instance.LastEventTutorial();
        }
    }

    private void AppleItemAdd()
    {
        if (AppleToAdd != null && playerGold.coin >= 5)
        {
            // ItemSO를 기반으로 InventoryItem을 생성
            InventoryItem newItem = new InventoryItem
            {
                item = AppleToAdd,
                quantity = 1,  // 추가할 수량 (기본값: 1)
                itemState = new List<ItemParameter>()  // 기본 아이템 상태 설정 (필요에 따라 수정 가능)
            };

            // 인벤토리에 아이템 추가
            int reminder = inventoryData.AddItem(newItem.item, newItem.quantity);
            UseGolde(5);

            TutorialManager.instance.LastEventTutorial();

        }

    }
    private void HealthpotionItemAdd()
    {
        if (AppleToAdd != null && playerGold.coin >= 10)
        {
            // ItemSO를 기반으로 InventoryItem을 생성
            InventoryItem newItem = new InventoryItem
            {
                item = HealthPotionToAdd,
                quantity = 1,  // 추가할 수량 (기본값: 1)
                itemState = new List<ItemParameter>()  // 기본 아이템 상태 설정 (필요에 따라 수정 가능)
            };

            // 인벤토리에 아이템 추가
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
