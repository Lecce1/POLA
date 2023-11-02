using System.Collections;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    public bool isTransparent { get; private set; } = false;
    private MeshRenderer[] renderers;
    private float alpha = 0.25f;
    private float max_Time = 0.5f;
    private bool isReseting = false;
    private float timer = 0f;
    private Coroutine timeCheckCoroutine;
    private Coroutine resetCoroutine;
    private Coroutine becomeTransparentCoroutine;
    WaitForSeconds delay = new WaitForSeconds(0.001f);
    WaitForSeconds resetDelay = new WaitForSeconds(0.005f);
    
    void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
    } 
    
    public void Transparent()
    {
        if (isTransparent)
        {
            timer = 0f;
            return;
        }

        if (resetCoroutine != null && isReseting)
        {
            isReseting = false;
            isTransparent = false;
            StopCoroutine(resetCoroutine);
        }

        SetMaterialTransparent();
        isTransparent = true;
        becomeTransparentCoroutine = StartCoroutine(Transparent_Coroutine());
    }
    
    void SetMaterialTransparent()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            foreach(Material material in renderers[i].materials)
            {
                SetRenderingMode(material, 3f, 3000);
            }
        }
    }
    
    // 0 = Opaque, 1 = Cutout, 2 = Fade, 3 = Transparent
    void SetRenderingMode(Material material, float mode, int renderQueue)
    {
        material.SetFloat("_Mode", mode);      
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = renderQueue;
    }

    public void ResetOriginalTransparent()
    {
        resetCoroutine = StartCoroutine(ResetTransparent_Coroutine());
    }

    IEnumerator Transparent_Coroutine()
    {
        while (true)
        {
            bool isComplete = false;

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.color.a < alpha)
                {
                    isComplete = true;
                }

                Color color = renderers[i].material.color;
                color.a -= Time.deltaTime;
                renderers[i].material.color = color;
            }

            if (isComplete)
            {
                Check();
                break;
            }

            yield return delay;
        }
    }

    IEnumerator ResetTransparent_Coroutine()
    {
        isTransparent = false;

        while (true)
        {
            bool isComplete = false;

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.color.a >= 1f)
                {
                    isComplete = true;
                }

                Color color = renderers[i].material.color;
                color.a += Time.deltaTime;
                renderers[i].material.color = color;
            }

            if (isComplete)
            {
                isReseting = false;
                break;
            }

            yield return resetDelay;
        }
    }

    public void Check()
    {
        if (timeCheckCoroutine != null)
        {
            StopCoroutine(timeCheckCoroutine);
        }

        timeCheckCoroutine = StartCoroutine(Check_Couroutine());
    }

    IEnumerator Check_Couroutine()
    {
        timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            if(timer > max_Time)
            {
                isReseting = true;
                ResetOriginalTransparent();
                break;
            }

            yield return null;
        }
    }
}