using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource audio;
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(PlayMusic), 1f);
    }

    void PlayMusic()
    {
        
        audio.Play();
    }
}
