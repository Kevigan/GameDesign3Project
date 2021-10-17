using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipementPanel : MonoBehaviour
{
    [SerializeField] Transform equipmentSlotsParent;
    [SerializeField] EquipementSlot[] equipementSlots;

    public event Action<BaseItemSlot> OnRightClickEvent;
    public event Action<BaseItemSlot> OnBeginDragEvent;
    public event Action<BaseItemSlot> OnEndDragEvent;
    public event Action<BaseItemSlot> OnPointerEnterEvent;
    public event Action<BaseItemSlot> OnPointerExitEvent;
    public event Action<BaseItemSlot> OnDropEvent;
    public event Action<BaseItemSlot> OnDragEvent;

    private void Start()
    {
        for (int i = 0; i < equipementSlots.Length; i++)
        {
            equipementSlots[i].OnRightClickEvent += OnRightClickEvent;
            equipementSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            equipementSlots[i].OnEndDragEvent += OnEndDragEvent;
            equipementSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            equipementSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            equipementSlots[i].OnDropEvent += OnDropEvent;
            equipementSlots[i].OnDragEvent += OnDragEvent;
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
