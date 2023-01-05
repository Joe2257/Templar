using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_System : Player_Controller
{
    [Header("PlayerSystem")]
    [SerializeField] private int           _playerLevel   = 0;
    [SerializeField] private DamageTrigger _damageTrigger = null;

    [Header("Audio")]
    [Tooltip("0 = Melee / 1 = Slam / 2 = Lightning / 3 = Execute / 4 = Heal / 5 = Dead")]
    public AudioClip[] clips;

    public int playerLevel
    { get { return _playerLevel; } set { _playerLevel = value; } }


    //Skills
    private bool _genericCDExpired = true;

    private bool _qSkillUnlocked = false;
    private bool _wSkillUnlocked = false;
    private bool _eSkillUnlocked = false;
    private bool _rSkillUnlocked = false;

    public bool qSkill
    { get; set; }
    public bool wSkill
    { get; set; }
    public bool eSkill
    { get; set; }
    public bool rSkill
    { get; set; }

    private float _skillCoolDown = 0;
    private float _manaCost      = 0;

    public float healAmmount
    { get; set; }

    protected override void Start()
    {
        base.Start();

        qSkill = true;
        wSkill = true;
        eSkill = true;
        rSkill = true;

        healAmmount = 10;

        UnlockSkills(_playerLevel);
    }

    protected override void Update()
    {
        base.Update();

        RegenHpAndMp();
        RegisterSkillInput();
        LevelUp(expToNextLevel);
    }

    //_____________LevelingSystem_______________\\

    public void StatsIncrease(int _Level, bool LoopOnStart)
    {
        if (!LoopOnStart)
        {
            baseStrenght  += 1;
            baseDexterity += 1;
            baseIntellect += 1;
            baseVitality  += 1;
            baseArmor     += 1;
            baseDamage    += 1;
        }
        else
        if(LoopOnStart)
        {
            for (int i = 0; i < _Level -1; i++)
            {
                baseStrenght  += 1;
                baseDexterity += 1;
                baseIntellect += 1;
                baseVitality  += 1;
                baseArmor     += 1;
                baseDamage    += 1;
            }
        }
    }

    private void LevelUp(float _ExpNeeded)
    {
        if (expCollected >= _ExpNeeded && _playerLevel < 15)
        {
            expCollected    = 0;
            expToNextLevel += 50f;

            _playerLevel++;
            StatsIncrease(_playerLevel, false);
            UnlockSkills(_playerLevel);
            _playerUI.DisplayPlayerLevel(_playerLevel);
            _playerUI.DisplayRawPlayerAttributes();
        }
    }

    public void GetExpOnKill(float _Exp)
    {
        expCollected += _Exp;
    }

    public void UnlockSkills(int _PlayerLevel)
    {
        if (_PlayerLevel >= 3 && _PlayerLevel < 4)
        {
            _qSkillUnlocked = true;
            _playerUI.qskillPanel.cooldownFrame.SetActive(false);
        }
        else
        if (_PlayerLevel >= 5 && _PlayerLevel < 6)
        {
            _qSkillUnlocked = true;
            _wSkillUnlocked = true;

            _playerUI.qskillPanel.cooldownFrame.SetActive(false);
            _playerUI.wskillPanel.cooldownFrame.SetActive(false);
        }
        else
        if (_PlayerLevel >= 6 && _PlayerLevel < 8)
        {
            _qSkillUnlocked = true;
            _wSkillUnlocked = true;
            _eSkillUnlocked = true;

            _playerUI.qskillPanel.cooldownFrame.SetActive(false);
            _playerUI.wskillPanel.cooldownFrame.SetActive(false);
            _playerUI.eskillPanel.cooldownFrame.SetActive(false);
        }
        else
        if (_PlayerLevel >= 9)
        {
            _qSkillUnlocked = true;
            _wSkillUnlocked = true;
            _eSkillUnlocked = true;
            _rSkillUnlocked = true;

            _playerUI.qskillPanel.cooldownFrame.SetActive(false);
            _playerUI.wskillPanel.cooldownFrame.SetActive(false);
            _playerUI.eskillPanel.cooldownFrame.SetActive(false);
            _playerUI.rskillPanel.cooldownFrame.SetActive(false);
        }
    }

    //_____________Attributes______________\\

    public void UpdateAttributes()
    {
        healthPoints          = 100 + (baseVitality + baseStrenght / 2) / 5;
        runSpeed              = 7   + baseDexterity / 5;
        manaPoints            = 50  + baseIntellect / 5;
        _damageTrigger.damage = (baseDamage + baseStrenght) / 3;

        _playerUI._healthBar.maxValue = healthPoints;
        _playerUI._manaBar.maxValue   = manaPoints;
    }

    public void UpdateSkillsDamage()
    {
       playerAnimatorController.slamDamageTrigger.damage      = 15 + ((baseStrenght / 3) + (baseIntellect / 3));
       playerAnimatorController.lightningDamageTrigger.damage = 25 + ((baseStrenght / 3) + (baseIntellect / 3));
       playerAnimatorController.executeDamageTrigger.damage   = 35 + ((baseStrenght / 3) + (baseIntellect / 3));
       healAmmount = 15 + (baseVitality + baseIntellect) / 5;
    }

    //_____________Skills______________\\

    private void RegisterSkillInput()
    {
        if (_genericCDExpired)
        {
           if (playerInput.qskill && qSkill && _qSkillUnlocked)
               Qskill(); 
           else
           if (playerInput.wskill && wSkill && _wSkillUnlocked)
               Wskill();
           else
           if (playerInput.eskill && eSkill && _eSkillUnlocked)
               Eskill();
           else
           if (playerInput.rskill && rSkill && _rSkillUnlocked)
               Rskill();
        }
       
        if (hasUsedSkill)
            StartCoroutine(GeneralSkillCoolDown(1f));
    }

    private IEnumerator GeneralSkillCoolDown(float _CoolDown)
    {
        _genericCDExpired  = false;
        hasUsedSkill       = false;

        yield return new WaitForSeconds(_CoolDown);

        _genericCDExpired = true;
    }

    private IEnumerator SkillCoolDown(float _CoolDown, int _SkillIndex)
    {

        if (_SkillIndex == 1)
        { qSkill = false; }
        else
        if (_SkillIndex == 2)
        { wSkill = false; }
        else
        if (_SkillIndex == 3)
        { eSkill = false; }
        else
        if (_SkillIndex == 4)
        { rSkill = false;  }

        yield return new WaitForSeconds(_CoolDown);

        if (_SkillIndex == 1)
        { qSkill = true; }
        else
        if (_SkillIndex == 2)
        { wSkill = true; }
        else
        if (_SkillIndex == 3)
        { eSkill = true; }
        else
        if (_SkillIndex == 4)
        { rSkill = true; }
    }

    private void Qskill()
    {
        _manaCost = 10;

        if (currentManaPoints >= _manaCost)
        {
            hasUsedSkill         = true;
            currentManaPoints -= 10;

            _skillCoolDown     = 10f;
            _playerUI.qSkillCD = _skillCoolDown;

            playerAnimatorController.PlaySkillAnimation("Slam");
            StartCoroutine(SkillCoolDown(_skillCoolDown, 1));
        }
    }

    private void Wskill()
    {
        _manaCost = 10;

        if (currentManaPoints >= _manaCost)
        {
            hasUsedSkill         = true;
            currentManaPoints -= 10;

            _skillCoolDown     = 15f;
            _playerUI.wSkillCD = _skillCoolDown;

            playerAnimatorController.PlaySkillAnimation("Explosion");
            StartCoroutine(SkillCoolDown(_skillCoolDown, 2));
        }
    }

    private void Eskill()
    {
        _manaCost = 20;

        if (currentManaPoints >= _manaCost)
        {
            hasUsedSkill         = true;
            currentManaPoints -= 20;

            _skillCoolDown     = 45f;
            _playerUI.eSkillCD = _skillCoolDown;

            playerAnimatorController.PlaySkillAnimation("Execute");
            StartCoroutine(SkillCoolDown(_skillCoolDown, 3));
        }
    }

    private void Rskill()
    {
        _manaCost = 10;

        if (currentManaPoints >= _manaCost)
        {
            currentManaPoints -= 10;
            hasUsedSkill = true;

            _skillCoolDown     = 90f;
            _playerUI.rSkillCD = _skillCoolDown;

            playerAnimatorController.PlaySkillAnimation("Heal");
            currentHealthPoints += healAmmount;

            if(currentHealthPoints > healthPoints)
               currentHealthPoints = healthPoints;

            StartCoroutine(SkillCoolDown(_skillCoolDown, 4));
        }
    }

    //_____________Default______________\\

    private void RegenHpAndMp()
    {
        if (_playerState == PlayerState.Idle || _playerState == PlayerState.Move)
        {
            if(currentHealthPoints < healthPoints)
            currentHealthPoints += 0.2f * Time.deltaTime;

            if(currentManaPoints < manaPoints)
            currentManaPoints   += (1f + (baseIntellect / 50) ) * Time.deltaTime;
        }
    }
}
