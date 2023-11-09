using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    private Image fade;
    public static FadeManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        fade = GetComponent<Image>();
    }
    
    public IEnumerator FadeIn()
    {
        float count = 0;
        
        while (fade.color.a < 1)
        {
            count += 0.01f;
            fade.color = new Color(0, 0, 0, count);
            yield return null;
        }
    }
    
    public IEnumerator FadeOut()
    {
        float count = 1;
        
        while (fade.color.a > 0)
        {
            count -= 0.01f;
            fade.color = new Color(0, 0, 0, count);
            yield return null;
        }
    }
}
