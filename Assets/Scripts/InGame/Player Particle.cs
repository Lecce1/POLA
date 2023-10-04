using UnityEngine;

public class PlayerParticle : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    
    public float landingParticleMinSpeed = 5f;
    public ParticleSystem accelationTrails;
    public ParticleSystem landDust;
    public ParticleSystem walkDust;
    public ParticleSystem eatItem;
    
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
        if (player.isGrounded && !accelationTrails.isPlaying && !landDust.isPlaying && !player.stats.current.isDead)
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
        Play(landDust);
    }
    

    public void OnAccelation()
    {
        Play(accelationTrails);
    }

    public void OnEatItem()
    {
        Play(eatItem);
    }
    
    void Start()
    {
        player = GetComponent<PlayerController>();
    }
    
    void Update() => WalkParticle();
}
