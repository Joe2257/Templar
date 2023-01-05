using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct SkillPanel
{
    public Text       skillCD;
    public GameObject cooldownFrame;
}
public class PlayerUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider _healthBar = null;
    public Slider _manaBar   = null;
    public Slider _xpBar     = null;

    [Header("Inventory & UI Panels")] 
    [SerializeField] private GameObject _inventory    = null;
    [SerializeField] private GameObject _hud          = null;
    [SerializeField] private GameObject _pauseMenu    = null;
    [SerializeField] private GameObject _gameOverMenu = null;


    [Header("SkillPanels")]
    public SkillPanel qskillPanel;
    public SkillPanel wskillPanel;
    public SkillPanel eskillPanel;
    public SkillPanel rskillPanel;

    [Header("MissionObjectives")]
    public GameObject scroll;

    public float qSkillCD
    { get; set; }
    public float wSkillCD
    { get; set; }
    public float eSkillCD
    { get; set; }
    public float rSkillCD
    { get; set; }

    private bool qCoolDownExpired = false;
    private bool wCoolDownExpired = false;
    private bool eCoolDownExpired = false;
    private bool rCoolDownExpired = false;

    private bool _inventoryIsDisplayed = false;
    private bool _pauseMenuIsDisplayed = false;
    private bool _scrollIsDisplayed    = false;

    //AttributesToDisplay;
    public float healthPoints
    {get; set;}
    public float speed
    { get; set; }

    public int[] attributes;


    //Components
    public  PlayerInventory   playerInventory;
    public  Player_System   _playerSystem;

    private void Start()
    {
        DisplayRawPlayerAttributes();
    }

    void Update()
    {
        SlidersUpdate();
        ShowHideInventory();
        ShowHidePauseMenu();
        ShowHideMissionScroll();

        DisplayGameOver();

        QskillCooldown();
        WskillCooldown();
        EskillCooldown();
        RskillCooldown();
    }

    private void DisplayGameOver()
    {
        if (_playerSystem.currentHealthPoints <= 0)
        {
            _gameOverMenu.SetActive(true);
        }
    }

    //Update the sliders to match the values of the player health and mana;
    private void SlidersUpdate()
    {
        _healthBar.value = _playerSystem.currentHealthPoints;
        _manaBar.value   = _playerSystem.currentManaPoints;

        _xpBar.maxValue  = _playerSystem.expToNextLevel;
        _xpBar.value     = _playerSystem.expCollected;
    }

    private void ShowHideInventory()
    {
        if (_playerSystem.playerInput.inventoryButtonPressed && !_inventoryIsDisplayed)
        {
            _inventory.SetActive(true);
            _hud.SetActive(false);
            _inventoryIsDisplayed = true;
        }
        else
        if(_playerSystem.playerInput.inventoryButtonPressed && _inventoryIsDisplayed)
        {
            playerInventory.lootPanel.SetActive(false);
            _inventory.SetActive(false);
            _hud.SetActive(true);
            _inventoryIsDisplayed = false;
        }
    }

    public void DisplayInventoryFromSystem()
    {
        if (!_inventoryIsDisplayed)
        {
            _inventory.SetActive(true);
            playerInventory.lootPanel.SetActive(true);
            _hud.SetActive(false);
            _inventoryIsDisplayed = true;
        }
        else
        {
            playerInventory.lootPanel.SetActive(true);
        }
    }

    public void ShowHidePauseMenu()
    {
        if (_playerSystem.playerInput.pauseButtonPressed && !_pauseMenuIsDisplayed)
        {
            Time.timeScale = 0;

            _pauseMenuIsDisplayed = true;

            _hud.SetActive(false);
            _inventory.SetActive(false);
            _pauseMenu.SetActive(true);
        }
        else
        if (_playerSystem.playerInput.pauseButtonPressed && _pauseMenuIsDisplayed)
        {
            OnReturnButton();
        }
    }

    public void ShowHideMissionScroll()
    {
        if (_playerSystem.playerInput.scrollButtonPressed && !_scrollIsDisplayed)
        {
            scroll.SetActive(true);
            _scrollIsDisplayed = true;
        }
        else
        if (_playerSystem.playerInput.scrollButtonPressed && _scrollIsDisplayed)
        {
            scroll.SetActive(false);
            _scrollIsDisplayed = false;
        }
    }

    //_____________DisplayPlayerAttributes_______________\\

    //Initialize the player base attributes when the scene is loaded;
    public void DisplayRawPlayerAttributes()
    {
        attributes[0] = _playerSystem.baseStrenght;
        attributes[1] = _playerSystem.baseDexterity;
        attributes[2] = _playerSystem.baseIntellect;
        attributes[3] = _playerSystem.baseVitality;
        attributes[4] = _playerSystem.baseArmor;
        attributes[5] = _playerSystem.baseDamage;

        playerInventory.inventoryPanel.attributes[0].text = "Strength = "  + attributes[0].ToString();
        playerInventory.inventoryPanel.attributes[1].text = "Dexterity = " + attributes[1].ToString();
        playerInventory.inventoryPanel.attributes[2].text = "Intellect = " + attributes[2].ToString();
        playerInventory.inventoryPanel.attributes[3].text = "Vitality = "  + attributes[3].ToString();
        playerInventory.inventoryPanel.attributes[4].text = "Armor = "     + attributes[4].ToString();
        playerInventory.inventoryPanel.attributes[5].text = "Damage = "    + attributes[5].ToString();
    }

    //Add the Item Attributes to the player base attributes when an Items is equipped and
    // display the correct values in the InventoryPanel;
    public void IncreaseAttributesFromEquipment(ItemSlot _Slot)
    {
        _playerSystem.baseStrenght  += _Slot.item.attributes[0];
        _playerSystem.baseDexterity += _Slot.item.attributes[1];
        _playerSystem.baseIntellect += _Slot.item.attributes[2];
        _playerSystem.baseVitality  += _Slot.item.attributes[3];
        _playerSystem.baseArmor     += _Slot.item.attributes[4];
        _playerSystem.baseDamage    += _Slot.item.attributes[5];

        _playerSystem.UpdateAttributes();
        _playerSystem.UpdateSkillsDamage();

        attributes[0] = _playerSystem.baseStrenght;
        attributes[1] = _playerSystem.baseDexterity;
        attributes[2] = _playerSystem.baseIntellect;
        attributes[3] = _playerSystem.baseVitality;
        attributes[4] = _playerSystem.baseArmor;
        attributes[5] = _playerSystem.baseDamage;

        playerInventory.inventoryPanel.attributes[0].text = "Strength = "  + attributes[0].ToString();
        playerInventory.inventoryPanel.attributes[1].text = "Dexterity = " + attributes[1].ToString();
        playerInventory.inventoryPanel.attributes[2].text = "Intellect = " + attributes[2].ToString();
        playerInventory.inventoryPanel.attributes[3].text = "Vitality = "  + attributes[3].ToString();
        playerInventory.inventoryPanel.attributes[4].text = "Armor = "     + attributes[4].ToString();
        playerInventory.inventoryPanel.attributes[5].text = "Damage = "    + attributes[5].ToString();
    }

    public void IncreaseAttributesFromRawItem(InventoryItem _Slot)
    {

        _playerSystem.baseStrenght  += _Slot.attributes[0];
        _playerSystem.baseDexterity += _Slot.attributes[1];
        _playerSystem.baseIntellect += _Slot.attributes[2];
        _playerSystem.baseVitality  += _Slot.attributes[3];
        _playerSystem.baseArmor     += _Slot.attributes[4];
        _playerSystem.baseDamage    += _Slot.attributes[5];

        _playerSystem.UpdateAttributes();
        _playerSystem.UpdateSkillsDamage();

        attributes[0] = _playerSystem.baseStrenght;
        attributes[1] = _playerSystem.baseDexterity;
        attributes[2] = _playerSystem.baseIntellect;
        attributes[3] = _playerSystem.baseVitality;
        attributes[4] = _playerSystem.baseArmor;
        attributes[5] = _playerSystem.baseDamage;

        playerInventory.inventoryPanel.attributes[0].text = "Strength = "  + attributes[0].ToString();
        playerInventory.inventoryPanel.attributes[1].text = "Dexterity = " + attributes[1].ToString();
        playerInventory.inventoryPanel.attributes[2].text = "Intellect = " + attributes[2].ToString();
        playerInventory.inventoryPanel.attributes[3].text = "Vitality = "  + attributes[3].ToString();
        playerInventory.inventoryPanel.attributes[4].text = "Armor = "     + attributes[4].ToString();
        playerInventory.inventoryPanel.attributes[5].text = "Damage = "    + attributes[5].ToString();
    }

    //Subtract the Item Attributes to the player attributes when an Items is unequipped and
    // display the correct values in the InventoryPanel;
    public void SubtractEquipmentAttributes(ItemSlot _Slot)
    {
         _playerSystem.baseStrenght  -= _Slot.item.attributes[0];
         _playerSystem.baseDexterity -= _Slot.item.attributes[1];
         _playerSystem.baseIntellect -= _Slot.item.attributes[2];
         _playerSystem.baseVitality  -= _Slot.item.attributes[3];
         _playerSystem.baseArmor     -= _Slot.item.attributes[4];
         _playerSystem.baseDamage    -= _Slot.item.attributes[5];
         
         attributes[0] = _playerSystem.baseStrenght;
         attributes[1] = _playerSystem.baseDexterity;
         attributes[2] = _playerSystem.baseIntellect;
         attributes[3] = _playerSystem.baseVitality;
         attributes[4] = _playerSystem.baseArmor;
         attributes[5] = _playerSystem.baseDamage;
         
         _playerSystem.UpdateAttributes();
         _playerSystem.UpdateSkillsDamage();

         playerInventory.inventoryPanel.attributes[0].text = "Strength = "  + attributes[0].ToString();
         playerInventory.inventoryPanel.attributes[1].text = "Dexterity = " + attributes[1].ToString();
         playerInventory.inventoryPanel.attributes[2].text = "Intellect = " + attributes[2].ToString();
         playerInventory.inventoryPanel.attributes[3].text = "Vitality = "  + attributes[3].ToString();
         playerInventory.inventoryPanel.attributes[4].text = "Armor = "     + attributes[4].ToString();
         playerInventory.inventoryPanel.attributes[5].text = "Damage = " + attributes[5].ToString();
    }

    public void SwapItemAttributes(ItemSlot _CurrentSlot, ItemSlot _NewSlot)
    {
        _playerSystem.baseStrenght  -= _CurrentSlot.item.attributes[0];
        _playerSystem.baseDexterity -= _CurrentSlot.item.attributes[1];
        _playerSystem.baseIntellect -= _CurrentSlot.item.attributes[2];
        _playerSystem.baseVitality  -= _CurrentSlot.item.attributes[3];
        _playerSystem.baseArmor     -= _CurrentSlot.item.attributes[4];
        _playerSystem.baseDamage    -= _CurrentSlot.item.attributes[5];
       
        _playerSystem.baseStrenght  += _NewSlot.item.attributes[0];
        _playerSystem.baseDexterity += _NewSlot.item.attributes[1];
        _playerSystem.baseIntellect += _NewSlot.item.attributes[2];
        _playerSystem.baseVitality  += _NewSlot.item.attributes[3];
        _playerSystem.baseArmor     += _NewSlot.item.attributes[4];
        _playerSystem.baseDamage    += _NewSlot.item.attributes[5];
       
        _playerSystem.UpdateAttributes();
        _playerSystem.UpdateSkillsDamage();

        attributes[0] = _playerSystem.baseStrenght;
        attributes[1] = _playerSystem.baseDexterity;
        attributes[2] = _playerSystem.baseIntellect;
        attributes[3] = _playerSystem.baseVitality;
        attributes[4] = _playerSystem.baseArmor;
        attributes[5] = _playerSystem.baseDamage;

        playerInventory.inventoryPanel.attributes[0].text = "Strength = "  + attributes[0].ToString();
        playerInventory.inventoryPanel.attributes[1].text = "Dexterity = " + attributes[1].ToString();
        playerInventory.inventoryPanel.attributes[2].text = "Intellect = " + attributes[2].ToString();
        playerInventory.inventoryPanel.attributes[3].text = "Vitality = "  + attributes[3].ToString();
        playerInventory.inventoryPanel.attributes[4].text = "Armor = "     + attributes[4].ToString();
        playerInventory.inventoryPanel.attributes[5].text = "Damage = " + attributes[5].ToString();
    }

    //_____________SkillsCooldown_______________\\

    private void QskillCooldown()
    {
        if (qSkillCD > 0 && !qCoolDownExpired)
        {
            qSkillCD -= 1 * Time.deltaTime;

            qskillPanel.skillCD.text = qSkillCD.ToString("0");
            qskillPanel.cooldownFrame.SetActive(true);

            if (qSkillCD <= 0)
                qCoolDownExpired = true;
        }
        else 
        if(qCoolDownExpired)
        {
            qskillPanel.skillCD.text = "";
            qskillPanel.cooldownFrame.SetActive(false);

            qCoolDownExpired = false;
        }
    }

    private void WskillCooldown()
    {
        if (wSkillCD > 0 && !wCoolDownExpired)
        {
            wSkillCD -= 1 * Time.deltaTime;

            wskillPanel.skillCD.text = wSkillCD.ToString("0");
            wskillPanel.cooldownFrame.SetActive(true);

            if (wSkillCD <= 0)
                wCoolDownExpired = true;
        }
        else
        if (wCoolDownExpired)
        {
            wskillPanel.skillCD.text = "";
            wskillPanel.cooldownFrame.SetActive(false);

            wCoolDownExpired = false;
        }
    }

    private void EskillCooldown()
    {
        if (eSkillCD > 0 && !eCoolDownExpired)
        {
            eSkillCD -= 1 * Time.deltaTime;

            eskillPanel.skillCD.text = eSkillCD.ToString("0");
            eskillPanel.cooldownFrame.SetActive(true);

            if (eSkillCD <= 0)
                eCoolDownExpired = true;
        }
        else
        if (eCoolDownExpired)
        {
            eskillPanel.skillCD.text = "";
            eskillPanel.cooldownFrame.SetActive(false);

            eCoolDownExpired = false;
        }
    }

    private void RskillCooldown()
    {
        if (rSkillCD > 0 && !rCoolDownExpired)
        {
            rSkillCD -= 1 * Time.deltaTime;

            rskillPanel.skillCD.text = rSkillCD.ToString("0");
            rskillPanel.cooldownFrame.SetActive(true);

            if (rSkillCD <= 0)
                rCoolDownExpired = true;
        }
        else
        if (rCoolDownExpired)
        {
            rskillPanel.skillCD.text = "";
            rskillPanel.cooldownFrame.SetActive(false);

            rCoolDownExpired = false;
        }
    }

    //_____________MenusButtons_______________\\

    public void OnReturnButton()
    {
        Time.timeScale = 1;

        _pauseMenuIsDisplayed = false;

        _hud.SetActive(true);
        _inventory.SetActive(false);
        _pauseMenu.SetActive(false);
    }

    public void OnQuitToMenuButton()
    {
        _playerSystem.gameManager.LoadSceneByString("MainMenu");
    }

    public void OnQuitGameButton()
    {
        Application.Quit();
    }

    public void OnLoadButton()
    {
        if (_playerSystem.gameManager._currentScene.name == "Village")
        {
            _playerSystem.gameManager.LoadSceneByString("Village");
        }
        else
        {
            _playerSystem._sharedVar.loadPlayer = true;

            DataToSave savedData = SaveSystem.LoadPlayer();

            _playerSystem.gameManager.LoadSceneByString(savedData.levelToLoad);
        }

        Time.timeScale = 1;
    }

    //_____________TextSetUps_______________\\

    public void SetMissionObjective(string _Objective)
    {
        Text scrollText = scroll.GetComponentInChildren<Text>();

        scrollText.text = _Objective;
    }

    public void DisplayPlayerLevel(int _PlayerLevel)
    {
        Text levelText = _xpBar.GetComponentInChildren<Text>();

        levelText.text = "Level " + _PlayerLevel;
    }
}
