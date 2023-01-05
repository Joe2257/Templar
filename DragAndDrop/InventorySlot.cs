using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType {Null, Inventory, Equipment, LootTable, ItemRemover }
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public SlotType  slotType;
    public Item_Type itemType;

    public PlayerUI _playerUI;

    private Vector3       _thisSlotPosition;
    private RectTransform _thisRectTransform;
    public ItemSlot      _itemSlot = null;

    public RectTransform thisRectTransform
    { get { return _thisRectTransform; } }

    public Vector3 thisSlotPosition
    { get { return _thisSlotPosition; } set { _thisSlotPosition = value; } }

    public ItemSlot itemSlot
    { get { return _itemSlot; } set { _itemSlot = value; } }

    private void Awake()
    {
        _thisRectTransform = GetComponent<RectTransform>();
        _thisSlotPosition  = _thisRectTransform.anchoredPosition;
    }

    public void OnDrop(PointerEventData pointerEventData)
    {
        if (slotType != SlotType.LootTable)
        {
            ItemSlot slot = pointerEventData.pointerDrag.GetComponent<ItemSlot>();

            if (slotType == SlotType.ItemRemover)
            {
                RemoveItemFromInventory(slot);
            }

            if (!itemSlot && slot.item.itemType == itemType || !itemSlot && itemType == Item_Type.All)
            {
                //If the ItemSlot is dropped in an empty InventorySlot this part of the function is called;
                slot.dropped = true;

                if (slotType == SlotType.Equipment)
                {
                    if (slot.parentSlot.slotType == SlotType.LootTable)
                        _playerUI.playerInventory._itemsEquipped.Add(slot.item);

                    if (slot.parentSlot.slotType == SlotType.Inventory)
                    {
                        _playerUI.playerInventory._itemsEquipped.Add(slot.item);
                        _playerUI.playerInventory._collectedItems.Remove(slot.item);
                    }

                    _playerUI.IncreaseAttributesFromEquipment(slot);
                }
                else
                if (slotType == SlotType.Inventory && slot.parentSlot.slotType == SlotType.Equipment)
                {
                    _playerUI.SubtractEquipmentAttributes(slot);
                }

                if (slotType == SlotType.Inventory && slot.parentSlot.slotType != SlotType.Inventory)
                {

                    if (slot.parentSlot.slotType == SlotType.LootTable)
                        _playerUI.playerInventory._collectedItems.Add(slot.item);

                    if (slot.parentSlot.slotType == SlotType.Equipment)
                    {
                        _playerUI.playerInventory._collectedItems.Add(slot.item);
                        _playerUI.playerInventory._itemsEquipped.Remove(slot.item);
                    }
                }

                slot.transform.SetParent(transform);
                slot.rectTransform.position = _thisRectTransform.position;
                slot.startingPosition = _thisRectTransform.position;

                slot.parentSlot._itemSlot = null;
                slot.parentSlot = this;
                _itemSlot = slot;
            }
            else
            if (itemSlot && slot.item.itemType == itemType || itemSlot && itemType == Item_Type.All)
            {
                //If the ItemSlot is dropped on another ItemSlot, this part of the function
                // determine how to interact with it and how to swap the ItemSlots in order to 
                // be positioned correctly in the inventory or in the EquipmentSlots;

                if (slotType == SlotType.Equipment)
                {
                    if (slot.item.itemType == itemType)
                    {
                        slot.dropped = true;
                        SwapItemsFromInventoryToEquipment(slot);
                    }
                }
                else
                if (slotType == SlotType.Inventory && slot.parentSlot.slotType != SlotType.Equipment)
                {
                    slot.dropped = true;
                    SwapItemsInInventory(slot);
                }
            }
            
            
        }
    }

    //Swap the position of 2 ItemSlots in the inventory;
    private void SwapItemsInInventory(ItemSlot slot)
    {
        if (slot.parentSlot.slotType != SlotType.LootTable)
        {
            itemSlot.transform.SetParent(slot.parentSlot.transform);
            itemSlot.rectTransform.anchoredPosition = slot.parentSlot.thisRectTransform.anchoredPosition;
            itemSlot.startingPosition = itemSlot.rectTransform.position;
            itemSlot.parentSlot = slot.parentSlot;


            slot.transform.SetParent(transform);
            slot.rectTransform.anchoredPosition = thisSlotPosition;
            slot.startingPosition = slot.rectTransform.position;
            slot.parentSlot.itemSlot = itemSlot;

            itemSlot = slot;
            slot.parentSlot = this;
        }
        else
            slot.dropped = false;
    }

    //Swap the position of an ItemSlot in the inventory with one in the EquipmentSlots;
    private void SwapItemsFromInventoryToEquipment(ItemSlot slot)
    {
        if (slot.parentSlot.slotType != SlotType.LootTable)
        {
            _playerUI.SwapItemAttributes(itemSlot, slot);

            Vector3 lastPosition = itemSlot.rectTransform.anchoredPosition;

            itemSlot.transform.SetParent(slot.parentSlot.transform);
            itemSlot.rectTransform.anchoredPosition = slot.parentSlot.thisRectTransform.anchoredPosition;
            itemSlot.startingPosition = itemSlot.rectTransform.position;
            itemSlot.parentSlot = slot.parentSlot;

            _playerUI.playerInventory._collectedItems.Add(itemSlot.item);
            _playerUI.playerInventory._itemsEquipped.Remove(itemSlot.item);
            _playerUI.playerInventory._itemsEquipped.Add(slot.item);
            _playerUI.playerInventory._collectedItems.Remove(slot.item);

            slot.transform.SetParent(transform);
            slot.rectTransform.anchoredPosition = lastPosition;
            slot.startingPosition = slot.rectTransform.position;
            slot.parentSlot.itemSlot = itemSlot;

            itemSlot = slot;
            slot.parentSlot = this;
        }
        else
            slot.dropped = false;
    }

    private void RemoveItemFromInventory(ItemSlot slot)
    {
        slot.parentSlot.itemSlot = null;

        if (slot.parentSlot.slotType == SlotType.Equipment)
            _playerUI.playerInventory._itemsEquipped.Remove(slot.item);
        else 
        if(slot.parentSlot.slotType == SlotType.Inventory)
            _playerUI.playerInventory._collectedItems.Remove(slot.item);

        Destroy(slot.gameObject);
    }
}
