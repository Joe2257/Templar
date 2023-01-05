using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LootPool/Loot")]
public class LootPool : ScriptableObject
{
    public InventoryItem[] lootableItems;
}
