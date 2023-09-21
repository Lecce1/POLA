using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    /// <summary>
    /// 인스턴스 생성
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// 뒤로가기 버튼
    /// </summary>
    public void Back()
    {
        DBManager.instance.nextScene = "Lobby";
        SceneManager.LoadScene("Loading");
    }

    /// <summary>
    /// 게임 재시작의 딜레이
    /// </summary>
    public void Reset()
    {
        Invoke(nameof(Reset2), 2f);
    }
    
    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void Reset2()
    {
        SceneManager.LoadScene("Game");
    }
}
