using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [FoldoutGroup("텍스트")] 
    [Title("스테이지 이름 버튼")]
    public Text stage_Name_Text;
    [FoldoutGroup("텍스트")] 
    [Title("입장 버튼")] 
    public Text join_Btn_Text;

    [FoldoutGroup("기타")] 
    [Title("입장 버튼 타입")] 
    [SerializeField]
    public string join_Btn_Type;
    [FoldoutGroup("기타")] 
    [Title("패널 열림 여부")] 
    [SerializeField]
    public bool isPanelOpen = false;
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
        EventSystem.current.SetSelectedGameObject(join_Btn);
    }
    
    public void DoorInit(string name, string btnText, string nameText, bool isLock)
    {
        if (!isLock)
        {
            join_Btn_Type = name;
            join_Btn.GetComponent<Button>().onClick.RemoveAllListeners();
            Join_Btn_OnOff(true, false);
            join_Btn.GetComponent<Button>().onClick.AddListener(() => LobbyManager.instance.Button(name));
            join_Btn_Text.text = btnText;
            stage_Name_Text.text = nameText;
            Join_Btn_OnOff(true, false);
        }
        else
        {
            stage_Name_Text.text = nameText;
            Join_Btn_OnOff(true, true);
        }
    }

    public void Join_Btn_OnOff(bool isOn, bool onlyStage)
    {
        if (isOn == true)
        {
            if (!onlyStage && join_Btn.activeSelf)
            {
                join_Btn.GetComponent<Animator>().Play("JoinOn");
            }
            
            stage_Name_Text.GetComponent<Animator>().Play("StageNameOn");
        }
        else if (isOn == false)
        {
            if (!onlyStage && join_Btn.activeSelf)
            {
                join_Btn.GetComponent<Animator>().Play("JoinOff");
            }
            
            stage_Name_Text.GetComponent<Animator>().Play("StageNameOff");
        }
    }
    
    public void Sign_Btn(bool isOn)
    {
        if (isOn == true)
        {
            join_Btn.GetComponent<Animator>().Play("On");
        }
        else if (isOn == false)
        {
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

            case "Set":
                if (!set.activeSelf)
                {
                    set.SetActive(true);
                    backStack.Push(set);
                    isPanelOpen = true;

                    if (join_Btn.activeSelf)
                    {
                        join_Btn.SetActive(false);
                    }
                }
                break;
            
            case "Shop":
                if (!shop.activeSelf)
                {
                    shop.SetActive(true);
                    backStack.Push(shop);
                    isPanelOpen = true;
                    
                    if (join_Btn.activeSelf)
                    {
                        join_Btn.SetActive(false);
                    }
                }
                break;
            
            case "Sign":
                if (!sign.activeSelf)
                {
                    sign.SetActive(true);
                    backStack.Push(sign);
                    isPanelOpen = true;
                }
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
                if (set.activeSelf)
                {
                    set.SetActive(false);
                    isPanelOpen = false;
                    
                    if (join_Btn.activeSelf == false)
                    {
                        join_Btn.SetActive(true);
                    }
                }
                break;
            
            case "Shop":
                if (shop.activeSelf)
                {
                    shop.SetActive(false);
                    isPanelOpen = false;

                    if (join_Btn.activeSelf == false)
                    {
                        join_Btn.SetActive(true);
                    }
                }
                break;
            
            case "Sign":
                if (sign.activeSelf)
                {
                    sign.SetActive(false);
                    isPanelOpen = false;
                }
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
