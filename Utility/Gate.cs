using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{

    public AI_Main enemy;

    private Vector3 distance = new Vector3(0, 0, -10);
    private Vector3 currentPosition;

    private void Start()
    {
        currentPosition = transform.position;
    }

    void Update()
    {
        OpenGateWhenDead();
    }

    private void OpenGateWhenDead()
    {
        float enemyHp = enemy.healthPoints;

        if (enemyHp <= 0 && transform.position.y >= 45f)
            transform.Translate(distance * 1 * Time.deltaTime);
    }
}
