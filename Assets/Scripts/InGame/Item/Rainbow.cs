public class Rainbow : Effect
{
    /// <summary>
    /// 무적기
    /// </summary>
    public override void OnStepEffect(PlayerController player)
    {
        player.stats.current.isInvincibility = true;
    }

    public override void OnExitEffect(PlayerController player)
    {
        player.stats.current.isInvincibility = false;
    }
}
