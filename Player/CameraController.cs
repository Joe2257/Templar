using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform _playerPosition = null;

    [Range(0.01f, 1.0f)]
    [SerializeField] private float _smoothFactor = 0.5f;

    private Vector3 _offset;

   
    void Start()
    {
        _offset = transform.position - _playerPosition.position;
    }

    void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {

        Vector3 newPos = _playerPosition.position + _offset;

        transform.position = Vector3.Lerp(transform.position, newPos, _smoothFactor);
        
    }
}
