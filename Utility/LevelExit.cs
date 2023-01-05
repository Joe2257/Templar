using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public EncounterTrigger trigger;
    public GameObject exit;

    public float maxHeight;

    private bool canMove = true;
    
    void Update()
    {
        UncoverExit();
    }

    private void UncoverExit()
    {
        if (trigger.aiSkeleton.currentHp <= 0 && canMove)
        {
            if (transform.position.y >= maxHeight)
                canMove = false;

            exit.transform.Translate(new Vector3(0, 1, 0) * 15f * Time.deltaTime);
        }
    }
}
