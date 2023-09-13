using System;
using System.Collections;
using UnityEngine;

public class MagneticEffect : Effect
{
    private Collider[] overlaps = new Collider[15];
    
    public override void RunEffect(float duration)
    {
        base.RunEffect(duration);
        StopCoroutine(Magnetic(0));
        StartCoroutine(Magnetic(duration));

    }

    IEnumerator Magnetic(float duration)
    {
        float start = Time.time;
        
        while (start + duration > Time.time)
        {
            var overlap = Physics.OverlapSphereNonAlloc(transform.position, 20f, overlaps);
            Debug.Log(overlap);

            for (int i = 0; i < overlap; i++)
            {
                if (overlaps[i].CompareTag("Collectable"))
                {
                    overlaps[i].transform.position =
                        Vector3.MoveTowards(overlaps[i].transform.position, transform.position, 20f * Time.deltaTime);
                }
            }
            yield return null;
        }
    }
}