using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    public string[] effects;
    public List<Effect> effectList = new ();
    public Image[] effectGauges;
    private PlayerController player;

    private List<Effect> GetEffectList()
    {
        return Effect.CreateListFromStringArray(effects);
    }

    public void RunEffect(int index, float duration, bool isLingering)
    {
        if (isLingering)
        {
            effectGauges[index].fillAmount = 1f;
            var coroutine = EffectOnStep(index, duration);
            
            StopCoroutine(coroutine);
            StartCoroutine(coroutine);
        }
        else
        {
            effectList[index].RunEffect(player);
        }
    }

    IEnumerator EffectOnStep(int index, float duration)
    {
        float time = Time.time;

        while (time + duration > Time.time)
        {
            Debug.Log(Time.time - time + ", " + duration);
            effectGauges[index].fillAmount -= Time.deltaTime / duration;
            effectList[index].RunEffect(player);
            yield return null;
        }
    }
    
    protected void Start()
    {
        player = GetComponent<PlayerController>();
        effectList = GetEffectList();
    }
}