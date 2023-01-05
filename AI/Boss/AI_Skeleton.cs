using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public enum SkeletonState {Null, Idle, Chase, Dead }
public class AI_Skeleton : MonoBehaviour
{
    public SkeletonState skeletonState;

    [SerializeField] private float _healthPoints  = 0;
    [SerializeField] private float _walkSpeed     = 0;
    [SerializeField] private float _runSpeed      = 0;
    //[SerializeField] private float _specialDamage = 0;

    [SerializeField] private float _specialAttackTimer = 0;

    private Quaternion _lookRotation;

    [Space]
    public GameObject fireBall;
    public GameObject fireWall;
    public Transform  spawnPoint;
    [Space]
    public Slider healthBar;

    public string specialPar = "";

    [Header("Audio")]
    [Tooltip("0 = Fireball / 1 = FireSlam / 2 = Dead")]
    public AudioClip[] clips;
    private bool _playFireball = true;
    private bool _playFireSlam = true;


    
    private bool _specialAttack     = true;
    private bool _spawnFireBall     = false;
    private bool _spawnFireWall     = false;
    private bool _2ndPhase          = false;
    private bool _isDead            = false;

    private int _specialHash = -1;

    public float currentHp
    { get; set; }
    public bool playerInRange
    { get; set; }
    public float healthPoints
    { get { return healthPoints; } }

    public GameManager gameManager;

    private Animator     _animator;
    private NavMeshAgent _navAgent;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();

        _specialHash = Animator.StringToHash(specialPar);

        currentHp         = _healthPoints;
        healthBar.maxValue = currentHp;

        playerInRange = false;
    }

    private void FixedUpdate()
    {
        switch (skeletonState)
        {
            case SkeletonState.Idle:
                Idle();
                break;
            case SkeletonState.Chase:
                Chase();
                break;
            case SkeletonState.Dead:
                Dead();
                break;
        }
    }

    void Update()
    {
        SpecialAttack();
        FireBall();
        FireWall();

        ExecuteDeath();

        LookAtTarget();
        HealthBar();
    }

    private void HealthBar()
    {
        healthBar.value = currentHp;

        healthBar.transform.LookAt(Camera.main.transform);
    }

    private void Idle()
    {
        _animator.SetFloat("Speed", 0);
        _navAgent.speed = 0;

        if (Vector3.Distance(transform.position, gameManager.playerPosition) > 3 && _animator.GetFloat("SpecialPar") < 1 && playerInRange)
        {
            skeletonState = SkeletonState.Chase;
        }
    }

    private void Chase()
    {
        if (_2ndPhase)
        { _animator.SetFloat("Speed", 2); _navAgent.speed = _runSpeed; }
        else
        { _animator.SetFloat("Speed", 1); _navAgent.speed = _walkSpeed; }

        if (_animator.GetFloat("SpecialPar") < 1)
        {
            _navAgent.SetDestination(gameManager.playerPosition);
        }

        if (Vector3.Distance(transform.position, gameManager.playerPosition) <= 3)
        {
            skeletonState = SkeletonState.Idle;
        }
    }

    private void LookAtTarget()
    {
        _lookRotation = Quaternion.LookRotation(gameManager.playerPosition - transform.position);

        if (skeletonState != SkeletonState.Dead)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, 5f * Time.deltaTime);
        }
    }

    private void SpecialAttack()
    {
        if (_specialAttack && playerInRange)
        {
            _specialAttack  = false;

            if (Vector3.Distance(transform.position, gameManager.playerPosition) <= 4)
                StartCoroutine(FireWall(_specialAttackTimer));
            else
                StartCoroutine(FireBall(_specialAttackTimer));
        }
    }

    private IEnumerator FireBall(float Time)
    {
        _spawnFireBall = true;
        _animator.SetTrigger("FireBall");
        
        yield return new WaitForSeconds(Time);
        _specialAttack = true;
        _spawnFireBall = false;
    }

    private void FireBall()
    {

        if (_animator.GetFloat("SpecialPar") > 0 && _spawnFireBall)
        {
            _navAgent.speed = 0;
            _playFireball   = true;
            skeletonState   = SkeletonState.Idle;
        }

        if (_animator.GetFloat("SpecialPar") > 1 && _spawnFireBall)
        {
            _spawnFireBall = false;

            GameObject spell = Instantiate(fireBall, spawnPoint.position, Quaternion.identity);
            Rigidbody spellRB = spell.GetComponent<Rigidbody>();

            spell.transform.parent = null;
            spellRB.AddForce(transform.forward * 50f, ForceMode.Impulse);

            if (_playFireball)
            {
                gameManager.PlayAudio(clips[0], transform.position);
                _playFireball = false;
            }
        }
    }

    private IEnumerator FireWall(float Time)
    {
        _spawnFireWall = true;
        _animator.SetTrigger("FireWall");

        yield return new WaitForSeconds(Time);
        _specialAttack = true;
        _spawnFireWall = false;
    }

    private void FireWall()
    {
        if (_animator.GetFloat("SpecialPar") > 0 && _spawnFireWall)
        {
            _navAgent.speed = 0;
            skeletonState = SkeletonState.Idle;
        }

        if (_animator.GetFloat("SpecialPar") > 1 && _spawnFireWall)
        {
            _spawnFireBall = false;

            fireWall.SetActive(true);

            if (_playFireSlam)
            {
                gameManager.PlayAudio(clips[1], transform.position);
                _playFireSlam = false;
            }
        }
        else
        if (_animator.GetFloat("SpecialPar") < 1)
        { fireWall.SetActive(false); _playFireSlam = true; }
    }

    public void TakeDamage(float Damage)
    {
        currentHp -= Damage;
    }

    private void Dead()
    {
        if (!_isDead)
        {
            gameManager.PlayAudio(clips[2], transform.position);

            StopAllCoroutines();

            _animator.SetFloat("Speed", 0);
            _animator.SetTrigger("Dead");

            _navAgent.enabled = false;
            healthBar.gameObject.SetActive(false);

            _isDead = true;
        }
    }

    private void ExecuteDeath()
    {
        if (currentHp <= 0)
        {
            skeletonState = SkeletonState.Dead;
        }
    }
}
