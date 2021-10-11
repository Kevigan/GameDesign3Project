using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public CharacterStat Strength;
    public CharacterStat Agility;
    public CharacterStat Intelligence;
    public CharacterStat Vitality;

    [SerializeField] private Inventory inventory;
    [SerializeField] private EquipementPanel equipementPanel;
    [SerializeField] StatPanel statPanel;

    private void Awake()
    {
        statPanel.SetStats(Strength, Agility, Intelligence, Vitality);
        statPanel.UpdateStatValues();

        inventory.OnItemRightClickedEvent += EquipFromInventory;
        equipementPanel.OnItemRightClickedEvent += UnequipFromEquipmentPanel;
    }

    private void EquipFromInventory(Item item)
    {
        if(item is EquipableItem)
        {
            Equip((EquipableItem)item);
        }
    }
    private void UnequipFromEquipmentPanel(Item item)
    {
        if (item is EquipableItem)
        {
            Unequip((EquipableItem)item);
        }
    }

    public void Equip(EquipableItem item)
    {
        if (inventory.RemoveItem(item))                         //remove from inventory
        {
            EquipableItem previousItem;
            if(equipementPanel.AddItem(item, out previousItem)) //add to equipmentPanel
            {
                if(previousItem != null)                        //if something was allready in that slot, return it to inventory
                {
                    inventory.AddItem(previousItem);
                    previousItem.Unequip(this);
                    statPanel.UpdateStatValues();
                }
                item.Equip(this);
                statPanel.UpdateStatValues();
            }
            else
            {
                inventory.AddItem(item);                        //if you cant equip the item, also return it to inventory
            }
        }
    }

    public void Unequip(EquipableItem item)
    {
        if(!inventory.IsFull() && equipementPanel.RemoveItem(item)) //check if inventory isnt full, then remove item
        {
            item.Unequip(this);
            statPanel.UpdateStatValues();
            inventory.AddItem(item);                                // add to inventory
        }
    }
}
