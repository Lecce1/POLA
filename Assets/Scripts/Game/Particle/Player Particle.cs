using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerParticle : MonoBehaviour
{
    [FoldoutGroup("일반")]
    [SerializeField]
    private PlayerController player;

    [FoldoutGroup("파티클")]
    public ParticleSystem landDust;
    
    [FoldoutGroup("파티클")]
    public ParticleSystem walkDust;
    
    [FoldoutGroup("파티클")]
    public GameObject attack;
    
    [FoldoutGroup("플레이어")]
    [Title("파티클 폴더")]
    public GameObject particleFolder;
    
    void Start()
    {
        player = GetComponent<PlayerController>();
    }
    
    void Update() => WalkParticle();
    
    public void Play(GameObject effect)
    {
        GameObject temp = Instantiate(effect);
        temp.transform.parent = particleFolder.transform;
        temp.transform.position = particleFolder.transform.position;
        
        if (!temp.GetComponent<ParticleSystem>().isPlaying)
        {
            temp.GetComponent<ParticleSystem>().Play();
            StartCoroutine("Check", temp);
        }
    }

    IEnumerator Check(GameObject particle)
    {
        while (particle.GetComponent<ParticleSystem>().isPlaying)
        {
            yield return null;
        }
        
        Destroy(particle);
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
        /*if (player.isGrounded && !landDust.isPlaying)
        {
            Play(walkDust);
        }
        else
        {
            Stop(walkDust);
        }*/
    }

    public void AttackParticle()
    {
        Play(attack);
    }
}
