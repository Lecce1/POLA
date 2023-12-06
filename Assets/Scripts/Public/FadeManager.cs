using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [Title("PlayerInput")] 
    public PlayerInput playerInput;
    [Title("Fade 이미지")]
    public Image fade;
    [Title("FadeManager")] 
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
        switch (playerInput.currentControlScheme)
        {
            case "PC":
                DBManager.instance.currentPlatform = "PC";
                break;
            
            case "CONSOLE":
                DBManager.instance.currentPlatform = "CONSOLE";
                break;
            
            case "MOBILE":
                DBManager.instance.currentPlatform = "MOBILE";
                break;
        }
        
        float count = 0;
        
        if (fade == null)
        {
            fade = GetComponent<Image>();
        }

        fade.raycastTarget = true;
        fade.color = new Color(0, 0, 0, 0);
        
        while (fade.color.a < 1)
        {
            count += Time.deltaTime;
            fade.color = new Color(0, 0, 0, count);
            yield return null;
        }

        fade.color = new Color(0, 0, 0, 1);

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
        
        while (fade.color.a > 0)
        {
            count -= Time.deltaTime;
            fade.color = new Color(0, 0, 0, count);
            yield return null;
        }
        
        fade.color = new Color(0, 0, 0, 0);
    }
}
