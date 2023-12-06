using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
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
        
        if (fade == null)
        {
            fade = GetComponent<Image>();
        }

        fade.raycastTarget = true;
        fade.color = new Color(0, 0, 0, 0);

        Debug.Log("1" + fade.color.a);
        while (fade.color.a < 1)
        {
            Debug.Log("2" + fade.color.a);
            count += Time.deltaTime;
            fade.color = new Color(0, 0, 0, count);
            yield return null;
        }

        if (DBManager.instance.nextScene == DBManager.instance.gameSceneName || DBManager.instance.nextScene == DBManager.instance.latencySceneName)
        {
            SceneManager.LoadScene("Loading");
        }
    }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            

    public IEnumerator FadeOut()
    {
        float count = 1;
        
        if (fade == null)
        {
            fade = GetComponent<Image>();
        }

        fade.raycastTarget = false;
        fade.color = new Color(0, 0, 0, 1);
        
        if (fade != null)
        {
            while (fade.color.a > 0)
            {
                count -= Time.deltaTime;
                fade.color = new Color(0, 0, 0, count);
                yield return null;
            }
        }
    }
}
