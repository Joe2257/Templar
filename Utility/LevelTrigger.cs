using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    public GameManager gameManager;
    public string levelToTrigger = "";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SaveSystem.SavePlayer(gameManager.playerSystem);

            gameManager.LoadSceneByString(levelToTrigger);
            gameManager.playerSystem._sharedVar.loadPlayer = true;
        }
    }
}
