using System;
using System.Collections.Generic;

public abstract class Effect
{
    public abstract void RunEffect(PlayerController player);
    
    private static Effect CreateFromString(string effectName)
    {
        Type type = Type.GetType(effectName);
        Effect effect = (Effect)Activator.CreateInstance(type);
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