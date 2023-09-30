using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Camera))]
public class PostProcessing : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume volume;
    
    [SerializeField]
    private DepthOfField dof;

    [SerializeField] 
    private ChromaticAberration ca;

    [SerializeField] 
    private LensDistortion ld;
    
    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out dof);
        volume.profile.TryGetSettings(out ca);
        volume.profile.TryGetSettings(out ld);

        ItemEaten();
    }

    IEnumerator AdjustDepthOfField(float focusDistance, float aperture)
    {
        float startTime = Time.time;
        float duration = 0.4f;

        while (Time.time - startTime < duration)
        {
            if (dof != null)
            {
                dof.focusDistance.value = focusDistance;
                dof.aperture.value = aperture;
            }
            
            yield return null;
        }
    }

    IEnumerator AdjustChromaticAberration()
    {
        float startTime = Time.time;
        float duration = 1f;

        while (Time.time - startTime < duration)
        {
            if (dof != null)
            {
                ca.intensity.value = 1f;
                ld.intensity.value = -100f;
                ld.scale.value = 0.9f;
            }
            
            yield return null;
        }
    }

    IEnumerator ApplyEffects()
    {
        yield return StartCoroutine(AdjustChromaticAberration());
        ResetCa();
    }

    void ResetDof()
    {
        if (dof != null)
        {
            dof.focusDistance.value = 10.0f;
            dof.aperture.value = 2.8f;
        }
    }

    void ResetCa()
    {
        if (ca != null)
        {
            ca.intensity.value = 0;
            ld.intensity.value = 0f;
            ld.scale.value = 1f;
        }
    }
    
    public void ItemEaten()
    {
        StartCoroutine(ApplyEffects());
    }
}
