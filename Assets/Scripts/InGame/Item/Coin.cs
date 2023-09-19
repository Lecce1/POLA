using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Effect
{
    public static Coin instance;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public override void RunEffect(PlayerController player)
    {
        
    }

    public override void RunExit(PlayerController player)
    {
        
    }
}
