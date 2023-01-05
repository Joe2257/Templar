using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct ItemDescription
{
    public GameObject descriptionPanel;
    public Image      descriptionImage;
    public Text       name;
    public Text[]     attributes;
}

[System.Serializable]
public struct InventoryPanel
{
    public Text[] attributes;
    [Space]
    public InventorySlot[] itemsEquippedSlots;
    public InventorySlot[] inventorySlots;
}

[System.Serializable]
public struct InventorySlotStruct
{
    public Sprite        image;
    public GameObject    inventoryItem;
}

public class PlayerInventory : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{

    [Header("Inventory")]
    public ItemDescription _itemDescription;
    public InventoryPanel   inventoryPanel;
    [Space]
    public ItemSlot itemSlot;

    [SerializeField] public List<InventoryItem> _itemsEquipped  = new List<InventoryItem>();
    [SerializeField] public List<InventoryItem> _collectedItems = new List<InventoryItem>();

    //This variable is needed to ensure the sprite will be the correct size after the ItemSlot is instantiated;
    private Vector2 slotSizeCorrection = new Vector2(8, 5);
    private float   offset = 2.5f;

    [Header("Loot")]
    public GameObject      lootPanel;
    public InventorySlot[] lootSlots;

    public   PlayerUI      playerUI;
    public   LootPool      lootPool;

    private Player_System _playerSystem;

    private void Start()
    {
        _playerSystem = playerUI._playerSystem;

       InitializeInventory();

       gameObject.SetActive(false);
    }

    //Initialize the inventory with the items already collected at the start of the scene;
    private void InitializeInventory()
    {
        int itemsInInventory = 0;
        int itemsToEquip    = 0;


        if (_playerSystem._sharedVar.loadPlayer)
        {
            LoadPlayerAndInventory();
        }
        

        foreach (InventorySlot slot in inventoryPanel.inventorySlots)
        {
            if (itemsInInventory < _collectedItems.Count)
            {
                ItemSlot newItemSlot = Instantiate(itemSlot);
                newItemSlot.transform.SetParent(slot.transform, false);
                newItemSlot.rectTransform.anchoredPosition = slot.thisRectTransform.anchoredPosition;
                newItemSlot.rectTransform.sizeDelta = slot.thisRectTransform.sizeDelta + slotSizeCorrection;

                newItemSlot.item = _collectedItems[itemsInInventory];

                if (newItemSlot.item)
                {
                    newItemSlot.SetUpSlot();
                    slot.itemSlot = newItemSlot;

                    newItemSlot.name = itemsInInventory.ToString();
                }
            }

            itemsInInventory++;
        }

        for (int i = 0; i < inventoryPanel.itemsEquippedSlots.Length; i++)
        {
            if (itemsToEquip < _itemsEquipped.Count)
            {
                for (int newI = 0; newI < 5; newI++)
                {
                    if (_itemsEquipped[itemsToEquip].itemType == inventoryPanel.itemsEquippedSlots[newI].itemType)
                    {
                        ItemSlot newItemSlot = Instantiate(itemSlot);
                        newItemSlot.transform.SetParent(inventoryPanel.itemsEquippedSlots[newI].transform, false);
                        newItemSlot.rectTransform.anchoredPosition = inventoryPanel.itemsEquippedSlots[newI].thisRectTransform.anchoredPosition;

                        newItemSlot.rectTransform.offsetMax = new Vector2(offset, -offset);
                        newItemSlot.rectTransform.offsetMin = new Vector2(-offset, offset);

                        newItemSlot.startingPosition = newItemSlot.transform.position;

                        newItemSlot.item = _itemsEquipped[itemsToEquip];

                        if (newItemSlot.item)
                        {
                            newItemSlot.SetUpSlot();
                            inventoryPanel.itemsEquippedSlots[newI].itemSlot = newItemSlot;

                            newItemSlot.name = itemsInInventory.ToString();
                        }
                    }
                }

                itemsToEquip++;
            }
        }
    }

    //_____________ItemDescription_______________\\


    //Display the item description OnClick and 
    //set up all the item attributes and graphics to be displayed in the Description Panel;
    public void OnPointerClick(PointerEventData eventData)
    {
        DisplayItemDescription(eventData.pointerEnter.GetComponent<ItemSlot>());
    }

    //Hide the item description when the cursor is not on the object anymore;
    public void OnPointerExit(PointerEventData eventData)
    {
        HideItemDescription();
    }

    private void DisplayItemDescription(ItemSlot slot)
    {
        if (slot && slot.item)
        {
            _itemDescription.descriptionPanel.SetActive(true);

            _itemDescription.descriptionImage.sprite = slot.image.sprite;
            _itemDescription.name.text = slot.item.name;

            _itemDescription.attributes[0].text = "Strength = " + slot.item.attributes[0].ToString();
            _itemDescription.attributes[1].text = "Dexterity = " + slot.item.attributes[1].ToString();
            _itemDescription.attributes[2].text = "Intellect = " + slot.item.attributes[2].ToString();
            _itemDescription.attributes[3].text = "Vitality = " + slot.item.attributes[3].ToString();
            _itemDescription.attributes[4].text = "Armor = " + slot.item.attributes[4].ToString();
            _itemDescription.attributes[5].text = "Damage = " + slot.item.attributes[5].ToString();
        }
    }

    private void HideItemDescription()
    {
        _itemDescription.descriptionPanel.SetActive(false);
    }

    //_____________LoadInventory_______________\\

    private void LoadPlayerAndInventory()
    {
        DataToSave playerdata = SaveSystem.LoadPlayer();

        _playerSystem.playerLevel         = playerdata.playerlevel;
        _playerSystem.currentHealthPoints = playerdata.healthPoints;

        _playerSystem.StatsIncrease(_playerSystem.playerLevel, true);
         playerUI.DisplayPlayerLevel(_playerSystem.playerLevel);

        _playerSystem.UnlockSkills(_playerSystem.playerLevel);

        int itemToFetch = 0;
        int equipmentToFetch = 0;

        for (int i = 0; i < playerdata.itemId.Length; i++)
        {
            if (playerdata.itemId[i] > 0)
            {
                for (int itemNumber = 0; itemNumber < lootPool.lootableItems.Length; itemNumber++)
                {
                    if (lootPool.lootableItems[itemNumber].itemId == playerdata.itemId[itemToFetch])
                    {
                        InventoryItem itemClone = Instantiate(lootPool.lootableItems[itemNumber]);

                        _collectedItems.Add(itemClone);
                    }
                }

                itemToFetch++;
            }
        }

        for (int i = 0; i < playerdata.equipmentId.Length; i++)
        {
            if (playerdata.equipmentId[i] > 0)
            {
                for (int itemNumber = 0; itemNumber < lootPool.lootableItems.Length; itemNumber++)
                {
                    // Debug.Log("iteration 3 /item number " + itemNumber + " _ " + "Item ID is " + playerdata.itemId[itemToFetch]);

                    if (lootPool.lootableItems[itemNumber].itemId == playerdata.equipmentId[equipmentToFetch])
                    {
                        InventoryItem itemClone = Instantiate(lootPool.lootableItems[itemNumber]);

                        _itemsEquipped.Add(itemClone);
                    }
                }

                equipmentToFetch++;
            }
        }

        //Values for equipped items
        if (_itemsEquipped.Count > 0)
        { _itemsEquipped[0].attributes = playerdata.equipped0; }

        if (_itemsEquipped.Count > 1)
        { _itemsEquipped[1].attributes = playerdata.equipped1; }

        if (_itemsEquipped.Count > 2)
        { _itemsEquipped[2].attributes = playerdata.equipped2; }

        if (_itemsEquipped.Count > 3)
        { _itemsEquipped[3].attributes = playerdata.equipped3; }

        if (_itemsEquipped.Count > 4)
        { _itemsEquipped[4].attributes = playerdata.equipped4; }


        //Values for items in inventory
        if (_collectedItems.Count > 0)
            _collectedItems[0].attributes = playerdata.item0;

        if (_collectedItems.Count > 1)
            _collectedItems[1].attributes = playerdata.item1;

        if (_collectedItems.Count > 2)
            _collectedItems[2].attributes = playerdata.item2;

        if (_collectedItems.Count > 3)
            _collectedItems[3].attributes = playerdata.item3;

        if (_collectedItems.Count > 4)
            _collectedItems[4].attributes = playerdata.item4;

        if (_collectedItems.Count > 5)
            _collectedItems[5].attributes = playerdata.item5;

        if (_collectedItems.Count > 6)
            _collectedItems[6].attributes = playerdata.item6;

        if (_collectedItems.Count > 7)
            _collectedItems[7].attributes = playerdata.item7;

        if (_collectedItems.Count > 8)
            _collectedItems[8].attributes = playerdata.item8;

        if (_collectedItems.Count > 9)
            _collectedItems[9].attributes = playerdata.item9;

        if (_collectedItems.Count > 10)
            _collectedItems[10].attributes = playerdata.item10;

        if (_collectedItems.Count > 11)
            _collectedItems[11].attributes = playerdata.item11;

        if (_collectedItems.Count > 12)
            _collectedItems[12].attributes = playerdata.item12;

        if (_collectedItems.Count > 13)
            _collectedItems[13].attributes = playerdata.item13;

        if (_collectedItems.Count > 14)
            _collectedItems[14].attributes = playerdata.item14;

        if (_collectedItems.Count > 15)
            _collectedItems[15].attributes = playerdata.item15;

        if (_collectedItems.Count > 16)
            _collectedItems[16].attributes = playerdata.item16;

        if (_collectedItems.Count > 17)
            _collectedItems[17].attributes = playerdata.item17;

        for (int i = 0; i < _itemsEquipped.Count; i++)
        {
            playerUI.IncreaseAttributesFromRawItem(_itemsEquipped[i]);
        }

        _playerSystem._sharedVar.loadPlayer = false;
    }
}
