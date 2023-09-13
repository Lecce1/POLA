using UnityEngine;

public class MagnetItem : Item
{
    public void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        player.effects.RunEffect(0, duration);
    }
}