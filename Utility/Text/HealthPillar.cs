using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPillar : MonoBehaviour
{
    public TextMesh text;
    public GameObject stoneLight;
    [Space]
    public float healingAmount = 50;

    private bool _displayText = true;

    private Camera _camera;
    
    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        DisplayText();
        HealingCapacity();
    }

    private void HealingCapacity()
    {
        if (healingAmount <= 0)
        {
            _displayText = false;
        }
    }

    private void DisplayText()
    {
        if (_displayText)
            text.transform.LookAt(2 * transform.position - _camera.transform.position);
        else
        { text.gameObject.SetActive(false); stoneLight.SetActive(false); }
    }
}
