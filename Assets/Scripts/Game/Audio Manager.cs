using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
    
    [FoldoutGroup("오디오 소스")]
    [Title("Attack")]
    public AudioSource attackAudio;
    [FoldoutGroup("오디오 소스")]
    [Title("SFX")]
    public AudioSource sfxAudio;
    [FoldoutGroup("오디오 소스")]
    [Title("UI")]
    public AudioSource uiAudio;
    [FoldoutGroup("오디오 소스")]
    [Title("Result")]
    public AudioSource resultAudio;
    
    [FoldoutGroup("오디오 클립")]
    [Title("공격")]
    public AudioClip attackClip;
    [FoldoutGroup("오디오 클립")]
    [Title("UI 버튼")]
    public AudioClip uiButtonClip;
    [FoldoutGroup("오디오 클립")]
    [Title("결과 - 성공")]
    public AudioClip successClip;
    [FoldoutGroup("오디오 클립")]
    [Title("결과 - 실패")]
    public AudioClip failClip;

    [FoldoutGroup("변수")] 
    [Title("로딩")] 
    [SerializeField]
    private bool isLoaded;
    
    [FoldoutGroup("기타")] 
    [Title("플레이어 컨트롤러")]
    public PlayerController playerController;

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
    
    void Update()
    {
        if (!isLoaded)
        {
            return;
        }
        
        foreach (Intervals interval in intervals)
        {
            float sampledTime = audio.timeSamples / (audio.clip.frequency * interval.GetIntervalLength(bpm));
            interval.CheckForNewInterval(sampledTime);
        }
    }

    IEnumerator Init()
    {
        while (!DBManager.instance.isJsonLoad)
        {
            yield return null;
        }
        
        audioMixer.SetFloat("Music", DBManager.instance.musicValue * 80 - 80);
        audioMixer.SetFloat("FX", DBManager.instance.sfxValue * 80 - 80);
        isLoaded = true;
    }
    
    public void PlayAudio(string type)
    {
        switch (type)
        {
            case "Attack":
                attackAudio.PlayOneShot(attackClip);
                break;
            
            case "Result":
                if (playerController.isDead)
                {
                    resultAudio.PlayOneShot(failClip);
                }
                else
                {
                    resultAudio.PlayOneShot(successClip);
                }

                break;
            
            case "Button":
                if (GameManager.instance.set.activeSelf && DBManager.instance.currentPlatform == "MOBILE" && EventSystem.current.currentSelectedGameObject.name == "Slider")
                {
                    
                }
                else
                {
                    uiAudio.PlayOneShot(uiButtonClip);
                }
                break;
        }
    }

    public IEnumerator Progress()
    {
        while (audio.time < audio.clip.length && !GameManager.instance.playerController.isDead)
        {
            GameManager.instance.progress.value = audio.time / audio.clip.length;
            yield return null;
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