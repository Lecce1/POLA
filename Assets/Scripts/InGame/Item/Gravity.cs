public class Gravity : Effect
{
    public override void OnEnterEffect(PlayerController player)
    {
        
    }
    
    public override void OnStepEffect(PlayerController player)
    {
        player.OnReverseGravity();
    }

    public override void OnExitEffect(PlayerController player)
    {
        
    }
}
