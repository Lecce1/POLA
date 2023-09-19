public class Rainbow : Effect
{
    public override void OnStepEffect(PlayerController player)
    {
        player.stats.current.isInvincibility = true;
    }

    public override void OnExitEffect(PlayerController player)
    {
        player.stats.current.isInvincibility = false;
    }
}
