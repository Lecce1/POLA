public class Accelation : Effect
{
    /// <summary>
    /// 가속 효과
    /// </summary>
    public override void OnStepEffect(PlayerController player)
    {
        player.stats.current.maxSpeed = 100f;
        player.stats.current.acceleration = 5000f;
    }

    public override void OnExitEffect(PlayerController player)
    {
        player.stats.current.maxSpeed = player.stats.origin.maxSpeed;
        player.stats.current.acceleration = player.stats.origin.acceleration;
    }
}