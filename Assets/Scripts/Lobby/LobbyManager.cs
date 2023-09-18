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
    [Title("로비")] 
    public GameObject lobby;
    [FoldoutGroup("패널")] 
    [Title("인벤토리")] 
    public GameObject inventory;
    [FoldoutGroup("패널")] 
    [Title("모드")] 
    public GameObject mode;
    [FoldoutGroup("패널")] 
    [Title("결과")] 
    public GameObject result;
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
    
    [FoldoutGroup("정보")] 
    [Title("닉네임")] 
    public Text nickName_Text;
    [FoldoutGroup("정보")] 
    [Title("코인")] 
    public Text coin_Text;
    [FoldoutGroup("정보")] 
    [Title("크리스탈")] 
    public Text crystal_Text;
    
    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static LobbyManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        backStack = new Stack<GameObject>();
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        nickName_Text.text = DBManager.instance.nickName;
        coin_Text.text = DBManager.instance.coin.ToString();
        crystal_Text.text = DBManager.instance.crystal.ToString();
    }

    public void Button(string type)
    {
        switch (type)
        {
            case "Mode":
                mode.SetActive(true);
                backStack.Push(mode);
                break;
            
            case "Stage":
                DBManager.instance.nextScene = "Game";
                SceneManager.LoadScene("Loading");
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
            case "Mode":
                mode.SetActive(false);
                break;
            
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
