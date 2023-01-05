using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WitchState {Null, Alive, Dead }
public class AI_Witch : MonoBehaviour
{
    public WitchState witchState;

    [SerializeField] private float _healthPoints = 0;

    public float currentHp
    { get; set; }
    private float _hpLeft;

    public Slider healthBar;

    public float timeBetweenAttacks = 0;

    private int _newTeleportPosition = 0;

    public string teleportPar = "";
    public string summonPar   = "";
    public string attackPar   = "";

    public float healthPoints
    { get { return healthPoints; } }
    public bool playerInRange
    { get; set; }

    private int _teleportHash = -1;
    private int _summonHash   = -1;
    private int _attackHash   = -1;

    private bool _zombieAreSpawned = false;
    private bool _canAttack        = true;
    private bool _canTeleport      = true;
    private bool _isShielded       = false;
    private bool _forceCicle       = false;
    private bool _isDead           = false;

    private Quaternion _lookRotation;

    public GameObject zombieToSpawn;
    public List<AI_Controller> _zombies = new List<AI_Controller>();

    public GameObject     flames;
    public GameObject     slam;
    public GameObject     shield;
    public ParticleSystem teleport;

    public Transform   startPosition;
    public Transform[] teleportPoints;
    public Transform[] spawnPoints;

    [Header("Audio")]
    [Tooltip("0 = Flames / 1 = Slam / 2 = Teleport / 3 = SpawnZombie / 4 = Dead")]
    public AudioClip[] clips;
    private bool _playFlames      = true;
    private bool _playSlam        = true;
    private bool _playTeleport    = true;
    private bool _playSpawnZombie = true;


    public GameManager gameManager;
    private Animator   _animator;

    void Start()
    {
        witchState = WitchState.Alive;

        _animator = GetComponent<Animator>();

        _teleportHash = Animator.StringToHash(teleportPar);
        _summonHash   = Animator.StringToHash(summonPar);
        _attackHash   = Animator.StringToHash(attackPar);

        transform.position = teleportPoints[_newTeleportPosition].position;

        _newTeleportPosition++;

        currentHp = _healthPoints;
        _hpLeft    = currentHp - 250;
        healthBar.maxValue = currentHp;

        playerInRange = false;
    }

    
    void Update()
    {
        if (playerInRange)
        {
            CicleTeleport();
            SpawnZombies();

            Spells();
            Flames();
            Slam();

            ExecuteDeath();

            LookAtTarget();
            HealthBar();
        }
            
    }

    private void HealthBar()
    {
        healthBar.value = currentHp;

        healthBar.transform.LookAt(Camera.main.transform);
    }

    private void CicleTeleport()
    {
       
       if (_newTeleportPosition == 4)
           _canTeleport = false;

       if (_forceCicle == true || currentHp <= _hpLeft)
       {
           if (_canTeleport)
           {
               _animator.SetBool("Teleport", true);
               //flames.SetActive(false);
               //slam.SetActive(false);
           }

           if (_animator.GetFloat(teleportPar) > 1 && _canTeleport)
           {
                if (_playTeleport)
                {
                    gameManager.PlayAudio(clips[2], transform.position);
                    _playTeleport = false;
                }

               _animator.SetBool("Teleport", false);

               teleport.Play();

               transform.position = teleportPoints[_newTeleportPosition].position;

               _newTeleportPosition++;

               _hpLeft = currentHp - 250;

               _zombieAreSpawned = false;
               _forceCicle = false;
            }
           else
                _playTeleport = true;
       }
    }

    private void Spells()
    {
        if (_animator.GetFloat(teleportPar) < 1 && _animator.GetFloat(summonPar) < 1)
        {
            if (!_isShielded)
            {
                if (Vector3.Distance(transform.position, gameManager.playerPosition) > 7 && _canAttack)
                { StartCoroutine(FlameRoutine(timeBetweenAttacks)); _canAttack = false; }
                else
                if (Vector3.Distance(transform.position, gameManager.playerPosition) < 7 && _canAttack)
                { StartCoroutine(SlamRoutine(timeBetweenAttacks)); _canAttack = false; }
            }
        }
    }

    private IEnumerator FlameRoutine(float Time)
    {
        _animator.SetFloat("Attack", 1);
        
        yield return new WaitForSeconds(Time);

        _canAttack = true;
    }

    private IEnumerator SlamRoutine(float Time)
    {
        _animator.SetFloat("Attack", 2);
        
        yield return new WaitForSeconds(Time);

        _canAttack = true;
    }

    private void Flames()
    {
        if (_animator.GetFloat("AttackPar") == 2 && _animator.GetFloat("Shield") < 1)
        {
            flames.SetActive(true);
            _animator.SetFloat("Attack", 0);

            if (_playFlames)
            {
                gameManager.PlayAudio(clips[0], transform.position);
                _playFlames = false;
            }
        }
        else
        if (_animator.GetFloat("AttackPar") < 1 && _animator.GetFloat("Shield") < 1)
        { flames.SetActive(false); _playFlames = true; }
    }

    private void Slam()
    {
        if (_animator.GetFloat("AttackPar") == 3 && !_animator.GetBool("Teleport"))
        {
            slam.SetActive(true);
            _animator.SetFloat("Attack", 0);

            if (_playSlam)
            {
                gameManager.PlayAudio(clips[1], transform.position);
                _playSlam = false;
            }
        }
        else
        if (_animator.GetFloat("AttackPar") < 1)
        { slam.SetActive(false); _playSlam = true; }
    }

    private void SpawnZombies()
    {
        if (_newTeleportPosition == 4 && !_zombieAreSpawned)
        {
            _animator.SetFloat("Shield", 1);
            
            if (_animator.GetFloat(summonPar) > 1)
            {
                transform.position = startPosition.position;
                shield.SetActive(true);

                if (_playSpawnZombie)
                {
                    gameManager.PlayAudio(clips[3], transform.position);
                    _playSpawnZombie = false;
                }

                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    GameObject zombie = Instantiate(zombieToSpawn, spawnPoints[i].position, spawnPoints[i].rotation);

                    _zombies[i] = zombie.GetComponentInChildren<AI_Controller>();
                    _zombies[i]._aiState = AI_State.Chase;
                }

                _isShielded       = true;
                _zombieAreSpawned = true;
            }
        }

        if(_isShielded)
            StopShield();
    }

    private void StopShield()
    {
       if (_zombies[0] != null || _zombies[1] != null || _zombies[2] != null || _zombies[3] != null)
       {
           if (_zombies[0] != null)
           {
               if(_zombies[0].healthPoints <= 0)
                  _zombies[0] = null;
           }
           if (_zombies[1] != null)
           {
               if (_zombies[1].healthPoints <= 0)
                   _zombies[1] = null;
           }
           if (_zombies[2] != null)
           {
               if (_zombies[2].healthPoints <= 0)
                   _zombies[2] = null;
           }
           if (_zombies[3] != null)
           {
               if (_zombies[3].healthPoints <= 0)
                   _zombies[3] = null;
           }
       }
        
       if (_zombies[0] == null && _zombies[1] == null && _zombies[2] == null && _zombies[3] == null && _animator.GetFloat("Shield") > 0)
       {
           _animator.SetFloat("Shield", 0);
           shield.SetActive(false);

           _newTeleportPosition = 0;

           _isShielded = false;
           _canTeleport = true;
           _forceCicle  = true;

            _playSpawnZombie = true;
       }
    }

    private void LookAtTarget()
    {
        _lookRotation = Quaternion.LookRotation(gameManager.playerPosition - transform.position);

        if (_animator.GetFloat("AttackPar") <= 1 && witchState != WitchState.Dead)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, 5f * Time.deltaTime);
        }
    }

    private void Dead()
    {
        if (!_isDead)
        {
            gameManager.PlayAudio(clips[4], transform.position);

            StopAllCoroutines();

            healthBar.gameObject.SetActive(false);

            _animator.SetFloat("Attack", 0);

            _animator.SetTrigger("Dead");

            _canTeleport = false;
            _isShielded = false;

            witchState = WitchState.Dead;

            _isDead = true;
        }
    }

    private void ExecuteDeath()
    {
        if (currentHp <= 0)
        {
            Dead();
        }
    }

    public void TakeDamage(float Damage)
    {
        if(!_isShielded)
        currentHp -= Damage;
        else
        currentHp += Damage;

    }
}
