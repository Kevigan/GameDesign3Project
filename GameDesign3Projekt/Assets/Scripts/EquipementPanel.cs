using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipementPanel : MonoBehaviour
{
    [SerializeField] Transform equipmentSlotsParent;
    [SerializeField] EquipementSlot[] equipementSlots;

    public event Action<Item> OnItemRightClickedEvent;

    private void Start()
    {
        for (int i = 0; i < equipementSlots.Length; i++)
        {
            equipementSlots[i].OnRightClickEvent += OnItemRightClickedEvent;
        }
    }

    private void OnValidate()
    {
        equipementSlots = equipmentSlotsParent.GetComponentsInChildren<EquipementSlot>();
    }

    public bool AddItem(EquipableItem item, out EquipableItem previosItem)
    {
        for (int i = 0; i < equipementSlots.Length; i++)
        {
            if (equipementSlots[i].EquipmentType == item.EquipmentType)
            {
                previosItem = (EquipableItem)equipementSlots[i].Item;
                equipementSlots[i].Item = item;
                return true;
            }
        }
        previosItem = null;
        return false;
    }

    public bool RemoveItem(EquipableItem item)
    {
        for (int i = 0; i < equipementSlots.Length; i++)
        {
            if (equipementSlots[i].Item == item)
            {
                equipementSlots[i].Item = null;
                return true;
            }
        }
        return false;
    }
}
