using System.Collections;
using UnityEngine;

public class Accelation : Effect
{
    public static Accelation instance;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public override void RunEffect(PlayerController player)
    {
        player.stats.current.maxSpeed = 100f;
        player.stats.current.acceleration = 5000f;
    }

    public override void RunExit(PlayerController player)
    {
        player.stats.current.maxSpeed = player.stats.origin.maxSpeed;
        player.stats.current.acceleration = player.stats.origin.acceleration;
    }
}