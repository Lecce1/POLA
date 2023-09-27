using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Camera))]
public class PostProcessing : MonoBehaviour
{
    public PostProcessVolume volume;
    public DepthOfField dof;

    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out dof);

        ItemEaten();
    }

    IEnumerator AdjustDepthOfField(float focusDistance, float aperture)
    {
        float startTime = Time.time;
        float duration = 5.0f;

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


    IEnumerator ApplyEffects()
    {
        yield return StartCoroutine(AdjustDepthOfField(1f, 1.2f));
        ResetEffects();
    }

    void ResetEffects()
    {
        if (dof != null)
        {
            dof.focusDistance.value = 10.0f;
            dof.aperture.value = 2.8f;
        }
    }
    public void ItemEaten()
    {
        StartCoroutine(ApplyEffects());
    }
}
