using UnityEngine;

public class MagnetItem : Item
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        player.effects.RunEffect(0, duration, isLingering);
    }
}