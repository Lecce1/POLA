using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class LobbyAudioManager : MonoBehaviour
{
    [FoldoutGroup("오디오 소스")]
    [Title("BGM")]
    public AudioSource bgmAudio;
    [FoldoutGroup("오디오 소스")]
    [Title("UI")]
    public AudioSource uiAudio;
    [FoldoutGroup("오디오 소스")]
    [Title("Effect")]
    public AudioSource effectAudio;
    
    [FoldoutGroup("오디오 클립")]
    [Title("UI-Button")]
    public AudioClip uiButtonClip;
    [FoldoutGroup("오디오 클립")]
    [Title("Player")]
    public AudioClip playerClip;

    [FoldoutGroup("오디오 믹서")]
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

    public void PlayAudio(string type)
    {
        switch (type)
        {
            case "Button":
                if (LobbyManager.instance.set.activeSelf && DBManager.instance.currentPlatform == "MOBILE" && EventSystem.current.currentSelectedGameObject.name == "Slider")
                {
                    
                }
                else
                {
                    uiAudio.PlayOneShot(uiButtonClip);
                }
                break;
            
            case "Player":
                effectAudio.PlayOneShot(playerClip);
                break;
        }
    }
}
