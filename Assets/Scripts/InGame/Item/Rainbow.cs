using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rainbow : Effect
{
    public static Rainbow instance;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public override void RunEffect(PlayerController player)
    {
        player.stats.current.isInvincibility = true;
    }

    public override void RunExit(PlayerController player)
    {
        player.stats.current.isInvincibility = false;
    }
}
