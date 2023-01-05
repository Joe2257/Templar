using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : DragDrop
{
    public InventoryItem item;

    public void SetUpSlot()
    {
        image.sprite = item.image;
    }
}
