using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum AI_State { Null, Idle, Patrol, Chase, Attack, Dead }
public class AI_Main : MonoBehaviour
{
    public AI_State _aiState;

    [Header("General")]
    [SerializeField] private float _healthPoints = 0;
    [SerializeField] private float _walkSpeed    = 0;
    [SerializeField] private float _runSpeed     = 0;
    [SerializeField] private float _damage       = 0;
    [SerializeField] private float _resistance   = 0;

    [Header("Navigation")]
    [SerializeField] private bool _fixedPatrol  = false;
    [SerializeField] private bool _randomPatrol = false;
    [Space]
    [SerializeField] private Transform     _targetTrigger = null;
    [SerializeField] private SensorTrigger _sensorTrigger;

    [SerializeField] private List<Transform> _waypoints = new List<Transform>();

    [Header("UI")]
    public Slider _healthBar;

    [Header("Level&Loot")]
    [SerializeField] private int   _level      = 0;
    [SerializeField] private float _expOnKill  = 0;
    [SerializeField] private int   _goldOnKill = 0;

    private Vector3 _newPosition;
    public Vector3  startingPosition
    {get; set;}
    
    private int _currentWaypoint = 0;
    private bool _destinationReached = false;
    private bool _isInIdleState = true;
    private bool _isDead = false;
    private bool _isInDeadState = false;

    //Components
    public Player_Controller player
    {get; set;}
    public GameManager gameManager
    {get; set;}

    public Animator animator
    {get; set;}
    public NavMeshAgent navAgent
    { get; set; }
    public CapsuleCollider aiCollider
    { get; set; }

    //Getters
    public float walkSpeed
    { get { return _walkSpeed; } }

    public float runSpeed
    { get { return _runSpeed; } }

    public float healthPoints
    { get { return _healthPoints; } set { _healthPoints = value; } }

    public float damage
    { get { return _damage; } set { _damage = value; } }

    public float resistance
    { get { return _resistance; } set { _resistance = value; } }

    public float expOnKill
    { get { return _expOnKill; }  set { _expOnKill = value; } }

    public float goldOnKill
    { get { return _goldOnKill; } }

    public int level
    { get { return _level; } }

    public bool fixedPatrol
    { get { return _fixedPatrol; } }

    public bool isDead
    { get { return _isDead; } set { _isDead = value; } }

    public bool randomPatrol
    { get { return _randomPatrol; } }

    public bool destinationReached
    { get { return _destinationReached; } set { _destinationReached = value; } }

    public bool isInIdleState
    { get { return _isInIdleState; } set { _isInIdleState = value; } }

    public SensorTrigger sensorTrigger
    { get { return _sensorTrigger; } set { _sensorTrigger = value; } }

    protected virtual void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        player     = gameManager.playerSystem.GetComponent<Player_Controller>();
        animator   = GetComponent<Animator>();
        navAgent   = GetComponent<NavMeshAgent>();
        aiCollider = GetComponent<CapsuleCollider>();

        startingPosition = transform.position;
    }

    protected virtual void Update()
    {
        Health();
    }

    //_______________Navigation_______________\\


    public void SetDestination(Vector3 Destination)
    {
        navAgent.SetDestination(Destination);
    }

    private void OnDestinationReached(Vector3 NewPosition)
    {
        if (Vector3.Distance(transform.position, NewPosition) < 1.5f)
            _destinationReached = true;

        if (_destinationReached && _aiState != AI_State.Attack)
        { _aiState = AI_State.Idle; _isInIdleState = true; }
    }

    public void PatrolWaypoints()
    {
        if (_destinationReached)
        {
            PatrolRoutine();
        }

        OnDestinationReached(_newPosition);
    }

    private void PatrolRoutine()
    {
        if (_fixedPatrol)
        {

            if (_currentWaypoint >= _waypoints.Count)
                _currentWaypoint = 0;

            _targetTrigger.transform.position = _waypoints[_currentWaypoint].transform.position;

            _newPosition = _waypoints[_currentWaypoint].transform.position;
            _destinationReached = false;

            SetDestination(_targetTrigger.position);

            _currentWaypoint++;

        }
        else
        if (_randomPatrol)
        {
            int previousWaypoint = _currentWaypoint;
            _currentWaypoint = Random.Range(0, _waypoints.Count);

            _targetTrigger.transform.position = _waypoints[_currentWaypoint].transform.position;

            _newPosition = _waypoints[_currentWaypoint].transform.position;
            _destinationReached = false;

            SetDestination(_targetTrigger.position);

            if (_currentWaypoint == previousWaypoint)
                _destinationReached = true;
        } 
    }

    public void TakeDamage(float DamageAmount)
    {
        _healthPoints -= DamageAmount - (resistance /7);
    }

    //_______________HealthBar_______________\\

    private void Health()
    {
        if (_aiState != AI_State.Dead)
        {
            _healthBar.value = _healthPoints;
            _healthBar.transform.LookAt(Camera.main.transform);
        }
       
        if (_healthPoints <= 0 && !_isInDeadState)
        { _aiState = AI_State.Dead; _isDead = true; _isInDeadState = true; }
    }

}
