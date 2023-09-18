using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Back()
    {
        DBManager.instance.nextScene = "Lobby";
        SceneManager.LoadScene("Loading");
    }

    public void Reset()
    {
        Invoke(nameof(Reset2), 2f);
    }
    
    public void Reset2()
    {
        SceneManager.LoadScene("Game");
    }
}
