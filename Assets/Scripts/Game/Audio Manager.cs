using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    [FoldoutGroup("음악")]
    [Title("BPM")]
    public float bpm;
    
    [FoldoutGroup("음악")]
    [Title("오디오")]
    public AudioSource audio;
    
    [FoldoutGroup("음악")]
    [Title("인터벌")]
    [TableList]
    public List<Intervals> intervals = new ();
    
    
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