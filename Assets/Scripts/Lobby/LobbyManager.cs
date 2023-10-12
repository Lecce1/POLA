using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("설정")] 
    public GameObject set;
    [FoldoutGroup("패널")] 
    [Title("상점")] 
    public GameObject shop;
    [FoldoutGroup("패널")] 
    [Title("표지판")] 
    public GameObject sign;
    [FoldoutGroup("패널")] 
    [Title("에러_네트워크")] 
    public GameObject error_Network;
    [FoldoutGroup("패널")] 
    [Title("입장 버튼")] 
    public GameObject join_Btn;

    [FoldoutGroup("기타")]
    [Title("경쟁모드 버튼 클릭 여부")] 
    [SerializeField]
    private bool isMatching = false;
    [FoldoutGroup("기타")] 
    [Title("입장 버튼 여부")]
    public bool isJoinOn = false;

    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static LobbyManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        backStack = new Stack<GameObject>();
    }

    public void Join_Btn(bool isOn)
    {
        if (isOn == true)
        {
            isJoinOn = true;
            join_Btn.GetComponent<Animator>().Play("On");
        }
        else if (isOn == false)
        {
            isJoinOn = false;
            join_Btn.GetComponent<Animator>().Play("Off");
        }
    }
    
    public void Sign_Btn(bool isOn)
    {
        if (isOn == true)
        {
            isJoinOn = true;
            join_Btn.GetComponent<Animator>().Play("On");
        }
        else if (isOn == false)
        {
            isJoinOn = false;
            join_Btn.GetComponent<Animator>().Play("Off");
        }
    }

    public void Button(string type)
    {
        switch (type)
        {
            case "Stage":
                if (DBManager.instance != null)
                {
                    DBManager.instance.nextScene = "Game 1";
                }

                SceneManager.LoadScene("Loading");
                break;
            
            case "PVP":
                if (isMatching == false)
                {
                    isMatching = true;

                    if (TCPServerManager.instance != null)
                    {
                        TCPServerManager.instance.Send("Matching", "True");
                    }
                }
                else
                {
                    isMatching = false;

                    if (TCPServerManager.instance != null)
                    {
                        TCPServerManager.instance.Send("Matching", "False");
                    }
                }
                
                break;
            
            case "Set":
                set.SetActive(true);
                backStack.Push(set);
                break;
            
            case "Shop":
                shop.SetActive(true);
                backStack.Push(shop);
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
        
        switch (backStack.Pop().name)
        {
            case "Set":
                set.SetActive(false);
                break;
            
            case "Shop":
                shop.SetActive(false);
                break;
        }
    }
    
    public void Set_Logout()
    {
        PlayerPrefs.DeleteAll();
        DBManager.instance.Init();
        SceneManager.LoadScene("Title");
    }
    
    public void Set_DeleteAccount()
    {
        StartCoroutine(HttpServerManager.instance.DeleteAccount());
    }
}
