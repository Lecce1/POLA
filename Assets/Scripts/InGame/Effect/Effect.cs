using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    private PlayerController player;
    public Image effectGauge;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    public virtual void RunEffect(float duration)
    {
        if (duration != 0)
        {
            effectGauge.fillAmount = 1f;
            StopCoroutine(GaugeDecrease(0));
            StartCoroutine(GaugeDecrease(duration));
        }
    }
    
    IEnumerator GaugeDecrease(float duration)
    {
        float start = Time.time;

        while (start + duration > Time.time)
        {
            effectGauge.fillAmount -= Time.time / duration;
            yield return null;
        }
    }
    
    private static Effect CreateFromString(string effectName)
    {
        Type type = Type.GetType(effectName);
        Effect effect = (Effect)Activator.CreateInstance(type);
        Debug.Log(effect);
        return effect;
    }


    public static List<Effect> CreateListFromStringArray(string[] array)
    {
        var list = new List<Effect>();
    
        if (array != null && array.Length > 0)
        {
            foreach (var effectName in array)
            {
                Effect effect = CreateFromString(effectName);
                if (effect != null)
                {
                    list.Add(effect);
                }
            }
        }

        return list;
    }

}