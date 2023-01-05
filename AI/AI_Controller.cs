using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AI_Controller : AI_Main
{

    [Header("Attack")]
    [SerializeField] private string _attackParameter = "";
    public DamageTrigger            _damageTrigger;

    [Header("SpecialAttack")]
    [SerializeField] private string specialParameter   = "";
    [SerializeField] private float  specialAttackTimer = 0;

    public ParticleSystem specialFX;
    public BoxCollider    specialCollider;

    [Header("Audio")]
    [Tooltip("0 = Alerted / 1 = Melee / 2 = Special / 3 = Dead")]
    public AudioClip[] clips;
    private bool _playChase   = true;
    private bool _playMelee   = true;
    private bool _playSpecial = true;



    private int  _attackHash    = -1;
    private int  _specialHash   = -1;
    private bool _specialAttack        = false;
    private bool _isPlaying            = false;
    private bool _isInStartingPosition = true;

    private Quaternion _lookRotation;

    protected override void Start()
    {
        base.Start();

        _damageTrigger.damage = damage;
        _attackHash  = Animator.StringToHash(_attackParameter);
        _specialHash = Animator.StringToHash(specialParameter);

        LevelScaling(level);
    }

    //______________Updates_______________\\

    protected override void Update()
    {
        base.Update();

        StateController();
        SpecialAttack();
    }

    private void StateController()
    {
        switch (_aiState)
        {
            case AI_State.Idle:
                Idle();
                break;
            case AI_State.Patrol:
                Patrol();
                break;
            case AI_State.Chase:
                Chase();
                break;
            case AI_State.Attack:
                Attack();
                break;
            case AI_State.Dead:
                Dead();
                break;
        }
    }

    //______________States_______________\\

    private void Idle()
    {
        animator.SetFloat("Speed", 0);
        animator.SetFloat("Attack", 0);

        destinationReached = true;

        if (sensorTrigger.playerInRange&& player._playerState != PlayerState.Dead)
        {
            _aiState = AI_State.Chase;

            _isInStartingPosition = false;
        }

        if (isInIdleState && fixedPatrol || randomPatrol)
        { StartCoroutine(IdleRoutine(3f)); isInIdleState = false;}
    }

    private IEnumerator IdleRoutine(float WaitingTime)
    {
        yield return new WaitForSeconds(WaitingTime);

        _aiState = AI_State.Patrol;

        yield break;
    }

    private void Patrol()
    {

        animator.SetFloat("Speed", 1);
        animator.SetFloat("Attack", 0);

        navAgent.speed = walkSpeed;
        navAgent.stoppingDistance = 0;

        PatrolWaypoints();

        if (sensorTrigger.playerInRange && player._playerState != PlayerState.Dead)
        {
            _aiState = AI_State.Chase;

            _isInStartingPosition = false;
        }
    }

    private void Chase()
    {
        animator.SetFloat("Speed", 2);
        animator.SetFloat("Attack", 0);

        navAgent.stoppingDistance = 2;

        if (animator.GetFloat(specialParameter) < 1)
            navAgent.speed = runSpeed;
        else
            navAgent.speed = 0;

        if (animator.GetFloat(_attackParameter) < 1 && player._playerState != PlayerState.Dead)
        ChasePlayer();

    }

    private void Attack()
    {
        if (player._playerState != PlayerState.Dead && animator.GetFloat(specialParameter) < 1)
        {

            animator.SetFloat("Speed", 0);
            animator.SetFloat("Attack", 1);

            if (animator.GetFloat(_attackParameter) > 1)
            {
                _damageTrigger.gameObject.SetActive(true);

                if (_playMelee)
                {
                    gameManager.PlayAudio(clips[1], transform.position);
                    _playMelee = false;
                }
            }
            else
            { _damageTrigger.gameObject.SetActive(false); _playMelee = true; }

            navAgent.speed = 0;

            LookAtTarget();

            if (Vector3.Distance(transform.position, gameManager.playerPosition) > 3f && animator.GetFloat("AttackPar") < 1)
            {
                animator.SetFloat("Attack", 0);
                _aiState = AI_State.Chase;
            }
        }
    }

    private void Dead()
    {
        if (isDead)
        {
            gameManager.PlayAudio(clips[3], transform.position);
            gameManager.playerSystem.GetExpOnKill(expOnKill);

            StopAllCoroutines();

            navAgent.enabled = false;

            animator.SetFloat("Speed", 0);
            animator.SetFloat("Attack", 0);
            animator.SetTrigger("Die");

            _healthBar.gameObject.SetActive(false);
            _damageTrigger.gameObject.SetActive(false);
            specialFX.gameObject.SetActive(false);
            aiCollider.enabled = false;

            isDead = false;
        }
       
    }

    //______________Combat_______________\\

    public void ChasePlayer()
    {
        if (_playChase)
        {
            gameManager.PlayAudio(clips[0], transform.position);

            _playChase = false;
        }

        SetDestination(gameManager.playerPosition);

        if (Vector3.Distance(transform.position, gameManager.playerPosition) < 3f)
        {
            _aiState = AI_State.Attack;
        }

        if (!sensorTrigger.playerInRange && !_isInStartingPosition)
        {
            SetDestination(startingPosition);
            navAgent.stoppingDistance = 0;

            if (Vector3.Distance(transform.position, startingPosition) < 0.5f)
            {
                _aiState = AI_State.Idle;

                _isInStartingPosition = true;
                _playChase = true;
            }
        }
    }

    private void SpecialAttack()
    {
        if (!_specialAttack && _aiState == AI_State.Attack && animator.GetFloat("Speed") < 1)
        {
            StartCoroutine(SpecialAttack(specialAttackTimer));
        }

        if (animator.GetFloat(specialParameter) > 1 && !_isPlaying)
        {
            navAgent.speed = 0; specialFX.Play();
            specialCollider.enabled = true;
            _isPlaying = true;

            if (_playSpecial)
            {
                gameManager.PlayAudio(clips[2], transform.position);
                _playSpecial = false;
            }
        }
        else
        if(animator.GetFloat(specialParameter) < 2 && _isPlaying)
        { navAgent.speed = 0; specialFX.Stop(); specialCollider.enabled = false; _isPlaying = false; _playSpecial = true; }
    }

    private IEnumerator SpecialAttack(float Timer)
    {
        _specialAttack = true;

        yield return new WaitForSeconds(Timer);

        navAgent.speed = 0;

        animator.SetFloat("Attack", 0);
        animator.SetTrigger("SpecialAttack");
        _specialAttack = false;
    }

    private void LookAtTarget()
    {
        _lookRotation = Quaternion.LookRotation(gameManager.playerPosition - transform.position);

        if (animator.GetFloat("AttackPar") <= 1 && _aiState != AI_State.Dead)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, 5f * Time.deltaTime);
        }
    }

    //______________LevelScaling_______________\\

    private void LevelScaling(int level)
    {
        healthPoints += 10 * level;
        damage += 5f * level;
        resistance += 5 * level;
        expOnKill += 10 * level;

        _healthBar.maxValue = healthPoints;
    }
}
