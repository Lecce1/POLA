using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private PlayerController player;

    public static ItemManager instance;
    
    List<Pair<Coroutine, Type>> coroutineList = new();

    private class Pair<T, U>
    {
        public T First;
        public U Second;

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void RunEffect<T>(float duration, bool isLingering) where T : Effect, new()
    {
        T effect = new T();
        if (isLingering)
        {
            int idx = coroutineList.FindIndex(s => s.Second == effect.GetType());
            
            if (idx == -1)
            {
                idx = coroutineList.Count;
                coroutineList.Add(new Pair<Coroutine, Type>(StartCoroutine(EffectOnStep(effect, duration, idx)), effect.GetType()));
            }
            else
            {
                StopCoroutine(coroutineList[idx].First);
                coroutineList[idx].First = StartCoroutine(EffectOnStep(effect, duration, idx));
            }
        }
        else
        {
            effect.OnStepEffect(player);
        }
    }

    IEnumerator EffectOnStep<T>(T type, float duration, int coroutineIndex) where T : Effect
    {
        float time = Time.time;
        
        while (time + duration > Time.time)
        {
            type.OnStepEffect(player);
            yield return null;
        }
        
        type.OnExitEffect(player);
        coroutineList.RemoveAt(coroutineIndex);
    }
}