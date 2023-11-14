using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    [FoldoutGroup("음악")]
    [Title("오디오 믹서")]
    public AudioMixer audioMixer;
    
    [FoldoutGroup("음악")]
    [Title("BPM")]
    public float bpm;
    
    [FoldoutGroup("음악")]
    [Title("오디오")]
    public new AudioSource audio;
    
    [FoldoutGroup("음악")]
    [Title("인터벌")]
    [TableList]
    public List<Intervals> intervals = new ();

    public static AudioManager instance;

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
        
        audioMixer.SetFloat("Music", 140 * DBManager.instance.musicValue - 71);
        audioMixer.SetFloat("FX", 140 * DBManager.instance.sfxValue - 71);
    }

    void Update()
    {
        foreach (Intervals interval in intervals)
        {
            float sampledTime = audio.timeSamples / (audio.clip.frequency * interval.GetIntervalLength(bpm));
            interval.CheckForNewInterval(sampledTime);
        }
    }
}

/// <summary>
/// 인터벌
/// </summary>
[Serializable]
public class Intervals
{
    [SerializeField]
    public float steps;
    [SerializeField]
    private UnityEvent trigger;
    private int lastInterval;
    
    /// <summary>
    /// 인터벌 길이
    /// </summary>
    /// <param name="bpm">BPM</param>
    /// <returns>비트 시간</returns>
    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * steps);
    }

    /// <summary>
    /// 다음 인터벌까지의 시간
    /// </summary>
    /// <param name="interval">인터벌 길이</param>
    public void CheckForNewInterval(float interval)
    {
        if (Mathf.FloorToInt(interval) != lastInterval)
        {
            lastInterval = Mathf.FloorToInt(interval);
            trigger.Invoke();
        }
    }
}