using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class Agentequipment : MonoBehaviour
{
    [SerializeField]
    private EquippableItemSO equipment;

    [SerializeField]
    private InventorySO inventoryData;

    [SerializeField]
    private List<ItemParameter> parametersTomdify, itemCurrentState;

    public void SetEquipment(EquippableItemSO equipmentItemSO,List<ItemParameter> itemstate)
    {
        if (equipment != null)
        {
            inventoryData.AddItem(equipment, 1, itemCurrentState);
        }
        this.equipment = equipmentItemSO;
        this.itemCurrentState = new List<ItemParameter>(itemstate);
        ModifyParameters();
    }

    private void ModifyParameters()
    {
        foreach (var parameter in parametersTomdify)
        {
            if (itemCurrentState.Contains(parameter))
            {
                int index = itemCurrentState.IndexOf(parameter);
                float newValue = itemCurrentState[index].value + parameter.value;
                itemCurrentState[index] = new ItemParameter
                {
                    itemParameter = parameter.itemParameter,
                    value = newValue
                };
            }
        }
    }


}
