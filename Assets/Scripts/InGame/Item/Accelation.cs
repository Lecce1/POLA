using UnityEngine;
public class Accelation : Effect
{
    /// <summary>
    /// 가속 효과
    /// </summary>
    public override void OnEnterEffect(PlayerController player)
    {
        particle = player.GetComponent<PlayerParticle>();
        //PP = Camera.GetComponent<PostProcessing>();
    }
    public override void OnStepEffect(PlayerController player)
    {
        particle.OnAccelation();
        player.stats.current.maxSpeed = 100f;
        player.stats.current.acceleration = 20000f;
    }

    public override void OnExitEffect(PlayerController player)
    {
        particle.Stop(particle.accelationTrails);
        player.stats.current.maxSpeed = player.stats.origin.maxSpeed;
        player.stats.current.acceleration = player.stats.origin.acceleration;
    }
}