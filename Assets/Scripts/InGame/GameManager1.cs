using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager1 : MonoBehaviour
{
    [FoldoutGroup("일반")] 
    public Text timerText;
    
    [FoldoutGroup("일반")] 
    public float timer = 0;
 
    public static GameManager1 instance;
    
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

    private void Start()
    {
        StartCoroutine(TimerCoroutine());
    }
    
    IEnumerator TimerCoroutine()
    {
        int min = 0;
        float sec = 0;
        
        while (true)
        {
            timer += Time.deltaTime;
            sec += Time.deltaTime;
            
            if (sec >= 60)
            {
                min++;
                sec -= 60;
            }
            
            timerText.text = min + ":" + sec.ToString("00.00");
            yield return null;
        }
    }
}
