using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("설정")] 
    public GameObject set;
    
    [FoldoutGroup("패널")] 
    [Title("상단 패널")] 
    public GameObject topPanel;
    
    [FoldoutGroup("패널")] 
    [Title("하단 패널")] 
    public GameObject bottomPanel;
    
    [FoldoutGroup("기타")] 
    [Title("패널 열림 여부")] 
    [SerializeField]
    public bool isPanelOpen = false;
    
    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        backStack = new Stack<GameObject>();
    }
    
    public void Button(string type)
    {
        switch (type)
        {
            case "Set":
                if (!set.activeSelf)
                {
                    if (topPanel.activeSelf)
                    {
                        topPanel.SetActive(false);
                    }
                    
                    if (bottomPanel.activeSelf)
                    {
                        bottomPanel.SetActive(false);
                    }
                    
                    Time.timeScale = 0;
                    set.SetActive(true);
                    backStack.Push(set);
                    isPanelOpen = true;
                }
                break;
            
            case "Stage":
                Time.timeScale = 1;
                
                if (DBManager.instance != null)
                {
                    DBManager.instance.nextScene = "Game 1";
                }

                SceneManager.LoadScene("Loading");
                break;
            
            case "Back":
                Time.timeScale = 1;
                
                if (DBManager.instance != null)
                {
                    DBManager.instance.nextScene = "Lobby";
                }

                SceneManager.LoadScene("Loading");
                break;
        }
    }
    
    public void Back()
    {
        if (backStack.Count <= 0)
        {
            Debug.LogError("뒤로가기 패널이 비었습니다.");
            return;
        }

        bool isCheck = false;

        switch (backStack.Pop().name)
        {
            case "Set":
                if (set.activeSelf)
                {
                    set.SetActive(false);
                    isCheck = true;
                    
                    if (!topPanel.activeSelf)
                    {
                        topPanel.SetActive(true);
                    }
                    
                    if (!bottomPanel.activeSelf)
                    {
                        bottomPanel.SetActive(true);
                    }
                    
                    Time.timeScale = 1;
                }
                break;
        }

        if (isCheck)
        {
            isPanelOpen = false;
        }
    }
}