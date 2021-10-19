using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ UsableItems")]
public class UsableItem : Item
{
    public bool IsConsumable;
    public List<UsableItemEffect> Effects;
    public virtual void Use(InventoryManager c)
    {
        foreach (UsableItemEffect effect in Effects)
        {
            effect.ExecuteEffect(this, c);
        }
    }
    public override string GetItemType()
    {
        return IsConsumable ? "Consumable" : "Usable";
    }

    public override string GetDescription()
    {
        sb.Length = 0;

        foreach (UsableItemEffect effect in Effects)
        {
            sb.AppendLine(effect.GetDescription());
        }

        return sb.ToString();
    }
}
