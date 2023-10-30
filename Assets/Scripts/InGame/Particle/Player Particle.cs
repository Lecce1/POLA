using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerParticle : MonoBehaviour
{
    [FoldoutGroup("일반")]
    [SerializeField]
    private NewPlayerController player;

    [FoldoutGroup("파티클")]
    public ParticleSystem landDust;
    
    [FoldoutGroup("파티클")]
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
        if (player.isGrounded && !landDust.isPlaying)
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
    
    void Start()
    {
        player = GetComponent<NewPlayerController>();
    }
    
    void Update() => WalkParticle();
}
