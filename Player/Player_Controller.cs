using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerState {Null, Idle, Move, Attack, Dead }
public class Player_Controller : MonoBehaviour
{
    public PlayerState _playerState;

    [Header("Attributes")]
    [SerializeField] private float _healthPoints = 0;
    [SerializeField] private float _manaPoints   = 0;
    [SerializeField] private float _runSpeed     = 0;
    [SerializeField] private int   _strength     = 0;
    [SerializeField] private int   _dexterity    = 0;
    [SerializeField] private int   _intellect    = 0;
    [SerializeField] private int   _vitality     = 0;
    [SerializeField] private int   _armor        = 0;
    [SerializeField] private int   _damage       = 0;

    [Header("Experience")]
    private float _expCollected   = 0;
    private float _expToNextLevel = 0;

    //LayerMasks
    public LayerMask   _movementLayermask;
    public LayerMask   _interactionLayermask;
    [Space]


    private Vector3 _nextDestination;
    private bool    _destinationReached = true;

    private GameObject _newEnemy      = null;

    private bool       _usingSkill = false;

    public float currentHealthPoints
    { get; set; }
    public float currentManaPoints
    { get; set; }

    //Getters
    public float healthPoints
    { get { return _healthPoints; } set { _healthPoints = value; } }

    public float manaPoints
    { get { return _manaPoints; } set { _manaPoints = value; } }

    public float runSpeed
    { get { return _runSpeed; } set { _runSpeed = value; } }

    public int baseStrenght
    { get { return _strength; } set { _strength = value; } }

    public int baseDexterity
    { get { return _dexterity; } set { _dexterity = value; } }

    public int baseIntellect
    { get { return _intellect; } set { _intellect = value; } }

    public int baseVitality
    { get { return _vitality; } set { _vitality = value; } }

    public int baseArmor
    { get { return _armor; } set { _armor = value; } }

    public int baseDamage
    { get { return _damage; } set { _damage = value; } }

    public float expCollected
    { get { return _expCollected; } set { _expCollected = value; } }

    public float expToNextLevel
    { get { return _expToNextLevel; } set { _expToNextLevel = value; } }

    public bool hasUsedSkill
    { get { return _usingSkill; } set { _usingSkill = value; } }

    public GameObject newEnemy
    { get { return _newEnemy; }}

    public Animator animator
    { get { return _animator; } set { _animator = value; } }

    public Player_Input playerInput
    { get { return _playerInput; } }

    public GameManager gameManager
    { get { return _gameManager; } }

    public Player_AnimatorController playerAnimatorController
    { get { return _playerAnimatorController; } set { _playerAnimatorController = value; } }


    //Components
    public  PlayerUI                  _playerUI;
    public  PlayerInventory           _playerInventory;
    public  Shared                    _sharedVar;
    private GameManager               _gameManager;
    private Player_Input              _playerInput;
    private Animator                  _animator;         
    private NavMeshAgent              _navAgent;
    private CapsuleCollider           _playerCollider;
    private Player_AnimatorController _playerAnimatorController;



    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();

        _playerAnimatorController = GetComponent<Player_AnimatorController>();
        _playerInput              = GetComponent<Player_Input>();
        _animator                 = GetComponent<Animator>();
        _playerCollider           = GetComponent<CapsuleCollider>();
        _navAgent                 = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        currentHealthPoints = healthPoints;
        currentManaPoints   = manaPoints;
    }

    //_____________Updates_______________\\

    protected virtual void Update()
    {
        InputUpdates();

       _playerAnimatorController.StopMovementDuringSkills(_navAgent);

        ChaseEnemy(_newEnemy);
        PlayerIsDead();
    }

    private void InputUpdates()
    {
        if (_playerInput.mousePositionInput)
        {
            Move();
        }

        if (_playerInput.mouseInteractInput)
        {
            SelectObject();
        }

        OnDestinationReached(_nextDestination);
    }

    //______________Navigation_______________\\

    private void Move()
    {
        if (animator.GetFloat("AttackPar") < 1 && _playerAnimatorController.intExecute < 1 && animator.GetFloat(_playerAnimatorController.lightningParameter) < 1)
        {
            _destinationReached = false;

            RaycastHit hitPosition;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitPosition, 100, _movementLayermask))
            {

                if (hitPosition.collider.CompareTag("Walkable"))
                {
                    _navAgent.ResetPath();
                    _navAgent.isStopped = false;

                    _newEnemy = null;

                    SetNewDestination(hitPosition.point);

                    _nextDestination = hitPosition.point;
                }
                else
                { return; }
            }
        }
    }

    private void SetNewDestination(Vector3 Destination)
    {
        _navAgent.stoppingDistance = 0;

        _navAgent.SetDestination(Destination);
        _playerState = PlayerState.Move;
    }

    private void OnDestinationReached(Vector3 NewPosition)
    {
        if (Vector3.Distance(transform.position, NewPosition) < 1f)
            _destinationReached = true;

        if (_destinationReached && _playerState != PlayerState.Attack)
            _playerState = PlayerState.Idle;
    }

    //______________Combat_______________\\

    private void ChaseEnemy(GameObject Enemy)
    {
        if (_newEnemy && Vector3.Distance(transform.position, Enemy.transform.position) > 2.5f)
        {
            SetNewDestination(Enemy.transform.position);

            _navAgent.stoppingDistance = 2;

            _playerState = PlayerState.Move;
        }
        else
        if (_newEnemy && Vector3.Distance(transform.position, Enemy.transform.position) < 3f)
        {
            _navAgent.ResetPath();

            if (_newEnemy.GetComponent<AI_Controller>())
            {
                AI_Controller enemy = _newEnemy.GetComponent<AI_Controller>();

                if (enemy._aiState != AI_State.Dead)
                {
                    _playerState = PlayerState.Attack;
                }
                else
                {
                    _playerState = PlayerState.Idle;
                }
            }
            else
            if (_newEnemy.GetComponent<AI_Skeleton>())
            {
                AI_Skeleton skeleton = _newEnemy.GetComponent<AI_Skeleton>();

                if (skeleton.skeletonState != SkeletonState.Dead)
                {
                    _playerState = PlayerState.Attack;
                }
                else
                {
                    _playerState = PlayerState.Idle;
                }
            }
            else
            if (_newEnemy.GetComponent<AI_Witch>())
            {
                AI_Witch witch = _newEnemy.GetComponent<AI_Witch>();

                if (witch.witchState != WitchState.Dead)
                {
                    _playerState = PlayerState.Attack;
                }
                else
                {
                    _playerState = PlayerState.Idle;
                }
            }
        }
    }

    private void PlayerIsDead()
    {
        if (currentHealthPoints <= 0)
        {
            _playerState = PlayerState.Dead;

            _navAgent.isStopped     = true;
            _playerCollider.enabled = false;
        }
    }

    public void TakeDamage(float DamageAmount)
    {
        currentHealthPoints -= DamageAmount / (baseArmor / 10);
    }

    //______________Interactions_______________\\

    public void LookAt(Transform objectToLookAt)
    {
        transform.LookAt(objectToLookAt.transform, Vector3.up);
    }

    private void GetInteractableObject(GameObject Object)
    {
        if (Object.CompareTag("Loot"))
        {
            LootChest chest = Object.GetComponent<LootChest>();

            if (Vector3.Distance(transform.position, Object.transform.position) < 5f)
            {
                if (chest.canBeOpened)
                {
                    _playerState = PlayerState.Idle;
                    chest.OpenChest(_playerInventory, _playerUI, _playerInventory.lootSlots);
                }
                else
                { _playerUI.DisplayInventoryFromSystem(); }
            }
        }
    }

    private void SelectObject()
    {
        RaycastHit objectHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out objectHit, 100, _interactionLayermask))
        {
            if (objectHit.transform.gameObject.CompareTag("AI_Entity"))
            {
                _newEnemy = objectHit.transform.gameObject;
            }
            else
                GetInteractableObject(objectHit.transform.gameObject);

            _nextDestination = objectHit.transform.gameObject.transform.position;
        }
    }

    //______________Triggers_______________\\

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Explosion"))
        {
            currentHealthPoints -= 30f;
        }
        else
        if (other.gameObject.CompareTag("Fire"))
        {
            currentHealthPoints -= 15f;
        }
       
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("DOT"))
        {
            currentHealthPoints -= 10f * Time.deltaTime;
        }
        else
        if (other.gameObject.CompareTag("HealthPillar"))
        {
            HealthPillar pillar = other.GetComponent<HealthPillar>();

            if (currentHealthPoints != healthPoints && pillar.healingAmount > 0)
            { currentHealthPoints += 5f * Time.deltaTime; pillar.healingAmount -= 5f * Time.deltaTime; }
        }
    }

}
