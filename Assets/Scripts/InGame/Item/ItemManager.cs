using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    private PlayerController player;

    public static ItemManager instance;

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
        T t = new T();
        if (isLingering)
        {
            var coroutine = EffectOnStep(t, duration);
            
            StopCoroutine(coroutine);
            StartCoroutine(coroutine);
        }
        else
        {
            t.RunEffect(player);
        }
    }

    IEnumerator EffectOnStep<T>(T type, float duration) where T : Effect
    {
        float time = Time.time;

        while (time + duration > Time.time)
        {
            Debug.Log(Time.time - time + ", " + duration);
            type.RunEffect(player);
            yield return null;
        }
        type.RunExit(player);
    }
}