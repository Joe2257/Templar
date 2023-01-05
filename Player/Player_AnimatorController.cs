using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class Player_AnimatorController : MonoBehaviour
{
    [Header("Parameters")]
    public string attackParameter    = "";
    public string lightningParameter = "";
    public string healPar            = "";

    [Header("DamageTriggers")]
    public GameObject      meleeDamageTrigger;
    public SphereCollider  slamTrigger;
    public CapsuleCollider lightningTrigger;
    public SphereCollider  executeTrigger;

    public DamageTrigger slamDamageTrigger
    { get; set; }
    public DamageTrigger lightningDamageTrigger
    { get; set; }
    public DamageTrigger executeDamageTrigger
    { get; set; }

    [Header("SkillsFX")]
    public GameObject     executeParticle;
    public GameObject     slamParticle;
    public GameObject     swordLighting;
    public ParticleSystem lightning;
    public ParticleSystem heal;
    public GameObject     healLight;
    public int intExecute = 0;
    public int intSlam    = 0;

    private int  _attackHash    = -1;
    private int  _lightningHash = -1;
    private int  _healHash      = -1;

    private bool _isUsingSkill        = false;
    private bool _isDead              = false;
    private bool _playSwordSound      = true;
    private bool _playSlamSound       = true;
    private bool _playLightiningSound = true;
    private bool _playExecuteSound    = true;
    private bool _playHealSound       = true;

    private Player_System     _playerSystem;



    private void Awake()
    {
        _playerSystem  = GetComponent<Player_System>();

        _attackHash    = Animator.StringToHash(attackParameter);
        _lightningHash = Animator.StringToHash(lightningParameter);
        _healHash      = Animator.StringToHash(healPar);

        slamDamageTrigger      = slamTrigger.gameObject.GetComponent<DamageTrigger>();
        lightningDamageTrigger = lightningTrigger.gameObject.GetComponent<DamageTrigger>();
        executeDamageTrigger   = executeTrigger.gameObject.GetComponent<DamageTrigger>();

        _playerSystem.UpdateSkillsDamage();
    }

    
    private void Update()
    {
        AnimatorStateController();

        Execute();
        Slam();
        Lightning();
        Heal();
    }

    private void AnimatorStateController()
    {
        _isUsingSkill = (intSlam < 1 && intExecute < 1 && _playerSystem.animator.GetFloat(lightningParameter) < 1) ? false : true;

        switch (_playerSystem._playerState)
        {
            case PlayerState.Idle:
                IdleState();
                break;
            case PlayerState.Move:
                MoveState();
                break;
            case PlayerState.Attack:
                AttackState();
                break;
            case PlayerState.Dead:
                DeadState();
                break;
        }
    }

    private void IdleState()
    {
        _playerSystem.animator.SetFloat("Speed", 0f);
        _playerSystem.animator.SetFloat("Attack", 0f);
        meleeDamageTrigger.SetActive(false);
    }

    private void MoveState()
    {
        _playerSystem.animator.SetFloat("Attack", 0f);
        _playerSystem.animator.SetFloat("Speed", 2f);
    }

    public void StopMovementDuringSkills(NavMeshAgent agent)
    {
        if (intExecute > 0 || _playerSystem.animator.GetFloat(lightningParameter) > 0 || _playerSystem.animator.GetFloat(healPar) > 0)
        { 
            agent.isStopped = true;
            _playerSystem._playerState = PlayerState.Idle;
            _playerSystem.animator.SetFloat("Speed", 0f);
        } 
    }

    private void AttackState()
    {
        if (!_playerSystem.hasUsedSkill)
        {
            _playerSystem.animator.SetFloat("Speed", 0f);

            if(_playerSystem.animator.GetFloat(attackParameter) < 1 && !_isUsingSkill)
            _playerSystem.animator.SetFloat("Attack", Random.Range(0, 100));
            else
            { _playerSystem.animator.SetFloat("Attack", 0);}

            if (_playerSystem.animator.GetFloat(attackParameter) > 1 && !_isUsingSkill)
            {
                meleeDamageTrigger.SetActive(true);

                if(_playSwordSound)
                {
                    _playerSystem.gameManager.PlayAudio(_playerSystem.clips[0], transform.position);
                    _playSwordSound = false;
                }
            }
            else
            { meleeDamageTrigger.SetActive(false);  _playSwordSound = true; }
        }

        _playerSystem.LookAt(_playerSystem.newEnemy.transform);
    }

    private void DeadState()
    {
        if(!_isDead)
        {
            _playerSystem.gameManager.PlayAudio(_playerSystem.clips[5], transform.position);

            StopAllCoroutines();

            _playerSystem.animator.SetFloat("Speed", 0f);
            _playerSystem.animator.SetFloat("Attack", 0f);
            _playerSystem.animator.SetTrigger("Die");

            _isDead = true;
        }
    }

    public void PlaySkillAnimation(string SkillName)
    {
       _playerSystem.animator.SetTrigger(SkillName);
    }

    private void Slam()
    {
        if (intSlam > 1)
        {
            slamParticle.SetActive(true); slamTrigger.enabled = true;

            if (_playSlamSound)
            {
                _playerSystem.gameManager.PlayAudio(_playerSystem.clips[1], transform.position);
                _playSlamSound = false;
            }
        }
        else
        { slamParticle.SetActive(false); slamTrigger.enabled = false; _playSlamSound = true; }
    }

    private void Lightning()
    {
        if (_playerSystem.animator.GetFloat(lightningParameter) == 2)
            swordLighting.SetActive(true);
        else
            swordLighting.SetActive(false);

        if (_playerSystem.animator.GetFloat(lightningParameter) == 3)
        {
            lightning.Play();
            lightningTrigger.enabled = true;

            if (_playLightiningSound)
            {
                _playerSystem.gameManager.PlayAudio(_playerSystem.clips[2], transform.position);

                _playLightiningSound = false;
            }
        }
        else
        { lightningTrigger.enabled = false; _playLightiningSound = true; }
    }

    private void Execute()
    {
        if (intExecute > 1)
        {
            executeParticle.SetActive(true); executeTrigger.enabled = true;

            if (_playExecuteSound)
            {
                _playerSystem.gameManager.PlayAudio(_playerSystem.clips[3], transform.position);

                _playExecuteSound = false;
            }
        }
        else
        { executeParticle.SetActive(false); executeTrigger.enabled = false; _playExecuteSound = true; }
    }

    private void Heal()
    {
        if (_playerSystem.animator.GetFloat(healPar) == 2)
        {
            heal.Play();
            healLight.SetActive(true);

            if (_playHealSound)
            {
                _playerSystem.gameManager.PlayAudio(_playerSystem.clips[4], transform.position);

                _playHealSound = false;
            }
        }
        else
        if (_playerSystem.animator.GetFloat(healPar) < 1)
        {healLight.SetActive(false); _playHealSound = true; }
    }
}
