using System;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    public float bpm;
    public AudioSource audio;
    public Intervals[] intervals;

    private void Update()
    {
        foreach (Intervals interval in intervals)
        {
            float sampledTime = (audio.timeSamples / (audio.clip.frequency * interval.GetIntervalLength(bpm)));
            interval.CheckForNewInterval(sampledTime);
        }
    }

    void Start()
    {
        Invoke(nameof(PlayMusic), 1f);
    }

    void PlayMusic()
    {
        
        audio.Play();
    }
}

[Serializable]
public class Intervals
{
    [SerializeField]
    private float steps;
    [SerializeField]
    private UnityEvent trigger;
    private int lastInterval;
    
    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * steps);
    }

    public void CheckForNewInterval(float interval)
    {
        if (Mathf.FloorToInt(interval) != lastInterval)
        {
            lastInterval = Mathf.FloorToInt(interval);
            trigger.Invoke();
        }
    }
}