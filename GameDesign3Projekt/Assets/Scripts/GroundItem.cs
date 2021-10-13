using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour, IInteractable
{
    [SerializeField] private Item item;
    [SerializeField] private bool autoPickable;
    
    public void AddItemToInventory(Inventory inventory)
    {
        inventory.AddItem(item);
        Destroy(gameObject);
    }
}
