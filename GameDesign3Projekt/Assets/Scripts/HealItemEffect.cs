using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ItemEffects/Heal")]
public class HealItemEffect : UsableItemEffect
{
    public int HealthAmount;
    public override void ExecuteEffect(UsableItem parentItem, InventoryManager character)
    {
        character.Health += HealthAmount;
    }

    public override string GetDescription()
    {
        return "Heals for " + HealthAmount + " health.";
    }
}
