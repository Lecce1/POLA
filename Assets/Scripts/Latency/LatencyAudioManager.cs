using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class LatencyAudioManager : MonoBehaviour
{
    [FoldoutGroup("음악")]
    [Title("오디오 믹서")]
    public AudioMixer audioMixer;
    [FoldoutGroup("음악")]
    [Title("BPM")]
    public int bpm;
    [FoldoutGroup("음악")]
    [Title("드럼 오디오 소스")]
    public AudioSource audioSource;
    [FoldoutGroup("음악")]
    [Title("BGM 오디오 소스")]
    public AudioSource audioBGM;
    [FoldoutGroup("음악")]
    [Title("인터벌")]
    [TableList]
    public List<Intervals> intervals = new ();
    
    [FoldoutGroup("변수")] 
    [Title("로딩")] 
    [SerializeField]
    private bool isLoaded;

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
            float sampledTime = audioBGM.timeSamples / (audioBGM.clip.frequency * interval.GetIntervalLength(bpm));
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
        audioBGM.clip.LoadAudioData();
        audioMixer.SetFloat("Music", DBManager.instance.musicValue * 80 - 80);
        audioMixer.SetFloat("FX", DBManager.instance.sfxValue * 80 - 80);
        isLoaded = true;
    }

    public void PlayBeatRepeatedly()
    {
        audioSource.Play();
    }

    public int GetSampledTime()
    {
        return (int)(1000f * audioBGM.timeSamples / audioBGM.clip.frequency);
    }
}
