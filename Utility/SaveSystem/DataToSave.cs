using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataToSave
{
    //PlayerGenerics
    public int     playerlevel;
    public float   healthPoints;
    public float   manaPoints;
    public bool    despawnEnemies;

    //Level
    public string levelToLoad;

    //EquippedItems
    public int[] equippedItemId;

    public int[] equipped0;
    public int[] equipped1;
    public int[] equipped2;
    public int[] equipped3;
    public int[] equipped4;

    //PlayerInventory
    public int[]   itemId;
    public int[]   equipmentId;

    public int[] item0;
    public int[] item1;
    public int[] item2;
    public int[] item3;
    public int[] item4;
    public int[] item5;
    public int[] item6;
    public int[] item7;
    public int[] item8;
    public int[] item9;
    public int[] item10;
    public int[] item11;
    public int[] item12;
    public int[] item13;
    public int[] item14;
    public int[] item15;
    public int[] item16;
    public int[] item17;

    public DataToSave (Player_System playerSystem)
    {
        itemId         = new int[18];
        equipmentId    = new int[5];

        equipped0 = new int[6];
        equipped1 = new int[6];
        equipped2 = new int[6];
        equipped3 = new int[6];
        equipped4 = new int[6];

        item0  = new int[6];
        item1  = new int[6];
        item2  = new int[6];
        item3  = new int[6];
        item4  = new int[6];
        item5  = new int[6];
        item6  = new int[6];
        item7  = new int[6];
        item8  = new int[6];
        item9  = new int[6];
        item10 = new int[6];
        item11 = new int[6];
        item12 = new int[6];
        item13 = new int[6];
        item14 = new int[6];
        item15 = new int[6];
        item16 = new int[6];
        item17 = new int[6];

        playerlevel         = playerSystem.playerLevel;
        healthPoints        = playerSystem.currentHealthPoints;
        manaPoints          = playerSystem.currentManaPoints;

        levelToLoad    = playerSystem.gameManager.nextScene;
        
        for (int i = 0; i < playerSystem._playerInventory._collectedItems.Count; i++)
        {
            itemId[i] = playerSystem._playerInventory._collectedItems[i].itemId;
        }

        for (int i = 0; i < playerSystem._playerInventory._itemsEquipped.Count; i++)
        {
            equipmentId[i] = playerSystem._playerInventory._itemsEquipped[i].itemId;
        }

        //EquippedItems
        if (playerSystem._playerInventory._itemsEquipped.Count > 0)
            equipped0 = playerSystem._playerInventory._itemsEquipped[0].attributes;

        if (playerSystem._playerInventory._itemsEquipped.Count > 1)
            equipped1 = playerSystem._playerInventory._itemsEquipped[1].attributes;

        if (playerSystem._playerInventory._itemsEquipped.Count > 2)
            equipped2 = playerSystem._playerInventory._itemsEquipped[2].attributes;

        if (playerSystem._playerInventory._itemsEquipped.Count > 3)
            equipped3 = playerSystem._playerInventory._itemsEquipped[3].attributes;

        if (playerSystem._playerInventory._itemsEquipped.Count > 4)
            equipped4 = playerSystem._playerInventory._itemsEquipped[4].attributes;


        //InventoryItems
        if (playerSystem._playerInventory._collectedItems.Count > 0)
            item0 = playerSystem._playerInventory._collectedItems[0].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 1)
            item1 = playerSystem._playerInventory._collectedItems[1].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 2)
            item2 = playerSystem._playerInventory._collectedItems[2].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 3)
            item3 = playerSystem._playerInventory._collectedItems[3].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 4)
            item4 = playerSystem._playerInventory._collectedItems[4].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 5)
            item5 = playerSystem._playerInventory._collectedItems[5].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 6)
            item6 = playerSystem._playerInventory._collectedItems[6].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 7)
            item7 = playerSystem._playerInventory._collectedItems[7].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 8)
            item8 = playerSystem._playerInventory._collectedItems[8].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 9)
            item9 = playerSystem._playerInventory._collectedItems[9].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 10)
            item10 = playerSystem._playerInventory._collectedItems[10].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 11)
            item11 = playerSystem._playerInventory._collectedItems[11].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 12)
            item12 = playerSystem._playerInventory._collectedItems[12].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 13)
            item13 = playerSystem._playerInventory._collectedItems[13].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 14)
            item14 = playerSystem._playerInventory._collectedItems[14].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 15)
            item15 = playerSystem._playerInventory._collectedItems[15].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 16)
            item16 = playerSystem._playerInventory._collectedItems[16].attributes;

        if (playerSystem._playerInventory._collectedItems.Count > 17)
            item17 = playerSystem._playerInventory._collectedItems[17].attributes;
    }
}
