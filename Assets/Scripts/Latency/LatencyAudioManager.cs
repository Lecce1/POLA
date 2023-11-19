using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class LatencyAudioManager : MonoBehaviour
{
    [FoldoutGroup("음악")]
    [Title("오디오 믹서")]
    public AudioMixer audioMixer;
    [FoldoutGroup("음악")]
    [Title("BPM")]
    public int bpm;
    [FormerlySerializedAs("audio")]
    [FoldoutGroup("음악")]
    [Title("박자")]
    public AudioSource audioSource;
    [FoldoutGroup("음악")]
    [Title("오디오")]
    public AudioSource audioBGM;
    [FoldoutGroup("음악")]
    [Title("인터벌")]
    [TableList]
    public List<Intervals> intervals = new ();

    public static LatencyAudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        audioSource.clip.LoadAudioData();
        audioBGM.clip.LoadAudioData();
    }
        
    void Update()
    {
        foreach (Intervals interval in intervals)
        {
            float sampledTime = audioBGM.timeSamples / (audioBGM.clip.frequency * interval.GetIntervalLength(bpm));
            interval.CheckForNewInterval(sampledTime);
        }
    }

    public void PlayBeatRepeatedly()
    {
        audioSource.Play();
    }
}
