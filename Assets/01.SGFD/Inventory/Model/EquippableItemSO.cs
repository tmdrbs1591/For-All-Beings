using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        public string ActionName => "Equip";

        public AudioClip actionSFX { get; private set; }

        public bool PerformAction(GameObject character,List<ItemParameter> itemState = null)
        {
            Agentequipment equipmentSystem = character.GetComponent<Agentequipment>();
            if (equipmentSystem != null) {
                equipmentSystem.SetEquipment(this, itemState == null ? DefaultParmetersList : itemState);
                return true;
            }
            return false;
        }
    }
    
}

