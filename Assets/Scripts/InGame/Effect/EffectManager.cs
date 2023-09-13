using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    public string[] effects;
    public List<Effect> effectList = new ();
    public Image[] effectGauges;

    private List<Effect> GetEffectList()
    {
        return Effect.CreateListFromStringArray(effects);
    }
    
    private void InitializeEffect()
    {
        for (int i = 0; i < effectList.Count; i++)
        {
            effectList[i].effectGauge = effectGauges[i];
        }
    }

    public void RunEffect(int index, float duration)
    {
        effectList[index].RunEffect(duration);
    }
    
    protected void Start()
    {
        effectList = GetEffectList();
        InitializeEffect();
    }
}