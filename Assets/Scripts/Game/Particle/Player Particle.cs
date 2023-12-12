using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerParticle : MonoBehaviour
{
    [FoldoutGroup("파티클")]
    public ParticleSystem walkDust;
    
    [FoldoutGroup("파티클")]
    public GameObject attack;
    
    [FoldoutGroup("파티클")]
    public GameObject destoyObstacle;
    
    [FoldoutGroup("플레이어")]
    [Title("파티클 폴더")]
    public GameObject particleFolder;
    
    void Update() => WalkParticle();
    
    public void Play(GameObject effect)
    {
        GameObject temp = Instantiate(effect);
        temp.transform.parent = particleFolder.transform;
        if (effect == attack)
        {
            temp.transform.position = particleFolder.transform.position + transform.up * 1.9f + transform.forward * 1.5f;    
        }
        else if (effect = destoyObstacle)
        {
            temp.transform.position = particleFolder.transform.position + transform.up * 1.25f + transform.forward * 2f;    
        }
        
        
        if (!temp.GetComponent<ParticleSystem>().isPlaying)
        {
            temp.GetComponent<ParticleSystem>().Play();
            StartCoroutine(nameof(Check), temp);
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

    public void PlayWalk(ParticleSystem particle)
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
        if (!GameManager.instance.isCountDown)
        {
            PlayWalk(walkDust);    
        }
        else
        {
            Stop(walkDust);
        }
    }

    public void AttackParticle()
    {
        Play(attack);
    }

    public void DestroyPartice()
    {
        Play(destoyObstacle);
    }
}
