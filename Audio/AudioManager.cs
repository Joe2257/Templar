using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioPlayer[] audioArray;


    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void PlayAudio(AudioClip Clip, Vector3 position)
    {
        for (int i = 0; i < audioArray.Length; i++)
        {
            if (!audioArray[i].isPlaying)
            {
                audioArray[i].transform.position = position;
                audioArray[i]._audioSource.PlayOneShot(Clip);

                break;
            }
        }
        
    }
}
