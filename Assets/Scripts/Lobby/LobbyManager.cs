using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("인벤토리")] 
    public GameObject inventory;
    [FoldoutGroup("패널")] 
    [Title("설정")] 
    public GameObject set;
    [FoldoutGroup("패널")] 
    [Title("상점")] 
    public GameObject shop;
    [FoldoutGroup("패널")] 
    [Title("에러")] 
    public GameObject error;
    [FoldoutGroup("패널")] 
    [Title("에러_네트워크")] 
    public GameObject error_Network;
    [FoldoutGroup("패널")] 
    [Title("입장 버튼")] 
    public GameObject join_Btn;
    
    [FoldoutGroup("DB")] 
    [Title("닉네임")] 
    public Text nickName_Text;
    [FoldoutGroup("DB")] 
    [Title("코인")] 
    public Text coin_Text;
    [FoldoutGroup("DB")] 
    [Title("크리스탈")] 
    public Text crystal_Text;

    [FoldoutGroup("기타")] 
    [Title("경쟁모드 버튼 클릭 여부")] 
    [SerializeField]
    private bool isMatching = false;

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

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (DBManager.instance != null)
        {
            nickName_Text.text = DBManager.instance.nickName;
            coin_Text.text = DBManager.instance.coin.ToString();
            crystal_Text.text = DBManager.instance.crystal.ToString();
        }

        if (join_Btn.activeSelf == true)
        {
            join_Btn.SetActive(false);
        }
    }

    public void Button(string type)
    {
        switch (type)
        {
            case "Stage":
                if (DBManager.instance != null)
                {
                    DBManager.instance.nextScene = "Game";
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
            
            case "Inventory":
                inventory.SetActive(true);
                backStack.Push(inventory);
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
            
            case "Inventory":
                inventory.SetActive(false);
                break;
            
            case "Error":
                error.SetActive(false);
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
