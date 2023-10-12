using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerParticle : MonoBehaviour
{
    [FoldoutGroup("일반")]
    [SerializeField]
    private PlayerController player;
    
    [FoldoutGroup("파티클")] 
    public ParticleSystem accelationTrails;
    
    [FoldoutGroup("파티클")]
    public ParticleSystem landDust;
    
    [FoldoutGroup("파티클")]
    public ParticleSystem walkDust;
    
    [FoldoutGroup("파티클")]
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
        Stop(eatItem);
    }
    
    void Update() => WalkParticle();
}
