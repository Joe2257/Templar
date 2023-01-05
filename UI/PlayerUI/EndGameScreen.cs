using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScreen : MonoBehaviour
{
    public AI_Witch witch;

    public GameObject backGroundPanel;


    void Update()
    {
        CheckWitchHP();
    }

    private void CheckWitchHP()
    {
        if (witch.currentHp <= 0)
        {
            StartCoroutine(EndGame());
        }
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5f);

        backGroundPanel.SetActive(true);

    }
}
