using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class LobbyAudioManager : MonoBehaviour
{
    [FoldoutGroup("음악")]
    [Title("오디오")]
    public new AudioSource audio;
    
    [FoldoutGroup("음악")]
    [Title("오디오 믹서")]
    public AudioMixer audioMixer;
    
    public static LobbyAudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    void Start()
    {
        StartCoroutine("Init");
    }

    IEnumerator Init()
    {
        while (!DBManager.instance.isJsonLoad)
        {
            yield return null;
        }
        
        audioMixer.SetFloat("Music", DBManager.instance.musicValue * 80 - 80);
        audioMixer.SetFloat("FX", DBManager.instance.sfxValue * 80 - 80);
    }
}
