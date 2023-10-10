using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [FoldoutGroup("일반")]
    [SerializeField]
    private PlayerController player;
    
    public static ItemManager instance;
    List<Pair<Coroutine, Type>> coroutineList = new();

    /// <summary>
    /// 제네릭에 대한 입력값 쌍을 구현
    /// </summary>
    private class Pair<T, U>
    {
        public T First;
        public U Second;

        public Pair(T first, U second)
        {
            First = first;
            Second = second;
        }
    }
    
    /// <summary>
    /// 인스턴스 생성
    /// </summary>
    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        
        if (instance == null)
        {
            instance = this;
        }
    }
    
    /// <summary>
    /// 먹은 아이템이 무엇인지 받아와서 그에 맞는 아이템 효과를 실행
    /// </summary>
    /// <param name="isLingering">잔류형 아이템인지 사용 즉시 효과가 실행되고 끝인지를 나타내는 bool형 파라미터</param>
    /// <param name="duration">잔류형 아이템이라면 그 시간이 얼마나 지속되는지를 나타내는 float형 파라미터</param>
    public void RunEffect<T>(float duration, bool isLingering) where T : Effect, new()
    {
        T effect = new T();

        player.particle.OnEatItem();
        effect.OnEnterEffect(player);
        if (isLingering)
        {
            int idx = coroutineList.FindIndex(s => s.Second == effect.GetType());
            
            if (idx == -1)
            {
                coroutineList.Add(new Pair<Coroutine, Type>(StartCoroutine(EffectOnStep(effect, duration)), effect.GetType()));
            }
            else
            {
                StopCoroutine(coroutineList[idx].First);
                coroutineList[idx].First = StartCoroutine(EffectOnStep(effect, duration));
            }
        }
        else
        {
            effect.OnStepEffect(player);
        }
    }

    /// <summary>
    /// 아이템 효과를 지속시간만큼 실행
    /// </summary>
    /// <param name="type">아이템의 타입</param>
    IEnumerator EffectOnStep<T>(T type, float duration) where T : Effect
    {
        float time = Time.time;
        
        while (time + duration > Time.time)
        {
            type.OnStepEffect(player);
            yield return null;
        }
        
        type.OnExitEffect(player);
        
        int idx = coroutineList.FindIndex(s => s.Second == type.GetType());
        coroutineList.RemoveAt(idx);
    }
}