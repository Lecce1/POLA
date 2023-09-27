using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    // public Text timerText;
    // public int timer = 0;
    
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

    // private void Start()
    // {
    //     StartCoroutine(TimerCoroution());
    // }
    //
    // IEnumerator TimerCoroution()
    // {
    //     timer += 1;
    //
    //     timerText.text = (timer / 3600).ToString("D2") + ":" + (timer / 60 % 60).ToString("D2") + ":" + (timer % 60).ToString("D2");
    //     
    //     yield return new WaitForSeconds(1f);
    //
    //     StartCoroutine(TimerCoroution());
    // }
}
