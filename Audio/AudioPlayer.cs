using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource _audioSource
    { get; set; }

    public bool isPlaying = false;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

   
    void Update()
    {
        if (_audioSource.isPlaying)
            isPlaying = true;
        else
            isPlaying = false;
    }
}
