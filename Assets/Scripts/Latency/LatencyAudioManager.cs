using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class LatencyAudioManager : MonoBehaviour
{
    [FoldoutGroup("음악")]
    [Title("오디오 믹서")]
    public AudioMixer audioMixer;
    [FoldoutGroup("음악")]
    [Title("BPM")]
    public int bpm;
    [FoldoutGroup("음악")]
    [Title("인터벌")]
    [TableList]
    public List<Intervals> intervals = new ();
    
    [FoldoutGroup("오디오 소스")]
    [Title("드럼 오디오 소스")]
    public AudioSource audioSource;
    [FoldoutGroup("오디오 소스")]
    [Title("BGM")]
    public AudioSource bgmAudio;
    [FoldoutGroup("오디오 소스")]
    [Title("UI")]
    public AudioSource uiAudio;
    
    [FoldoutGroup("오디오 클립")]
    [Title("UI-Button")]
    public AudioClip uiButtonClip;

    [FoldoutGroup("변수")] 
    [Title("로딩")]
    public bool isLoaded;

    public static LatencyAudioManager instance;

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
        
    void Update()
    {
        if (!isLoaded)
        {
            return;
        }
        
        foreach (Intervals interval in intervals)
        {
            float sampledTime = bgmAudio.timeSamples / (bgmAudio.clip.frequency * interval.GetIntervalLength(bpm));
            interval.CheckForNewInterval(sampledTime);
        }
    }
    
    IEnumerator Init()
    {
        while (!DBManager.instance.isJsonLoad)
        {
            yield return null;
        }
        
        audioSource.clip.LoadAudioData();
        bgmAudio.clip.LoadAudioData();
        audioMixer.SetFloat("Music", DBManager.instance.musicValue * 80 - 80);
        audioMixer.SetFloat("FX", DBManager.instance.sfxValue * 80 - 80);
        isLoaded = true;
        bgmAudio.Play();
    }

    public void PlayBeatRepeatedly()
    {
        audioSource.Play();
    }

    public int GetSampledTime()
    {
        return (int)(1000f * bgmAudio.timeSamples / bgmAudio.clip.frequency);
    }
    
    public void PlayAudio(string type)
    {
        switch (type)
        {
            case "Button":
                if (LatencyManager.instance.set.activeSelf && DBManager.instance.currentPlatform == "MOBILE" && EventSystem.current.currentSelectedGameObject.name == "Slider")
                {
                    
                }
                else
                {
                    uiAudio.PlayOneShot(uiButtonClip);
                }
                break;
        }
    }
}
