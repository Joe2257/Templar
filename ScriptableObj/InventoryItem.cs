using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the base class for all the Items in the game;

public enum Item_Class {Null, Armor, Weapon, Amulet }
public enum Item_Type {All, Weapon, Shield, Head, Chest, Amulet }
[CreateAssetMenu(menuName = "InventoryItem/Item/Base")]
public class InventoryItem : ScriptableObject
{
    public Item_Class itemClass;
    public Item_Type itemType;
    public Sprite image;
    public float durability;
    public int itemId;

    public string itemName = "";

    [Header("Attributes")]
    [SerializeField] private int[] _attributes = new int[6];

    public int[] attributes
    { get { return _attributes; } set { _attributes = value; } }

    private void Awake()
    {
        this.name = itemName;
    }
}
