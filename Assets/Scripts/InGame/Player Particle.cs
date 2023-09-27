using UnityEngine;

public class PlayerParticle : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    
    public float landingParticleMinSpeed = 5f;
    public ParticleSystem accelationTrails;
    public ParticleSystem landDust;
    public ParticleSystem walkDust;
    
    public void Play(ParticleSystem particle)
    {
        if (!particle.isPlaying)
        {
            particle.Play();
        }
    }
    
    public void Stop(ParticleSystem particle, bool clear = false)
    {
        if (particle.isPlaying)
        {
            var mode = clear ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting;
            particle.Stop(true, mode);
        }
    }
    
    public void WalkParticle()
    {
        if (player.isGrounded && !accelationTrails.isPlaying && !landDust.isPlaying)
        {
            Play(walkDust);
        }
        else
        {
            Stop(walkDust);
        }
    }

    public void LandParticle()
    {
        if (Mathf.Abs(player.GetComponent<Rigidbody>().velocity.y) >= landingParticleMinSpeed)
        {
            Play(landDust);
        }
    }
    

    public void OnAccelation()
    {
        Play(accelationTrails);
    }
    
    void Start()
    {
        player = GetComponent<PlayerController>();
        
    }
    
    void Update() => WalkParticle();
}
