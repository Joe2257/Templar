using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootChest : MonoBehaviour
{
    public GameObject text;

    public bool isBossChest;
    public bool isFixedChest;
    public GameObject boss;
    public int fixedChestRarity;
    
    public int[] lootTable = {600, 300, 100};
    public int itemsToGenerate;

    public LootPool        lootPool;
    public InventoryItem[] generatedItems;
    private bool           _displayText = true;
    
    private int  _rarityRandomization = 0;

    private Camera _camera;

    public bool canBeOpened
    { get; set; }
    public int lootChestRarity
    { get; set; }

    [Header("Loot")]
    public InventorySlot[] lootSlots;
    public ItemSlot        itemSlot;

    private float offset = 2.5f;

    private AudioSource _audioSource;
    private Animator _animator;
    
    private void Start()
    {
        _animator    = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _camera = Camera.main;

        DetermineLootChestRarity();
        canBeOpened = true;
    }

    private void Update()
    {
        TextLookAtCamera();
    }

    private void TextLookAtCamera()
    {
        if(_displayText)
        text.transform.LookAt(2 * text.transform.position - _camera.transform.position);
    }

    private void DetermineLootChestRarity()
    {
        if (!isFixedChest)
        {
            _rarityRandomization = Random.Range(1, 1000);

            if (_rarityRandomization <= lootTable[0])
                lootChestRarity = 1;
            else
            {
                _rarityRandomization -= lootTable[0];

                if (_rarityRandomization <= lootTable[1])
                    lootChestRarity = 2;
                else
                {
                    _rarityRandomization -= lootTable[1];
                }

                if (_rarityRandomization <= lootTable[2])
                {
                    if (isBossChest)
                        lootChestRarity = 3;
                    else
                        lootChestRarity = 2;
                }
            }
        }
        else
            lootChestRarity = fixedChestRarity;

        GenerateItems(lootChestRarity);
    }

    public void OpenChest(PlayerInventory PlayerInventory, PlayerUI PlayerUI, InventorySlot[] LootSlots)
    {
        lootSlots = LootSlots;

        if (isBossChest)
        {
            if (boss.GetComponent<AI_Skeleton>())
            {
                AI_Skeleton skeleton = boss.GetComponent<AI_Skeleton>();

                if (skeleton.skeletonState == SkeletonState.Dead)
                {
                    canBeOpened = false;

                    _animator.SetBool("Open", true);
                    PlayerUI.DisplayInventoryFromSystem();
                    GenerateLoot(lootChestRarity, this);

                    StartCoroutine(DeactivateChest());
                }
            }
            else
            if (boss.GetComponent<AI_Witch>())
            {
                AI_Witch witch = boss.GetComponent<AI_Witch>();

                if (witch.witchState == WitchState.Dead)
                {
                    canBeOpened = false;

                    _animator.SetBool("Open", true);
                    PlayerUI.DisplayInventoryFromSystem();
                    GenerateLoot(lootChestRarity, this);

                    StartCoroutine(DeactivateChest());
                }
            }
        }
        else
        if (isFixedChest)
        {
            canBeOpened = false;

            _animator.SetBool("Open", true);
            PlayerUI.DisplayInventoryFromSystem();
            GenerateLoot(fixedChestRarity, this);
        }
        else
        {

            canBeOpened = false;

            _animator.SetBool("Open", true);
            PlayerUI.DisplayInventoryFromSystem();
            GenerateLoot(lootChestRarity, this);

            StartCoroutine(DeactivateChest());
        }

        _displayText = false;
        text.SetActive(false);
        _audioSource.Play();

    }

    public void GenerateLoot(int ChestRarity, LootChest Chest)
    {
        for (int i = 0; i < Chest.itemsToGenerate; i++)
        {
            if (lootSlots[i].itemSlot)
            {
                Destroy(lootSlots[i].itemSlot.gameObject);
                lootSlots[i].itemSlot = null;
            }

            ItemSlot newItemSlot = Instantiate(itemSlot);
            newItemSlot.transform.SetParent(lootSlots[i].transform, false);
            newItemSlot.rectTransform.anchoredPosition = lootSlots[i].thisRectTransform.anchoredPosition;

            newItemSlot.rectTransform.offsetMax = new Vector2(offset, -offset);
            newItemSlot.rectTransform.offsetMin = new Vector2(-offset, offset);

            newItemSlot.startingPosition = newItemSlot.transform.position;

            newItemSlot.item = Chest.generatedItems[i];
            lootSlots[i].itemSlot = newItemSlot;

            if (ChestRarity == 1)
            {
                for (int newI = 0; newI < newItemSlot.item.attributes.Length; newI++)
                {
                   newItemSlot.item.attributes[newI] = Random.Range(0, 9);
                }
            }
            else
            if (ChestRarity == 2)
            {
                for (int newI = 0; newI < newItemSlot.item.attributes.Length; newI++)
                {
                    newItemSlot.item.attributes[newI] = Random.Range(10, 15);
                }
            }
            else
            {
                for (int newI = 0; newI < newItemSlot.item.attributes.Length; newI++)
                {
                    newItemSlot.item.attributes[newI] = Random.Range(15, 25);
                }
            }

            newItemSlot.SetUpSlot();
        }
    }

    public void GenerateItems(int ChestRarity)
    {
        if (ChestRarity == 1) itemsToGenerate = 3;
        else
        if (ChestRarity == 2) itemsToGenerate = 6;
        else
            itemsToGenerate = 9;

        for(int i = 0; i < itemsToGenerate; i++)
        {
            generatedItems[i] = Instantiate(lootPool.lootableItems[Random.Range(0, lootPool.lootableItems.Length)]);
        }
    }

    private IEnumerator DeactivateChest()
    {
        yield return new WaitForSeconds(180f);

        gameObject.SetActive(false);
    }
}
