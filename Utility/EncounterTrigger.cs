using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterTrigger : MonoBehaviour
{
    public GameObject arenaBoundaries;
    public GameObject boss;

    public float maxBoundaryHeight;

    private AudioSource _audioSource;

    private AI_Witch   aiWitch;
    public AI_Skeleton aiSkeleton
    { get; set; }
    

    private bool canMove = false;

    private void Start()
    {
        if (boss.GetComponent<AI_Skeleton>())
            aiSkeleton = boss.GetComponent<AI_Skeleton>();
        else
            aiWitch = boss.GetComponent<AI_Witch>();

        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        MoveBoundaries();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartEncounter();
        }
    }

    private void StartEncounter()
    {
        canMove = true;

        if (aiSkeleton)
            aiSkeleton.playerInRange = true;
        else
            aiWitch.playerInRange = true;
    }

    private void MoveBoundaries()
    {
        if (canMove)
        {
            arenaBoundaries.transform.Translate(new Vector3(0, 1, 0) * 15f * Time.deltaTime);

            if (arenaBoundaries.transform.position.y >= maxBoundaryHeight)
            { canMove = false; _audioSource.Play(); }
        }
    }
}
