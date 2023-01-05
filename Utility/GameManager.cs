using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : AudioManager
{

    [TextArea(0, 20)]
    public string firstMissionObjective = "";
    [TextArea(0, 20)]
    public string secondMissionObjective = "";
    [TextArea(0, 20)]
    public string thirdMissionObjective = "";

    public Vector3 playerPosition
    { get; set; }

    public Player_System playerSystem;

    public Scene _currentScene
    { get; set; }
    public string nextScene
    { get; set; }

    void Start()
    {
        _currentScene = SceneManager.GetActiveScene();

        DetermineScene();

        if (_currentScene.name == "Village")
        { nextScene = "Canion"; }
        else
        if (_currentScene.name == "Canion")
        { nextScene = "Forest"; }
        if (_currentScene.name == "Forest")
        { nextScene = "Forest"; }
    }

    void Update()
    {
        PlayerPosition();
    }

    private void PlayerPosition()
    {
        if (playerSystem)
        playerPosition = playerSystem.transform.position;
        else 
        if (!playerSystem && FindObjectOfType<Player_System>())
            playerSystem = FindObjectOfType<Player_System>();
    }

    public void LoadSceneByString(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void DetermineScene()
    {
        if (_currentScene.name == "Village")
        {
            playerSystem._playerUI.SetMissionObjective(firstMissionObjective);
        }
        else
        if (_currentScene.name == "Canion")
        {
            playerSystem._playerUI.SetMissionObjective(secondMissionObjective);
        }
        else
        if (_currentScene.name == "Forest")
        {
            playerSystem._playerUI.SetMissionObjective(thirdMissionObjective);
        }
    }
}
