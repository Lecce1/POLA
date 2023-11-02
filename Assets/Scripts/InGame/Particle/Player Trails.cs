using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class PlayerTrails : MonoBehaviour
{ 
    [FoldoutGroup("일반")] 
    [SerializeField]
    private TrailRenderer trail;
    
    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        transform.localPosition = Vector3.zero;
        trail.enabled = false;
    }

    public IEnumerator Trails()
    {
        trail.enabled = true;
        float start = Time.time;

        while (Time.time - start < 0.5f)
        {
            yield return null;
        }

        trail.enabled = false;
    }
}